using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnTemplateRepository : RepositoryBase<BpmnTemplate>, IBpmnTemplateRepository
{
    public SsBpmnTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
