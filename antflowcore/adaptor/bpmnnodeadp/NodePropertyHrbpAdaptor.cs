using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

 /// <summary>
    /// NodePropertyHrbpAdp Class for HRBP Node Properties
    /// </summary>
    public class NodePropertyHrbpAdaptor : BpmnNodeAdaptor
    {
        private readonly BpmnNodeHrbpConfService _bpmnNodeHrbpConfService;
        private readonly ILogger<NodePropertyHrbpAdaptor> _logger;

        public NodePropertyHrbpAdaptor(BpmnNodeHrbpConfService bpmnNodeHrbpConfService, ILogger<NodePropertyHrbpAdaptor> logger)
        {
            _bpmnNodeHrbpConfService = bpmnNodeHrbpConfService;
            _logger = logger;
        }

        public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            var bpmnNodeHrbpConf = _bpmnNodeHrbpConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (bpmnNodeHrbpConf != null)
            {
                bpmnNodeVo.Property = new BpmnNodePropertysVo
                {
                    HrbpConfType = bpmnNodeHrbpConf.HrbpConfType
                };
            }

            return bpmnNodeVo;
        }
        

        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            var bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            var bpmnNodeHrbpConf = new BpmnNodeHrbpConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                HrbpConfType = bpmnNodePropertysVo.HrbpConfType,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName()
            };

            _bpmnNodeHrbpConfService.baseRepo.Insert(bpmnNodeHrbpConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_HRBP);
        }
    }