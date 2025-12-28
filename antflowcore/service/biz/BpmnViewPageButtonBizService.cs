using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;

namespace antflowcore.service.biz;

public class BpmnViewPageButtonBizService
{
    private readonly BpmnViewPageButtonService _bpmnViewPageButtonService;

    public BpmnViewPageButtonBizService(BpmnViewPageButtonService bpmnViewPageButtonService)
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