using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnConfLfFormdataRepository : RepositoryBase<BpmnConfLfFormdata>, IBpmnConfLfFormdataRepository
{
    public FsBpmnConfLfFormdataRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
