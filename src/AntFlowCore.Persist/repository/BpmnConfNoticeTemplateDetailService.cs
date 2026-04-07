using AntFlowCore.Core.entity;
using AntFlowCore.Entity;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateDetailService: AFBaseCurdRepositoryService<BpmnConfNoticeTemplateDetail>
{
    public BpmnConfNoticeTemplateDetailService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
}