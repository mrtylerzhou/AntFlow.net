using AntFlowCore.Core.entity;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateDetailService: AFBaseCurdRepositoryService<BpmnConfNoticeTemplateDetail>
{
    public BpmnConfNoticeTemplateDetailService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
}