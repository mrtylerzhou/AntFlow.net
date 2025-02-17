using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class UndertakeProcessImpl : IProcessOperationAdaptor
    {
        private readonly AFTaskService _taskService;
        private readonly TaskMgmtService _taskMgmtService;
       
        private readonly BpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;

        public UndertakeProcessImpl(
            AFTaskService taskService,
            TaskMgmtService taskMgmtService,
            BpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService)
        {
            _taskService = taskService;
            _taskMgmtService = taskMgmtService;
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

            List<BpmAfTask> list = _taskMgmtService.GetAgencyList(vo.TaskId, ProcessEnum.AgencyType.Code, task.ProcInstId);
            if (list.Any())
            {
                foreach (var t in list)
                {
                    //todo update read node
                    _taskMgmtService.DeleteTask(t.Id);
                }
            }

            _bpmVariableMultiplayerPersonnelService.Undertake(vo.ProcessNumber, task.TaskDefKey);
        }

        public void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE);
            ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE);
        }
        
    }