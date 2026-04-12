using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyCustomizeAdaptor: AbstractAdditionSignNodeAdaptor
{
    private readonly IBpmnNodeCustomizeConfService _bpmnNodeCustomizeConfService;
    private readonly ILogger<NodePropertyCustomizeAdaptor> _logger;

    public NodePropertyCustomizeAdaptor(IBpmnNodeCustomizeConfService bpmnNodeCustomizeConfService,
        IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService,
        ILogger<NodePropertyCustomizeAdaptor> logger): base(bpmnNodeAdditionalSignConfService, roleService)
    {
        _bpmnNodeCustomizeConfService = bpmnNodeCustomizeConfService;
        _logger = logger;
    }
    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);
       List<BpmnNodeCustomizeConf> list=_bpmnNodeCustomizeConfService
           .baseRepo
           .Where(a=>a.BpmnNodeId == bpmnNodeVo.Id)
           .ToList();
       BpmnNodeCustomizeConf customizeConf = list[0];
       AfNodeUtils.AddOrEditProperty(bpmnNodeVo, a=>a.SignType= customizeConf.SignType);
      
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        base.EditBpmnNode(bpmnNodeVo);
        BpmnNodePropertysVo bpmnNodePropertysVo=bpmnNodeVo.Property??new BpmnNodePropertysVo();
        BpmnNodeCustomizeConf customizeConf = new BpmnNodeCustomizeConf()
        {
            BpmnNodeId = bpmnNodeVo.Id,
            SignType = bpmnNodePropertysVo.SignType,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmnNodeCustomizeConfService.baseRepo.Insert(customizeConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_CUSTOMIZE);
    }
}