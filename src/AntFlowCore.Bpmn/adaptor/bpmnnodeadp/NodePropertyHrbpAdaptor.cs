using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.adaptor;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

 /// <summary>
    /// NodePropertyHrbpAdp Class for HRBP Node Properties
    /// </summary>
    public class NodePropertyHrbpAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly IBpmnNodeHrbpConfService _bpmnNodeHrbpConfService;
        private readonly ILogger<NodePropertyHrbpAdaptor> _logger;

        public NodePropertyHrbpAdaptor(IBpmnNodeHrbpConfService bpmnNodeHrbpConfService,
            IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
            IRoleService roleService,
            ILogger<NodePropertyHrbpAdaptor> logger) : base(bpmnNodeAdditionalSignConfService, roleService)
        {
            _bpmnNodeHrbpConfService = bpmnNodeHrbpConfService;
            _logger = logger;
        }

        public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            base.FormatToBpmnNodeVo(bpmnNodeVo);
            var bpmnNodeHrbpConf = _bpmnNodeHrbpConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (bpmnNodeHrbpConf != null)
            {
                AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p=>p.HrbpConfType= bpmnNodeHrbpConf.HrbpConfType);
               
            }
            
        }
        

        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            base.EditBpmnNode(bpmnNodeVo);
            var bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            var bpmnNodeHrbpConf = new BpmnNodeHrbpConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                HrbpConfType = bpmnNodePropertysVo.HrbpConfType,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName(),
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _bpmnNodeHrbpConfService.baseRepo.Insert(bpmnNodeHrbpConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_HRBP);
        }
    }