using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnTemplateRepository : RepositoryBase<BpmnTemplate>, IBpmnTemplateRepository
{
    public FsBpmnTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
