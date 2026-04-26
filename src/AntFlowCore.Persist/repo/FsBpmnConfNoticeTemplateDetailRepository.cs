using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnConfNoticeTemplateDetailRepository : RepositoryBase<BpmnConfNoticeTemplateDetail>, IBpmnConfNoticeTemplateDetailRepository
{
    public FsBpmnConfNoticeTemplateDetailRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
