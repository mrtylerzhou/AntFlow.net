using System.Linq.Expressions;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class ProcessApprovalService: IProcessApprovalService
{
    private readonly IFormFactory _formFactory;
    private readonly IButtonOperationService _buttonOperationService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmVariableSignUpService _bpmVariableSignUpService;
    private readonly IProcessConstantsService _processConstantsService;
    private readonly IConfigFlowButtonContantService _configFlowButtonContantService;
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmProcessNameRelevancyService _processNameRelevancyService;
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly ISqlSugarClient _sqlSugar;
    private readonly IBpmProcessNameService _bpmProcessNameService;
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IAFTaskService _taskService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly ILogger _logger;

    public ProcessApprovalService(
        IFormFactory formFactory,
        IButtonOperationService buttonOperationService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmVariableSignUpService bpmVariableSignUpService,
        IProcessConstantsService processConstantsService,
        IConfigFlowButtonContantService configFlowButtonContantService,
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmProcessNameRelevancyService processNameRelevancyService,
        IBpmProcessForwardService bpmProcessForwardService,
        ISqlSugarClient sqlSugar,
        IBpmProcessNameService bpmProcessNameService,
        IBpmnConfCommonService bpmnConfCommonService,
        IAFTaskService taskService,
        IAfTaskInstService afTaskInstService,
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
        _sqlSugar = sqlSugar;
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
        _sqlSugar.Ado.BeginTran();
        try {
            dataVo = _buttonOperationService.ButtonsOperationTransactional(vo);
            _sqlSugar.Ado.CommitTran();
        } catch {
            _sqlSugar.Ado.RollbackTran();
            throw;
        }

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

        BusinessDataVo businessDataVo;
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

        if ((vo.IsOutSideAccessProc == null || !vo.IsOutSideAccessProc.Value) && vo.IsLowCodeFlow == 1)
        {
            UDLFApplyVo udlfApplyVo = (UDLFApplyVo)vo;
            List<LFFieldControlVO> lfFieldControlVOs = vo.ProcessRecordInfo.LfFieldControlVOs;
            Dictionary<string, object> lfFields = udlfApplyVo.LfFields;
            if (!lfFields.IsEmpty())
            {
                foreach (var item in lfFields)
                {
                    if (lfFieldControlVOs.IsEmpty())
                    {
                        continue;
                    }
                    LFFieldControlVO? lfFieldControlVo = lfFieldControlVOs.FirstOrDefault(a=>a.FieldId==item.Key);
                    if (lfFieldControlVo != null &&
                        StringConstants.HIDDEN_FIELD_PERMISSION.Equals(lfFieldControlVo.Perm))
                    {
                        lfFields[item.Key] = default;
                    }
                }
            }
        }
        dynamic d = businessDataVo;
        return d;
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
               page.Records=(this.BackToModifyList(page, vo));
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
    long count = _sqlSugar.Queryable<BpmProcessOperation>()
        .Where(a=>a.ProcessNode==taskMgmtVo.TaskName&&a.ProcessKey==taskMgmtVo.ProcessKey&&a.Type==taskMgmtVo.Type)
        .Count();
    return count <= 0;
}


List<TaskMgmtVO> ViewPcProcessList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO)
    {
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmAfTaskInst, BpmBusinessProcess>((h, b) => new JoinQueryInfos(
                JoinType.Left, h.ProcInstId == b.ProcInstId
            ))
            .OrderByDescending((h, b) => h.StartTime)
            .Select((h, b) => new TaskMgmtVO
            {
                ProcessInstanceId = h.ProcInstId,
                ProcessId = h.ProcDefId,
                ProcessNumber = b.BusinessNumber,
                UserId = b.CreateUser,
                BusinessId = b.BusinessId,
                Description = b.Description,
                ProcessState = b.ProcessState,
                RunTime = h.StartTime,
                ProcessDigest = b.ProcessDigest,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    
    List<TaskMgmtVO> ViewPcpNewlyBuildList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO){
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmBusinessProcess, BpmAfTaskInst>((b, h) => new JoinQueryInfos(
                JoinType.Left, h.ProcInstId == b.ProcInstId && h.TaskDefKey == "task1418018332271" && h.Priority == 0
            ))
            .Where((b, h) => b.CreateUser == taskMgmtVO.ApplyUser && b.IsDel == 0)
            .Select((b, h) => new TaskMgmtVO
            {
                ProcessInstanceId = b.ProcInstId,
                ProcessId = h.ProcDefId,
                UserId = b.CreateUser,
                CreateTime = h.StartTime,
                RunTime = h.StartTime,
                BusinessId = b.BusinessId,
                ProcessNumber = b.BusinessNumber,
                Description = b.Description,
                ProcessState = b.ProcessState,
                ProcessKey = b.ProcessinessKey,
                ProcessCode = b.ProcessinessKey,
                TaskStype = b.ProcessState,
                ProcessDigest = b.ProcessDigest,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending( a=> a.CreateTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcAlreadyDoneList(Page<TaskMgmtVO> page,  TaskMgmtVO taskMgmtVO){
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmAfTaskInst, BpmBusinessProcess>((h, b) => new JoinQueryInfos(
                JoinType.Left, h.ProcInstId == b.ProcInstId
            ))
            .Where((h, b) => h.Assignee == taskMgmtVO.ApplyUser && b.IsDel == 0 && h.EndTime != null && h.TaskDefKey != "task1418018332271")
            .Select((h, b) => new TaskMgmtVO
            {
                ProcessInstanceId = b.ProcInstId,
                ProcessKey = b.ProcessinessKey,
                UserId = b.CreateUser,
                BusinessId = b.BusinessId,
                Description = b.Description,
                TaskStype = b.ProcessState,
                ProcessNumber = b.BusinessNumber,
                RunTime = h.EndTime,
                ProcessState = b.ProcessState,
                ProcessDigest = b.ProcessDigest,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.RunTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcToDoList(Page<TaskMgmtVO> page,TaskMgmtVO taskMgmtVO)
    {
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmAfTask, BpmBusinessProcess>((a, b) => new JoinQueryInfos(
                JoinType.Left, a.ProcInstId == b.ProcInstId
            ))
            .Where((a, b) => a.Assignee == taskMgmtVO.ApplyUser && b.IsDel == 0)
            .Select((a, b) => new TaskMgmtVO
            {
                ProcessInstanceId = a.ProcInstId,
                ProcessKey = b.ProcessinessKey,
                UserId = b.CreateUser,
                CreateTime = b.CreateTime,
                BusinessId = b.BusinessId,
                Description = b.Description,
                ProcessNumber = b.BusinessNumber,
                TaskStype = b.ProcessState,
                TaskId = a.Id,
                RunTime = b.CreateTime,
                ProcessState = b.ProcessState,
                ProcessDigest = b.ProcessDigest,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.RunTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> AllProcessList(Page<TaskMgmtVO> page,TaskMgmtVO taskMgmtVO){
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        ISugarQueryable<BpmAfTask, BpmBusinessProcess> queryable;
        if (taskMgmtVO.IncludeAllFlag == 1)
        {
            queryable = _sqlSugar
                .Queryable<BpmAfTask, BpmBusinessProcess>((a, b) => new JoinQueryInfos(
                    JoinType.Right, a.ProcInstId == b.ProcInstId
                ));
        }
        else
        {
            queryable = _sqlSugar
                .Queryable<BpmAfTask, BpmBusinessProcess>((a, b) => new JoinQueryInfos(
                    JoinType.Left, a.ProcInstId == b.ProcInstId
                ));
        }
        List<TaskMgmtVO> taskMgmtVos =
            queryable
            .Where((a, b) => b.IsDel == 0)
            .Select((a, b) => new TaskMgmtVO
            {
                ProcessInstanceId = a.ProcInstId,
                ProcessKey = b.ProcessinessKey,
                UserId = b.CreateUser,
                BusinessId = b.BusinessId,
                Description = b.Description,
                TaskStype = b.ProcessState,
                ProcessNumber = b.BusinessNumber,
                CreateTime = b.CreateTime,
                RunTime = b.CreateTime,
                ProcessState = b.ProcessState,
                TaskId = a.Id,
                ProcessDigest = b.ProcessDigest,
                TaskOwner = a.Assignee,
                TaskName = a.TaskDefKey,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.RunTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> ViewPcForwardList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO){
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmBusinessProcess, BpmProcessForward>((a, b) => new JoinQueryInfos(
                JoinType.Left, a.ProcInstId == b.ProcessInstanceId
            ))
            .Where((a, b) => b.ForwardUserId == taskMgmtVO.ApplyUser && b.IsDel == 0 && a.IsDel == 0)
            .Select((a, b) => new TaskMgmtVO
            {
                ProcessInstanceId = a.ProcInstId,
                ProcessKey = a.ProcessinessKey,
                UserId = a.CreateUser,
                CreateTime = a.CreateTime,
                BusinessId = a.BusinessId,
                Description = a.Description,
                TaskStype = a.ProcessState,
                ProcessNumber = a.BusinessNumber,
                RunTime = a.CreateTime,
                ProcessState = a.ProcessState,
                IsRead = b.IsRead,
                ProcessDigest = a.ProcessDigest,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
        page.Total = totalCount;
        return taskMgmtVos;
    }
    List<TaskMgmtVO> BackToModifyList(Page<TaskMgmtVO> page,  TaskMgmtVO taskMgmtVO){
        PagingInfo basePagingInfo = page.ToPagingInfo();
        int totalCount = 0;
        List<TaskMgmtVO> taskMgmtVos = _sqlSugar
            .Queryable<BpmAfTask, BpmVerifyInfo, BpmBusinessProcess>(
                (t, c, b) => new JoinQueryInfos(
                    JoinType.Inner, c.RunInfoId == t.ProcInstId,
                    JoinType.Inner, b.ProcInstId == t.ProcInstId
                )
            )
            .Where((t, c, b) =>
                t.TaskDefKey == "task1418018332271" && c.VerifyStatus == 8 && c.IsDel == 0 && b.IsDel == 0)
            .Select((t, c, b) => new TaskMgmtVO
            {
                ProcessInstanceId = t.ProcInstId,
                ProcessKey = b.ProcessinessKey,
                UserId = b.CreateUser,
                BusinessId = b.BusinessId,
                Description = b.Description,
                TaskStype = b.ProcessState,
                ProcessNumber = b.BusinessNumber,
                CreateTime = b.CreateTime,
                RunTime = b.CreateTime,
                ProcessState = b.ProcessState,
                TaskId = t.Id,
                ProcessDigest = b.ProcessDigest,
                TaskOwner = t.Assignee,
                TaskName = t.TaskDefKey,
            })
            .MergeTable()
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);

        page.Total = totalCount;
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
            DateTime end = DateTime.Parse(paramVo.EndTime).AddDays(1).AddSeconds(-1);
            exp=exp.And(a => a.RunTime.Value >= start && a.RunTime.Value <= end);
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
        List<BpmAfTask> taskList = _taskService._repository
            .Find(a=>a.Assignee==logInEmpIdStr)
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

