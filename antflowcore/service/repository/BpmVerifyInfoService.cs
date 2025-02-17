using AntFlowCore.Entity;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmVerifyInfoService: AFBaseCurdRepositoryService<BpmVerifyInfo>
{
    private readonly BpmFlowrunEntrustService _bpmFlowrunEntrustService;

    public BpmVerifyInfoService(
        BpmFlowrunEntrustService bpmFlowrunEntrustService,
        IFreeSql freeSql
    ) : base(freeSql)
    {
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
    }

    public void AddVerifyInfo(BpmVerifyInfo verifyInfo)
    {
        BpmFlowrunEntrust entrustByTaskId = _bpmFlowrunEntrustService.GetEntrustByTaskId(SecurityUtils.GetLogInEmpIdStr(), verifyInfo.RunInfoId, verifyInfo.TaskId);
    }
}