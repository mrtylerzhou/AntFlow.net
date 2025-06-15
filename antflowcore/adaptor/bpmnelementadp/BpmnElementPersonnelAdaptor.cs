using antflowcore.constant.enus;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

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

            var assigneeMap = new SortedDictionary<string, string>();
            var assignee = new List<string>();
            foreach (var assigneeVo in assigneeList)
            {
                if (assigneeVo.IsDeduplication == 0)
                {
                    assignee.Add(assigneeVo.Assignee);
                    assigneeMap[assigneeVo.Assignee] = assigneeVo.AssigneeName;
                }
            }

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