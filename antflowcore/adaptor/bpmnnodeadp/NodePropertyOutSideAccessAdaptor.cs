using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

/// <summary>
    /// NodePropertyOutSideAccessAdp
    /// 外部节点访问属性适配器
    /// </summary>
    public class NodePropertyOutSideAccessAdaptor : IBpmnNodeAdaptor
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

        public  void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            var nodeOutSideAccessConf = _bpmnNodeOutSideAccessConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (nodeOutSideAccessConf != null)
            {
                bpmnNodeVo.Property = new BpmnNodePropertysVo
                {
                    SignType = nodeOutSideAccessConf.SignType,
                    NodeMark = nodeOutSideAccessConf.NodeMark
                };

                bpmnNodeVo.OrderedNodeType = (int)OrderNodeTypeEnum.OUT_SIDE_NODE;
            }
            
        }

        public  void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            var propertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            var nodeOutSideAccessConf = new BpmnNodeOutSideAccessConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                SignType = propertysVo.SignType,
                NodeMark = propertysVo.NodeMark
            };

            _bpmnNodeOutSideAccessConfService.baseRepo.Insert(nodeOutSideAccessConf);
        }
        
        public  void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS);
        }
    }