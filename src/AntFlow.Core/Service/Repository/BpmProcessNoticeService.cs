using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessNoticeService : AFBaseCurdRepositoryService<BpmProcessNotice>, IBpmProcessNoticeService
{
    public BpmProcessNoticeService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmProcessNotice> ProcessNoticeList(string processKey)
    {
        List<BpmProcessNotice> bpmProcessNotices = baseRepo.Where(a => a.ProcessKey == processKey).ToList();
        return bpmProcessNotices;
    }

    public void SaveProcessNotice(BpmProcessDeptVo vo)
    {
        string processKey = vo.ProcessKey;
        List<int> notifyTypeIds = vo.NotifyTypeIds;

        List<BpmnTemplateVo> templateVos = vo.TemplateVos;
        if (!templateVos.IsEmpty())
        {
            if (notifyTypeIds.IsEmpty())
            {
                List<int> list = templateVos.SelectMany(a => a.MessageSendTypeList).Select(a => Convert.ToInt32(a.Id))
                    .Distinct().ToList();
                notifyTypeIds = list;
            }

            Frsql.Delete<BpmnTemplate>()
                .Where(a => a.FormCode == processKey && a.NodeId != null)
                .ExecuteAffrows();

            BpmnConfVo confVo = new();
            confVo.FormCode = processKey;
            confVo.TemplateVos = templateVos;

            BpmnTemplateService bpmnTemplateService = ServiceProviderUtils.GetService<BpmnTemplateService>();
            bpmnTemplateService.EditBpmnTemplate(confVo, 0);
        }

        if (!notifyTypeIds.IsEmpty())
        {
            Frsql.Delete<BpmProcessNotice>()
                .Where(a => a.ProcessKey == processKey)
                .ExecuteAffrows();
            foreach (int notifyTypeId in notifyTypeIds)
            {
                baseRepo.Insert(new BpmProcessNotice { ProcessKey = processKey, Type = notifyTypeId });
            }
        }
    }

    public IDictionary<string, List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes)
    {
        List<BpmProcessNotice> bpmProcessNotices = baseRepo
            .Where(a => formCodes.Contains(a.ProcessKey))
            .ToList();
        Dictionary<string, List<BpmProcessNotice>> grouped = bpmProcessNotices
            .GroupBy(x => x.ProcessKey)
            .ToDictionary(g => g.Key, g => g.ToList());

        return grouped;
    }
}