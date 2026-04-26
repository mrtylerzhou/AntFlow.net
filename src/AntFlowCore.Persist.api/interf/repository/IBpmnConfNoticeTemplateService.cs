using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfNoticeTemplateService : IAntFlowRepositoryMix<BpmnConfNoticeTemplate, IBpmnConfNoticeTemplateRepository>
{
    long Insert(string bpmnCode);
    BpmnConfNoticeTemplateDetail? GetDetailByCodeAndType(string bpmnCode, int msgNoticeType);
}
