using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfNoticeTemplateDetailService : AFBaseCurdRepositoryService<BpmnConfNoticeTemplateDetail>
{
    public BpmnConfNoticeTemplateDetailService(IFreeSql freeSql) : base(freeSql)
    {
    }
}