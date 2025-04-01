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
    
    public void UpdateProcessForward(BpmProcessForward bpmProcessForward)
    {
        List<BpmProcessForward> bpmProcessForwards = this
            .baseRepo.
            Where(a=>a.ProcessInstanceId==bpmProcessForward.ProcessInstanceId&&a.ForwardUserId==bpmProcessForward.ForwardUserId)
            .ToList();
        foreach (BpmProcessForward processForward in bpmProcessForwards)
        {
            this.baseRepo.Update(processForward);
        }
    }
}