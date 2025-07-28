using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmProcessNoticeService: AFBaseCurdRepositoryService<BpmProcessNotice>
{
    public BpmProcessNoticeService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmProcessNotice> ProcessNoticeList(string processKey)
    {
        List<BpmProcessNotice> bpmProcessNotices = this.baseRepo.Where(a => a.ProcessKey == processKey).ToList();
        return bpmProcessNotices;
    }
    public void SaveProcessNotice(BpmProcessDeptVo vo) {
        String processKey=vo.ProcessKey; 
        List<int> notifyTypeIds=vo.NotifyTypeIds;
       
        List<BpmnTemplateVo> templateVos = vo.TemplateVos;
        if(!templateVos.IsEmpty())
        {
            if(notifyTypeIds.IsEmpty())
            {
                List<int> list = templateVos.SelectMany(a=>a.MessageSendTypeList).Select(a=>Convert.ToInt32(a.Id)).Distinct().ToList();
                notifyTypeIds=list;
            }
          
            Frsql.Delete<BpmnTemplate>()
                .Where(a => a.FormCode == processKey && a.NodeId != null)
                .ExecuteAffrows();
            
            BpmnConfVo confVo=new BpmnConfVo();
            confVo.FormCode=processKey;
            confVo.TemplateVos=templateVos;
            
            BpmnTemplateService bpmnTemplateService = ServiceProviderUtils.GetService<BpmnTemplateService>();
            bpmnTemplateService.EditBpmnTemplate(confVo,0);
        }
        if (!notifyTypeIds.IsEmpty())
        {
            this.Frsql.Delete<BpmProcessNotice>()
                .Where(a=>a.ProcessKey == processKey)
                .ExecuteAffrows();
            foreach (int notifyTypeId in notifyTypeIds)
            {
                this.baseRepo.Insert(new BpmProcessNotice()
                {
                    ProcessKey = processKey,
                    Type = notifyTypeId
                });
            }
        }
    }

    public IDictionary<String,List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes)
    {
        List<BpmProcessNotice> bpmProcessNotices = this.baseRepo
            .Where(a => formCodes.Contains(a.ProcessKey))
            .ToList();
        Dictionary<string,List<BpmProcessNotice>> grouped = bpmProcessNotices
            .GroupBy(x => x.ProcessKey)
            .ToDictionary(g => g.Key, g => g.ToList());

        return grouped;
    }
}