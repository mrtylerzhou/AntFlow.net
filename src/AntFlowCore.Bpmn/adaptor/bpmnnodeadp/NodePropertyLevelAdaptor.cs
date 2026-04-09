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
    /// NodePropertyLevelAdp Class for Level Node Properties
    /// </summary>
    public class NodePropertyLevelAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly IBpmnNodeAssignLevelConfService _bpmnNodeAssignLevelConfService;
        private readonly ILogger<NodePropertyLevelAdaptor> _logger;

        public NodePropertyLevelAdaptor(
            IBpmnNodeAssignLevelConfService bpmnNodeAssignLevelConfService, 
            IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
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