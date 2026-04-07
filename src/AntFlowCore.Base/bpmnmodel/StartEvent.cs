using System;
using System.Collections.Generic;

namespace AntFlowCore.Bpmn.Bpmn.bpmn;

public class StartEvent: AbstractEvent
{
   public String Initiator { get; set; }
   public String FormCode{ get; set; }
   public List<AFFormProperty> FormProperties { get; set; } = new List<AFFormProperty>();
}