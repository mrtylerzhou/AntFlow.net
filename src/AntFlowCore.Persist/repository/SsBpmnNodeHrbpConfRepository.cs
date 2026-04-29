using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeHrbpConfRepository : RepositoryBase<BpmnNodeHrbpConf>, IBpmnNodeHrbpConfRepository
{
    public SsBpmnNodeHrbpConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
