using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVerifyInfoService : IAntFlowRepositoryMix<BpmVerifyInfo, IBpmVerifyInfoRepository>
{
    void AddVerifyInfo(BpmVerifyInfo verifyInfo);
    string FindCurrentNodeIds(string processNumber);
    List<BpmVerifyInfoVo> FindTaskInfo(string procInstId);
    List<BpmVerifyInfoVo> FindTaskInfo(BpmBusinessProcess bpmBusinessProcess);
    List<BpmVerifyInfoVo> VerifyInfoList(string processNumber, string procInstId);
    List<BpmVerifyInfoVo> GetBpmVerifyInfoVoList(List<BpmVerifyInfoVo> list, string procInstId);
}
