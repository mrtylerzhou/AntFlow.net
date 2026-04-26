using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeConditionsParamConfRepository : RepositoryBase<BpmnNodeConditionsParamConf>, IBpmnNodeConditionsParamConfRepository
{
    public FsBpmnNodeConditionsParamConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
