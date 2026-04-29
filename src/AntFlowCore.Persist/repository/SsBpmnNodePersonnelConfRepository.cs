using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodePersonnelConfRepository : RepositoryBase<BpmnNodePersonnelConf>, IBpmnNodePersonnelConfRepository
{
    public SsBpmnNodePersonnelConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
