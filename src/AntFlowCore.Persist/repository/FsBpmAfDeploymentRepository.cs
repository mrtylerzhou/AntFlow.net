using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repositorysitory;

public class FsBpmAfDeploymentRepository: RepositoryBase<BpmAfDeployment>, IBpmAfDeploymentRepository
{
    public FsBpmAfDeploymentRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmAfDeployment QueryDeploymentbyprocessNumber(string processNumber)
    {
        BpmAfDeployment bpmAfDeployment = this._ormContext.FreeSql
            .Select<BpmBusinessProcess,BpmAfTask,BpmAfDeployment>()
            .InnerJoin((a,b,c)=>a.ProcInstId==b.ProcInstId)
            .InnerJoin((a,b,c)=>b.ProcDefId==c.Id)
            .Where((a,b,c)=>a.BusinessNumber==processNumber)
            .ToList<BpmAfDeployment>((a,b,c)=>c)
            .First();
        return bpmAfDeployment;
    }

    public void UpdateDeployment(BpmAfDeployment deployment)
    {
        _ormContext.FreeSql.Update<BpmAfDeployment>().SetSource(deployment).ExecuteAffrows();
    }
}
