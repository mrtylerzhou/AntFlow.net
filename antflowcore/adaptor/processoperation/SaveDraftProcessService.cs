using AntFlowCore.Enums;
using antflowcore.service.biz;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

/// <summary>
/// 保存草稿处理器
/// </summary>
public class SaveDraftProcessService : IProcessOperationAdaptor
{
    private readonly BpmProcessDraftBizService _draftBizService;

    public SaveDraftProcessService(BpmProcessDraftBizService draftBizService)
    {
        _draftBizService = draftBizService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        _draftBizService.SaveBusinessDraft(vo);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_SAVE_DRAFT);
    }
}
