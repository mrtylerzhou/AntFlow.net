using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.bpmnnodeadp;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
    /// NodePropertyOutSideAccessAdp
    /// 外部节点访问属性适配器
    /// </summary>
    public class NodePropertyOutSideAccessAdaptor : IBpmnNodeAdaptor
    {
        private readonly IBpmnNodeOutSideAccessConfService _bpmnNodeOutSideAccessConfService;
        private readonly ILogger<NodePropertyOutSideAccessAdaptor> _logger;

        public NodePropertyOutSideAccessAdaptor(
            IBpmnNodeOutSideAccessConfService bpmnNodeOutSideAccessConfService,
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