using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeAssignLevelConfRepository : RepositoryBase<BpmnNodeAssignLevelConf>, IBpmnNodeAssignLevelConfRepository
{
    public FsBpmnNodeAssignLevelConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
