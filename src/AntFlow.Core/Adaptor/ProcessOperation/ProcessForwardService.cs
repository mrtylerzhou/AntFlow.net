using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

/// <summary>
///     流程转发实现�?    ///
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
                    ProcessNumber = vo.ProcessNumber,
                    TenantId = MultiTenantUtil.GetCurrentTenantId()
                });
            });
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_FORWARD);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_FORWARD);
    }
}