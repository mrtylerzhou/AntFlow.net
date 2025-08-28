using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfNoticeTemplateService : AFBaseCurdRepositoryService<BpmnConfNoticeTemplate>,
    IBpmnConfNoticeTemplateService
{
    public BpmnConfNoticeTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public long Insert(string bpmnCode)
    {
        BpmnConfNoticeTemplate bpmnConfNoticeTemplate = new() { BpmnCode = bpmnCode, CreateTime = DateTime.Now };
        baseRepo.Insert(bpmnConfNoticeTemplate);
        long id = bpmnConfNoticeTemplate.Id;

        List<BpmnConfNoticeTemplateDetail> list = new();

        foreach (MsgNoticeTypeEnum msgNoticeTypeEnum in Enum.GetValues<MsgNoticeTypeEnum>())
        {
            BpmnConfNoticeTemplateDetail detail = new()
            {
                BpmnCode = bpmnCode,
                NoticeTemplateType = (int)msgNoticeTypeEnum,
                NoticeTemplateDetail = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode((int)msgNoticeTypeEnum),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId()
            };
            list.Add(detail);
        }

        int affrows = Frsql.Insert(list).ExecuteAffrows();
        return id;
    }

    public BpmnConfNoticeTemplateDetail? GetDetailByCodeAndType(string bpmnCode, int msgNoticeType)
    {
        BpmnConfNoticeTemplateDetailService bpmnConfNoticeTemplateDetailService =
            ServiceProviderUtils.GetService<BpmnConfNoticeTemplateDetailService>();
        List<BpmnConfNoticeTemplateDetail> bpmnConfNoticeTemplateDetails = bpmnConfNoticeTemplateDetailService
            .baseRepo
            .Where(a => a.BpmnCode == bpmnCode && a.NoticeTemplateType == msgNoticeType)
            .OrderByDescending(a => a.Id)
            .ToList();
        return bpmnConfNoticeTemplateDetails.IsEmpty() ? null : bpmnConfNoticeTemplateDetails[0];
    }
}