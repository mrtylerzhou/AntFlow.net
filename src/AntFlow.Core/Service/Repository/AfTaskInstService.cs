using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Util;

namespace AntFlow.Core.Service.Repository;

public class AfTaskInstService : AFBaseCurdRepositoryService<BpmAfTaskInst>, IAfTaskInstService
{
    public AfTaskInstService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public int DoneTodayProcess(string createUserId)
    {
        long count = baseRepo
            .Where(a => a.TaskDefKey != ProcessEnum.StartTaskKey.Desc && a.Assignee == createUserId
                                                                      && a.EndTime != null &&
                                                                      a.EndTime.Value.Date >= DateTime.Now.Date)
            .Count();
        return Convert.ToInt32(count);
    }

    public int DoneCreateProcess(string createUserId)
    {
        BpmBusinessProcessService bpmBusinessProcessService =
            ServiceProviderUtils.GetService<BpmBusinessProcessService>();
        long count = bpmBusinessProcessService
            .baseRepo
            .Where(a => a.CreateUser == createUserId && a.CreateTime.Value.Date == DateTime.Now.Date)
            .Count();
        return Convert.ToInt32(count);
    }
}