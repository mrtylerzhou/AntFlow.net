
using System.Collections.Generic;

namespace AntFlowCore.Bpmn.Bpmn.bpmn;

public abstract class AbstractEvent: AbstractFlowNode
{
    public List<AbstractEventDefinition> EventDefinitions { get; set; } = new List<AbstractEventDefinition>();
}