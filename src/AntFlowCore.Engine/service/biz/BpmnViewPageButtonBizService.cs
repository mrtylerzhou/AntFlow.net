using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class BpmnViewPageButtonBizService : IBpmnViewPageButtonBizService
{
    private readonly IBpmnViewPageButtonService _bpmnViewPageButtonService;

    public BpmnViewPageButtonBizService(IBpmnViewPageButtonService bpmnViewPageButtonService)
    {
        _bpmnViewPageButtonService = bpmnViewPageButtonService;
    }
    public void EditBpmnViewPageButton(BpmnConfVo bpmnConfVo, long confId) {
        BpmnViewPageButtonBaseVo viewPageButtons = bpmnConfVo.ViewPageButtons;
        if (viewPageButtons==null) {
            return;
        }

        _bpmnViewPageButtonService
            .Frsql
            .Delete<BpmnViewPageButton>()
            .Where(x => x.ConfId == confId)
            .ExecuteAffrows();

        List<int> viewPageStarts = viewPageButtons.ViewPageStart;
        if (!viewPageStarts.IsEmpty())
        {
            List<BpmnViewPageButton> viewPageButtonList = viewPageStarts
                .Select(start =>
                    BpmnViewPageButton.BuildViewPageButton(confId, start, (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START))
                .ToList();
            _bpmnViewPageButtonService.baseRepo.Insert(viewPageButtonList);
        }

        List<int> viewPageOthers = viewPageButtons.ViewPageOther;

        if (!viewPageOthers.IsEmpty()) {
            List<BpmnViewPageButton> viewPageButtonList = viewPageOthers
                .Select(other => BpmnViewPageButton.BuildViewPageButton(confId, other,(int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER))
                .ToList();
            _bpmnViewPageButtonService.baseRepo.Insert(viewPageButtonList);
        }
    }
}