using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.factory;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.factory;
using AntFlowCore.Engine.service.processor;
using AntFlowCore.Persist.api.interf.repository;
using System.Text.Json;

namespace AntFlowCore.Engine.service.biz;

public class BpmnConfBizService : IBpmnConfBizService
{
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmnNodeToService _bpmnNodeToService;
    private readonly IAdaptorFactory _adaptorFactory;
    private readonly IOutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;
    private readonly IOutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly IBpmProcessAppApplicationService _bpmProcessAppApplicationService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly IInformationTemplateService _informationTemplateService;
    private readonly ITaskMgmtService _taskMgmtService;
    private readonly IFormFactory _formFactory;
    private readonly IBpmnStartFormatFactory _bpmnStartFormatFactory;

    public BpmnConfBizService(
        IBpmnConfService bpmnConfService,
        IBpmnNodeService bpmnNodeService,
        IBpmnNodeToService bpmnNodeToService,
        IAdaptorFactory adaptorFactory,
        IOutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService,
        IOutSideBpmBusinessPartyService outSideBpmBusinessPartyService,
        IBpmProcessAppApplicationService bpmProcessAppApplicationService,
        IBpmnEmployeeInfoProviderService employeeInfoProviderService,
        IInformationTemplateService informationTemplateService,
        ITaskMgmtService taskMgmtService,
        IFormFactory formFactory,
        IBpmnStartFormatFactory bpmnStartFormatFactory
        )
    {
        _bpmnConfService = bpmnConfService;
        _bpmnNodeService = bpmnNodeService;
        _bpmnNodeToService = bpmnNodeToService;
        _adaptorFactory = adaptorFactory;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
        _bpmProcessAppApplicationService = bpmProcessAppApplicationService;
        _employeeInfoProviderService = employeeInfoProviderService;
        _informationTemplateService = informationTemplateService;
        _taskMgmtService = taskMgmtService;
        _formFactory = formFactory;
        _bpmnStartFormatFactory = bpmnStartFormatFactory;
    }
    private const String LinkMark = "_";

    public void Edit(BpmnConfVo bpmnConfVo)
    {
        String bpmnName = bpmnConfVo.BpmnName;
        String bpmnCode = GetBpmnCode(bpmnName);
        String formCode = bpmnConfVo.FormCode;
        //todo 注意查看映射效果
        bpmnConfVo.ConfConfigJson = JsonConfUtil.ToConfConfigJson(BpmnConfConfigHolder.BuildConfConfig(bpmnConfVo));
        BpmnConf bpmnConf = bpmnConfVo.MapToEntity();
       
        bpmnConf.BpmnCode=bpmnCode;
        bpmnConf.FormCode = formCode;
        bpmnConf.CreateUser=SecurityUtils.GetLogInEmpNameSafe();
        bpmnConf.CreateTime=DateTime.Now;
        bpmnConf.UpdateUser=SecurityUtils.GetLogInEmpNameSafe();
        bpmnConf.UpdateTime=DateTime.Now;
        bpmnConf.Remark=bpmnConfVo.Remark??"";
        bpmnConf.TenantId = MultiTenantUtil.GetCurrentTenantId();
        _bpmnConfService._repository.Add(bpmnConf);
        //notice template - service removed, now handled via conf_config_json
        long confId = bpmnConf.Id;
        if(confId.IsNullOrZero()){
            throw new AFBizException($"conf id for formcode:{formCode} can not be null");
        }
        bpmnConfVo.Id=confId;
        // view page buttons and template editing - services removed, now handled via conf_config_json
        int? isOutSideProcess = bpmnConfVo.IsOutSideProcess;
        int? isLowCodeFlow = bpmnConfVo.IsLowCodeFlow;
        
        ProcessorFactory.ExecutePreWriteProcessors(bpmnConfVo);
        
        List<BpmnNodeVo> confNodes = bpmnConfVo.Nodes;
        // 构建nodeId->BpmnNodeVo映射,供选择条件贴标签时查找子节点
        Dictionary<string, BpmnNodeVo> nodeIdMap = new Dictionary<string, BpmnNodeVo>();
        foreach (var n in confNodes)
        {
            if (!string.IsNullOrEmpty(n.NodeId) && !nodeIdMap.ContainsKey(n.NodeId))
            {
                nodeIdMap[n.NodeId] = n;
            }
        }
        int hasStartUserChooseModules=0;
        int hasCopy=0;
        int hasLastNodeCopy=0;
        int hasChooseFromLowCodeform=0;
        foreach (BpmnNodeVo bpmnNodeVo in confNodes)
        {
            AfNodeUtils.NodeSpecialProcess(bpmnNodeVo);
            if (bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_APPROVER
                && bpmnNodeVo.NodeProperty==null) {
                throw new AFBizException("apporver node has no property,can not be saved！");
            }
            
            if((int)NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE==bpmnNodeVo.NodeProperty){
                hasStartUserChooseModules=BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES.Code;
            }
            if((int)NodeTypeEnum.NODE_TYPE_COPY==(bpmnNodeVo.NodeType))
            {
                hasCopy = BpmnConfFlagsEnum.HAS_COPY.Code;
            }
            if((int)NodePropertyEnum.NODE_PROPERTY_FORM_RELATED==bpmnNodeVo.NodeProperty){
                hasChooseFromLowCodeform = BpmnConfFlagsEnum.HAS_FORM_RELATED_ASSIGNEES.Code;
            }
            
            // NodeSpecialProcess 将 nodeType=8/12/13 转为 nodeType=4 并置标记位,
            // 此处根据标记位将对应标签写入 LabelList,最终持久化到 buttonSignConf.labels
            if ((int)NodeTypeEnum.NODE_TYPE_APPROVER == bpmnNodeVo.NodeType)
            {
                BpmnNodeLabelVO nodeLabelVO = null;
                if (bpmnNodeVo.IsCarbonCopyNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.CopyNodeV2;
                }
                else if (bpmnNodeVo.IsAutomaticNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.AutomaticNode;
                }
                else if (bpmnNodeVo.IsConditionFinishNode == true)
                {
                    // 条件完成节点:条件推进(nodeType=12)子类型,目标自动算最后一个审批人,运行时复用条件推进处理器
                    // 必须先于 IsConditionApproveNode / IsConditionAdvanceNode 判断(条件完成节点三者都可能为 true)
                    nodeLabelVO = NodeLabelConstants.ConditionFinishNode;
                }
                else if (bpmnNodeVo.IsConditionAdvanceNode == true)
                {
                    // 条件推进节点:条件审批(nodeType=12)子类型,自动勾选推进按钮(42,别名同意),满足条件自动推进到固定目标
                    // 必须先于 IsConditionApproveNode 判断(条件推进节点两者都为 true),否则会误贴 condition_approve_node 标签导致运行时走条件审批
                    nodeLabelVO = NodeLabelConstants.ConditionAdvanceNode;
                }
                else if (bpmnNodeVo.IsConditionApproveNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.ConditionApproveNode;
                }
                else if (bpmnNodeVo.IsConditionCopyNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.ConditionCopyNode;
                }
                else if (bpmnNodeVo.IsAssistNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.AssistNode;
                }
                else if (bpmnNodeVo.IsAutoAdvanceNode == true)
                {
                    nodeLabelVO = NodeLabelConstants.AutoAdvanceNode;
                }
                if (nodeLabelVO != null)
                {
                    bpmnNodeVo.SetOrAddLabelList(nodeLabelVO);
                }

                // 自动完成节点:自动推进(18)子类型,额外贴 auto_complete_node 标签(仅前端反显区分+颜色区分,运行时复用 auto_advance_node 处理器)
                if (bpmnNodeVo.IsAutoCompleteNode == true)
                {
                    bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.AutoCompleteNode);
                }

                // 选择条件:验证子节点包含动态条件网关,满足则贴标签
                if (bpmnNodeVo.IsPickCondition == true)
                {
                    bool hasDynamicGatewayChild = bpmnNodeVo.NodeTo != null && bpmnNodeVo.NodeTo.Any(childId =>
                    {
                        nodeIdMap.TryGetValue(childId, out var child);
                        return child != null && (int)NodeTypeEnum.NODE_TYPE_GATEWAY == child.NodeType;
                    });
                    if (hasDynamicGatewayChild)
                    {
                        bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.PickCondition);
                    }
                }
            }

            bpmnNodeVo.IsOutSideProcess=isOutSideProcess;
            bpmnNodeVo.IsLowCodeFlow=isLowCodeFlow;
            bpmnNodeVo.ConfId=confId;
            bpmnNodeVo.FormCode = formCode;

            //if the node has no property,the node property default is "1-no property"
            bpmnNodeVo.NodeProperty=bpmnNodeVo.NodeProperty ?? 1;
            EditNodeExtraFlags(bpmnNodeVo);
            PrepareNodeConditionsForJson(bpmnNodeVo);
            // Build node-level JSON config from VO data
            BpmnNodeConfigHolder.SetButtonSignConf(bpmnNodeVo);
            // 条件审批/条件抄送节点:持久化 autoNodeConf 条件配置
            BpmnNodeConfigHolder.SetAutoNodeConf(bpmnNodeVo);
            BpmnNodeConfigHolder.SetTemplateConf(bpmnNodeVo);
            // Populate formdataId on LF field control VOs (comes from conf level, not frontend)
            var lfFormDataId = bpmnConfVo.LfFormDataId;
            if (lfFormDataId != null && bpmnNodeVo.LfFieldControlVOs != null && bpmnNodeVo.LfFieldControlVOs.Count > 0)
            {
                foreach (var fc in bpmnNodeVo.LfFieldControlVOs)
                {
                    fc.FormdataId = lfFormDataId.Value;
                }
            }
            BpmnNodeConfigHolder.SetLowCodeConf(bpmnNodeVo);
            BpmnNode bpmnNode = bpmnNodeVo.MapToEntity();
            bpmnNode.ConfId=confId;
            bpmnNode.CreateTime=DateTime.Now;
            bpmnNode.CreateUser=SecurityUtils.GetLogInEmpNameSafe();
            bpmnNode.Remark ??= "";
            bpmnNode.TenantId = MultiTenantUtil.GetCurrentTenantId();
            BpmnNode node = _bpmnNodeService._repository.Add(bpmnNode);
            long bpmnNodeId = bpmnNode.Id;
            if(bpmnNodeId.IsNullOrZero()){
                throw new AFBizException("can not get bpmn node id!");
            }
            bpmnNodeVo.Id = bpmnNodeId;

            //edit node to
            _bpmnNodeToService.EditNodeTo(bpmnNodeVo, bpmnNodeId);

            // Call the appropriate SetXxxConf based on node property
            BuildNodeConfigJsonFromVo(bpmnNodeVo);

            // Serialize node config JSON to DB
            string? nodeConfigJsonStr = bpmnNodeVo.SerializeNodeConfigJson();
            if (nodeConfigJsonStr != null)
            {
                bpmnNode.NodeConfigJson=nodeConfigJsonStr;
                _bpmnNodeService._repository.Update(bpmnNode);
            }
            
            if((int)NodeTypeEnum.NODE_TYPE_COPY==bpmnNodeVo.NodeType&&bpmnNodeVo.NodeTo!=null&&bpmnNodeVo.NodeTo.Any()){
                hasLastNodeCopy=BpmnConfFlagsEnum.HAS_LAST_NODE_COPY.Code;
            }
        }

        int extraFlags = bpmnConfVo.ExtraFlags??0;
        int currentFlags=hasStartUserChooseModules|hasCopy|hasLastNodeCopy|hasChooseFromLowCodeform;
        if(currentFlags>0){
            int binariedOr = BpmnConfFlagsEnum.BinaryOr(extraFlags, currentFlags);
            bpmnConfVo.ExtraFlags=binariedOr;
        }
        if (bpmnConfVo.ExtraFlags!=null) {
            _bpmnNodeService.UpdateConfExtraFlags(confId, bpmnConfVo.ExtraFlags);
            
        }
        ProcessorFactory.ExecutePostProcessors(bpmnConfVo);
    }

    public ResultAndPage<BpmnConfVo> SelectPage(PageDto pageDto, BpmnConfVo vo)
    {
        Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
        List<BpmnConfVo> bpmnConfVos = _bpmnConfService.SelectPageList(page,vo);

        if (bpmnConfVos == null || !bpmnConfVos.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }
    
        if (vo.IsOutSideProcess == 1)
        {
            List<BpmProcessAppApplication> bizAppList = _bpmProcessAppApplicationService.SelectApplicationList();
            var bizAppMap = bizAppList.ToDictionary(p => p.ProcessKey, p => p.Title);
        
            foreach (var record in bpmnConfVos)
            {
                if (record.IsOutSideProcess == 1)
                {
                    record.FormCodeDisplayName = bizAppMap.GetValueOrDefault(record.FormCode);
                }
            }
        }
    
        if (vo.IsOutSideProcess == 0)
        {
            List<DIYProcessInfoDTO> diyFormCodeList = _taskMgmtService.ViewProcessInfo();
            var diyFormCodes = diyFormCodeList.ToDictionary(p => p.Key, p => p.Value);
        
            foreach (var record in bpmnConfVos)
            {
                if(record.IsLowCodeFlow.IsNullOrZero() && record.IsOutSideProcess.IsNullOrZero()) 
                {
                    record.FormCodeDisplayName = diyFormCodes.GetValueOrDefault(record.FormCode);
                }
            }
        }
    
        page.Records = bpmnConfVos.Select(o =>
        {
            o.DeduplicationTypeName = DeduplicationTypeEnumExtensions.GetDescByCode(o.DeduplicationType.Value);
            return o;
        }).ToList();
    
        return PageUtils.GetResultAndPage(page);
    }

    private void BuildNodeConfigJsonFromVo(BpmnNodeVo bpmnNodeVo)
    {
        int? nodeProperty = bpmnNodeVo.NodeProperty;
        int? nodeType = bpmnNodeVo.NodeType;

        // Node property-based adaptors
        if (nodeProperty != null)
        {
            if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL)
            {
                BpmnNodeConfigHolder.SetPersonnelConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_ROLE)
            {
                BpmnNodeConfigHolder.SetRoleConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_LOOP)
            {
                BpmnNodeConfigHolder.SetLoopConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_LEVEL)
            {
                BpmnNodeConfigHolder.SetAssignLevelConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_HRBP)
            {
                BpmnNodeConfigHolder.SetHrbpConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE)
            {
                BpmnNodeConfigHolder.SetCustomizeConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS)
            {
                BpmnNodeConfigHolder.SetOutSideAccessConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE)
            {
                BpmnNodeConfigHolder.SetBusinessTableConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_ZDY_RULES)
            {
                BpmnNodeConfigHolder.SetUdrConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_FORM_RELATED)
            {
                BpmnNodeConfigHolder.SetFormRelatedUserConf(bpmnNodeVo);
            }
            else if (nodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_PREV_NODE_RELATED)
            {
                BpmnNodeConfigHolder.SetPrevNodeRelatedUserConf(bpmnNodeVo);
            }
        }

        // Set back type for all nodes (migrated from bpm_process_node_back)
        BpmnNodeConfigHolder.SetBackType(bpmnNodeVo);

        // 推进配置(forwardType/forwardNodeIds):自动推进(18)/自动完成(18子类型)节点持久化到 node_config_json
        BpmnNodeConfigHolder.SetForwardConf(bpmnNodeVo);

        // Transfer draw-back button config from VO to node config JSON
        int? drawBackType = bpmnNodeVo.DrawBackType;
        if (drawBackType != null && drawBackType != 0)
        {
            var nodeCfgJson = bpmnNodeVo.GetOrCreateNodeConfigJson();
            nodeCfgJson.DrawBackType = drawBackType;
            if (drawBackType == 4 || drawBackType == 5)
            {
                var drawBackNodeIds = bpmnNodeVo.DrawBackNodeIds;
                if (drawBackNodeIds == null || drawBackNodeIds.Count == 0)
                {
                    throw new AFBizException($"节点[{bpmnNodeVo.NodeName}]配置了退回指定节点但未选择目标节点!");
                }
                nodeCfgJson.DrawBackNodeIds = drawBackNodeIds;
            }
        }

        // Node type-based adaptors
        if (nodeType != null)
        {
            if (nodeType == (int)NodeTypeEnum.NODE_TYPE_COPY)
            {
                BpmnNodeConfigHolder.SetPersonnelConf(bpmnNodeVo);
            }
            // Conditions: build JSON directly from VO
            if (nodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITIONS
                || nodeType == (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS)
            {
                PrepareNodeConditionsForJson(bpmnNodeVo);
            }
        }
    }

    /**
    * get adaptor config enum
    *
    * @param bpmnNodeVo
    * @return
    */
    private BpmnNodeAdpConfEnum? GetBpmnNodeAdpConfEnum(BpmnNodeVo bpmnNodeVo)
    {

        BpmnNodeAdpConfEnum? bpmnNodeAdpConfEnum = null;


        NodeTypeEnum? nodeTypeEnumByCode = NodeTypeEnumExtensions.GetNodeTypeEnumByCode(bpmnNodeVo.NodeType);

        if (!nodeTypeEnumByCode.IsNullOrZero()) {
            if (NodeTypeEnum.NODE_TYPE_APPROVER==nodeTypeEnumByCode) {
                NodePropertyEnum? nodePropertyEnum = NodePropertyEnumExtensions.GetByCode(bpmnNodeVo.NodeProperty);
                bpmnNodeAdpConfEnum = BpmnNodeAdpConfEnumExtensions.GetBpmnNodeAdpConfEnumByEnum(nodePropertyEnum)??bpmnNodeAdpConfEnum;
            } else {
                bpmnNodeAdpConfEnum = BpmnNodeAdpConfEnumExtensions.GetBpmnNodeAdpConfEnumByEnum(nodeTypeEnumByCode)??bpmnNodeAdpConfEnum;
            }
        } else {

            NodePropertyEnum? nodePropertyEnum = NodePropertyEnumExtensions.GetByCode(bpmnNodeVo.NodeProperty);
            bpmnNodeAdpConfEnum = BpmnNodeAdpConfEnumExtensions.GetBpmnNodeAdpConfEnumByEnum(nodePropertyEnum)??bpmnNodeAdpConfEnum;
        }
        return bpmnNodeAdpConfEnum;
    }
    /**
    * get node adaptor
    *
    * @param bpmnNodeAdpConfEnum
    * @return
    */
    private IBpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum? bpmnNodeAdpConfEnum) {
        if (bpmnNodeAdpConfEnum == null)
        {
            throw new AFBizException("node has no property");
        }
        return _adaptorFactory.GetBpmnNodeAdaptor(bpmnNodeAdpConfEnum.Value);
    }
    private string GetBpmnCode(string bpmnName)
    {
        BpmnConf.ValidateBpmnName(bpmnName);
        String bpmnFirstLetters = StrUtils.GetFirstLetters(bpmnName);
        String maxBpmnCode = _bpmnConfService.GetMaxBpmnCode(bpmnFirstLetters);
        if (!string.IsNullOrEmpty(maxBpmnCode)) {
           return _bpmnConfService.ReCheckBpmnCode(bpmnFirstLetters,maxBpmnCode);
        }
        return _bpmnConfService.ReCheckBpmnCode(bpmnFirstLetters, bpmnFirstLetters);
    }
    public BpmnConfVo Detail(long id)
    {
        BpmnConf bpmnConf = _bpmnConfService._repository.Find(a => a.Id == id).FirstOrDefault();
        return FormatConfVo(GetBpmnConfVo(bpmnConf));
    }

   

    public BpmnConfVo Detail(String bpmnCode)
    {
        BpmnConf bpmnConf = _bpmnConfService._repository.Find(a => a.BpmnCode.Equals(bpmnCode)).FirstOrDefault();
        
        return GetBpmnConfVo(bpmnConf);
    }

    private BpmnConfVo GetBpmnConfVo(BpmnConf bpmnConf)
    {
        if (bpmnConf == null)
        {
            return new BpmnConfVo();
        }

        BpmnConfVo bpmnConfVo = bpmnConf.MapToVo();
        String conditionsUrl = "";
        if (bpmnConfVo.IsOutSideProcess != null && bpmnConf.IsOutSideProcess == 1)
        {
            OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = _outSideBpmCallbackUrlConfService
                .GetOutSideBpmCallbackUrlConf(bpmnConf.BusinessPartyId.Value);
            if (outSideBpmCallbackUrlConf!=null) {
                bpmnConfVo.BpmConfCallbackUrl=outSideBpmCallbackUrlConf.BpmFlowCallbackUrl;//process config call back url
                bpmnConfVo.BpmFlowCallbackUrl=outSideBpmCallbackUrlConf.BpmFlowCallbackUrl;//process flow call back url
            }
            //query business party's info
            OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyService._repository.Find(a => a.Id.Equals(bpmnConf.BusinessPartyId)).First();
            //format outside form code and reset value
            String formCode = FormatOutSideFormCode(bpmnConfVo);
            bpmnConfVo.FormCode=formCode;
            
            //set business party's name
            bpmnConfVo.BusinessPartyName=(outSideBpmBusinessParty.Name);

            //set business party's mark,mark just like record is a unique identifier for a certain business party,but for human readability
            bpmnConfVo.BusinessPartyMark=outSideBpmBusinessParty.BusinessPartyMark;

            //set business party's business type
            bpmnConfVo.Type=outSideBpmBusinessParty.Type;

            //query business application url
            BpmProcessAppApplicationVo applicationUrl = _bpmProcessAppApplicationService.GetApplicationUrl(outSideBpmBusinessParty.BusinessPartyMark, formCode);
            
            //set view url,submit url and condition url
            if (applicationUrl!=null) {
                bpmnConfVo.ViewUrl=applicationUrl.LookUrl;//view url
                bpmnConfVo.SubmitUrl=applicationUrl.SubmitUrl;//submit url
                bpmnConfVo.ConditionsUrl=(applicationUrl.ConditionUrl);//condition url
                bpmnConfVo.AppId=applicationUrl.Id;//关联应用Id
                conditionsUrl = applicationUrl.ConditionUrl;
            }
        }
      
        ProcessorFactory.ExecutePreReadProcessors(bpmnConfVo);
        List<BpmnNode> bpmnNodes = _bpmnNodeService._repository.Find(a=>a.ConfId.Equals(bpmnConf.Id)&&a.IsDel==0);
        bool isOutSideProcess=bpmnConf.IsOutSideProcess!=null&&bpmnConf.IsOutSideProcess==1;
        bool isLowCodeFlow=bpmnConf.IsLowCodeFlow!=null&&bpmnConf.IsLowCodeFlow==1;
        if(isOutSideProcess||isLowCodeFlow||bpmnConfVo.ExtraFlags!=null){
            foreach (BpmnNode bpmnNode in bpmnNodes) {
                bpmnNode.IsOutSideProcess=bpmnConf.IsOutSideProcess;
                bpmnNode.IsLowCodeFlow=bpmnConf.IsLowCodeFlow;
                bpmnNode.ConfExtraFlags=bpmnConf.ExtraFlags;
            }
        }
        bpmnConfVo.Nodes=GetBpmnNodeVoList(bpmnNodes, conditionsUrl);
        if (!ObjectUtils.IsEmpty(bpmnConfVo.Nodes))
        {
            foreach (BpmnNodeVo node in bpmnConfVo.Nodes)
            {
                node.FormCode=bpmnConfVo.FormCode;
                AfNodeUtils.NodeLabelSpecialProcess(node);
                if((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY==node.NodeType){
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(node, bpmnConfVo.Nodes);
                    if(aggregationNode==null){
                        throw new AFBizException("can not find parallel gateway's aggregation node!");
                    }
                    aggregationNode.AggregationNode=true;
                    aggregationNode.DeduplicationExclude=true;
                }
            }
           
        }
        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(bpmnConfVo.ConfConfigJson);
        if (!SetViewPageButtonFromJson(bpmnConfVo, confConfig))
        {
            SetViewPageButton(bpmnConfVo);
        }

        if (!SetBpmnTemplateVosFromJson(bpmnConfVo, confConfig))
        {
            SetBpmnTemplateVos(bpmnConfVo);
        }
        return bpmnConfVo;
    }
    /// <summary>
    /// Set conf-level notice templates from JSON (no DB table read).
    /// Reads conf_config_json -> confTemplates[] for the given formCode.
    /// </summary>
    public void SetBpmnTemplateVos(BpmnConfVo bpmnConfVo)
    {
        var bpmnConf = _bpmnConfService._repository.GetQueryable()
            .Where(a => a.FormCode == bpmnConfVo.FormCode && a.EffectiveStatus == 1 && a.ConfConfigJson != null)
            .First();
        
        if (bpmnConf == null || string.IsNullOrEmpty(bpmnConf.ConfConfigJson))
        {
            bpmnConfVo.TemplateVos = new List<BpmnTemplateVo>();
            return;
        }
        
        var confConfig = JsonConfUtil.ParseConfConfig(bpmnConf.ConfConfigJson);
        if (confConfig?.ConfTemplates == null || confConfig.ConfTemplates.Count == 0)
        {
            bpmnConfVo.TemplateVos = new List<BpmnTemplateVo>();
            return;
        }
        
        SetBpmnTemplateVosFromJson(bpmnConfVo, confConfig);
    }

    private bool SetBpmnTemplateVosFromJson(BpmnConfVo bpmnConfVo, BpmnConfConfigJson? confConfig)
    {
        if (confConfig?.ConfTemplates == null)
        {
            return false;
        }


        bpmnConfVo.TemplateVos = confConfig.ConfTemplates.Select(c =>
        {
            var vo = new BpmnTemplateVo
            {
                Event = c.Event,
                TemplateId = c.TemplateId ?? 0,
                FormCode = c.FormCode
            };

            if (c.InformIdList != null && c.InformIdList.Count > 0)
            {
                vo.InformIdList = c.InformIdList;
                vo.Informs = string.Join(",", c.InformIdList);
            }

            if (c.EmpList != null && c.EmpList.Count > 0)
            {
                vo.EmpList = c.EmpList;
                vo.EmpIdList = c.EmpList.Select(a => a.Id).ToList();
                vo.Emps = string.Join(",", vo.EmpIdList);
            }

            if (c.RoleList != null && c.RoleList.Count > 0)
            {
                vo.RoleList = c.RoleList;
                vo.RoleIdList = c.RoleList.Select(a => a.Id).ToList();
                vo.Roles = string.Join(",", vo.RoleIdList);
            }

            if (c.FuncList != null && c.FuncList.Count > 0)
            {
                vo.FuncList = c.FuncList;
                vo.FuncIdList = c.FuncList.Select(a => a.Id).ToList();
                vo.Funcs = string.Join(",", vo.FuncIdList);
            }

            return vo;
        }).ToList();
        HydrateBpmnTemplateVos(bpmnConfVo.TemplateVos);
        return true;
    }

    private bool SetViewPageButtonFromJson(BpmnConfVo bpmnConfVo, BpmnConfConfigJson? confConfig)
    {
        if (confConfig?.ViewPageButtons == null)
        {
            return false;
        }

        bpmnConfVo.ViewPageButtons = new BpmnViewPageButtonBaseVo
        {
            ViewPageStart = confConfig.ViewPageButtons
                .Where(o => o.ViewType == (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START)
                .Select(o => o.ButtonType)
                .ToList(),
            ViewPageOther = confConfig.ViewPageButtons
                .Where(o => o.ViewType == (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER)
                .Select(o => o.ButtonType)
                .ToList()
        };

        return true;
    }

    private void HydrateBpmnTemplateVos(List<BpmnTemplateVo>? templateVos)
    {
        if (templateVos == null || templateVos.Count == 0)
        {
            return;
        }

        foreach (var vo in templateVos)
        {
            vo.EventValue = EventTypeEnumExtensions.GetDescByCode(vo.Event);

            if (vo.InformIdList.IsEmpty() && !string.IsNullOrEmpty(vo.Informs))
            {
                vo.InformIdList = vo.Informs.Split(',').ToList();
            }

            if (vo.EmpIdList.IsEmpty() && !string.IsNullOrEmpty(vo.Emps))
            {
                vo.EmpIdList = vo.Emps.Split(',').ToList();
            }

            if (!vo.EmpIdList.IsEmpty() && vo.EmpList.IsEmpty())
            {
                var employeeInfo = _employeeInfoProviderService.ProvideEmployeeInfo(vo.EmpIdList);
                vo.EmpList = vo.EmpIdList
                    .Select(id => new BaseIdTranStruVo
                    {
                        Id = id,
                        Name = employeeInfo.ContainsKey(id) ? employeeInfo[id] : string.Empty
                    })
                    .ToList();
            }

            if (vo.TemplateId > 0)
            {
                vo.TemplateName = _informationTemplateService._repository.GetQueryable()
                    .Where(a=>a.Id==vo.TemplateId).FirstOrDefault()
                    ?.Name ?? vo.TemplateName;
            }
        }
    }

    private void HydrateApproveRemindVo(BpmnApproveRemindVo? vo)
    {
        if (vo == null)
        {
            return;
        }

        vo.IsInuse = vo.TemplateId != null && !string.IsNullOrEmpty(vo.Days);
        if (!string.IsNullOrEmpty(vo.Days) && vo.DayList.IsEmpty())
        {
            vo.DayList = vo.Days.Split(',').Select(int.Parse).ToList();
        }

        if (vo.TemplateId != null)
        {
            vo.TemplateName = _informationTemplateService._repository.GetQueryable()
                .Where(a=>a.Id==vo.TemplateId).FirstOrDefault()
                ?.Name ?? vo.TemplateName;
        }
    }
    private List<BpmnNodeVo> GetBpmnNodeVoList(List<BpmnNode> bpmnNodeList, String conditionsUrl)
    {
        List<long> idList = bpmnNodeList.Select(a => a.Id).ToList();
        Dictionary<long,List<string>> bpmnNodeToMap = GetBpmnNodeToMap(idList);

        List<BpmnNodeVo> bpmnNodeVoList = bpmnNodeList
            .Select(o => GetBpmnNodeVo(o, bpmnNodeToMap, conditionsUrl))
            .ToList();

        // 动态条件节点是网关节点,找到网关节点的上一级节点,然后打上标签,
        // 流程执行过程中如果有相应标签,则执行动态条件判断
        // node id -> VO map, for looking up prev nodes by nodeId
        Dictionary<string, BpmnNodeVo> nodeVoByNodeId = bpmnNodeVoList
            .Where(o => o.NodeId != null)
            .ToDictionary(o => o.NodeId, o => o);
        // node id -> VO id list (for finding nodes that point to a given node)
        Dictionary<string, List<long>> nodeToReverseMap = new Dictionary<string, List<long>>();
        foreach (var entry in bpmnNodeToMap)
        {
            foreach (var nodeTo in entry.Value)
            {
                if (!nodeToReverseMap.TryGetValue(nodeTo, out var list))
                {
                    list = new List<long>();
                    nodeToReverseMap[nodeTo] = list;
                }
                list.Add(entry.Key);
            }
        }

        foreach (BpmnNodeVo bpmnNodeVo in bpmnNodeVoList)
        {
            if (bpmnNodeVo.IsDynamicCondition != null && bpmnNodeVo.IsDynamicCondition.Value)
            {
                // find the previous node by NodeFrom
                BpmnNodeVo prevNodeVo = null;
                if (bpmnNodeVo.NodeFrom != null && nodeVoByNodeId.TryGetValue(bpmnNodeVo.NodeFrom, out prevNodeVo))
                {
                    List<BpmnNodeVo> nodesToLabel = new List<BpmnNodeVo> { prevNodeVo };

                    // if prev node is a gateway, also find all nodes pointing to it or the current node
                    if (prevNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_GATEWAY)
                    {
                        List<long> dynamicLabelNodeIds = new List<long>();
                        // nodes pointing to prev gateway node
                        if (prevNodeVo.NodeId != null && nodeToReverseMap.TryGetValue(prevNodeVo.NodeId, out var pointingToPrev))
                        {
                            dynamicLabelNodeIds.AddRange(pointingToPrev);
                        }
                        // nodes pointing to current dynamic-condition node
                        if (bpmnNodeVo.NodeId != null && nodeToReverseMap.TryGetValue(bpmnNodeVo.NodeId, out var pointingToCurrent))
                        {
                            dynamicLabelNodeIds.AddRange(pointingToCurrent);
                        }

                        if (dynamicLabelNodeIds.Count > 0)
                        {
                            var dynamicLabelNodes = bpmnNodeVoList
                                .Where(a => a.Id != 0 && dynamicLabelNodeIds.Contains(a.Id))
                                .ToList();
                            nodesToLabel.AddRange(dynamicLabelNodes);
                        }
                    }

                    // attach DynamicCondition label to each identified node
                    foreach (BpmnNodeVo nodeToLabel in nodesToLabel)
                    {
                        nodeToLabel.LabelList ??= new List<BpmnNodeLabelVO>();
                        if (!nodeToLabel.LabelList.Any(l => NodeLabelConstants.DynamicCondition.LabelValue == l.LabelValue))
                        {
                            nodeToLabel.LabelList.Add(NodeLabelConstants.DynamicCondition);
                        }
                    }
                }
                else
                {
                    // can not find prev node for the dynamic-condition node
                }
            }
        }

        return bpmnNodeVoList;
    }

    private Dictionary<long, List<String>> GetBpmnNodeToMap(List<long> idList)
    {
        List<BpmnNodeTo> bpmnNodeTos = _bpmnNodeToService._repository
            .Find(a => idList.Contains(a.BpmnNodeId) && a.IsDel == 0).ToList();
        Dictionary<long,List<string>> result = bpmnNodeTos
            .GroupBy(a=>a.BpmnNodeId)
            .ToDictionary(g=>g.Key,g=>g.Select(x=>x.NodeTo).ToList());

        return result;
    }
    private BpmnNodeVo GetBpmnNodeVo(BpmnNode bpmnNode, Dictionary<long, List<String>> bpmnNodeToMap, String conditionsUrl)
    {
        BpmnNodeVo bpmnNodeVo = bpmnNode.MapToVo();
        bpmnNodeVo.ConditionsUrl = conditionsUrl;

        long bpmnNodeId = bpmnNode.Id;
        //set nodeto (still from DB — t_bpmn_node_to is kept)
        bpmnNodeVo.NodeTo = bpmnNodeToMap.ContainsKey(bpmnNodeId) ? bpmnNodeToMap[bpmnNodeId] : null;

        //parse node config JSON
        BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
        if (nodeConfig == null)
        {
            throw new AFBizException("migration error,please contact the author");
        }
        bpmnNodeVo.NodeConfigJsonObj = nodeConfig;

        // 推进配置反显: 从 node_config_json 读回 forwardType/forwardNodeIds(自动推进18/自动完成18子类型)
        // 自动完成节点的目标由前端在提交时 refill 填充, 反显时需读回给前端只读展示
        bpmnNodeVo.ForwardType = nodeConfig.ForwardType;
        bpmnNodeVo.ForwardNodeIds = nodeConfig.ForwardNodeIds;

        //set buttons from buttonSignConf
        BpmnNodeButtonSignConfJson? bsConf = nodeConfig.ButtonSignConf;
        if (bsConf?.ButtonConfList != null && bsConf.ButtonConfList.Count > 0)
        {
            bpmnNodeVo.Buttons = new BpmnNodeButtonConfBaseVo
            {
                StartPage = bsConf.ButtonConfList
                    .Where(b => b.ButtonPageType == (int)ButtonPageTypeEnum.INITIATE)
                    .Select(b => new BpmnConfCommonButtonPropertyVo
                    {
                        ButtonType = b.ButtonType,
                        ButtonName = b.ButtonName
                    })
                    .ToList(),
                ApprovalPage = bsConf.ButtonConfList
                    .Where(b => b.ButtonPageType == (int)ButtonPageTypeEnum.AUDIT)
                    .Select(b => new BpmnConfCommonButtonPropertyVo
                    {
                        ButtonType = b.ButtonType,
                        ButtonName = b.ButtonName
                    })
                    .ToList(),
                ViewPage = bsConf.ButtonConfList
                    .Where(b => b.ButtonPageType == (int)ButtonPageTypeEnum.TOVIEW)
                    .Select(b => new BpmnConfCommonButtonPropertyVo
                    {
                        ButtonType = b.ButtonType,
                        ButtonName = b.ButtonName
                    })
                    .ToList()
            };
        }

        //set node property name
        bpmnNodeVo.NodePropertyName = NodePropertyEnumExtensions.GetDescByCode(bpmnNodeVo.NodeProperty);

        //set templates from templateConf
        BpmnNodeTemplateConfJson? tcConf = nodeConfig.TemplateConf;
        if (tcConf?.Templates != null && tcConf.Templates.Count > 0)
        {
            bpmnNodeVo.TemplateVos = tcConf.Templates;
        }
        HydrateBpmnTemplateVos(bpmnNodeVo.TemplateVos);

        //set approve remind from templateConf
        if (tcConf?.ApproveRemind != null)
        {
            bpmnNodeVo.ApproveRemindVo = tcConf.ApproveRemind;
        }
        HydrateApproveRemindVo(bpmnNodeVo.ApproveRemindVo);

        //call adaptor formatToBpmnNodeVo — adaptor will read from nodeConfigJsonObj
        BpmnNodeAdpConfEnum? adpConfEnum = GetBpmnNodeAdpConfEnum(bpmnNodeVo);
        if (adpConfEnum != null)
        {
            GetBpmnNodeAdaptor(adpConfEnum).FormatToBpmnNodeVo(bpmnNodeVo);
        }

        if ((int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS == bpmnNode.NodeType)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITIONS;
        }

        //set sign up conf from buttonSignConf
        if (bsConf?.SignUpConf != null)
        {
            BpmnNodePropertysVo propertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();
            propertysVo.AfterSignUpWay = bsConf.SignUpConf.AfterSignUpWay ?? 0;
            propertysVo.SignUpType = bsConf.SignUpConf.SignUpType ?? 0;
            bpmnNodeVo.Property = propertysVo;
        }

        //set field controls from lowCodeConf
        BpmnNodeLowCodeConfJson? lowCodeConf = nodeConfig.LowCodeConf;
        if (lowCodeConf?.FieldControls != null && lowCodeConf.FieldControls.Count > 0)
        {
            bpmnNodeVo.LfFieldControlVOs = lowCodeConf.FieldControls;
        }

        //set labels from buttonSignConf
        if (bsConf?.Labels != null && bsConf.Labels.Count > 0)
        {
            var labelVOList = bsConf.Labels
                .Select(l => new BpmnNodeLabelVO(l.LabelValue, l.LabelName))
                .ToList();
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.CopyNodeV2.LabelValue))
            {
                bpmnNodeVo.DeduplicationExclude = true;
                bpmnNodeVo.IsCarbonCopyNode = true;
            }
            // 条件审批节点:标签匹配时设置标记位
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.ConditionApproveNode.LabelValue))
            {
                bpmnNodeVo.IsConditionApproveNode = true;
            }
            // 条件推进节点:标签匹配时设置标记位,前端据此反显为条件推进(推进设置tab/图标/颜色)
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.ConditionAdvanceNode.LabelValue))
            {
                bpmnNodeVo.IsConditionAdvanceNode = true;
            }
            // 条件完成节点:标签匹配时设置标记位,前端据此反显为条件完成(只读目标/图标/颜色)
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.ConditionFinishNode.LabelValue))
            {
                bpmnNodeVo.IsConditionFinishNode = true;
            }
            // 条件抄送节点:标签匹配时设置标记位
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.ConditionCopyNode.LabelValue))
            {
                bpmnNodeVo.IsConditionCopyNode = true;
            }
            // 上一节点指定审批人节点:标签匹配时设置标记位,前端据此在编辑面板回显"上一节点指定"选项
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.PrevNodeAppointed.LabelValue))
            {
                bpmnNodeVo.IsPrevNodeAppointed = true;
            }
            // 自动完成节点:标签匹配时设置标记位,前端据此反显为自动完成(颜色/图标/只读目标)
            if (NodeLabelConstants.NodeLabelContainsAny(labelVOList, NodeLabelConstants.AutoCompleteNode.LabelValue))
            {
                bpmnNodeVo.IsAutoCompleteNode = true;
            }
            bpmnNodeVo.LabelList = labelVOList;
        }

        //set autoNodeConf from nodeConfig (for condition-approve 12 / condition-copy 13)
        if (nodeConfig.AutoNodeConf != null)
        {
            bpmnNodeVo.AutoNodeConf = new AutoNodeConfVo
            {
                GroupRelation = nodeConfig.AutoNodeConf.GroupRelation,
                // 反序列化每个 JsonElement 还原为 BpmnNodeConditionsConfVueVo
                ConditionList = nodeConfig.AutoNodeConf.ConditionList?
                    .Select(group => group?
                        .Select(item => item.ValueKind == JsonValueKind.Null
                            ? null
                            : JsonSerializer.Deserialize<BpmnNodeConditionsConfVueVo>(item.GetRawText()))
                        .Where(x => x != null)
                        .Select(x => x!)
                        .ToList() ?? new List<BpmnNodeConditionsConfVueVo>())
                    .ToList() ?? new List<List<BpmnNodeConditionsConfVueVo>>()
            };
        }

        // set draw-back config from node config JSON (for display)
        if (nodeConfig.DrawBackType != null && nodeConfig.DrawBackType != 0)
        {
            bpmnNodeVo.DrawBackType = nodeConfig.DrawBackType;
            bpmnNodeVo.DrawBackNodeIds = nodeConfig.DrawBackNodeIds;
        }

        return bpmnNodeVo;
    }

    private void SetViewPageButton(BpmnConfVo bpmnConfVo)
    {
        // BpmnViewPageButton entity and service have been removed; view page buttons are now in conf_config_json
        bpmnConfVo.ViewPageButtons = new BpmnViewPageButtonBaseVo();
    }

    private string FormatOutSideFormCode(BpmnConfVo bpmnConfVo)
    {
       
        String formCode = bpmnConfVo.FormCode;

        return formCode.Substring(formCode.IndexOf("_") + 1);
    }

    public BpmnConfVo DetailByFormCode(String formCode)
    {

        BpmnConf bpmnConf = _bpmnConfService._repository.Find(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .FirstOrDefault();
        if(bpmnConf==null){
            throw new AFBizException("can not get a bpmnConf by provided formCode");
        }
        return GetBpmnConfVo(bpmnConf);
    }

    public int? GetCustomizeNodeSignType(long nodeId)
    {
        // GetCustomizeNodeSignType has been removed from IBpmnNodeService
        return null;
    }
    private BpmnConfVo FormatConfVo(BpmnConfVo confVo)
    {
        if(confVo==null){
            throw new AFBizException("has not confVo");
        }
        List<BpmnNodeVo> nodes = confVo.Nodes;
        if(nodes==null||!nodes.Any()){
            throw new AFBizException("confVo has empty nodes");
        }
        foreach (BpmnNodeVo node in nodes)
        {
            BpmnNodePropertysVo property = node.Property;
            if(property!=null)
            {
                property.ConditionsConf = null;
            }
        }
        return confVo;
    }
     private void EditNodeExtraFlags(BpmnNodeVo bpmnNodeVo){
        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        if(property!=null){
            int flags=0;
            List<ExtraSignInfoVo> additionalSignInfoList = property.AdditionalSignInfoList;
            if(!additionalSignInfoList.IsEmpty()){
                List<BpmnNodeFlagsEnum> additionalFlags=new List<BpmnNodeFlagsEnum>();
                foreach (ExtraSignInfoVo extraSignInfoVo in additionalSignInfoList) {
                    int? nodeProperty = extraSignInfoVo.NodeProperty;
                    int? propertyType = extraSignInfoVo.PropertyType;
                    NodePropertyEnum nodePropertyEnum = (NodePropertyEnum)nodeProperty;
                    if(nodePropertyEnum==null){
                        throw new AFBizException(BusinessError.STATUS_ERROR,"额外审批人节点类型未定义!");
                    }
                    switch (nodePropertyEnum){
                        case NodePropertyEnum.NODE_PROPERTY_ROLE:
                            if(propertyType==1){
                                additionalFlags.Add(BpmnNodeFlagsEnum.HAS_ADDITIONAL_ASSIGNEE_ROLE);
                            }else if(propertyType==2){
                                additionalFlags.Add(BpmnNodeFlagsEnum.HAS_EXCLUDE_ASSIGNEE_ROLE);
                            }else{
                                throw new AFBizException(BusinessError.STATUS_ERROR,"额外审批人节propertyType点类型未定义!");
                            }
                            break;
                        case NodePropertyEnum.NODE_PROPERTY_PERSONNEL:
                            if(propertyType==1){
                                additionalFlags.Add(BpmnNodeFlagsEnum.HAS_ADDITIONAL_ASSIGNEE);
                            }else if(propertyType==2){
                                additionalFlags.Add(BpmnNodeFlagsEnum.HAS_EXCLUDE_ASSIGNEE);
                            }else{
                                throw new AFBizException(BusinessError.STATUS_ERROR,"额外审批人节propertyType点类型未定义!");
                            }
                            break;
                        default:
                            throw new AFBizException(BusinessError.STATUS_ERROR,"暂不支持的额外操作类型!");
                    }
                }
                foreach (BpmnNodeFlagsEnum additionalFlag in additionalFlags)
                {
                    flags = flags | additionalFlag.Code;
                }
                bpmnNodeVo.ExtraFlags=flags;
            }
        }
    }

    private static bool IsConditionNodeType(int nodeType)
    {
        return nodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITIONS
               || nodeType == (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS;
    }

    private static void PrepareNodeConditionsForJson(BpmnNodeVo bpmnNodeVo)
    {
        if (!IsConditionNodeType(bpmnNodeVo.NodeType) || bpmnNodeVo.Property == null)
        {
            return;
        }

        BpmnNodePropertysVo property = bpmnNodeVo.Property;
        string? outSideConditionsId = property.ConditionsConf?.OutSideConditionsId;
        BpmnNodeConditionsConfBaseVo conditionsConf = property.IsDefault == 1
            ? new BpmnNodeConditionsConfBaseVo
            {
                IsDefault = property.IsDefault,
                Sort = property.Sort,
                GroupRelation = ConditionRelationShipEnum.GetCodeByValue(property.GroupRelation),
                ExtJson = property.ConditionList == null
                    ? null
                    : JsonConfUtil.ToJsonString(property.ConditionList)
            }
            : BpmnConfNodePropertyConverter.FromVue3Model(property);

        if (!string.IsNullOrWhiteSpace(outSideConditionsId))
        {
            conditionsConf.OutSideConditionsId = outSideConditionsId;
        }

        // Build ConditionGroup matching Java's buildConditionsJsonFromVo
        var conditionGroup = new BpmnNodeConditionsConfJson.ConditionGroup
        {
            IsDefault = conditionsConf.IsDefault,
            GroupRelation = conditionsConf.GroupRelation,
            Sort = conditionsConf.Sort,
            ExtJson = conditionsConf.ExtJson,
            Params = new List<BpmnNodeConditionsConfJson.ConditionParam>()
        };

        // Store in nodeConfigJson.ConditionsConf (matching Java's BpmnNodeConfigHolder.setConditionsConf)
        BpmnNodeConfigHolder.SetConditionsConf(bpmnNodeVo,
            new List<BpmnNodeConditionsConfJson.ConditionGroup> { conditionGroup },
            outSideConditionsId);
    }

    public void SaveProcessNotices(ProcessConfVo vo)
    {
        var processKey = vo.ProcessKey;
        if (string.IsNullOrWhiteSpace(processKey))
        {
            throw new AFBizException("processKey can not be null");
        }

        var notifyTypeIds = vo.NotifyTypeIds;
        var templateVos = vo.TemplateVos;

        // Load the existing effective BpmnConf record for this formCode
        var bpmnConf = _bpmnConfService._repository.GetBpmnConfByFormCode(processKey);
        if (bpmnConf == null || bpmnConf.Id == 0)
        {
            throw new AFBizException($"can not find bpmn conf for formCode:{processKey}");
        }

        // Parse existing conf-level JSON (or start fresh)
        var confConfig = JsonConfUtil.ParseConfConfig(bpmnConf.ConfConfigJson);
        confConfig ??= new BpmnConfConfigJson();

        // --- Advanced notification templates ---
        if (templateVos is { Count: > 0 })
        {
            // If advanced notifications are set but no ordinary notice types, assign advanced to ordinary
            if (confConfig.NoticeChannelTypes is not { Count: > 0 })
            {
                var advancedNotifyIds = templateVos
                    .Where(t => t.MessageSendTypeList is { Count: > 0 })
                    .SelectMany(t => t.MessageSendTypeList!)
                    .Select(a => (int)a.Id)
                    .Distinct()
                    .ToList();
                if (advancedNotifyIds.Count > 0)
                {
                    notifyTypeIds = new List<int>(advancedNotifyIds);
                }
            }
            // Replace conf-level templates (equivalent to delete-all + re-insert)
            confConfig.ConfTemplates = BpmnConfConfigHolder.BuildConfTemplates(templateVos, processKey);
        }

        // --- Notice channel types ---
        if (notifyTypeIds is { Count: > 0 })
        {
            confConfig.NoticeChannelTypes = notifyTypeIds;
        }
        else
        {
            confConfig.NoticeChannelTypes = null;
        }

            bpmnConf.ConfConfigJson = JsonConfUtil.ToConfConfigJson(confConfig);
        bpmnConf.UpdateTime=DateTime.Now;
        bpmnConf.UpdateUser = SecurityUtils.GetLogInEmpId();
        _bpmnConfService._repository.Update(bpmnConf);
    }

    /// <summary>
    /// 动态条件迁移预校验:重新评估条件,检查条件是否发生变化.
    /// 对应 Java BpmnConfBizServiceImpl.migrationCheckConditionsChange.
    /// 1. 根据bpmnCode获取流程配置
    /// 2. 获取表单适配器,调用LaunchParameters获取启动条件
    /// 3. 设置isPreview=true, isMigration=true
    /// 4. 调用条件过滤(formatBpmnConf),触发条件重新评估
    /// 5. 如果ConditionService抛出CONDITION_CHANGED异常,则返回true
    /// </summary>
    public bool MigrationCheckConditionsChange(BusinessDataVo vo)
    {
        // 根据bpmnCode获取流程配置
        BpmnConfVo bpmnConfVo = Detail(vo.BpmnCode);
        if (bpmnConfVo == null || bpmnConfVo.Id == 0)
        {
            throw new AFBizException("未找到对应的 bpmnConf 记录");
        }

        // 获取表单适配器,调用LaunchParameters获取启动条件
        var formAdapter = _formFactory.GetFormAdaptor(vo);
        BpmnStartConditionsVo bpmnStartConditionsVo = formAdapter.LaunchParameters(vo);
        bpmnStartConditionsVo.IsPreview = true;
        bpmnStartConditionsVo.ProcessNum = vo.ProcessNumber;
        bpmnStartConditionsVo.IsMigration = true;

        // 调用条件过滤,触发条件重新评估
        try
        {
            _bpmnStartFormatFactory.formatBpmnConf(bpmnConfVo, bpmnStartConditionsVo);
        }
        catch (AFBizException ex)
        {
            if (StringConstants.CONDITION_CHANGED.Equals(ex.Code))
            {
                return true;
            }
            throw;
        }

        return false;
    }
}