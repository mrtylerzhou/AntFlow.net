using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateDetailService : IBpmnConfNoticeTemplateDetailService
{
    public BpmnConfNoticeTemplateDetailService(IBpmnConfNoticeTemplateDetailRepository repository)
    {
        _repository = repository;
    }

    public IBpmnConfNoticeTemplateDetailRepository _repository { get; }
}
