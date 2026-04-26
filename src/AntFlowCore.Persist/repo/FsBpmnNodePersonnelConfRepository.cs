using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodePersonnelConfRepository : RepositoryBase<BpmnNodePersonnelConf>, IBpmnNodePersonnelConfRepository
{
    public FsBpmnNodePersonnelConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
