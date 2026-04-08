using AntFlowCore.Core.entity;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmVerifyInfoBizService
{
    string FindCurrentNodeIds(string processNumber);
    List<BpmVerifyInfoVo> GetBpmVerifyInfoVos(string processNumber, bool finishFlag);
    Dictionary<string, string> GetSignUpNodeCollectionNameMap(long variableId);
    BpmVerifyInfo? GetLastProcessNodeByAssignee(string processNumber, string assignee);
}
