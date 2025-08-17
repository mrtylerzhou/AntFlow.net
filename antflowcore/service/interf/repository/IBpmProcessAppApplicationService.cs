using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;

namespace antflowcore.service.interf.repository;

public interface IBpmProcessAppApplicationService
{
    BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey);
    List<BpmProcessAppApplication> SelectApplicationList();
    ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo);
    Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page);
    bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo);
}