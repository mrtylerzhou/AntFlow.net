using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnNodePersonnelEmplConfRepository : RepositoryBase<BpmnNodePersonnelEmplConf>, IBpmnNodePersonnelEmplConfRepository
{
    public FsBpmnNodePersonnelEmplConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
