using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmVerifyInfoRepository : IBaseRepository<BpmVerifyInfo>
{
    List<BpmVerifyInfoVo> GetVerifyInfo(BpmVerifyInfoVo vo);
    BpmVerifyInfo? FindByProcessCodeAndVerifyUserId(string processNumber, string assignee);
    List<BpmVerifyInfo> FindByRunInfoIdAndTaskDefKey(string runInfoId, string taskDefKey);
}
