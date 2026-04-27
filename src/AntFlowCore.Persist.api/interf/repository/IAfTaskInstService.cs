using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAfTaskInstService : IAntFlowRepositoryMix<BpmAfTaskInst, IBpmAfTaskInstRepository>
{
    public int DoneTodayProcess(String createUserId);
    public int DoneCreateProcess(String createUserId);
}
