using antflowcore.service.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ProcessDeptBizService
{
    private readonly BpmProcessNoticeService _bpmProcessNoticeService;
    private readonly ILogger<ProcessDeptBizService> _logger;

    public ProcessDeptBizService(BpmProcessNoticeService bpmProcessNoticeService,
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