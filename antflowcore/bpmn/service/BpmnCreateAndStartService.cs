using antflowcore.bpmn;
using antflowcore.bpmn.service;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class BpmnCreateAndStartService
{
    private readonly RepositoryService _repositoryService;
    private readonly RuntimeService _runtimeService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;

    public BpmnCreateAndStartService(
        RepositoryService repositoryService,
        RuntimeService runtimeService,
        BpmBusinessProcessService bpmBusinessProcessService)
    {
        _repositoryService = repositoryService;
        _runtimeService = runtimeService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }
    public void CreateBpmnAndStart(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        string deploymentId = _repositoryService.CreateDeployment(bpmnConfCommonVo, bpmnStartConditions);
        ExecutionEntity startProcessInstance = _runtimeService.StartProcessInstance(bpmnConfCommonVo,bpmnStartConditions, deploymentId);
        string processNum = bpmnStartConditions.ProcessNum;
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.baseRepo.Where(a=>a.BusinessNumber==processNum).First();
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"can not find bpmn process by processNum:{processNum}");
        }
        
        _bpmBusinessProcessService.Frsql
            .Update<BpmBusinessProcess>()
            .Set(a => a.ProcInstId,startProcessInstance.ProcessInstanceId)
            .Where(a => a.Id == bpmBusinessProcess.Id)
            .ExecuteAffrows();
    }
}