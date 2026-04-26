using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateDetailService: AFBaseCurdRepositoryService<BpmnConfNoticeTemplateDetail>
{
    public BpmnConfNoticeTemplateDetailService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
}