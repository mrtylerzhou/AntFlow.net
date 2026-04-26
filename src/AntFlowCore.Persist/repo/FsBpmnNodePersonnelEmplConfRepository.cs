using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodePersonnelEmplConfRepository : RepositoryBase<BpmnNodePersonnelEmplConf>, IBpmnNodePersonnelEmplConfRepository
{
    public FsBpmnNodePersonnelEmplConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
