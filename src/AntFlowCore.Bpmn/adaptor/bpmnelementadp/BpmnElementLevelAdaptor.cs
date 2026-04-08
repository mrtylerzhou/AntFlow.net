
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Extensions.Extensions.adaptor.bpmnelementadp;

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

