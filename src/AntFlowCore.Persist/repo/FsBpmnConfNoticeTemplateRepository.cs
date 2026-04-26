using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnConfNoticeTemplateRepository : RepositoryBase<BpmnConfNoticeTemplate>, IBpmnConfNoticeTemplateRepository
{
    public FsBpmnConfNoticeTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
