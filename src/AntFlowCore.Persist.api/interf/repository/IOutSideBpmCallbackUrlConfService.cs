using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmCallbackUrlConfService : IBaseRepositoryService<OutSideBpmCallbackUrlConf>
{
    OutSideBpmCallbackUrlConf GetOutSideBpmCallbackUrlConf(long businessPartyId);
    List<OutSideBpmCallbackUrlConf> SelectListByFormCode(string formCode);
    OutSideBpmCallbackUrlConfVo Detail(int id);
    void Edit(OutSideBpmCallbackUrlConfVo vo);
}
