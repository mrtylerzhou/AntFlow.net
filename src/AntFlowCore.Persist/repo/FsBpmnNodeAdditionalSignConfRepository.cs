using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeAdditionalSignConfRepository : RepositoryBase<BpmnNodeAdditionalSignConf>, IBpmnNodeAdditionalSignConfRepository
{
    public FsBpmnNodeAdditionalSignConfRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
