using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.adaptor;

 /// <summary>
    /// Abstract personnel adaptor to find personnel.
    /// AntFlow mainly has three extension points: node adaptor, element adaptor, and personnel adaptor.
    /// </summary>
    public abstract class AbstractBpmnPersonnelAdaptor: IAdaptorService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
        private readonly IBpmnPersonnelProviderService _bpmnPersonnelProviderService;

        public AbstractBpmnPersonnelAdaptor(
            IBpmnPersonnelProviderService bpmnPersonnelProviderService,
            IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService
            )
        {
            _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService?? throw new ArgumentNullException(nameof(bpmnEmployeeInfoProviderService));
            _bpmnPersonnelProviderService = bpmnPersonnelProviderService?? throw new ArgumentNullException(nameof(bpmnPersonnelProviderService));
        }

        public void SetNodeParams(
            BpmnNodeVo nodeVo, 
            BpmnStartConditionsVo startConditionsVo, 
            BpmnNodeParamTypeEnum nodeParamTypeEnum, 
            string nextId, 
            Dictionary<string, BpmnNodeVo> mapPreNodes, 
            HashSet<BpmnNodeVo> setAddNodes)
        {
            if (nodeVo == null)
            {
                throw new InvalidOperationException("nodeVo cannot be null if you want to set properties to it.");
            }

            if (nodeParamTypeEnum == null)
            {
                throw new InvalidOperationException("Parameter nodeParamTypeEnum cannot be null.");
            }

            var paramsVo = nodeVo.Params;
            var nodeName = nodeVo.NodeName;

            if (paramsVo == null)
            {
                throw new InvalidOperationException("Parameter paramsVo cannot be null.");
            }

            var orderedNodeType = nodeVo.OrderedNodeType;
            if (orderedNodeType.HasValue)
            {
                var orderNodeTypeEnum = OrderNodeTypeEnumExtensions.GetByCode(orderedNodeType.Value);
                var abstractOrderedSignNodeAdps = ServiceProviderUtils.GetServices<AbstractOrderedSignNodeAdp>();
                AbstractOrderedSignNodeAdp orderedSignNodeAdp = null;

                foreach (var abstractOrderedSignNodeAdp in abstractOrderedSignNodeAdps)
                {
                    if (((IAdaptorService)abstractOrderedSignNodeAdp).IsSupportBusinessObject(orderNodeTypeEnum))
                    {
                        orderedSignNodeAdp = abstractOrderedSignNodeAdp;
                        break;
                    }
                }

                orderedSignNodeAdp?.FormatNodes(nodeVo, startConditionsVo, nextId, mapPreNodes, setAddNodes);
                return;
            }

            paramsVo.ParamType = (int)nodeParamTypeEnum;
            int approvalStandard = nodeVo.ApprovalStandard;
            if (ApprovalStandardEnum.FROM_PREV_NODE.Code == approvalStandard
                || (nodeVo.NodeProperty.HasValue
                    && nodeVo.NodeProperty.Value == (int)NodePropertyEnum.NODE_PROPERTY_PREV_NODE_RELATED))
            {
                BpmnNodePropertysVo property = nodeVo.Property;
                if(property==null){

                }else
                {
                    BpmnNodeVo bpmnNodeVo = mapPreNodes[nodeVo.NodeId];
                    if(bpmnNodeVo==null){

                    }else
                    {
                        List<BaseIdTranStruVo> emplList = bpmnNodeVo.Property.EmplList;
                        property.ContextEmplList = emplList;
                    }
                }
            }

            // 上一节点指定审批人: 当前节点有 prev_node_appointed 标签时, 给上一节点贴 appoint_next_node_approver 标签
            // 上一节点审批页根据该标签渲染[指定下一节点审批人]按钮
            List<BpmnNodeLabelVO> currentLabels = nodeVo.LabelList;
            if (currentLabels != null && currentLabels.Count > 0)
            {
                bool hasPrevNodeAppointedLabel = currentLabels
                    .Any(l => StringConstants.AF_SYSLABEL_PREV_NODE_APPOINTED.Equals(l.LabelValue));
                if (hasPrevNodeAppointedLabel)
                {
                    if (mapPreNodes != null && mapPreNodes.TryGetValue(nodeVo.NodeId, out var prevNode) && prevNode != null)
                    {
                        List<BpmnNodeLabelVO> prevLabels = prevNode.LabelList;
                        bool prevHasLabel = prevLabels != null && prevLabels.Count > 0
                            && prevLabels.Any(l => StringConstants.AF_SYSLABEL_APPOINT_NEXT_NODE_APPROVER.Equals(l.LabelValue));
                        if (!prevHasLabel)
                        {
                            prevNode.SetOrAddLabelList(NodeLabelConstants.AppointNextNodeApprover);
                        }
                    }
                }
            }

            List<BpmnNodeParamsAssigneeVo> assigneeList = AssigneeListUniq(
                _bpmnPersonnelProviderService.GetAssigneeList(nodeVo, startConditionsVo));
            SetAssigneeOrList(paramsVo, assigneeList, nodeParamTypeEnum);
            SetEmployeeName(assigneeList, nodeName);
        }

        private void SetEmployeeName(List<BpmnNodeParamsAssigneeVo> assigneeList, string nodeName)
        {
            var ids = assigneeList.Select(a => a.Assignee).ToList();
            var empIdNameMap = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(ids);

            foreach (var assigneeVo in assigneeList)
            {
                if (empIdNameMap.TryGetValue(assigneeVo.Assignee, out var value))
                {
                    assigneeVo.AssigneeName = value;
                }
            }
        }

        private void SetAssigneeOrList(BpmnNodeParamsVo paramsVo, 
                                       List<BpmnNodeParamsAssigneeVo> assigneeList, 
                                       BpmnNodeParamTypeEnum nodeParamTypeEnum)
        {
            if (paramsVo == null)
            {
                throw new InvalidOperationException("Parameter paramsVo cannot be null.");
            }

            if (assigneeList == null || !assigneeList.Any())
            {
                throw new InvalidOperationException("The assignee list must contain personnel information!");
            }

            if (nodeParamTypeEnum == BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE)
            {
                paramsVo.Assignee = assigneeList.First();
            }
            else
            {
                paramsVo.AssigneeList = assigneeList;
            }
        }

        private List<BpmnNodeParamsAssigneeVo> AssigneeListUniq(List<BpmnNodeParamsAssigneeVo> paramsList)
        {
            var result = new List<BpmnNodeParamsAssigneeVo>();
            if (paramsList == null || !paramsList.Any())
            {
                return result;
            }

            var uniqList = new HashSet<string>();
            foreach (var vo in paramsList)
            {
                if (!uniqList.Contains(vo.Assignee))
                {
                    result.Add(vo);
                }
                uniqList.Add(vo.Assignee);
            }

            return result;
        }

        public abstract void SetSupportBusinessObjects();
    }