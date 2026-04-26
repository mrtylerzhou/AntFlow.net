using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class AfTaskInstService: IAfTaskInstService
{
    public AfTaskInstService(IBpmAfTaskInstRepository repository)
    {
        _repository = repository;
    }
    

    public int DoneTodayProcess(String createUserId)
    {
        long count = this._repository.GetQueryable()
            .Where(a=>a.TaskDefKey!=ProcessEnum.StartTaskKey.Desc&&a.Assignee==createUserId 
                       &&a.EndTime!=null &&a.EndTime.Value.Date>=DateTime.Now.Date)
            .Count();
        return Convert.ToInt32(count);
    }

  public  int DoneCreateProcess(String createUserId)
    {
        BpmBusinessProcessService bpmBusinessProcessService = ServiceProviderUtils.GetService<BpmBusinessProcessService>();
        long count = bpmBusinessProcessService
            ._repository.Count((a => a.CreateUser == createUserId && a.CreateTime.Value.Date == DateTime.Now.Date));
        return Convert.ToInt32(count);
    }

    public IBpmAfTaskInstRepository _repository { get; }
}