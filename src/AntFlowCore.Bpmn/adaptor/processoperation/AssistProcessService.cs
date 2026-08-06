using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// 协助节点按钮操作策略.
/// 协助节点语义为"办理"而非"审批",不代表同意/不同意,但流程仍需向下流转.
/// 因此委托到同意(AGREE)处理策略完成任务推进.
/// 对应 Java AssistProcessImpl.
/// </summary>
public class AssistProcessService : IProcessOperationAdaptor
{
    private readonly ResubmitProcessService _resubmitProcess;
    private readonly ILogger<AssistProcessService> _logger;

    public AssistProcessService(
        ResubmitProcessService resubmitProcess,
        ILogger<AssistProcessService> logger)
    {
        _resubmitProcess = resubmitProcess;
        _logger = logger;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        _logger.LogInformation("协助节点办理,委托同意逻辑推进流程. processNumber:{ProcessNumber}", vo.ProcessNumber);
        // 保持operationType=41(协助)不篡改,审批记录正确反映操作类型
        // 委托到同意处理策略(ResubmitProcessService同时处理RESUBMIT/AGREE/JP)
        _resubmitProcess.DoProcessButton(vo);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_ASSIST);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_ASSIST);
    }
}
