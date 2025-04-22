using antflowcore.entity;
using antflowcore.exception;

namespace antflowcore.service.repository;

public class BpmBusinessProcessService: AFBaseCurdRepositoryService<BpmBusinessProcess>
{
    public BpmBusinessProcessService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public bool CheckProcessData(String entryId)
    {
        long number = baseRepo.Where(a => a.EntryId.Equals(entryId)).Count();
        
        return number <= 0;
    }

    public BpmBusinessProcess GetBpmBusinessProcess(string processNumber)
    {
        List<BpmBusinessProcess> bpmBusinessProcesses = this.baseRepo.Where(a => a.BusinessNumber.Equals(processNumber)).ToList();
        if (bpmBusinessProcesses.Count > 1)
        {
            throw new AFBizException($"get more than one bpm business process by processNumber:{processNumber}");
        }else if (bpmBusinessProcesses.Count < 1)
        {
            throw new AFBizException($"can not get bpm business process by processNumber:{processNumber},most of the times it means that the process does not in exists");
        }
        return bpmBusinessProcesses[0];
    }

    public void Update(BpmBusinessProcess bpmBusinessProcess)
    {
        
        //todo 看这里更新正常么
        this.Frsql.Update<BpmBusinessProcess>()
            .SetDto(bpmBusinessProcess)
            .Where(a => a.BusinessNumber.Equals(bpmBusinessProcess.BusinessNumber))
            .ExecuteAffrows();
        
    }

    public void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess)
    {
        baseRepo.Insert(bpmBusinessProcess);
    }

    public void UpdateBusinessProcess(BpmBusinessProcess bpmBusinessProcess)
    {
        List<BpmBusinessProcess> bpmBusinessProcesses = this.baseRepo.Where(a=>a.BusinessNumber==bpmBusinessProcess.BusinessNumber).ToList();
        foreach (BpmBusinessProcess businessProcess in bpmBusinessProcesses)
        {
            businessProcess.ProcessState = bpmBusinessProcess.ProcessState;
            businessProcess.Description=bpmBusinessProcess.Description;
        }
        this.baseRepo.Update(bpmBusinessProcesses);
    }
}