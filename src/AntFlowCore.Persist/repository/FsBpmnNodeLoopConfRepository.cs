using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnNodeLoopConfRepository : RepositoryBase<BpmnNodeLoopConf>, IBpmnNodeLoopConfRepository
{
    public FsBpmnNodeLoopConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
