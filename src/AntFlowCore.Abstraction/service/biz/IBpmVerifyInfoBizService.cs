using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmVerifyInfoBizService
{
    string FindCurrentNodeIds(string processNumber);
    List<BpmVerifyInfoVo> GetBpmVerifyInfoVos(string processNumber, bool finishFlag);
    Dictionary<string, string> GetSignUpNodeCollectionNameMap(long variableId);
    BpmVerifyInfo? GetLastProcessNodeByAssignee(string processNumber, string assignee);
}
