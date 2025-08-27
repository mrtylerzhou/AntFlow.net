using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Interface;

public interface IBpmBusinessProcessService
{
    BpmBusinessProcess GetBpmBusinessProcess(string processNumber);
    BpmBusinessProcess GetBpmBusinessProcessByProcInstId(string procinstId);
    void Update(BpmBusinessProcess bpmBusinessProcess);
    void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess);
}