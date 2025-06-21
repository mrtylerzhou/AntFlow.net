using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util.Extension;

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
    public void SaveProcessNotice(String processKey, List<int> notifyTypeIds) {
        this.Frsql.Delete<BpmProcessNotice>()
            .Where(a=>a.ProcessKey == processKey)
            .ExecuteAffrows();

        if (!notifyTypeIds.IsEmpty())
        {
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
}