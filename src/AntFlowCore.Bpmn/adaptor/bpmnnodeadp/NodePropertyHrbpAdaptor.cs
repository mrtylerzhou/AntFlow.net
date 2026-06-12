using System.Text.Json;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

 /// <summary>
    /// NodePropertyHrbpAdp Class for HRBP Node Properties
    /// </summary>
    public class NodePropertyHrbpAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly ILogger<NodePropertyHrbpAdaptor> _logger;

        public NodePropertyHrbpAdaptor(
            IRoleService roleService,
            ILogger<NodePropertyHrbpAdaptor> logger) : base(roleService)
        {
            _logger = logger;
        }

        public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            base.FormatToBpmnNodeVo(bpmnNodeVo);

            // Prefer JSON config if available
            var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
            if (nodeConfig?.ApproverConf?.HrbpConf != null)
            {
                AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p => p.HrbpConfType = nodeConfig.ApproverConf.HrbpConf.HrbpConfType);
                return;
            }

            throw new AFBizException("migration error,please contact the author");
        }

        public PersonnelRuleVo FormaFieldAttributeInfoVO()
        {
            return null;
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_HRBP);
        }
    }
