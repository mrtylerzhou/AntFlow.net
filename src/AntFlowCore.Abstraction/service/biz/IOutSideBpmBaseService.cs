using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IOutSideBpmBaseService
{
    List<OutSideBpmBusinessPartyVo> GetEmplBusinessPartys(string name, params string[] permCodes);
}
