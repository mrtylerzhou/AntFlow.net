using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessAppApplicationService : IAntFlowRepositoryMix<BpmProcessAppApplication, IBpmProcessAppApplicationRepository>
{
    BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey);
    List<BpmProcessAppApplication> SelectApplicationList();
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo);
    Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page);
    bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo);
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsPageList(PageDto page, BpmProcessAppApplicationVo vo);
}
