using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

public class NodePropertyCustomizeAdaptor: AbstractAdditionSignNodeAdaptor
{
    private readonly BpmnNodeCustomizeConfService _bpmnNodeCustomizeConfService;
    private readonly ILogger<NodePropertyCustomizeAdaptor> _logger;

    public NodePropertyCustomizeAdaptor(BpmnNodeCustomizeConfService bpmnNodeCustomizeConfService,
        BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
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