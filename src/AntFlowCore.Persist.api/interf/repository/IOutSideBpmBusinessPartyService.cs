using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmBusinessPartyService : IBaseRepositoryService<OutSideBpmBusinessParty>
{
    ResultAndPage<OutSideBpmBusinessPartyVo> ListPage(PageDto pageDto, OutSideBpmBusinessPartyVo vo);
    List<OutSideBpmBusinessPartyVo> SelectPageList(Page<OutSideBpmBusinessPartyVo> page);
    OutSideBpmBusinessPartyVo Detail(int id);
    void Edit(OutSideBpmBusinessPartyVo vo);
    long CheckData(OutSideBpmBusinessPartyVo vo);
}
