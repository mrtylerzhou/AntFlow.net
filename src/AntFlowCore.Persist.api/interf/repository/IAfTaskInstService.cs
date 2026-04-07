using AntFlowCore.Core.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAfTaskInstService : IBaseRepositoryService<BpmAfTaskInst>
{
    public int DoneTodayProcess(String createUserId);
    public int DoneCreateProcess(String createUserId);
}
