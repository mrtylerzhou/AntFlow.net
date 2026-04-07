using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class ProcessDeptBizService : IProcessDeptBizService
{
    private readonly IBpmProcessNoticeService _bpmProcessNoticeService;
    private readonly ILogger<ProcessDeptBizService> _logger;

    public ProcessDeptBizService(IBpmProcessNoticeService bpmProcessNoticeService,
        ILogger<ProcessDeptBizService> logger)
    {
        _bpmProcessNoticeService = bpmProcessNoticeService;
        _logger = logger;
    }
    public void EditProcessConf(BpmProcessDeptVo vo) {
        //todo save process's other info
        _bpmProcessNoticeService.SaveProcessNotice(vo);
    }
}