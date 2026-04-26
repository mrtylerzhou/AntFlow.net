using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeOutSideAccessConfRepository : RepositoryBase<BpmnNodeOutSideAccessConf>, IBpmnNodeOutSideAccessConfRepository
{
    public FsBpmnNodeOutSideAccessConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
