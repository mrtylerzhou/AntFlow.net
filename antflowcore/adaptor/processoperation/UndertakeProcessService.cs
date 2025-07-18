using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class UndertakeProcessService : IProcessOperationAdaptor
    {
        private readonly AFTaskService _taskService;
        private readonly TaskMgmtService _taskMgmtService;
        private readonly AFExecutionService _afExecutionService;

        private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

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

            BpmAfTask task = _taskService.baseRepo.Where(a=>a.Id==vo.TaskId).First();
           
            if (task == null)
            {
                throw new AFBizException("当前流程节点已经被人承办！");
            }

            List<BpmAfTask> list = _taskMgmtService.GetAgencyList(vo.TaskId, 1, task.ProcInstId);
            if (list.Any())
            {
                foreach (var t in list)
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
            ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE);
        }
        
    }