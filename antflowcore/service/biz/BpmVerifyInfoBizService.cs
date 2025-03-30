using antflowcore.service.repository;

namespace antflowcore.service.biz;

public class BpmVerifyInfoBizService
{
    private readonly BpmVerifyInfoService _bpmVerifyInfoService;

    public BpmVerifyInfoBizService(BpmVerifyInfoService bpmVerifyInfoService)
    {
        _bpmVerifyInfoService = bpmVerifyInfoService;
    }
    public  String  FindCurrentNodeIds(String processNumber) {
        return  _bpmVerifyInfoService.FindCurrentNodeIds(processNumber);
    }
}