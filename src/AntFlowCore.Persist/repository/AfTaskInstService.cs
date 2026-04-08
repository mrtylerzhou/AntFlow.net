using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class AfTaskInstService: AFBaseCurdRepositoryService<BpmAfTaskInst>,IAfTaskInstService
{
    public AfTaskInstService(IFreeSql freeSql) : base(freeSql)
    {
    }
    

    public int DoneTodayProcess(String createUserId)
    {
        long count = this.baseRepo
            .Where(a=>a.TaskDefKey!=ProcessEnum.StartTaskKey.Desc&&a.Assignee==createUserId 
                       &&a.EndTime!=null &&a.EndTime.Value.Date>=DateTime.Now.Date)
            .Count();
        return Convert.ToInt32(count);
    }

  public  int DoneCreateProcess(String createUserId)
    {
        BpmBusinessProcessService bpmBusinessProcessService = ServiceProviderUtils.GetService<BpmBusinessProcessService>();
        long count = bpmBusinessProcessService
            .baseRepo
            .Where(a => a.CreateUser == createUserId && a.CreateTime.Value.Date == DateTime.Now.Date)
            .Count();
        return Convert.ToInt32(count);
    }
}