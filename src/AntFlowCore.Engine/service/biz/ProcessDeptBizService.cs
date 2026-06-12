using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class ProcessDeptBizService : IProcessDeptBizService
{
    private readonly ILogger<ProcessDeptBizService> _logger;

    public ProcessDeptBizService(ILogger<ProcessDeptBizService> logger)
    {
        _logger = logger;
    }
    public void EditProcessConf(BpmProcessDeptVo vo) {
        // IBpmProcessNoticeService has been removed; process notice saving is no longer supported
    }
}
