using System.Linq.Expressions;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Util;
using AntFlowCore.Vo;
using FreeSql;
using FreeSql.Internal.Model;

namespace antflowcore.service.biz;

public class ProcessApprovalService
{
    private readonly FormFactory _formFactory;
    private readonly ButtonOperationService _buttonOperationService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly ProcessConstantsService _processConstantsService;
    private readonly ConfigFlowButtonContantService _configFlowButtonContantService;
    private readonly BpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly BpmProcessNameRelevancyService _processNameRelevancyService;
    private readonly BpmProcessForwardService _bpmProcessForwardService;
    private readonly IFreeSql _freeSql;
    private readonly BpmProcessNameService _bpmProcessNameService;
    private readonly BpmnConfCommonService _bpmnConfCommonService;
    private readonly AFTaskService _taskService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly ILogger _logger;

    public ProcessApprovalService(
        FormFactory formFactory,
        ButtonOperationService buttonOperationService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmVariableSignUpService bpmVariableSignUpService,
        ProcessConstantsService processConstantsService,
        ConfigFlowButtonContantService configFlowButtonContantService,
        BpmVariableMultiplayerService bpmVariableMultiplayerService,
        BpmProcessNameRelevancyService processNameRelevancyService,
        BpmProcessForwardService bpmProcessForwardService,
        IFreeSql freeSql,
        BpmProcessNameService bpmProcessNameService,
        BpmnConfCommonService bpmnConfCommonService,
        AFTaskService taskService,
        AfTaskInstService afTaskInstService,
        ILogger<ProcessApprovalService> logger
    )
    {
        _formFactory = formFactory;
        _buttonOperationService = buttonOperationService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _processConstantsService = processConstantsService;
        _configFlowButtonContantService = configFlowButtonContantService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _processNameRelevancyService = processNameRelevancyService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _freeSql = freeSql;
        _bpmProcessNameService = bpmProcessNameService;
        _bpmnConfCommonService = bpmnConfCommonService;
        _taskService = taskService;
        _afTaskInstService = afTaskInstService;
        _logger = logger;
    }

    public BusinessDataVo ButtonsOperation(String parameters, String formCode)
    {
        _logger.LogInformation($"params:{parameters},formCode:{formCode}");
        //deserialize parameters that passed in
        BusinessDataVo vo = _formFactory.DataFormConversion(parameters, formCode);
        //To determine the operation Type
        ProcessOperationEnum? poEnum = ProcessOperationEnumExtensions.GetEnumByCode(vo.OperationType);
        if (poEnum == null)
        {
            throw new AFBizException("unknown operation type,please Contact the Administrator");
        }

        formCode = vo.FormCode;
        ThreadLocalContainer.Set(StringConstants.FORM_CODE, formCode);
        //set the operation Flag
        if (poEnum == ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE || poEnum == ProcessOperationEnum.BUTTON_TYPE_STOP)
        {
            vo.Flag = false;
        }
        else if (poEnum == ProcessOperationEnum.BUTTON_TYPE_ABANDON)
        {
            vo.Flag = true;
        }

        //set start user Info
        if (string.IsNullOrEmpty(vo.StartUserId))
        {
            vo.StartUserId = SecurityUtils.GetLogInEmpId();
            vo.StartUserName = SecurityUtils.GetLogInEmpName();
        }

        BusinessDataVo dataVo = null;
        _freeSql.Ado.Transaction(() => { dataVo = _buttonOperationService.ButtonsOperationTransactional(vo); });

        return dataVo;

    }

    public dynamic GetBusinessInfo(string parameters, string formCode)
    {
        var vo = _formFactory.DataFormConversion(parameters, formCode);
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);

        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"processNumber {vo.ProcessNumber}, its data does not exist!");
        }

        vo.BusinessId = bpmBusinessProcess.BusinessId;

        dynamic businessDataVo;
        if (vo.IsOutSideAccessProc == null || !vo.IsOutSideAccessProc.Value || vo.IsLowCodeFlow == 1)
        {
            var formAdaptor = _formFactory.GetFormAdaptor(vo);
            formAdaptor.OnQueryData(vo);
            businessDataVo = vo;
        }
        else
        {
            businessDataVo = vo;
        }

        // 设置业务 ID
        businessDataVo.BusinessId = bpmBusinessProcess.BusinessId;

        // 设置其他重要信息
        businessDataVo.FormCode = vo.FormCode;
        businessDataVo.ProcessNumber = vo.ProcessNumber;

        // 校验流程权限，并从业务表中获取信息
        businessDataVo.ProcessRecordInfo = _processConstantsService.ProcessInfo(bpmBusinessProcess);
        businessDataVo.ProcessKey = bpmBusinessProcess.BusinessNumber;
        businessDataVo.ProcessState = bpmBusinessProcess.ProcessState != (int)ProcessStateEnum.END_STATE &&
                                      bpmBusinessProcess.ProcessState != (int)ProcessStateEnum.REJECT_STATE;

        bool flag = businessDataVo.ProcessRecordInfo.StartUserId == SecurityUtils.GetLogInEmpIdStr();

        bool isJurisdiction = false; // TODO: 目前未实现

        // 设置操作按钮
        businessDataVo.ProcessRecordInfo.PcButtons = _configFlowButtonContantService.GetButtons(
            bpmBusinessProcess.BusinessNumber,
            businessDataVo.ProcessRecordInfo.NodeId,
            businessDataVo.ProcessRecordInfo.ViewNodeIds,
            isJurisdiction,
            flag
        );

        // 检查当前节点是否为报名节点，并设置属性
        string nodeId = businessDataVo.ProcessRecordInfo.NodeId;
        bool nodeIsSignUp = _bpmVariableSignUpService.CheckNodeIsSignUp(vo.ProcessNumber, nodeId);
        businessDataVo.IsSignUpNode = nodeIsSignUp;

        // 如果是报名节点，则添加“选择审核人”按钮
        if (nodeIsSignUp)
        {
            AddApproverButton(businessDataVo);
        }

        return businessDataVo;
    }

    private void AddApproverButton(BusinessDataVo businessDataVo)
    {
        // Set the approver button
        ProcessActionButtonVo addApproverButton = new ProcessActionButtonVo
        {
            ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_JP,
            Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_JP)
        };

        // Set add approver button on the PC
        var pcButtons = businessDataVo.ProcessRecordInfo.PcButtons;
        if (!pcButtons.TryGetValue(ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT),
                out var pcProcButtons))
        {
            pcProcButtons = new List<ProcessActionButtonVo>();
            pcButtons[ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT)] = pcProcButtons;
        }

        if (!pcProcButtons.Any(a => ConfigFlowButtonSortEnum.BUTTON_TYPE_JP.Code.Equals(a.ButtonType)))
        {
            pcProcButtons.Add(addApproverButton);
        }

    }

    public ResultAndPage<TaskMgmtVO> FindPcProcessList(PageDto pageDto, TaskMgmtVO vo)
    {
        SortedDictionary<String, SortTypeEnum> orderFieldMap = new SortedDictionary<string, SortTypeEnum>();
        Page<TaskMgmtVO> page = PageUtils.GetPageByPageDto<TaskMgmtVO>(pageDto, orderFieldMap);

        vo.ApplyUser = SecurityUtils.GetLogInEmpIdStr();

        switch (vo.Type)
        {
            // view process record
            case 1:
                // get the records that current logged in user has access right
                //todo to be implemented
                break;
            // mornitor current processes
            case 2:
                page.Records =this.ViewPcProcessList(page,vo) ;
                break;
            // recently build task
            case 3:
                if (!string.IsNullOrEmpty(vo.ProcessType)) {
                    vo.ProcessKeyList=_processNameRelevancyService.ProcessKeyList(Convert.ToInt64(vo.ProcessType));
                }
                page.Records=(this.ViewPcpNewlyBuildList(page, vo));
                break;
            // already finished tasks
            case 4:
                if (!string.IsNullOrEmpty(vo.ProcessType)) {
                    vo.ProcessKeyList=_processNameRelevancyService.ProcessKeyList(Convert.ToInt64(vo.ProcessType));
                }
                page.Records=(this.ViewPcAlreadyDoneList(page, vo));
                break;
            // running tasks
            case 5:
                if (!string.IsNullOrEmpty(vo.ProcessType)) {
                    vo.ProcessKeyList=_processNameRelevancyService.ProcessKeyList(Convert.ToInt64(vo.ProcessType));
                }
                page.Records=(this.ViewPcToDoList(page, vo));
                break;
            // my draft
            case 6:
                page.Records=(this.AllProcessList(page, vo));
                break;
            // delegated tasks
            case 7:
                //todo tobe implemented
                break;
            //for administrator to view all the processes
            case 8:
                page.Records=(this.AllProcessList(page, vo));
                break;
            //转发流程
            case 9:
                page.Records=(this.ViewPcForwardList(page,vo));
                //todo tobe implemented
                break;
        }
        if (page.Records!=null&&page.Records.Any()) {
            if (vo.Type==(ProcessTypeEnum.ENTRUST_TYPE.Code) || vo.Type==(ProcessTypeEnum.ADMIN_TYPE.Code)) {
                _bpmProcessForwardService.LoadProcessForward(SecurityUtils.GetLogInEmpId());
                _bpmProcessForwardService.LoadTask(SecurityUtils.GetLogInEmpId());
            }
            this.GetPcProcessData(page, vo.Type);
        }
        return PageUtils.GetResultAndPage(page);
    }

   private void GetPcProcessData(Page<TaskMgmtVO>page, int type)
{
    var formCodes = page.Records
        .Select(r => r.ProcessKey)
        .Where(x => !string.IsNullOrEmpty(x))
        .Distinct()
        .ToList();

    List<BpmnConf> bpmnConfs = _bpmnConfCommonService.GetBpmnConfByFormCodeBatch(formCodes);
    Dictionary<string,BpmnConf> bpmnConfMap = new Dictionary<string, BpmnConf>();

    if (bpmnConfs != null && bpmnConfs.Any())
    {
        bpmnConfMap = bpmnConfs
            .GroupBy(x => x.FormCode)
            .ToDictionary(g => g.Key, g => g.Last());

        foreach (var record in page.Records)
        {
            if (bpmnConfMap.TryGetValue(record.ProcessKey, out var bpmnConf))
            {
                record.IsOutSideProcess = bpmnConf.IsOutSideProcess == 1;
                record.IsLowCodeFlow = bpmnConf.IsLowCodeFlow == 1;
                record.ConfId = bpmnConf.Id;
            }

          
            // TODO: 实际用户信息从 DB 获取
            record.ActualName = SecurityUtils.GetLogInEmpName();

            // 设置任务状态名称
            record.TaskState = ProcessStateEnumExtensions.GetDescByCode(record.ProcessState ?? 0);

            if (type == ProcessTypeEnum.ENTRUST_TYPE.Code)
            {
                
                record.IsForward = _bpmProcessForwardService.IsForward(record.ProcessInstanceId);

                if (!string.IsNullOrEmpty(record.TaskName))
                {
                    record.IsBatchSubmit = IsOperatable(new TaskMgmtVO
                    {
                        ProcessKey = record.ProcessKey,
                        TaskName = record.TaskName,
                        Type = ProcessButtonEnum.VIEW_TYPE.Code
                    });

                    record.NodeType = ProcessNodeEnum.GetCodeByDesc(record.TaskName)??0;
                }
            }

            if (type == ProcessTypeEnum.ADMIN_TYPE.Code)
            {
                if (!string.IsNullOrEmpty(record.TaskName))
                {
                    record.NodeType = ProcessNodeEnum.GetCodeByDesc(record.TaskName)??0;
                }
            }

            if (!string.IsNullOrEmpty(record.ProcessKey))
            {
                var bpmProcessVo = _bpmProcessNameService.Get(record.ProcessKey);
                if (bpmProcessVo != null && !string.IsNullOrEmpty(bpmProcessVo.ProcessKey))
                {
                    record.ProcessTypeName = bpmProcessVo.ProcessName;
                    record.ProcessCode = bpmProcessVo.ProcessKey;
                }
            }
        }
    }
}

private bool IsOperatable(TaskMgmtVO taskMgmtVo)
{
    long count = _freeSql.Select<BpmProcessOperation>()
        .Where(a=>a.ProcessNode==taskMgmtVo.TaskName&&a.ProcessKey==taskMgmtVo.ProcessKey&&a.Type==taskMgmtVo.Type)
        .Count();
    return count <= 0;
}


List<TaskMgmtVO> ViewPcProcessList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmAfTaskInst, BpmBusinessProcess>()
            .LeftJoin((h, b) => h.ProcInstId == b.ProcInstId)
            .OrderByDescending((a, b) => a.StartTime)
            .WithTempQuery(a => new TaskMgmtVO
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessId = a.t1.ProcDefId,
                ProcessNumber = a.t2.BusinessNumber,
                UserId = a.t2.CreateUser,
                BusinessId = a.t2.BusinessId,
                Description = a.t2.Description,
                ProcessState = a.t2.ProcessState,
                RunTime = a.t1.StartTime,
                ProcessDigest = a.t2.ProcessDigest,
            })
            .Where(CommonCond(taskMgmtVO))
            .Page(basePagingInfo).ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    
    List<TaskMgmtVO> ViewPcpNewlyBuildList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO){
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmBusinessProcess,BpmAfTaskInst>()
            .LeftJoin((b,h) => h.ProcInstId == b.ProcInstId&&h.TaskDefKey=="task1418018332271"&&h.Priority==0)
            .Where((b,a)=>b.CreateUser==taskMgmtVO.ApplyUser&&b.IsDel==0)
            .WithTempQuery(a=>new TaskMgmtVO
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessId = a.t2.ProcDefId,
                UserId = a.t1.CreateUser,
                CreateTime = a.t2.StartTime,
                RunTime = a.t2.StartTime,
                BusinessId = a.t1.BusinessId,
                ProcessNumber = a.t1.BusinessNumber,
                Description = a.t1.Description,
                ProcessState = a.t1.ProcessState,
                ProcessKey = a.t1.ProcessinessKey,
                ProcessCode =a.t1.ProcessinessKey,
                TaskStype = a.t1.ProcessState,
                ProcessDigest = a.t1.ProcessDigest,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a=>a.CreateTime)
            .Page(basePagingInfo)
            .ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcAlreadyDoneList(Page<TaskMgmtVO> page,  TaskMgmtVO taskMgmtVO){
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmAfTaskInst, BpmBusinessProcess>()
            .LeftJoin((h, b) => h.ProcInstId == b.ProcInstId)
            .Where((a,b)=>a.Assignee==taskMgmtVO.ApplyUser&&b.IsDel==0&&a.EndTime!=null&&a.TaskDefKey!="task1418018332271")
            .WithTempQuery(a=>new TaskMgmtVO
            {
                ProcessInstanceId = a.t2.ProcInstId,
                ProcessKey = a.t2.ProcessinessKey,
                UserId = a.t2.CreateUser,
                BusinessId = a.t2.BusinessId,
                Description = a.t2.Description,
                TaskStype = a.t2.ProcessState,
                ProcessNumber = a.t2.BusinessNumber,
                RunTime = a.t1.EndTime,
                ProcessState = a.t2.ProcessState,
                ProcessDigest = a.t2.ProcessDigest,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a=>a.RunTime)
            .Page(basePagingInfo)
            .ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcToDoList(Page<TaskMgmtVO> page,TaskMgmtVO taskMgmtVO)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmAfTask, BpmBusinessProcess>()
            .LeftJoin((a, b) => a.ProcInstId == b.ProcInstId)
            .Where((a,b)=>a.Assignee==taskMgmtVO.ApplyUser&&b.IsDel==0)
            .WithTempQuery(a=>new TaskMgmtVO()
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessKey = a.t2.ProcessinessKey,
                UserId = a.t2.CreateUser,
                CreateTime = a.t2.CreateTime,
                BusinessId= a.t2.BusinessId,
                Description = a.t2.Description,
                ProcessNumber = a.t2.BusinessNumber,
                TaskStype = a.t2.ProcessState,
                TaskId = a.t1.Id,
                RunTime = a.t2.CreateTime,
                ProcessState = a.t2.ProcessState,
                ProcessDigest = a.t2.ProcessDigest,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a=>a.RunTime)
            .Page(basePagingInfo)
            .ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> AllProcessList(Page<TaskMgmtVO> page,TaskMgmtVO taskMgmtVO){
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        ISelect<BpmAfTask,BpmBusinessProcess> select = _freeSql
            .Select<BpmAfTask, BpmBusinessProcess>();
        if (taskMgmtVO.IncludeAllFlag == 1)
        {
            select.RightJoin((a, b) => a.ProcInstId == b.ProcInstId);
        }
        else
        {
            select.LeftJoin((a, b) => a.ProcInstId == b.ProcInstId);
        }
        List<TaskMgmtVO> taskMgmtVos =
            select
            .Where((a,b)=>b.IsDel==0)
            .WithTempQuery(a=>new TaskMgmtVO()
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessKey = a.t2.ProcessinessKey,
                UserId = a.t2.CreateUser,
                BusinessId= a.t2.BusinessId,
                Description = a.t2.Description,
                TaskStype = a.t2.ProcessState,
                ProcessNumber = a.t2.BusinessNumber,
                CreateTime = a.t2.CreateTime,
                RunTime = a.t2.CreateTime,
                ProcessState = a.t2.ProcessState,
                TaskId = a.t1.Id,
                ProcessDigest = a.t2.ProcessDigest,
                TaskOwner = a.t1.Assignee,
                TaskName = a.t1.TaskDefKey,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a=>a.RunTime)
            .Page(basePagingInfo)
            .ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcForwardList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO){
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmBusinessProcess,BpmProcessForward>()
            .LeftJoin((a,b)=>a.ProcInstId==b.ProcessInstanceId)
            .Where((a,b)=>b.ForwardUserId==taskMgmtVO.ApplyUser&&b.IsDel==0&&b.IsDel==0)
            .WithTempQuery(a=>new TaskMgmtVO()
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessKey = a.t1.ProcessinessKey,
                UserId = a.t1.CreateUser,
                CreateTime = a.t1.CreateTime,
                BusinessId = a.t1.BusinessId,
                Description = a.t1.Description,
                TaskStype = a.t1.ProcessState,
                ProcessNumber = a.t1.BusinessNumber,
                RunTime = a.t1.CreateTime,
                ProcessState = a.t1.ProcessState,
                IsRead = a.t2.IsRead,
                ProcessDigest = a.t1.ProcessDigest,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a=>a.CreateTime)
            .Page(basePagingInfo)
            .ToList();
        page.Total = (int)basePagingInfo.Count;
        return taskMgmtVos;
    }
    private Expression<Func<TaskMgmtVO, bool>> CommonCond(TaskMgmtVO paramVo)
    {
        var param = Expression.Parameter(typeof(TaskMgmtVO), "a");
        var left = Expression.Constant(1);
        var right = Expression.Constant(1);
        var body = Expression.Equal(left, right);
        var exp = Expression.Lambda<Func<TaskMgmtVO, bool>>(body, param);
        
        if (!string.IsNullOrEmpty(paramVo.Search))
        {
            exp=exp.And(a => a.Search.Contains(paramVo.Search));
        }

        if (paramVo.ApplyUserId != 0)
        {
            exp=exp.And(a => a.ApplyUserId == paramVo.ApplyUserId);
        }

        if (!string.IsNullOrEmpty(paramVo.Description))
        {
            exp=exp.And(a => a.Description.Contains(paramVo.Description));
        }

        if (!string.IsNullOrEmpty(paramVo.ProcessNumber))
        {
            exp=exp.And(a => a.ProcessNumber == paramVo.ProcessNumber);
        }

        if (paramVo.ProcessState != null)
        {
            exp=exp.And(a => a.ProcessState == paramVo.ProcessState);
        }

        if (!string.IsNullOrEmpty(paramVo.StartTime) && !string.IsNullOrEmpty(paramVo.EndTime))
        {
            DateTime start = DateTime.Parse(paramVo.StartTime);
            DateTime end = DateTime.Parse(paramVo.EndTime);
            exp=exp.And(a => a.RunTime.Value.Date.Between(start, end));
        }

        if (paramVo.ProcessKeyList != null && !paramVo.ProcessKeyList.Any())
        {
            exp=exp.And(a => paramVo.ProcessKeyList.Contains(a.ProcessKey));
        }

        if (paramVo.ProcessNumbers != null && paramVo.ProcessNumbers.Any())
        {
            exp=exp.And(a => !paramVo.ProcessNumbers.Contains(a.ProcessNumber));
        }

        if (paramVo.VersionProcessKeys != null && !paramVo.VersionProcessKeys.Any())
        {
            exp=exp.And(a => !paramVo.VersionProcessKeys.Contains(a.ProcessKey));
        }

        if (!string.IsNullOrEmpty(paramVo.ProcessDigest))
        {
            exp=exp.And(a => !a.ProcessDigest.Contains(paramVo.ProcessDigest));
        }

        return exp;
    }

    public TaskMgmtVO ProcessStatistics()
    {
        string logInEmpIdStr = SecurityUtils.GetLogInEmpIdStr();
        List<BpmAfTask> taskList = _taskService.baseRepo
            .Where(a=>a.Assignee==logInEmpIdStr)
            .ToList();
        int doneTodayProcess = _afTaskInstService.DoneTodayProcess(logInEmpIdStr);
        int doneCreateProcess = _afTaskInstService.DoneCreateProcess(logInEmpIdStr);
        TaskMgmtVO taskMgmtVo = new TaskMgmtVO()
        {
            TodoCount = taskList.Count(),
            DoneTodayCount = doneTodayProcess,
            DoneCreateCount = doneCreateProcess,
        };
        return taskMgmtVo;
    }
}

