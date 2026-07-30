using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql;
using FreeSql.Internal.Model;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Persist.repository;

public class ProcessApprovalService : IProcessApprovalService
{
    private readonly IFormFactory _formFactory;
    private readonly IButtonOperationService _buttonOperationService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IProcessConstantsService _processConstantsService;
    private readonly IConfigFlowButtonContantService _configFlowButtonContantService;
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly IFreeSql _freeSql;
    private readonly IBpmnConfCommonService _bpmnConfCommonService;
    private readonly IAFTaskService _taskService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly ILogger _logger;

    public ProcessApprovalService(
        IFormFactory formFactory,
        IButtonOperationService buttonOperationService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IProcessConstantsService processConstantsService,
        IConfigFlowButtonContantService configFlowButtonContantService,
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmProcessForwardService bpmProcessForwardService,
        IFreeSql freeSql,
        IBpmnConfCommonService bpmnConfCommonService,
        IAFTaskService taskService,
        IAfTaskInstService afTaskInstService,
        ILogger<ProcessApprovalService> logger
    )
    {
        _formFactory = formFactory;
        _buttonOperationService = buttonOperationService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _processConstantsService = processConstantsService;
        _configFlowButtonContantService = configFlowButtonContantService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _freeSql = freeSql;
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

        // 上一节点指定审批人: 当前节点贴有 appoint_next_node_approver 标签时,
        // 渲染[指定下一节点审批人]按钮. 标签从 ProcessRecordInfo.FormKey (NodeExtraInfoDTO JSON) 中读取.
        if (HasAppointNextNodeApproverLabel(businessDataVo))
        {
            AddAppointNextNodeApproverButton(businessDataVo);
        }

        // 检查当前节点是否为报名节点，并设置属性
        string nodeId = businessDataVo.ProcessRecordInfo.NodeId;
        businessDataVo.IsSignUpNode = false;

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

    /// <summary>
    /// 检查当前节点是否需要渲染[指定下一节点审批人]按钮.
    /// 从 ProcessRecordInfoVo.FormKey 读取 NodeExtraInfoDTO, 检查是否包含
    /// af_syslabel_appoint_next_node_approver 标签.
    /// 对应 Java ProcessApprovalServiceImpl.hasAppointNextNodeApproverLabel.
    /// </summary>
    private bool HasAppointNextNodeApproverLabel(BusinessDataVo businessDataVo)
    {
        try
        {
            if (businessDataVo?.ProcessRecordInfo == null)
            {
                return false;
            }
            string formKey = businessDataVo.ProcessRecordInfo.FormKey;
            if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
            {
                return false;
            }
            NodeExtraInfoDTO? extraInfoDTO = System.Text.Json.JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
            if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
            {
                return false;
            }
            return NodeLabelConstants.NodeLabelContainsAny(
                extraInfoDTO.NodeLabelVOS,
                StringConstants.AF_SYSLABEL_APPOINT_NEXT_NODE_APPROVER);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "HasAppointNextNodeApproverLabel check failed");
            return false;
        }
    }

    /// <summary>
    /// 添加[指定下一节点审批人]按钮到 PC 审批页按钮列表.
    /// 对应 Java ProcessApprovalServiceImpl.addAppointNextNodeApproverButton.
    /// </summary>
    private void AddAppointNextNodeApproverButton(BusinessDataVo businessDataVo)
    {
        ProcessActionButtonVo button = new ProcessActionButtonVo
        {
            ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_APPOINT_NEXT_NODE_APPROVER,
            Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_APPOINT_NEXT_NODE_APPROVER)
        };

        var pcButtons = businessDataVo.ProcessRecordInfo.PcButtons;
        if (pcButtons == null)
        {
            return;
        }
        if (!pcButtons.TryGetValue(ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT),
                out var pcProcButtons))
        {
            pcProcButtons = new List<ProcessActionButtonVo>();
            pcButtons[ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT)] = pcProcButtons;
        }

        int buttonTypeCode = (int)ButtonTypeEnum.BUTTON_TYPE_APPOINT_NEXT_NODE_APPROVER;
        if (pcProcButtons != null && !pcProcButtons.Any(a => buttonTypeCode.Equals(a.ButtonType)))
        {
            pcProcButtons.Add(button);
        }
    }

    /// <summary>
    /// 检查当前节点是否贴有选择条件标签.
    /// 从 ProcessRecordInfoVo.FormKey 读取 NodeExtraInfoDTO, 检查是否包含
    /// af_syslabel_pick_condition 标签.
    /// 对应 Java ProcessApprovalServiceImpl.hasPickConditionLabel.
    /// </summary>
    private bool HasPickConditionLabel(BusinessDataVo businessDataVo)
    {
        try
        {
            if (businessDataVo?.ProcessRecordInfo == null)
            {
                return false;
            }
            string formKey = businessDataVo.ProcessRecordInfo.FormKey;
            if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
            {
                return false;
            }
            NodeExtraInfoDTO? extraInfoDTO = System.Text.Json.JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
            if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
            {
                return false;
            }
            return NodeLabelConstants.NodeLabelContainsAny(
                extraInfoDTO.NodeLabelVOS,
                StringConstants.AF_SYSLABEL_PICK_CONDITION);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "HasPickConditionLabel check failed");
            return false;
        }
    }

    /// <summary>
    /// 添加[选择分支]按钮并查询可选分支列表.
    /// 查找当前审批人节点下级的动态条件网关,再查网关下级的条件节点(排除默认条件).
    /// 对应 Java ProcessApprovalServiceImpl.addPickConditionButtonAndBranches.
    /// </summary>
    private void AddPickConditionButtonAndBranches(BusinessDataVo businessDataVo, BpmBusinessProcess bpmBusinessProcess)
    {
        // 添加选择分支按钮
        ProcessActionButtonVo button = new ProcessActionButtonVo
        {
            ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_PICK_CONDITION,
            Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_PICK_CONDITION)
        };

        var pcButtons = businessDataVo.ProcessRecordInfo.PcButtons;
        if (pcButtons == null)
        {
            return;
        }
        if (!pcButtons.TryGetValue(ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT),
                out var pcProcButtons))
        {
            pcProcButtons = new List<ProcessActionButtonVo>();
            pcButtons[ButtonPageTypeEnumExtensions.GetName(ButtonPageTypeEnum.AUDIT)] = pcProcButtons;
        }

        int buttonTypeCode = (int)ButtonTypeEnum.BUTTON_TYPE_PICK_CONDITION;
        if (pcProcButtons != null && !pcProcButtons.Any(a => buttonTypeCode.Equals(a.ButtonType)))
        {
            pcProcButtons.Add(button);
        }

        // 查询可选分支列表
        try
        {
            string elementId = businessDataVo.ProcessRecordInfo.NodeId;
            string processKey = bpmBusinessProcess.ProcessinessKey;

            // 获取流程配置confId
            var bpmnConf = _bpmnConfCommonService.GetBpmnConfByFormCode(processKey);
            if (bpmnConf == null || bpmnConf.Id == 0)
            {
                return;
            }
            long confId = bpmnConf.Id;

            // elementId(taskDefKey)转换为bpmn_node表的node_id(UUID):
            // 先通过BpmVariableMultiplayer拿到bpmn_node主键id,再查bpmn_node获取node_id
            string currentNodeId = null;
            var multiplayer = _freeSql.Select<BpmVariableMultiplayer, BpmVariable>()
                .InnerJoin((a, b) => a.VariableId == b.Id)
                .Where((a, b) => a.ElementId == elementId && b.ProcessNum == bpmBusinessProcess.BusinessNumber)
                .First();
            if (multiplayer != null && !string.IsNullOrEmpty(multiplayer.NodeId))
            {
                var currentNode = _freeSql.Select<BpmnNode>()
                    .Where(a => a.Id == long.Parse(multiplayer.NodeId))
                    .First();
                if (currentNode != null)
                {
                    currentNodeId = currentNode.NodeId;
                }
            }
            if (currentNodeId == null) return;

            // 查找当前审批人节点下级的动态条件网关
            var gateways = _freeSql.Select<BpmnNode>()
                .Where(a => a.ConfId == confId
                    && a.NodeFrom == currentNodeId
                    && a.IsDynamicCondition == true
                    && a.IsDel == 0)
                .ToList();

            if (gateways == null || gateways.Count == 0)
            {
                return;
            }

            string gatewayNodeId = gateways[0].NodeId;

            // 查找网关下级的条件节点(nodeType=3)
            var conditionNodes = _freeSql.Select<BpmnNode>()
                .Where(a => a.ConfId == confId
                    && a.NodeFrom == gatewayNodeId
                    && a.NodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITIONS
                    && a.IsDel == 0)
                .ToList();

            if (conditionNodes == null || conditionNodes.Count == 0)
            {
                return;
            }

            // 过滤默认条件节点,构建分支列表
            var branches = new List<PickConditionBranchVo>();
            foreach (var node in conditionNodes)
            {
                if (!IsDefaultConditionNode(node))
                {
                    branches.Add(new PickConditionBranchVo
                    {
                        Id = node.NodeId,
                        Name = node.NodeName ?? node.NodeId
                    });
                }
            }

            if (branches.Count > 0)
            {
                businessDataVo.PickConditionBranches = branches;
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "AddPickConditionButtonAndBranches query failed");
        }
    }

    /// <summary>
    /// 判断条件节点是否为默认条件.
    /// 解析 nodeConfigJson.conditionsConf.conditionGroups[0].isDefault.
    /// </summary>
    private bool IsDefaultConditionNode(BpmnNode node)
    {
        try
        {
            if (string.IsNullOrEmpty(node.NodeConfigJson))
            {
                return false;
            }
            var configJson = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            var groups = configJson?.ConditionsConf?.ConditionGroups;
            if (groups == null || groups.Count == 0)
            {
                return false;
            }
            return groups[0].IsDefault == 1;
        }
        catch
        {
            return false;
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
                page.Records=(this.ViewPcpNewlyBuildList(page, vo));
                break;
            // already finished tasks
            case 4:
                page.Records=(this.ViewPcAlreadyDoneList(page, vo));
                break;
            // running tasks
            case 5:
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
                // Read process name from t_bpmn_conf.BpmnName (migrated from bpm_process_name)
                if (bpmnConfMap.TryGetValue(record.ProcessKey, out var conf))
                {
                    record.ProcessTypeName = conf.BpmnName;
                    record.ProcessCode = conf.FormCode;
                }
            }
        }
    }
}

private bool IsOperatable(TaskMgmtVO taskMgmtVo)
    {
        // Read operationTypes from node_config_json (migrated from bpm_process_operation)
        if (string.IsNullOrEmpty(taskMgmtVo.ProcessKey) || string.IsNullOrEmpty(taskMgmtVo.TaskName))
        {
            return true;
        }

        var bpmnConf = _bpmnConfCommonService.GetBpmnConfByFormCode(taskMgmtVo.ProcessKey);
        if (bpmnConf == null || bpmnConf.Id == 0)
        {
            return true;
        }

        var nodes = _freeSql.Select<BpmnNode>()
            .Where(a => a.ConfId == bpmnConf.Id && a.NodeId == taskMgmtVo.TaskName && a.IsDel == 0)
            .ToList();

        if (nodes == null || nodes.Count == 0)
        {
            return true;
        }

        var node = nodes[0];
        var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
        if (nodeConfig?.ButtonSignConf?.OperationTypes == null || nodeConfig.ButtonSignConf.OperationTypes.Count == 0)
        {
            return true;
        }

        return !nodeConfig.ButtonSignConf.OperationTypes.Contains(taskMgmtVo.Type);
    }


List<TaskMgmtVO> ViewPcProcessList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
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
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
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
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
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
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmAfTask, BpmBusinessProcess>()
            .LeftJoin((a, b) => a.ProcInstId == b.ProcInstId)
            .Where((a,b)=>a.Assignee==taskMgmtVO.ApplyUser&&b.IsDel==0)
            .WithTempQuery(a=>new TaskMgmtVO()
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessKey = a.t2.ProcessinessKey,
                UserId = a.t2.CreateUser,
                UserName = a.t2.UserName,
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
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
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
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
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
    List<TaskMgmtVO> BackToModifyList(Page<TaskMgmtVO> page,  TaskMgmtVO taskMgmtVO){
        BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
        List<TaskMgmtVO> taskMgmtVos = _freeSql
            .Select<BpmAfTask, BpmVerifyInfo, BpmBusinessProcess>()
            .InnerJoin((t, c, b) => c.RunInfoId == t.ProcInstId)
            .InnerJoin((t, c, b) => b.ProcInstId == t.ProcInstId)
            .Where((t, c, b) =>
                t.TaskDefKey == "task1418018332271" && c.VerifyStatus == 8 && c.IsDel == 0 && b.IsDel == 0)
            .WithTempQuery(a => new TaskMgmtVO()
            {
                ProcessInstanceId = a.t1.ProcInstId,
                ProcessKey = a.t3.ProcessinessKey,
                UserId = a.t3.CreateUser,
                BusinessId= a.t3.BusinessId,
                Description = a.t3.Description,
                TaskStype = a.t3.ProcessState,
                ProcessNumber = a.t3.BusinessNumber,
                CreateTime = a.t3.CreateTime,
                RunTime = a.t3.CreateTime,
                ProcessState = a.t3.ProcessState,
                TaskId = a.t1.Id,
                ProcessDigest = a.t3.ProcessDigest,
                TaskOwner = a.t1.Assignee,
                TaskName = a.t1.TaskDefKey,
            })
            .Where(CommonCond(taskMgmtVO))
            .OrderByDescending(a => a.CreateTime)
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
            exp=LambadaExpressionExtensions.And(exp, a => a.Search.Contains(paramVo.Search));
        }

        if (paramVo.ApplyUserId != 0)
        {
            exp=LambadaExpressionExtensions.And(exp, a => a.ApplyUserId == paramVo.ApplyUserId);
        }

        if (!string.IsNullOrEmpty(paramVo.Description))
        {
            exp=LambadaExpressionExtensions.And(exp, a => a.Description.Contains(paramVo.Description));
        }

        if (!string.IsNullOrEmpty(paramVo.ProcessNumber))
        {
            exp=LambadaExpressionExtensions.And(exp, a => a.ProcessNumber == paramVo.ProcessNumber);
        }

        if (paramVo.ProcessState != null)
        {
            exp=LambadaExpressionExtensions.And(exp, a => a.ProcessState == paramVo.ProcessState);
        }

        if (!string.IsNullOrEmpty(paramVo.StartTime) && !string.IsNullOrEmpty(paramVo.EndTime))
        {
            DateTime start = DateTime.Parse(paramVo.StartTime);
            DateTime end = DateTime.Parse(paramVo.EndTime);
            exp=LambadaExpressionExtensions.And(exp, a => a.RunTime.Value.Date.Between(start, end));
        }

        if (paramVo.ProcessKeyList != null && !paramVo.ProcessKeyList.Any())
        {
            exp=LambadaExpressionExtensions.And(exp, a => paramVo.ProcessKeyList.Contains(a.ProcessKey));
        }

        if (paramVo.ProcessNumbers != null && paramVo.ProcessNumbers.Any())
        {
            exp=LambadaExpressionExtensions.And(exp, a => !paramVo.ProcessNumbers.Contains(a.ProcessNumber));
        }

        if (paramVo.VersionProcessKeys != null && !paramVo.VersionProcessKeys.Any())
        {
            exp=LambadaExpressionExtensions.And(exp, a => !paramVo.VersionProcessKeys.Contains(a.ProcessKey));
        }

        if (!string.IsNullOrEmpty(paramVo.ProcessDigest))
        {
            exp=LambadaExpressionExtensions.And(exp, a => !a.ProcessDigest.Contains(paramVo.ProcessDigest));
        }

        return exp;
    }

    public TaskMgmtVO ProcessStatistics()
    {
        string logInEmpIdStr = SecurityUtils.GetLogInEmpIdStr();
        List<BpmAfTask> taskList = _taskService._repository.Find(a => a.Assignee == logInEmpIdStr);
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

