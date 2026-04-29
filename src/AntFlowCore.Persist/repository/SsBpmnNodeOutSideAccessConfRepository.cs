using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeOutSideAccessConfRepository : RepositoryBase<BpmnNodeOutSideAccessConf>, IBpmnNodeOutSideAccessConfRepository
{
    public SsBpmnNodeOutSideAccessConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
