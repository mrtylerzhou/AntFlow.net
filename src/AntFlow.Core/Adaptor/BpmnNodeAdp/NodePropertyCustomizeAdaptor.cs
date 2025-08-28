using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

public class NodePropertyCustomizeAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodeCustomizeConfService _bpmnNodeCustomizeConfService;
    private readonly ILogger<NodePropertyCustomizeAdaptor> _logger;

    public NodePropertyCustomizeAdaptor(BpmnNodeCustomizeConfService bpmnNodeCustomizeConfService,
        ILogger<NodePropertyCustomizeAdaptor> logger)
    {
        _bpmnNodeCustomizeConfService = bpmnNodeCustomizeConfService;
        _logger = logger;
    }

    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        List<BpmnNodeCustomizeConf> list = _bpmnNodeCustomizeConfService
            .baseRepo
            .Where(a => a.BpmnNodeId == bpmnNodeVo.Id)
            .ToList();
        BpmnNodeCustomizeConf customizeConf = list[0];
        bpmnNodeVo.Property = new BpmnNodePropertysVo { SignType = customizeConf.SignType };
        return bpmnNodeVo;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();
        BpmnNodeCustomizeConf customizeConf = new()
        {
            BpmnNodeId = bpmnNodeVo.Id,
            SignType = bpmnNodePropertysVo.SignType,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };
        _bpmnNodeCustomizeConfService.baseRepo.Insert(customizeConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_CUSTOMIZE);
    }
}