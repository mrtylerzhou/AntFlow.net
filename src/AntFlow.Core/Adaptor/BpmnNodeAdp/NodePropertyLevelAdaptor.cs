using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

/// <summary>
///     NodePropertyLevelAdp Class for Level Node Properties
/// </summary>
public class NodePropertyLevelAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodeAssignLevelConfService _bpmnNodeAssignLevelConfService;
    private readonly ILogger<NodePropertyLevelAdaptor> _logger;

    public NodePropertyLevelAdaptor(
        BpmnNodeAssignLevelConfService bpmnNodeAssignLevelConfService,
        ILogger<NodePropertyLevelAdaptor> logger)
    {
        _bpmnNodeAssignLevelConfService = bpmnNodeAssignLevelConfService;
        _logger = logger;
    }

    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = _bpmnNodeAssignLevelConfService.baseRepo
            .Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

        if (bpmnNodeAssignLevelConf != null)
        {
            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                AssignLevelType = bpmnNodeAssignLevelConf.AssignLevelType,
                AssignLevelGrade = bpmnNodeAssignLevelConf.AssignLevelGrade
            };
        }

        return bpmnNodeVo;
    }


    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = new()
        {
            BpmnNodeId = bpmnNodeVo.Id,
            AssignLevelType = bpmnNodePropertysVo.AssignLevelType,
            AssignLevelGrade = bpmnNodePropertysVo.AssignLevelGrade ?? 0,
            CreateTime = DateTime.Now,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };

        _bpmnNodeAssignLevelConfService.baseRepo.Insert(bpmnNodeAssignLevelConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LEVEL);
    }
}