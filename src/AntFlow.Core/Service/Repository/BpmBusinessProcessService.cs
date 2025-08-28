using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface;

namespace AntFlow.Core.Service.Repository;

public class BpmBusinessProcessService : AFBaseCurdRepositoryService<BpmBusinessProcess>, IBpmBusinessProcessService
{
    public BpmBusinessProcessService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmBusinessProcess GetBpmBusinessProcess(string processNumber)
    {
        List<BpmBusinessProcess> bpmBusinessProcesses =
            baseRepo.Where(a => a.BusinessNumber.Equals(processNumber)).ToList();
        if (bpmBusinessProcesses.Count > 1)
        {
            throw new AFBizException($"get more than one bpm business process by processNumber:{processNumber}");
        }

        if (bpmBusinessProcesses.Count < 1)
        {
            throw new AFBizException(
                $"can not get bpm business process by processNumber:{processNumber},most of the times it means that the process does not in exists");
        }

        return bpmBusinessProcesses[0];
    }

    public BpmBusinessProcess GetBpmBusinessProcessByProcInstId(string procinstId)
    {
        List<BpmBusinessProcess> processes = baseRepo.Where(a => a.ProcInstId == procinstId).ToList();
        if (processes.Count > 1)
        {
            throw new AFBizException($"根据流程实例ID:{procinstId}查询到多个BPM业务流程");
        }

        if (processes.Count < 1)
        {
            throw new AFBizException($"根据流程实例ID:{procinstId}未查询到BPM业务流程");
        }

        return processes[0];
    }

    public void Update(BpmBusinessProcess bpmBusinessProcess)
    {
        //todo 实现更新业务流程逻辑
        Frsql.Update<BpmBusinessProcess>()
            .SetDto(bpmBusinessProcess)
            .Where(a => a.BusinessNumber.Equals(bpmBusinessProcess.BusinessNumber))
            .ExecuteAffrows();
    }

    public void AddBusinessProcess(BpmBusinessProcess bpmBusinessProcess)
    {
        baseRepo.Insert(bpmBusinessProcess);
    }

    public bool CheckProcessData(string entryId)
    {
        long number = baseRepo.Where(a => a.EntryId.Equals(entryId)).Count();

        return number <= 0;
    }
}