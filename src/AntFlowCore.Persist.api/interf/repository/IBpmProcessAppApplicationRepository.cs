using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessAppApplicationRepository : IBaseRepository<BpmProcessAppApplication>
{
    List<BpmProcessAppApplication> GetApplicationUrl(string businessCode, string processKey);
    List<BpmProcessAppApplication> SelectApplicationList();
    List<BpmProcessAppApplicationVo> NewListPage(PagingInfo pagingInfo);
    bool UpdateApplication(BpmProcessAppApplication entity);
    bool InsertApplication(BpmProcessAppApplication entity);
    bool ExistsByTitle(string title);
}
