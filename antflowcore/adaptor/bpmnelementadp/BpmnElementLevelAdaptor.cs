using antflowcore.constant.enus;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

    public class BpmnElementLevelAdaptor : BpmnElementAdaptor
    {
        private readonly ILogger<BpmnElementLevelAdaptor> _logger;

        public BpmnElementLevelAdaptor(ILogger<BpmnElementLevelAdaptor> logger)
        {
            _logger = logger;
        }

        protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
        {
            // Get the assignee and assignee name or initialize with default values
            var bpmnNodeParamsAssigneeVo = paramsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();
            string assignee = bpmnNodeParamsAssigneeVo.Assignee;
            string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;

            var singleAssigneeMap = new Dictionary<string, string>
            {
                { assignee, assigneeName }
            };

            return BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
                $"levelUser{elementCode}", assignee, singleAssigneeMap);
        }

        public override void SetSupportBusinessObjects()
        {
           ((IAdaptorService)this). AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_LEVEL);
        }
    }

