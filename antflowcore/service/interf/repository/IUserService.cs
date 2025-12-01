using antflowcore.dto;
using antflowcore.entity;
using antflowcore.entityj;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.interf.repository;

public interface IUserService
{
    List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<String> userIds);
    BaseIdTranStruVo QueryUserById(string userId);
    BaseIdTranStruVo QueryEmployeeDirectLeaderById(string startUserId);
    List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(String employeeId, int grade);
    BaseIdTranStruVo QueryEmployeeHrpbByEmployeeId(string startUserId);
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