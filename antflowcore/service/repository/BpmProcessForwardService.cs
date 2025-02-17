using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmProcessForwardService: AFBaseCurdRepositoryService<BpmProcessForward>
{
    public BpmProcessForwardService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void AddProcessForward(BpmProcessForward bpmProcessForward)
    {
        this.baseRepo.Insert(bpmProcessForward);
    }
}