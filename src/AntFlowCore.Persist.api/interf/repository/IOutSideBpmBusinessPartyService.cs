using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmBusinessPartyService : IAntFlowRepositoryMix<OutSideBpmBusinessParty, IOutSideBpmBusinessPartyRepository>
{
    ResultAndPage<OutSideBpmBusinessPartyVo> ListPage(PageDto pageDto, OutSideBpmBusinessPartyVo vo);
    List<OutSideBpmBusinessPartyVo> SelectPageList(Page<OutSideBpmBusinessPartyVo> page);
    OutSideBpmBusinessPartyVo Detail(int id);
    void Edit(OutSideBpmBusinessPartyVo vo);
    long CheckData(OutSideBpmBusinessPartyVo vo);
}
