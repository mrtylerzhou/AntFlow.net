using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

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

