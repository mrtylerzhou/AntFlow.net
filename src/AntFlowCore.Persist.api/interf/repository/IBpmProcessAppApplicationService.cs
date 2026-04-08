using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessAppApplicationService : IBaseRepositoryService<BpmProcessAppApplication>
{
    BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey);
    List<BpmProcessAppApplication> SelectApplicationList();
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo);
    Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page);
    bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo);
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsPageList(PageDto page, BpmProcessAppApplicationVo vo);
}
