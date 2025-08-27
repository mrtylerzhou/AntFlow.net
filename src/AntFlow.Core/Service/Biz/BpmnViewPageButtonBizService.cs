using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

public class BpmnViewPageButtonBizService
{
    private readonly BpmnViewPageButtonService _bpmnViewPageButtonService;

    public BpmnViewPageButtonBizService(BpmnViewPageButtonService bpmnViewPageButtonService)
    {
        _bpmnViewPageButtonService = bpmnViewPageButtonService;
    }

    public void EditBpmnViewPageButton(BpmnConfVo bpmnConfVo, long confId)
    {
        BpmnViewPageButtonBaseVo viewPageButtons = bpmnConfVo.ViewPageButtons;
        if (viewPageButtons == null)
        {
            return;
        }

        _bpmnViewPageButtonService
            .Frsql
            .Delete<BpmnViewPageButton>()
            .Where(x => x.ConfId == confId)
            .ExecuteAffrows();

        List<int> viewPageStarts = viewPageButtons.ViewPageStart;
        if (viewPageStarts != null)
        {
            List<BpmnViewPageButton> viewPageButtonList = viewPageStarts
                .Select(start =>
                    BpmnViewPageButton.BuildViewPageButton(confId, start, (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START))
                .ToList();

            _bpmnViewPageButtonService.baseRepo.Insert(viewPageButtonList);
        }

        List<int> viewPageOthers = viewPageButtons.ViewPageOther;

        if (viewPageOthers != null)
        {
            List<BpmnViewPageButton> viewPageButtonList = viewPageStarts
                .Select(other =>
                    BpmnViewPageButton.BuildViewPageButton(confId, other, (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER))
                .ToList();
            _bpmnViewPageButtonService.baseRepo.Insert(viewPageButtonList);
        }
    }
}