using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeAssignLevelConfRepository : RepositoryBase<BpmnNodeAssignLevelConf>, IBpmnNodeAssignLevelConfRepository
{
    public SsBpmnNodeAssignLevelConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
