using antflowcore.entity;

namespace antflowcore.service.interf;

public interface IBpmBusinessProcessService
{
    BpmBusinessProcess GetBpmBusinessProcess(string processNumber);
    BpmBusinessProcess GetBpmBusinessProcessByProcInstId(string procinstId);
    void Update(BpmBusinessProcess bpmBusinessProcess);
    void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess);
}