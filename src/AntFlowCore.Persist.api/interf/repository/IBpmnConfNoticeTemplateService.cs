using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfNoticeTemplateService : IBaseRepositoryService<BpmnConfNoticeTemplate>
{
    long Insert(string bpmnCode);
    BpmnConfNoticeTemplateDetail? GetDetailByCodeAndType(string bpmnCode, int msgNoticeType);
}
