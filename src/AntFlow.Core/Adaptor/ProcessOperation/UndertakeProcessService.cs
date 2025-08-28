using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

public class UndertakeProcessService : IProcessOperationAdaptor
{
    private readonly AFExecutionService _afExecutionService;

    private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly AFTaskService _taskService;

    public UndertakeProcessService(
        AFTaskService taskService,
        TaskMgmtService taskMgmtService,
        AFExecutionService afExecutionService,
        BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService)
    {
        _taskService = taskService;
        _taskMgmtService = taskMgmtService;
        _afExecutionService = afExecutionService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        if (string.IsNullOrEmpty(vo.TaskId))
        {
            throw new AFBizException("当前流程节点等于空！");
        }

        BpmAfTask task = _taskService.baseRepo.Where(a => a.Id == vo.TaskId).First();

        if (task == null)
        {
            throw new AFBizException("当前流程节点已经被人承办");
        }

        List<BpmAfTask> list = _taskMgmtService.GetAgencyList(vo.TaskId, 1, task.ProcInstId);
        if (list.Any())
        {
            foreach (BpmAfTask? t in list)
            {
                //todo update read node
                _taskMgmtService.DeleteTask(t.Id);
            }
        }

        _bpmVariableMultiplayerPersonnelService.Undertake(vo.ProcessNumber, task.TaskDefKey);

        _afExecutionService.Frsql.Update<BpmAfExecution>()
            .Set(a => a.TaskCount == 1)
            .Where(a => a.Id == task.ExecutionId)
            .ExecuteAffrows();
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE);
    }
}