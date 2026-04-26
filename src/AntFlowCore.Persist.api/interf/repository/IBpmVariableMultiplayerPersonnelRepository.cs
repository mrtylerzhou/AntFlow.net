using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVariableMultiplayerPersonnelRepository : IBaseRepository<BpmVariableMultiplayerPersonnel>
{
    void Undertake(string processNumber, string taskTaskDefKey);
    void UpdateAssignee(long id, string assignee, string assigneeName, string remark);
}
