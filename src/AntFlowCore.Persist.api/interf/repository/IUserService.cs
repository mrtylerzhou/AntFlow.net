using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserService : IAntFlowRepositoryMix<User, IUserRepository>
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
    /// <summary>
    /// 查询员工可用性(办公状态),并返回不可用时需要转办的目标人。
    /// 默认实现为骨架空实现(恒返回可用),真实数据由使用方对接员工表/工作日历表等自行实现。
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <returns>可用性结果:Available(是否可用)、UnavailableBeginTime/UnavailableEndTime(不可用时间窗口)、
    /// DelegateUser(不可用且需要转办时的目标人)</returns>
    UserAvailableVo CheckEmployeeEffective(string userId);
    DetailedUser GetDetailedUserById(string Id);
}
