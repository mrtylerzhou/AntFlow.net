using System;
using System.Collections.Generic;

namespace AntFlowCore.Bpmn.Bpmn.bpmn;

public class TaskDefinition
{
    public String key { get; set; }
    // task listeners
    public Dictionary<String, List<TaskListener>> taskListeners = new Dictionary<string, List<TaskListener>>();
}