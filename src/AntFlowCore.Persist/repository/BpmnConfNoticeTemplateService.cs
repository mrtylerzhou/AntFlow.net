using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateService : IBpmnConfNoticeTemplateService
{
    private readonly IBpmnConfNoticeTemplateDetailService _bpmnConfNoticeTemplateDetailService;

    public BpmnConfNoticeTemplateService(IBpmnConfNoticeTemplateRepository repository, IBpmnConfNoticeTemplateDetailService bpmnConfNoticeTemplateDetailService)
    {
        _repository = repository;
        _bpmnConfNoticeTemplateDetailService = bpmnConfNoticeTemplateDetailService;
    }

    public IBpmnConfNoticeTemplateRepository _repository { get; }

    public long Insert(string bpmnCode)
    {
        BpmnConfNoticeTemplate bpmnConfNoticeTemplate = new BpmnConfNoticeTemplate()
        {
            BpmnCode = bpmnCode,
            CreateTime = DateTime.Now,
        };
        _repository.Add(bpmnConfNoticeTemplate);
        long id = bpmnConfNoticeTemplate.Id;

        List<BpmnConfNoticeTemplateDetail> list = new List<BpmnConfNoticeTemplateDetail>();

        foreach (MsgNoticeTypeEnum msgNoticeTypeEnum in Enum.GetValues<MsgNoticeTypeEnum>())
        {
            BpmnConfNoticeTemplateDetail detail = new BpmnConfNoticeTemplateDetail
            {
                BpmnCode = bpmnCode,
                NoticeTemplateType = (int)msgNoticeTypeEnum,
                NoticeTemplateDetail = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode((int)msgNoticeTypeEnum),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            list.Add(detail);
        }

        _bpmnConfNoticeTemplateDetailService._repository.AddRange(list);
        return id;
    }

    public BpmnConfNoticeTemplateDetail? GetDetailByCodeAndType(string bpmnCode, int msgNoticeType)
    {
        List<BpmnConfNoticeTemplateDetail> bpmnConfNoticeTemplateDetails = _bpmnConfNoticeTemplateDetailService
            ._repository
            .Find(a => a.BpmnCode == bpmnCode && a.NoticeTemplateType == msgNoticeType)
            .OrderByDescending(a => a.Id)
            .ToList();
        return bpmnConfNoticeTemplateDetails.IsEmpty() ? null : bpmnConfNoticeTemplateDetails[0];
    }
}
