using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmAfDeploymentRepository: IBaseRepository<BpmAfDeployment>
{
    public BpmAfDeployment QueryDeploymentbyprocessNumber(string processNumber);
    void UpdateDeployment(BpmAfDeployment deployment);
}
