using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

 public class BpmnElementPersonnelAdaptor : BpmnElementAdaptor
    {
        protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo nodeParamsVo, int elementCode, string elementId)
        {
            var assigneeList = nodeParamsVo.AssigneeList;
            if (assigneeList == null || !assigneeList.Any())
            {
                Console.Error.WriteLine("Assignee list is empty or null.");
                return null; // Or handle the error as needed
            }

            string elementName = assigneeList[0].ElementName;

            string collectionName = "personnelList";

            int? signType = property.SignType;

            // Build assignee map respecting duplication strategy.
            // SKIP strategy: include deduplicated assignees (auto-skipped at runtime)
            // REMOVE strategy: exclude deduplicated assignees
            var startConditions = new BpmnStartConditionsVo();
            object strategyObj = ThreadLocalContainer.Get(StringConstants.DUPLICATION_PROCESS_STRATEGY);
            if (strategyObj is int strategy)
            {
                startConditions.DuplicationProcessStrategy = strategy;
            }
            var assigneeMap = AssigneeVoBuildUtils.DealingWithMultiPlayerNodeDuplication(nodeParamsVo, startConditions);

            string elementCodeStr = string.Join("",collectionName, elementCode);
            if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
            {
                return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
            else if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER)
            {
                return BpmnElementUtils.GetMultiplayerSignInOrderElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
            else
            {
                return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_PERSONNEL);
        }
    }
