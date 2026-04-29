using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeConditionsParamConfRepository : RepositoryBase<BpmnNodeConditionsParamConf>, IBpmnNodeConditionsParamConfRepository
{
    public SsBpmnNodeConditionsParamConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
