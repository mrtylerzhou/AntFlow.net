using System.Collections.Generic;

namespace AntFlowCore.Bpmn.Bpmn.bpmn;

public interface IAFExecutionListeners
{
    List<AFActivitiListener> GetExecutionListeners();
}