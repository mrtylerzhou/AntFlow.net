using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

/// <summary>
    /// NodePropertyLevelAdp Class for Level Node Properties
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
            BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = _bpmnNodeAssignLevelConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

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

            BpmnNodeAssignLevelConf bpmnNodeAssignLevelConf = new BpmnNodeAssignLevelConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                AssignLevelType = bpmnNodePropertysVo.AssignLevelType,
                AssignLevelGrade = bpmnNodePropertysVo.AssignLevelGrade ?? 0,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName()
            };

            _bpmnNodeAssignLevelConfService.baseRepo.Insert(bpmnNodeAssignLevelConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LEVEL);
        }
    }