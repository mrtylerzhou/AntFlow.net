using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.exception;
using AntFlowCore.Vo;

namespace antflowcore.util;

public static class BpmnFlowUtil
{
    public static List<BpmnConfCommonElementVo> GetFirstAssigneeNodes(List<BpmnConfCommonElementVo> commonElements)
    {
        if (commonElements.Count == 0)
        {
            throw new AFBizException("current process is empty");
        }

        BpmnConfCommonElementVo startEventNode = commonElements[0];
        if (startEventNode.ElementType != ElementTypeEnum.ELEMENT_TYPE_START_EVENT.Code)
        {
            throw new AFBizException("logic error,please contact the administrator");
        }
        //skip the first element
        int prevIndex = 0;
        string startUserElementId=string.Empty;
        for (var i = 1; i < commonElements.Count; i++)
        {
            prevIndex = i - 1;
            BpmnConfCommonElementVo currentNode = commonElements[i];
            if (currentNode.ElementType == ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code &&
                currentNode.ElementName == StringConstants.START_USER_NODE_NAME)
            {
                startUserElementId = currentNode.ElementId;
            }

            List<BpmnConfCommonElementVo> firstAssigneeNodes = new List<BpmnConfCommonElementVo>();
            if (!string.IsNullOrEmpty(startUserElementId))
            {
                //the first assignee node flows from the node which flow from the first user node
                if (currentNode.FlowFrom == startUserElementId)
                {
                    BpmnConfCommonElementVo prevNode = commonElements[prevIndex];
                    if (prevNode.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code)
                    {
                        List<BpmnConfCommonElementVo> gatewayFlowToAssigneeNodes = commonElements
                            .Where(a => a.FlowFrom == prevNode.ElementId)
                            .SelectMany(a => commonElements.Where(x => x.ElementId == a.FlowTo))
                            .ToList();
                        if(gatewayFlowToAssigneeNodes.Count<1||gatewayFlowToAssigneeNodes.Any(a=>a.ElementType!=ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code))
                        {
                            throw new AFBizException("logic error,please contact the administrator");
                        }
                        firstAssigneeNodes.AddRange(gatewayFlowToAssigneeNodes);
                    }
                    else
                    {
                        if (prevNode.ElementType!=ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code)
                        {
                            throw new AFBizException("logic error,please contact the administrator");
                        }
                        firstAssigneeNodes.Add(prevNode);
                    }
                    return firstAssigneeNodes;
                }
            }
        } 
        throw new AFBizException("can not find  first assignee node,logic error,please contact the administrator");
    }

    public static BpmnConfCommonElementVo GetCurrentTaskElement(List<BpmnConfCommonElementVo> commonElements,string taskDefKey )
    {
        foreach (BpmnConfCommonElementVo bpmnConfCommonElementVo in commonElements)
        {
            if (taskDefKey == bpmnConfCommonElementVo.ElementId)
            {
                return bpmnConfCommonElementVo;
            }
        }
        return null;
    }
    public static (BpmnConfCommonElementVo assigneeNode,BpmnConfCommonElementVo flowNode) GetNextAssigneeAndFlowNode(List<BpmnConfCommonElementVo> commonElements,string currentTaskDefKey)
    {
        for (var i = 0; i < commonElements.Count; i++)
        {
            BpmnConfCommonElementVo elementVo = commonElements[i];
            if (elementVo.ElementType.Equals(ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code) &&
                elementVo.FlowFrom == currentTaskDefKey)
            {
                return ( commonElements[i-1],elementVo);
            }
        }

        throw new AFBizException("logic error,can not find next element,please contact the administrator");
    }

    public static BpmnConfCommonElementVo GetLastSequenceFlow(List<BpmnConfCommonElementVo> commonElements)
    {
        List<BpmnConfCommonElementVo> lastElementVos = commonElements.Where(a=>a.ElementType==ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code&&a.IsLastSequenceFlow==1).ToList();
        if (lastElementVos.Count > 1)
        {
            throw new AFBizException("process flow can not have more than 1 end flow!");
        }

        return lastElementVos[0];
    }
}