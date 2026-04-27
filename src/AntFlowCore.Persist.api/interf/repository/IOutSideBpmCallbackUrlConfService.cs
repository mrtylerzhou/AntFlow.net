using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmCallbackUrlConfService : IAntFlowRepositoryMix<OutSideBpmCallbackUrlConf, IOutSideBpmCallbackUrlConfRepository>
{
    OutSideBpmCallbackUrlConf GetOutSideBpmCallbackUrlConf(long businessPartyId);
    List<OutSideBpmCallbackUrlConf> SelectListByFormCode(string formCode);
    OutSideBpmCallbackUrlConfVo Detail(int id);
    void Edit(OutSideBpmCallbackUrlConfVo vo);
}
