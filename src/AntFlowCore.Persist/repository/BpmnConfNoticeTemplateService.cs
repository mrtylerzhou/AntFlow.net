using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Common.util.Extension;
using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfNoticeTemplateService : AFBaseCurdRepositoryService<BpmnConfNoticeTemplate>,
    IBpmnConfNoticeTemplateService
{
    public BpmnConfNoticeTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public long Insert(String bpmnCode)
    {
        BpmnConfNoticeTemplate bpmnConfNoticeTemplate = new BpmnConfNoticeTemplate()
        {
            BpmnCode = bpmnCode,
            CreateTime = DateTime.Now,
        };
        this.baseRepo.Insert(bpmnConfNoticeTemplate);
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

        int affrows = Frsql.Insert(list).ExecuteAffrows();
        return id;
    }

    public BpmnConfNoticeTemplateDetail? GetDetailByCodeAndType(string bpmnCode, int msgNoticeType)
    {
        BpmnConfNoticeTemplateDetailService bpmnConfNoticeTemplateDetailService = ServiceProviderUtils.GetService<BpmnConfNoticeTemplateDetailService>();
        List<BpmnConfNoticeTemplateDetail> bpmnConfNoticeTemplateDetails = bpmnConfNoticeTemplateDetailService
            .baseRepo
            .Where(a => a.BpmnCode == bpmnCode && a.NoticeTemplateType == msgNoticeType)
            .OrderByDescending(a => a.Id)
            .ToList();
        return bpmnConfNoticeTemplateDetails.IsEmpty() ? null : bpmnConfNoticeTemplateDetails[0];
    }
}