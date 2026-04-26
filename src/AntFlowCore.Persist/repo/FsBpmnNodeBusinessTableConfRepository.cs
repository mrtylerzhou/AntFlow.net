using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeBusinessTableConfRepository : RepositoryBase<BpmnNodeBusinessTableConf>, IBpmnNodeBusinessTableConfRepository
{
    public FsBpmnNodeBusinessTableConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
