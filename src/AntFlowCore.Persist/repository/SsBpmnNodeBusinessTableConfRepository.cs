using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeBusinessTableConfRepository : RepositoryBase<BpmnNodeBusinessTableConf>, IBpmnNodeBusinessTableConfRepository
{
    public SsBpmnNodeBusinessTableConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
