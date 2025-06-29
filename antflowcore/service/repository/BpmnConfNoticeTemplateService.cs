using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnConfNoticeTemplateService : AFBaseCurdRepositoryService<BpmnConfNoticeTemplate>
{
    public BpmnConfNoticeTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public long Insert(String bpmnCode) {
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
            };
            list.Add(detail);
        }

        int affrows = Frsql.Insert(list).ExecuteAffrows();
        return id;
    }
}