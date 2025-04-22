using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Entity;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

/// <summary>
/// 流程转发实现类
/// </summary>
public class ProcessForwardService : IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmProcessForwardService _processForwardService;
    private readonly TaskMgmtService _taskMgmtService;

    public ProcessForwardService(
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmProcessForwardService processForwardService,
        TaskMgmtService taskMgmtService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _processForwardService = processForwardService;
        _taskMgmtService = taskMgmtService;
    }

    public void DoProcessButton(BusinessDataVo vo)
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
                    ProcessNumber = vo.ProcessNumber
                });
            });
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_FORWARD);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker, ProcessOperationEnum.BUTTON_TYPE_FORWARD);
    }
}