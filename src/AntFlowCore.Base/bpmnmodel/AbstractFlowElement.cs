using AntFlowCore.Bpmn.Bpmn.bpmn;

namespace AntFlowCore.Core.bpmnmodel;

public abstract class AbstractFlowElement: AFAbstractBaseElement,IAFExecutionListeners
{
   public String Name { get; set; }
   public String Documentation { get; set; }
   public List<AFActivitiListener> ExecutionListeners = new List<AFActivitiListener>();
   public List<AFActivitiListener> GetExecutionListeners()
    {
        return ExecutionListeners;
    }
}