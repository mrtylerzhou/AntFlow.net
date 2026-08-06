using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// Process operation adaptor for saving a draft (operationType = 30, BUTTON_TYPE_SAVE_DRAFT).
/// Invoked via the standard ButtonsOperation flow. Delegates to
/// <see cref="IBpmProcessDraftBizService.SaveBusinessDraft"/>.
/// </summary>
public class SaveDraftProcessService : IProcessOperationAdaptor
{
    private readonly IBpmProcessDraftBizService _bpmProcessDraftBizService;

    public SaveDraftProcessService(IBpmProcessDraftBizService bpmProcessDraftBizService)
    {
        _bpmProcessDraftBizService = bpmProcessDraftBizService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        _bpmProcessDraftBizService.SaveBusinessDraft(vo);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_SAVE_DRAFT);
    }
}
