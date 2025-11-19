using antflowcore.constant.enus;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;


    public class BpmnElementCustomizeAdaptor : BpmnElementAdaptor
    {
        private readonly ILogger<BpmnElementCustomizeAdaptor> _logger;

        public BpmnElementCustomizeAdaptor(ILogger<BpmnElementCustomizeAdaptor> logger)
        {
            _logger = logger;
        }

        protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
        {
            var assigneeList = paramsVo.AssigneeList;
            string elementName = assigneeList[0].ElementName;

            string collectionName = "customizeUser";
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

            string joinedCollectionName = $"{collectionName}{elementCode}";

            if ((int)SignTypeEnum.SIGN_TYPE_SIGN == signType)
            {
                return BpmnElementUtils.GetMultiplayerSignElement(
                    elementId,
                    elementName,
                    joinedCollectionName,
                    assigneeMap.Keys.ToList(),
                    assigneeMap
                );
            }
            else if ((int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == signType)
            {
                return BpmnElementUtils.GetMultiplayerSignInOrderElement(
                    elementId,
                    elementName,
                    joinedCollectionName,
                    assigneeMap.Keys.ToList(),
                    assigneeMap
                );
            }
            else
            {
                return BpmnElementUtils.GetMultiplayerOrSignElement(
                    elementId,
                    elementName,
                    joinedCollectionName,
                    assigneeMap.Keys.ToList(),
                    assigneeMap
                );
            }
        }

       

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE);
        }
    }
