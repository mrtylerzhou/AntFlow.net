using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeCustomizeConfRepository : RepositoryBase<BpmnNodeCustomizeConf>, IBpmnNodeCustomizeConfRepository
{
    public FsBpmnNodeCustomizeConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
