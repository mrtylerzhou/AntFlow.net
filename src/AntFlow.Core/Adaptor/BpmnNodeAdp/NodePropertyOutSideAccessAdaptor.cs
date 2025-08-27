using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

/// <summary>
///     NodePropertyOutSideAccessAdp
///     �ⲿ�ڵ��������������
/// </summary>
public class NodePropertyOutSideAccessAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodeOutSideAccessConfService _bpmnNodeOutSideAccessConfService;
    private readonly ILogger<NodePropertyOutSideAccessAdaptor> _logger;

    public NodePropertyOutSideAccessAdaptor(
        BpmnNodeOutSideAccessConfService bpmnNodeOutSideAccessConfService,
        ILogger<NodePropertyOutSideAccessAdaptor> logger)
    {
        _bpmnNodeOutSideAccessConfService = bpmnNodeOutSideAccessConfService;
        _logger = logger;
    }

    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodeOutSideAccessConf? nodeOutSideAccessConf = _bpmnNodeOutSideAccessConfService.baseRepo
            .Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

        if (nodeOutSideAccessConf != null)
        {
            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                SignType = nodeOutSideAccessConf.SignType, NodeMark = nodeOutSideAccessConf.NodeMark
            };

            bpmnNodeVo.OrderedNodeType = (int)OrderNodeTypeEnum.OUT_SIDE_NODE;
        }

        return bpmnNodeVo;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo? propertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        BpmnNodeOutSideAccessConf? nodeOutSideAccessConf = new()
        {
            BpmnNodeId = bpmnNodeVo.Id, SignType = propertysVo.SignType, NodeMark = propertysVo.NodeMark
        };

        _bpmnNodeOutSideAccessConfService.baseRepo.Insert(nodeOutSideAccessConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS);
    }
}