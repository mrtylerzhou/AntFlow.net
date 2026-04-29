using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnConfNoticeTemplateRepository : RepositoryBase<BpmnConfNoticeTemplate>, IBpmnConfNoticeTemplateRepository
{
    public SsBpmnConfNoticeTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
