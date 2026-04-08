using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVerifyInfoService : IBaseRepositoryService<BpmVerifyInfo>
{
    void AddVerifyInfo(BpmVerifyInfo verifyInfo);
    string FindCurrentNodeIds(string processNumber);
    List<BpmVerifyInfoVo> FindTaskInfo(string procInstId);
    List<BpmVerifyInfoVo> FindTaskInfo(BpmBusinessProcess bpmBusinessProcess);
    List<BpmVerifyInfoVo> VerifyInfoList(string processNumber, string procInstId);
    List<BpmVerifyInfoVo> GetBpmVerifyInfoVoList(List<BpmVerifyInfoVo> list, string procInstId);
}
