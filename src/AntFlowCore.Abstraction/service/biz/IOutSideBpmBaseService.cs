using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IOutSideBpmBaseService
{
    List<OutSideBpmBusinessPartyVo> GetEmplBusinessPartys(string name, params string[] permCodes);
}
