using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

/// <summary>
///     NodePropertyHrbpAdp Class for HRBP Node Properties
/// </summary>
public class NodePropertyHrbpAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodeHrbpConfService _bpmnNodeHrbpConfService;
    private readonly ILogger<NodePropertyHrbpAdaptor> _logger;

    public NodePropertyHrbpAdaptor(BpmnNodeHrbpConfService bpmnNodeHrbpConfService,
        ILogger<NodePropertyHrbpAdaptor> logger)
    {
        _bpmnNodeHrbpConfService = bpmnNodeHrbpConfService;
        _logger = logger;
    }

    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodeHrbpConf? bpmnNodeHrbpConf =
            _bpmnNodeHrbpConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

        if (bpmnNodeHrbpConf != null)
        {
            bpmnNodeVo.Property = new BpmnNodePropertysVo { HrbpConfType = bpmnNodeHrbpConf.HrbpConfType };
        }

        return bpmnNodeVo;
    }


    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo? bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        BpmnNodeHrbpConf? bpmnNodeHrbpConf = new()
        {
            BpmnNodeId = bpmnNodeVo.Id,
            HrbpConfType = bpmnNodePropertysVo.HrbpConfType,
            CreateTime = DateTime.Now,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };

        _bpmnNodeHrbpConfService.baseRepo.Insert(bpmnNodeHrbpConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_HRBP);
    }
}