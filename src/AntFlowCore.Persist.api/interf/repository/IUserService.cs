using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserService : IBaseRepositoryService<User>
{
    List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<String> userIds);
    BaseIdTranStruVo QueryUserById(string userId);
    List<BaseIdTranStruVo> QueryEmployeeDirectLeaderByIds(IEnumerable<string> userIds);
    List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(String employeeId, int grade);
    List<BaseIdTranStruVo> QueryEmployeeHrpbsByEmployeeIds(IEnumerable<string> userIds);
    List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndTier(String employeeId, int tier);
    BaseIdTranStruVo QueryLeaderByEmployeeIdAndLevel(string startUserId, int assignLevelGrade);
    Dictionary<string, string> ProvideRoleEmployeeInfo(List<string> roleIds);
    BaseIdTranStruVo GetById(string userId);
    ResultAndPage<BaseIdTranStruVo> SelectUserPageList(Page<BaseIdTranStruVo> page, TaskMgmtVO taskMgmtVo);
    List<BaseIdTranStruVo> SelectAll();
    DetailedUser GetEmployeeDetailById(string id);
    List<DetailedUser> GetEmployeeDetailByIds(IEnumerable<string> ids);
    List<BaseIdTranStruVo> GetLevelLeadersByEmployeeIdAndTier(string employeeId, int tier);
    long CheckEmployeeEffective(string userId);
    DetailedUser GetDetailedUserById(string Id);
}
