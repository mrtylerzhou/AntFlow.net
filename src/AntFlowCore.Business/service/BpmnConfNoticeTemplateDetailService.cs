using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmnConfNoticeTemplateDetailService : IBpmnConfNoticeTemplateDetailService
{
    public BpmnConfNoticeTemplateDetailService(IBpmnConfNoticeTemplateDetailRepository repository)
    {
        _repository = repository;
    }

    public IBpmnConfNoticeTemplateDetailRepository _repository { get; }
}
