using antflowcore.entity;
using antflowcore.service.repository;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ProcessConstantsService
{
    private readonly AfTaskInstService _afTaskInstService;
    private readonly ILogger<ProcessConstantsService> _logger;

    public ProcessConstantsService(AfTaskInstService afTaskInstService, ILogger<ProcessConstantsService> logger)
    {
        _afTaskInstService = afTaskInstService;
        _logger = logger;
    }
    public BpmAfTaskInst GetPrevTask(String taskDefKey,String procInstId)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            throw new ArgumentNullException(nameof(taskDefKey));
        }

        if (string.IsNullOrEmpty(procInstId))
        {
            throw new ArgumentNullException(nameof(procInstId));
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo.Where(a => a.ProcInstId == procInstId && a.TaskDefKey == taskDefKey)
            .OrderByDescending(a => a.StartTime).ToList();
        BpmAfTaskInst? bpmAfTaskInst = bpmAfTaskInsts.Where(a=>a.TaskDefKey!=taskDefKey).FirstOrDefault();
        return bpmAfTaskInst;
    }
}