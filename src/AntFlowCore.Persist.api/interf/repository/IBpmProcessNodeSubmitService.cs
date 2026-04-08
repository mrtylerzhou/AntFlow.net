using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNodeSubmitService : IBaseRepositoryService<BpmProcessNodeSubmit>
{
  
    BpmProcessNodeSubmit? FindBpmProcessNodeSubmit(string processInstanceId);
    bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit);
}
