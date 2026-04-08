using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class UndertakeProcessService : IProcessOperationAdaptor
    {
        private readonly IAFTaskService _taskService;
        private readonly ITaskMgmtService _taskMgmtService;
        private readonly IAFExecutionService _afExecutionService;

        private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

        public UndertakeProcessService(
            IAFTaskService taskService,
            ITaskMgmtService taskMgmtService,
            IAFExecutionService afExecutionService,
            IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService)
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