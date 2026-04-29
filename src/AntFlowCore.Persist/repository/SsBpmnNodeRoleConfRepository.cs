using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeRoleConfRepository : RepositoryBase<BpmnNodeRoleConf>, IBpmnNodeRoleConfRepository
{
    public SsBpmnNodeRoleConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
