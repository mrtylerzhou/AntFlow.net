using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;
using System.Collections.Concurrent;
using System.Text.Json;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Util;

public static class BpmnFlowUtil
{
    private static readonly ConcurrentDictionary<string, List<BpmnConfCommonElementVo>> cachedCommonElements = new();

    public static List<BpmnConfCommonElementVo> GetFirstAssigneeNodes(List<BpmnConfCommonElementVo> commonElements)
    {
        if (commonElements.Count == 0)
        {
            throw new AntFlowException.AFBizException("current process is empty");
        }

        BpmnConfCommonElementVo startEventNode = commonElements[0];
        if (startEventNode.ElementType != ElementTypeEnum.ELEMENT_TYPE_START_EVENT.Code)
        {
            throw new AntFlowException.AFBizException("logic error,please contact the administrator");
        }

        //skip the first element
        int prevIndex = 0;
        string startUserElementId = string.Empty;
        for (int i = 1; i < commonElements.Count; i++)
        {
            prevIndex = i - 1;
            BpmnConfCommonElementVo currentNode = commonElements[i];
            if (currentNode.ElementType == ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code &&
                currentNode.ElementName == StringConstants.START_USER_NODE_NAME)
            {
                startUserElementId = currentNode.ElementId;
            }

            List<BpmnConfCommonElementVo> firstAssigneeNodes = new();
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
                        if (gatewayFlowToAssigneeNodes.Count < 1 || gatewayFlowToAssigneeNodes.Any(a =>
                                a.ElementType != ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code))
                        {
                            throw new AntFlowException.AFBizException("logic error,please contact the administrator");
                        }

                        firstAssigneeNodes.AddRange(gatewayFlowToAssigneeNodes);
                    }
                    else
                    {
                        if (prevNode.ElementType != ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code)
                        {
                            throw new AntFlowException.AFBizException("logic error,please contact the administrator");
                        }

                        firstAssigneeNodes.Add(prevNode);
                    }

                    return firstAssigneeNodes;
                }
            }
        }

        throw new AntFlowException.AFBizException(
            "can not find  first assignee node,logic error,please contact the administrator");
    }

    public static BpmnConfCommonElementVo GetCurrentTaskElement(List<BpmnConfCommonElementVo> commonElements,
        string taskDefKey)
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

    public static (BpmnConfCommonElementVo assigneeNode, BpmnConfCommonElementVo flowNode) GetNextNodeAndFlowNode(
        List<BpmnConfCommonElementVo> commonElements, string currentTaskDefKey)
    {
        for (int i = 0; i < commonElements.Count; i++)
        {
            BpmnConfCommonElementVo elementVo = commonElements[i];
            if (elementVo.ElementType.Equals(ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code) &&
                elementVo.FlowFrom == currentTaskDefKey)
            {
                return (commonElements[i - 1], elementVo);
            }
        }

        throw new AntFlowException.AFBizException(
            "logic error,can not find next element,please contact the administrator");
    }

    /// <summary>
    ///     ��ȡ�ӵ�ǰ�ڵ������?����ǰ�ڵ���һ�ڵ�)����һ���ڵ�
    /// </summary>
    /// <param name="commonElements"></param>
    /// <param name="taskDefKey"></param>
    /// <returns></returns>
    public static BpmnConfCommonElementVo GetNodeFromCurrentNext(List<BpmnConfCommonElementVo> commonElements,
        string taskDefKey)
    {
        BpmnConfCommonElementVo? bpmnConfCommonElementVo = commonElements
            .Where(a => a.FlowFrom == taskDefKey)
            .SelectMany(a => commonElements.Where(x => x.ElementId == a.FlowTo)).ToList().FirstOrDefault();
        return bpmnConfCommonElementVo;
    }

    public static List<BpmnConfCommonElementVo> GetNodeFromCurrentNexts(List<BpmnConfCommonElementVo> commonElements,
        string taskDefKey)
    {
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = commonElements
            .Where(a => a.FlowFrom == taskDefKey)
            .SelectMany(a => commonElements.Where(x => x.ElementId == a.FlowTo)).ToList();
        return bpmnConfCommonElementVos;
    }

    public static BpmnConfCommonElementVo GetLastSequenceFlow(List<BpmnConfCommonElementVo> commonElements)
    {
        List<BpmnConfCommonElementVo> lastElementVos = commonElements.Where(a =>
            a.ElementType == ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code && a.IsLastSequenceFlow == 1).ToList();
        if (lastElementVos.Count > 1)
        {
            throw new AntFlowException.AFBizException("process flow can not have more than 1 end flow!");
        }

        return lastElementVos[0];
    }

    public static List<BpmnConfCommonElementVo> GetElementVosByDeployId(string deployId)
    {
        if (cachedCommonElements.Count > 100)
        {
            cachedCommonElements.Clear();
        }
        else if (cachedCommonElements.TryGetValue(deployId, out List<BpmnConfCommonElementVo>? vosByDeployId))
        {
            return vosByDeployId;
        }

        AFDeploymentService afDeploymentService = ServiceProviderUtils.GetService<AFDeploymentService>();
        BpmAfDeployment bpmAfDeployment = afDeploymentService.baseRepo.Where(a => a.Id == deployId).First();
        if (bpmAfDeployment == null)
        {
            throw new AntFlowException.AFBizException($"can not find deployment by id: {deployId}");
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        return elements;
    }
}