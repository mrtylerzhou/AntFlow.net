using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnConfNoticeTemplateDetailService: AFBaseCurdRepositoryService<BpmnConfNoticeTemplateDetail>
{
    public BpmnConfNoticeTemplateDetailService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
}