using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IConfigFlowButtonContantService
{
    Dictionary<string, List<ProcessActionButtonVo>> GetButtons(string processNum, string elementId, List<string> viewNodeIds,
        bool? isJurisdiction, bool? isInitiate);
}
