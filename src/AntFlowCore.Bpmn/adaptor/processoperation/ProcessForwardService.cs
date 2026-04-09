using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

 /// <summary>
    /// 流程转发实现类
    /// </summary>
    public class ProcessForwardService : IProcessOperationAdaptor
    {
        private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
        private readonly IBpmProcessForwardService _processForwardService;
        private readonly ITaskMgmtService _taskMgmtService;

        public ProcessForwardService(
            IBpmBusinessProcessService bpmBusinessProcessService,
            IBpmProcessForwardService processForwardService,
            ITaskMgmtService taskMgmtService)
        {
            _bpmBusinessProcessService = bpmBusinessProcessService;
            _processForwardService = processForwardService;
            _taskMgmtService = taskMgmtService;
        }

        public  void DoProcessButton(BusinessDataVo vo)
        {
            BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            if (bpmBusinessProcess != null)
            {
               
                vo.UserInfos.ForEach(o =>
                {
                    _processForwardService.AddProcessForward(new BpmProcessForward
                    {
                        CreateTime = DateTime.Now,
                        CreateUserId = SecurityUtils.GetLogInEmpId(),
                        ForwardUserId = o.Id,
                        ForwardUserName = o.Name,
                        ProcessInstanceId = bpmBusinessProcess.ProcInstId,
                        ProcessNumber = vo.ProcessNumber,
                        TenantId = MultiTenantUtil.GetCurrentTenantId(),
                    });
                });
            }
        }

        public  void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_FORWARD);
            ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_FORWARD);
        }
    }