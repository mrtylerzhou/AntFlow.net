using System.Runtime.InteropServices.JavaScript;
using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.service.processor;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using AntOffice.Base.Util;
using antflowcore.util.Extension;

namespace antflowcore.service.biz;

public class BpmnConfBizService
{
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmnConfNoticeTemplateService _bpmnConfNoticeTemplateService;
    private readonly BpmnNodeService _bpmnNodeService;
    private readonly BpmnNodeToService _bpmnNodeToService;
    private readonly BpmnNodeSignUpConfService _bpmnNodeSignUpConfService;
    private readonly BpmnTemplateService _bpmnTemplateService;
    private readonly BpmnApproveRemindService _approveRemindService;
    private readonly IAdaptorFactory _adaptorFactory;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly BpmProcessAppApplicationService _bpmProcessAppApplicationService;
    private readonly BpmnNodeButtonConfService _bpmnNodeButtonConfService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly InformationTemplateService _informationTemplateService;
    private readonly BpmnNodeLfFormdataFieldControlService _lfFormdataFieldControlService;
    private readonly BpmnViewPageButtonService _viewPageButtonService;
    private readonly TaskMgmtService _taskMgmtService;
  

    public BpmnConfBizService(
        BpmnConfService bpmnConfService,
        BpmnConfNoticeTemplateService bpmnConfNoticeTemplateService,
        BpmnNodeService bpmnNodeService,
        BpmnNodeToService bpmnNodeToService,
        BpmnNodeSignUpConfService bpmnNodeSignUpConfService,
        BpmnTemplateService bpmnTemplateService,
        BpmnApproveRemindService approveRemindService,
        IAdaptorFactory adaptorFactory,
        OutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService,
        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService,
        BpmProcessAppApplicationService bpmProcessAppApplicationService,
        BpmnNodeButtonConfService bpmnNodeButtonConfService,
        IBpmnEmployeeInfoProviderService employeeInfoProviderService,
        InformationTemplateService informationTemplateService,
        BpmnNodeLfFormdataFieldControlService lfFormdataFieldControlService,
        BpmnViewPageButtonService viewPageButtonService,
        TaskMgmtService taskMgmtService
        )
    {
        _bpmnConfService = bpmnConfService;
        _bpmnConfNoticeTemplateService = bpmnConfNoticeTemplateService;
        _bpmnNodeService = bpmnNodeService;
        _bpmnNodeToService = bpmnNodeToService;
        _bpmnNodeSignUpConfService = bpmnNodeSignUpConfService;
        _bpmnTemplateService = bpmnTemplateService;
        _approveRemindService = approveRemindService;
        _adaptorFactory = adaptorFactory;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
        _bpmProcessAppApplicationService = bpmProcessAppApplicationService;
        _bpmnNodeButtonConfService = bpmnNodeButtonConfService;
        _employeeInfoProviderService = employeeInfoProviderService;
        _informationTemplateService = informationTemplateService;
        _lfFormdataFieldControlService = lfFormdataFieldControlService;
        _viewPageButtonService = viewPageButtonService;
        _taskMgmtService = taskMgmtService;
     
    }
    private const String LinkMark = "_";

    public void Edit(BpmnConfVo bpmnConfVo)
    {
        String bpmnName = bpmnConfVo.BpmnName;
        String bpmnCode = GetBpmnCode(bpmnName);
        String formCode = bpmnConfVo.FormCode;
        //todo 注意查看映射效果
        BpmnConf bpmnConf = bpmnConfVo.MapToEntity();
       
        bpmnConf.BpmnCode=bpmnCode;
        bpmnConf.FormCode = formCode;
        bpmnConf.CreateUser=SecurityUtils.GetLogInEmpNameSafe();
        bpmnConf.CreateTime=DateTime.Now;
        bpmnConf.UpdateUser=SecurityUtils.GetLogInEmpNameSafe();
        bpmnConf.UpdateTime=DateTime.Now;
        _bpmnConfService.baseRepo.Insert(bpmnConf);
        //notice template
        _bpmnConfNoticeTemplateService.Insert(bpmnCode);
        long confId = bpmnConf.Id;
        if(confId.IsNullOrZero()){
            throw new AFBizException($"conf id for formcode:{formCode} can not be null");
        }
        bpmnConfVo.Id=confId;
        //todo 
        int? isOutSideProcess = bpmnConfVo.IsOutSideProcess;
        int? isLowCodeFlow = bpmnConfVo.IsLowCodeFlow;
        
        ProcessorFactory.ExecutePreWriteProcessors(bpmnConfVo);
        
        List<BpmnNodeVo> confNodes = bpmnConfVo.Nodes;
        foreach (BpmnNodeVo bpmnNodeVo in confNodes)
        {
            if (bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_APPROVER
                && bpmnNodeVo.NodeProperty==null) {
                throw new AFBizException("apporver node has no property,can not be saved！");
            }
            bpmnNodeVo.IsOutSideProcess=isOutSideProcess;
            bpmnNodeVo.IsLowCodeFlow=isLowCodeFlow;

            //if the node has no property,the node property default is "1-no property"
            bpmnNodeVo.NodeProperty=bpmnNodeVo.NodeProperty ?? 1;
            BpmnNode bpmnNode = bpmnNodeVo.MapToEntity();
            bpmnNode.ConfId=confId;
            bpmnNode.CreateTime=DateTime.Now;
            bpmnNode.CreateUser=SecurityUtils.GetLogInEmpNameSafe();
            bpmnNode.Remark ??= "";
            _bpmnNodeService.baseRepo.Insert(bpmnNode);
            long bpmnNodeId = bpmnNode.Id;
            if(bpmnNodeId.IsNullOrZero()){
                throw new AFBizException("can not get bpmn node id!");
            }
            //edit node to
            _bpmnNodeToService.EditNodeTo(bpmnNodeVo,bpmnNodeId);
            //edit node's button conf
            _bpmnNodeButtonConfService.EditButtons(bpmnNodeVo, bpmnNodeId);
            //edit node sign up
            _bpmnNodeSignUpConfService.EditSignUpConf(bpmnNodeVo,bpmnNodeId);
            
            bpmnNodeVo.Id=bpmnNodeId;
            bpmnNodeVo.ConfId=confId;
            BpmnNodeAdpConfEnum? bpmnNodeAdpConfEnum = GetBpmnNodeAdpConfEnum(bpmnNodeVo);
            //if it can not get the node's adapter,continue
            if (bpmnNodeAdpConfEnum==null) {
                continue;
            }
            //edit in node notice template
            _bpmnTemplateService.EditBpmnTemplate(bpmnNodeVo);
            //edit in node approver remind conf
            _approveRemindService.EditBpmnApproveRemind(bpmnNodeVo);
            //get node adaptor
            BpmnNodeAdaptor bpmnNodeAdaptor = GetBpmnNodeAdaptor(bpmnNodeAdpConfEnum);

            //then edit the node
            bpmnNodeAdaptor.EditBpmnNode(bpmnNodeVo);
        }
        ProcessorFactory.ExecutePostProcessors(bpmnConfVo);
    }

    public ResultAndPage<BpmnConfVo> SelectPage(PageDto pageDto, BpmnConfVo vo)
    {
        Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
        var bpmnConfVos = _bpmnConfService.SelectPageList(page,vo);

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
    private BpmnNodeAdaptor GetBpmnNodeAdaptor(BpmnNodeAdpConfEnum? bpmnNodeAdpConfEnum) {
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
        BpmnConf bpmnConf = _bpmnConfService.baseRepo.Where(a => a.Id == id).ToOne();
        return FormatConfVo(GetBpmnConfVo(bpmnConf));
    }

   

    public BpmnConfVo Detail(String bpmnCode)
    {
        BpmnConf bpmnConf = _bpmnConfService.baseRepo.Where(a => a.BpmnCode.Equals(bpmnCode)).ToOne();
        
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
            OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyService.baseRepo
                .Where(a => a.Id.Equals(bpmnConf.BusinessPartyId)).ToOne();
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
        List<BpmnNode> bpmnNodes = _bpmnNodeService.baseRepo.Where(a=>a.ConfId.Equals(bpmnConf.Id)&&a.IsDel==0).ToList();
        bool isOutSideProcess=bpmnConf.IsOutSideProcess!=null&&bpmnConf.IsOutSideProcess==1;
        bool isLowCodeFlow=bpmnConf.IsLowCodeFlow!=null&&bpmnConf.IsLowCodeFlow==1;
        if(isOutSideProcess||isLowCodeFlow){
            foreach (BpmnNode bpmnNode in bpmnNodes) {
                bpmnNode.IsOutSideProcess=bpmnConf.IsOutSideProcess;
                bpmnNode.IsLowCodeFlow=bpmnConf.IsLowCodeFlow;
            }
        }
        bpmnConfVo.Nodes=GetBpmnNodeVoList(bpmnNodes, conditionsUrl);
        if (!ObjectUtils.IsEmpty(bpmnConfVo.Nodes))
        {
            bpmnConfVo.Nodes.ForEach(node => node.FormCode = bpmnConfVo.FormCode);
        }
        //set viewpage buttons
        SetViewPageButton(bpmnConfVo);


        //set out node notice template
        SetBpmnTemplateVos(bpmnConfVo);
        return bpmnConfVo;
    }
    private void SetBpmnTemplateVos(BpmnConfVo bpmnConfVo)
    {
       
        List<BpmnTemplate> bpmnTemplates = _bpmnTemplateService.baseRepo
            .Where(a => a.ConfId == bpmnConfVo.Id && a.IsDel == 0 && a.NodeId == null).ToList();
        bpmnConfVo.TemplateVos  = bpmnTemplates.Select(o =>
        {
            BpmnTemplateVo vo = o.MapToVo();
            BuildBpmnTemplateVo(vo);
            return vo;
        }).ToList();
    }
    private List<BpmnNodeVo> GetBpmnNodeVoList(List<BpmnNode> bpmnNodeList, String conditionsUrl)
    {
        List<long> idList = bpmnNodeList.Select(a => a.Id).ToList();
        Dictionary<long,List<string>> bpmnNodeToMap = GetBpmnNodeToMap(idList);
        Dictionary<long,List<BpmnNodeButtonConf>> bpmnNodeButtonConfMap = GetBpmnNodeButtonConfMap(idList);
        Dictionary<long,BpmnNodeSignUpConf> bpmnNodeSignUpConfMap = GetBpmnNodeSignUpConfMap(idList);
        Dictionary<long,List<BpmnTemplateVo>> bpmnTemplateVoMap = GetBpmnTemplateVoMap(idList);
        Dictionary<long,BpmnApproveRemindVo> bpmnApproveRemindVoMap = GetBpmnApproveRemindVoMap(idList);
        int? isLowCodeFlow = bpmnNodeList[0].IsLowCodeFlow;
        Dictionary<long, List<BpmnNodeLfFormdataFieldControl>> bpmnNodeFieldControlConfMap=new();
        if (isLowCodeFlow is 1)
        {
            bpmnNodeFieldControlConfMap = GetBpmnNodeFieldControlConfMap(idList);
        }

        return bpmnNodeList
            .Select(o => GetBpmnNodeVo(o, bpmnNodeToMap, bpmnNodeButtonConfMap, bpmnNodeSignUpConfMap,
                bpmnTemplateVoMap, bpmnApproveRemindVoMap, bpmnNodeFieldControlConfMap, conditionsUrl))
            .ToList();
    }

    private Dictionary<long, List<String>> GetBpmnNodeToMap(List<long> idList)
    {
        List<BpmnNodeTo> bpmnNodeTos = _bpmnNodeToService.baseRepo
            .Where(a => idList.Contains(a.BpmnNodeId) && a.IsDel == 0).ToList();
        Dictionary<long,List<string>> result = bpmnNodeTos
            .GroupBy(a=>a.BpmnNodeId)
            .ToDictionary(g=>g.Key,g=>g.Select(x=>x.NodeTo).ToList());

        return result;
    }
    private Dictionary<long, List<BpmnNodeLfFormdataFieldControl>> GetBpmnNodeFieldControlConfMap(List<long> idList)
    {
        var bpmnNodeLfFormdataFieldControls = _lfFormdataFieldControlService
            .baseRepo.
            Where(a=>idList.Contains(a.Id)).ToList();

        return bpmnNodeLfFormdataFieldControls
            .GroupBy(x => x.NodeId)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );
    }

    private  Dictionary<long, BpmnNodeSignUpConf> GetBpmnNodeSignUpConfMap(List<long> idList)
    {
        var data = _bpmnNodeSignUpConfService.baseRepo
            .Where(x => idList.Contains(x.BpmnNodeId) && x.IsDel==0)
            .ToList();

        return data.ToDictionary(o => o.BpmnNodeId, o => o);
    }
    private Dictionary<long, BpmnApproveRemindVo> GetBpmnApproveRemindVoMap(List<long> ids)
    {
        if (ids == null || !ids.Any())
        {
            return new Dictionary<long, BpmnApproveRemindVo>();
        }

        var bpmnApproveRemindList = _approveRemindService.baseRepo
            .Where(a=>ids.Contains(a.Id)&&a.IsDel==0);
            

        return bpmnApproveRemindList
            .ToDictionary(
                o => o.NodeId,
                o =>
                {
                    BpmnApproveRemindVo vo = o.MapToVo();
                    vo.IsInuse = false;
                    
                    var template = _informationTemplateService.baseRepo.Where(a => a.Id == o.TemplateId).ToOne();
                    vo.TemplateName = template.Name;
                    
                    if (!string.IsNullOrEmpty(vo.Days))
                    {
                        vo.DayList = vo.Days.Split(',')
                            .Select(int.Parse)
                            .ToList();
                    }
                    // 检查是否启用
                    if (vo.TemplateId!=null && !string.IsNullOrEmpty(vo.Days))
                    {
                        vo.IsInuse = true;
                    }

                    return vo;
                });
    }

    private Dictionary<long, List<BpmnTemplateVo>> GetBpmnTemplateVoMap(List<long> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return new Dictionary<long, List<BpmnTemplateVo>>();
        }

        return _bpmnTemplateService.baseRepo
            .Where(x => x.NodeId!=null&&ids.Contains(x.NodeId.Value) && x.IsDel==0)
            .ToList()
            .GroupBy(x => x.NodeId.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Select(o =>
                {
                    var vo = o.MapToVo();
                    BuildBpmnTemplateVo(vo);
                    return vo;
                }).ToList()
            );
    }
    private void BuildBpmnTemplateVo(BpmnTemplateVo vo)
    {
        vo.EventValue = EventTypeEnumExtensions.GetDescByCode(vo.Event);

        if (!string.IsNullOrEmpty(vo.Informs))
        {
            vo.InformIdList = vo.Informs.Split(',').ToList();
            vo.InformList = vo.InformIdList
                .Select(id => new BaseIdTranStruVo
                {
                    Id = id,
                    Name = EventTypeEnumExtensions.GetDescByCode(int.Parse(id))
                })
                .ToList();
        }

        if (!string.IsNullOrEmpty(vo.Emps))
        {
            vo.EmpIdList = vo.Emps.Split(',').ToList();
            var employeeInfo = _employeeInfoProviderService.ProvideEmployeeInfo(vo.EmpIdList);
            vo.EmpList = vo.EmpIdList
                .Select(id => new BaseIdTranStruVo
                {
                    Id = id,
                    Name = employeeInfo.ContainsKey(id) ? employeeInfo[id] : string.Empty
                })
                .ToList();
        }

        vo.TemplateName = _informationTemplateService.baseRepo
            .Where(a=>a.Id==vo.TemplateId).ToOne()?
            .Name ?? string.Empty;
    }

    private  Dictionary<long, List<BpmnNodeButtonConf>> GetBpmnNodeButtonConfMap(List<long> idList)
    {
        
        var data = _bpmnNodeButtonConfService.baseRepo
            .Where(x => idList.Contains(x.BpmnNodeId) && x.IsDel==0)
            .ToList();

        return data
            .GroupBy(x => x.BpmnNodeId)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );
    }

    private BpmnNodeVo GetBpmnNodeVo(BpmnNode bpmnNode, Dictionary<long, List<String>> bpmnNodeToMap, 
        Dictionary<long, List<BpmnNodeButtonConf>> bpmnNodeButtonConfMap,
        Dictionary<long, BpmnNodeSignUpConf> bpmnNodeSignUpConfMap,
        Dictionary<long, List<BpmnTemplateVo>> bpmnTemplateVoMap,
        Dictionary<long, BpmnApproveRemindVo> bpmnApproveRemindVoMap,
        Dictionary<long, List<BpmnNodeLfFormdataFieldControl>> lfFieldControlMap,
        String conditionsUrl)
    {
        BpmnNodeVo bpmnNodeVo = bpmnNode.MapToVo();
      
        long bpmnNodeId = bpmnNode.Id;
        //set nodeto
        bpmnNodeVo.NodeTo=bpmnNodeToMap.ContainsKey(bpmnNodeId)?bpmnNodeToMap[bpmnNodeId]:null;
        
        //set buttons conf
        SetButtons(bpmnNodeVo, bpmnNodeButtonConfMap[bpmnNodeId]);
        //assign property name
        bpmnNodeVo.NodePropertyName=NodePropertyEnumExtensions.GetDescByCode(bpmnNodeVo.NodeProperty);
        //set in node notice template
        bpmnNodeVo.TemplateVos=bpmnTemplateVoMap.ContainsKey(bpmnNodeId)?bpmnTemplateVoMap[bpmnNodeId]:null;
        //set in node approvement remind
        bpmnNodeVo.ApproveRemindVo=bpmnApproveRemindVoMap.ContainsKey(bpmnNodeId)?bpmnApproveRemindVoMap[bpmnNodeId]:null;
        BpmnNodeAdpConfEnum? bpmnNodeAdpConfEnum = GetBpmnNodeAdpConfEnum(bpmnNodeVo);
        if (bpmnNodeAdpConfEnum==null) {
            return bpmnNodeVo;
        }
        //get node adaptor
        BpmnNodeAdaptor bpmnNodeAdaptor = GetBpmnNodeAdaptor(bpmnNodeAdpConfEnum);

        //use adaptor to format nodevo
        bpmnNodeAdaptor.FormatToBpmnNodeVo(bpmnNodeVo);
        if ((int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS==bpmnNode.NodeType) {
            bpmnNodeVo.NodeType=(int)NodeTypeEnum.NODE_TYPE_CONDITIONS;
        }
        //set sign up conf
        SetBpmnNodeSignUpConf(bpmnNode, bpmnNodeSignUpConfMap, bpmnNodeVo);
        SetFieldControlVOs(bpmnNode,lfFieldControlMap,bpmnNodeVo);
        return bpmnNodeVo;
    }

    private void SetFieldControlVOs(
        BpmnNode bpmnNode, 
        Dictionary<long, List<BpmnNodeLfFormdataFieldControl>> fieldControlMap, 
        BpmnNodeVo nodeVo)
    {
        bool isLowFlow = bpmnNode.IsLowCodeFlow == 1;
        if (!isLowFlow)
        {
            return;
        }

        if (fieldControlMap == null || fieldControlMap.Count == 0)
        {
            return;
        }

        if (!fieldControlMap.TryGetValue(bpmnNode.Id, out var fieldControls) || fieldControls == null || fieldControls.Count == 0)
        {
            return;
        }

        var fieldControlVOs = fieldControls
            .Select(fieldControl => new LFFieldControlVO
            {
                FieldId = fieldControl.FieldId,
                FieldName = fieldControl.FieldName,
                Perm = fieldControl.Perm
            })
            .ToList();

        nodeVo.LfFieldControlVOs = fieldControlVOs;
    }

    private void SetBpmnNodeSignUpConf(
        BpmnNode bpmnNode, 
        Dictionary<long, BpmnNodeSignUpConf> bpmnNodeSignUpConfMap, 
        BpmnNodeVo bpmnNodeVo)
    {
        if (bpmnNode.IsSignUp != 1)
        {
            return;
        }

        if (!bpmnNodeSignUpConfMap.TryGetValue(bpmnNode.Id, out var bpmnNodeSignUpConf) || bpmnNodeSignUpConf == null)
        {
            return;
        }

        var propertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        propertysVo.AfterSignUpWay = bpmnNodeSignUpConf.AfterSignUpWay;
        propertysVo.SignUpType = bpmnNodeSignUpConf.SignUpType;

        bpmnNodeVo.Property = propertysVo;
    }

    private void SetButtons(BpmnNodeVo bpmnNodeVo, List<BpmnNodeButtonConf> bpmnNodeButtonConfs)
    {

        if (!ObjectUtils.IsEmpty(bpmnNodeButtonConfs))
        {

            BpmnNodeButtonConfBaseVo buttons = new BpmnNodeButtonConfBaseVo();


            buttons.StartPage = GetButtons(bpmnNodeButtonConfs, ButtonPageTypeEnum.INITIATE);


            buttons.ApprovalPage=GetButtons(bpmnNodeButtonConfs, ButtonPageTypeEnum.AUDIT);


            bpmnNodeVo.Buttons=buttons;

        }
    }
    private List<int> GetButtons(List<BpmnNodeButtonConf> bpmnNodeButtonConfs, ButtonPageTypeEnum buttonPageTypeEnum)
    {
        return bpmnNodeButtonConfs
            .Where(o => o.ButtonPageType == (int)buttonPageTypeEnum)
            .Select(o => o.ButtonType)
            .Distinct()
            .ToList();
    }
    private void SetViewPageButton(BpmnConfVo bpmnConfVo)
    {
        List<BpmnViewPageButton> bpmnViewPageButtons = _viewPageButtonService.baseRepo
            .Where(a => a.ConfId == bpmnConfVo.Id && a.IsDel == 0).ToList();
        

        BpmnViewPageButtonBaseVo bpmnViewPageButtonBaseVo = new BpmnViewPageButtonBaseVo();

        //start user's view page
        bpmnViewPageButtonBaseVo.ViewPageStart=GetViewPageButtonsByType(bpmnViewPageButtons, ViewPageTypeEnum.VIEW_PAGE_TYPE_START);

        //approver's view page
        bpmnViewPageButtonBaseVo.ViewPageOther=GetViewPageButtonsByType(bpmnViewPageButtons, ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER);

        //set view page buttons
        bpmnConfVo.ViewPageButtons=bpmnViewPageButtonBaseVo;

    }
    private List<int> GetViewPageButtonsByType(
        List<BpmnViewPageButton> bpmnViewPageButtons, 
        ViewPageTypeEnum viewPageTypeEnum)
    {
        return bpmnViewPageButtons
            .Where(o => o.ViewType == (int)viewPageTypeEnum)
            .Select(o => o.ButtonType)
            .ToList();
    }

    private string FormatOutSideFormCode(BpmnConfVo bpmnConfVo)
    {
       
        String formCode = bpmnConfVo.FormCode;

        return formCode.Substring(formCode.IndexOf("_") + 1);
    }

    public BpmnConfVo DetailByFormCode(String formCode)
    {

        BpmnConf bpmnConf = _bpmnConfService.baseRepo.Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .First();
        if(bpmnConf==null){
            throw new AFBizException("can not get a bpmnConf by provided formCode");
        }
        return GetBpmnConfVo(bpmnConf);
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
}