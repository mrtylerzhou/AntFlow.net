using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

/// <summary>
    /// NodePropertyLevelAdp Class for Level Node Properties
    /// </summary>
    public class NodePropertyLevelAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly BpmnNodeAssignLevelConfService _bpmnNodeAssignLevelConfService;
        private readonly ILogger<NodePropertyLevelAdaptor> _logger;

        public NodePropertyLevelAdaptor(
            BpmnNodeAssignLevelConfService bpmnNodeAssignLevelConfService, 
            BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
            IRoleService roleService,
            ILogger<NodePropertyLevelAdaptor> logger) : base(bpmnNodeAdditionalSignConfService, roleService)
        {
            _bpmnNodeAssignLevelConfService = bpmnNodeAssignLevelConfService;
            _logger = logger;
        }

        public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            base.FormatToBpmnNodeVo(bpmnNodeVo);
            BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = _bpmnNodeAssignLevelConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (bpmnNodeAssignLevelConf != null)
            {
                AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
                {
                    p.AssignLevelType = bpmnNodeAssignLevelConf.AssignLevelType;
                    p.AssignLevelGrade = bpmnNodeAssignLevelConf.AssignLevelGrade;
                });
            }

        }

       
        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            base.EditBpmnNode(bpmnNodeVo);
            BpmnNodePropertysVo bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = new BpmnNodeAssignLevelConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                AssignLevelType = bpmnNodePropertysVo.AssignLevelType,
                AssignLevelGrade = bpmnNodePropertysVo.AssignLevelGrade ?? 0,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName(),
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _bpmnNodeAssignLevelConfService.baseRepo.Insert(bpmnNodeAssignLevelConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LEVEL);
        }
    }