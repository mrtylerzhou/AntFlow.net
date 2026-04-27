using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnNodeHrbpConfRepository : RepositoryBase<BpmnNodeHrbpConf>, IBpmnNodeHrbpConfRepository
{
    public FsBpmnNodeHrbpConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
