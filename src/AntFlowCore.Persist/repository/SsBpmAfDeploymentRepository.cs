using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmAfDeploymentRepository : RepositoryBase<BpmAfDeployment>, IBpmAfDeploymentRepository
{
    public SsBpmAfDeploymentRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmAfDeployment QueryDeploymentbyprocessNumber(string processNumber)
    {
        BpmAfDeployment bpmAfDeployment = Db.Queryable<BpmBusinessProcess>()
            .InnerJoin<BpmAfTask>((a, b) => a.ProcInstId == b.ProcInstId)
            .InnerJoin<BpmAfDeployment>((a, b, c) => b.ProcDefId == c.Id)
            .Where((a, b, c) => a.BusinessNumber == processNumber)
            .Select((a, b, c) => c)
            .First();
        return bpmAfDeployment;
    }

    public void UpdateDeployment(BpmAfDeployment deployment)
    {
        Db.Updateable(deployment).ExecuteCommand();
    }
}
