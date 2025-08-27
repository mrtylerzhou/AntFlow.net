using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmProcessAppApplicationService
{
    BpmProcessAppApplicationVo GetApplicationUrl(string businessCode, string processKey);
    List<BpmProcessAppApplication> SelectApplicationList();
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo);
    Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page);
    bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo);
}