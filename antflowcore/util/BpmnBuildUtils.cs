using AntFlowCore.Vo;

namespace antflowcore.util;

using antflowcore.bpmn.model;
using System.Collections.Generic;

public static class BpmnBuildUtils
{
    private const string EventTake = "take";
    private const string EventCreate = "create";

    public static StartEvent CreateStartEvent(string startEventId, string startEventName)
    {
        return new StartEvent
        {
            Id = startEventId,
            Name = startEventName
        };
    }

    public static UserTask CreateUserTask(string id, string name, string assigneeParamName)
    {
        var userTask = new UserTask
        {
            Id = id,
            Name = name,
            Assignee = FormatParamName(assigneeParamName)
        };
        SetTaskListener(userTask);
        return userTask;
    }

    public static UserTask CreateSignUserTask(string id, string name, string collectionName, string elementVariableName)
    {
        return CreateMultiplayerUserTask(id, name, collectionName, elementVariableName, false);
    }

    public static UserTask CreateOrSignUserTask(string id, string name, string collectionName, string elementVariableName)
    {
        return CreateMultiplayerUserTask(id, name, collectionName, elementVariableName, true);
    }

    private static UserTask CreateMultiplayerUserTask(string id, string name, string collectionName, string elementVariableName, bool isOrSign)
    {
        var userTask = new UserTask
        {
            Id = id,
            Name = name,
            LoopCharacteristics = new MultiInstanceLoopCharacteristics
            {
                Sequential = false,
                InputDataItem = FormatParamName(collectionName),
                ElementVariable = elementVariableName,
                CompletionCondition = isOrSign ? "${nrOfCompletedInstances >= 1 }" : null
            },
            Assignee = FormatParamName(elementVariableName)
        };
        SetTaskListener(userTask);
        return userTask;
    }

    public static UserTask CreateLoopUserTask(string id, string name, string assigneeParamName, string endLoopMark)
    {
        var userTask = new UserTask
        {
            Id = id,
            Name = name,
            LoopCharacteristics = new MultiInstanceLoopCharacteristics
            {
                Sequential = true,
                LoopCardinality = "10",
                CompletionCondition = FormatParamName(endLoopMark)
            },
            Assignee = FormatParamName(assigneeParamName)
        };
        SetTaskListener(userTask);
        return userTask;
    }

    public static UserTask CreateLoopUserTask(string id, string name, string loopCardinality, string collectionName, string elementVariableName)
    {
        var userTask = new UserTask
        {
            Id = id,
            Name = name,
            LoopCharacteristics = new MultiInstanceLoopCharacteristics
            {
                Sequential = true,
                LoopCardinality = FormatParamName(loopCardinality),
                InputDataItem = FormatParamName(collectionName),
                ElementVariable = elementVariableName
            },
            Assignee = FormatParamName(elementVariableName)
        };
        SetTaskListener(userTask);
        return userTask;
    }

    public static AfExclusiveGateway CreateExclusiveGateway(string id)
    {
        return new AfExclusiveGateway() { Id = id };
    }

    public static AFParallelGateway CreateParallelGateway(string id)
    {
        return new AFParallelGateway() { Id = id };
    }

    public static AFSequenceFlow CreateSequenceFlow(BpmnConfCommonElementVo elementVo)
    {
        var flow = new AFSequenceFlow()
        {
            Id = elementVo.ElementId,
            SourceRef = elementVo.FlowFrom,
            TargetRef = elementVo.FlowTo,
            ExecutionListeners = elementVo.IsLastSequenceFlow == 1
                ? new List<AFActivitiListener> { GetActivitiListener(EventTake, "${bpmnExecutionListener}") }
                : null
        };
        return flow;
    }

    public static AFSequenceFlow CreateSequenceFlow(string flowId, string from, string to, string conditionExpression)
    {
        return new AFSequenceFlow()
        {
            Id = flowId,
            SourceRef = from,
            TargetRef = to,
            ConditionExpression = FormatParamName(conditionExpression)
        };
    }

    public static EndEvent CreateEndEvent(string endEventId, string endEventName)
    {
        return new EndEvent
        {
            Id = endEventId,
            Name = endEventName
        };
    }

    private static void SetTaskListener(UserTask userTask)
    {
        userTask.TaskListeners = new List<AFActivitiListener>
            {
                GetActivitiListener(EventCreate, "${bpmnTaskListener}")
            };
    }

    private static AFActivitiListener GetActivitiListener(string eventName, string implementation)
    {
        return new AFActivitiListener()
        {
            Evt = eventName,
            ImplementationType = "delegateExpression",
            Implementation = implementation
        };
    }

    private static string FormatParamName(string paramName)
    {
        return $"${{{paramName}}}";
    }
}