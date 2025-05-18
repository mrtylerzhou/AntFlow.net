using System.Linq.Expressions;
using antflowcore.dto;
using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.util;
using AntFlowCore.Util;
using antflowcore.vo;
using AntFlowCore.Vo;
using FreeSql.Internal.Model;

namespace antflowcore.service.repository;

public class UserService: AFBaseCurdRepositoryService<User>
{
    public UserService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BaseIdTranStruVo QueryUserById(string userId)
    {
        BaseIdTranStruVo baseIdTranStruVo = baseRepo.Where(a=>a.Id==Convert.ToInt64(userId)).First().ToBaseIdTranStruVo();
        return baseIdTranStruVo;
    }
   public List<BaseIdTranStruVo> QueryUserByIds(IEnumerable<String> userIds)
    {
        IEnumerable<long> userIdLongList = AFCollectionUtil.StringToLongList(userIds);
        List<BaseIdTranStruVo> baseIdTranStruVos = baseRepo.Select.Where(a => userIdLongList.Contains(a.Id))
            .ToList().Select(a => a.ToBaseIdTranStruVo()).ToList();
        return baseIdTranStruVos;
    }
    public  List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndTier(String employeeId,int tier)
    {
        throw new NotImplementedException();
    }
    public  List<BaseIdTranStruVo> QueryLeadersByEmployeeIdAndGrade(String employeeId,int grade)
    {
        throw new NotImplementedException();
    }

    public BaseIdTranStruVo QueryEmployeeDirectLeaderById(string startUserId)
    {
        throw new NotImplementedException();
    }

    public BaseIdTranStruVo QueryEmployeeHrpbByEmployeeId(string startUserId)
    {
        throw new NotImplementedException();
    }

    public BaseIdTranStruVo QueryLeaderByEmployeeIdAndLevel(string startUserId, int assignLevelGrade)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string,string> ProvideRoleEmployeeInfo(List<string> roleIds)
    {
        throw new NotImplementedException();
    }

    public BaseIdTranStruVo GetById(string userId)
    {
        User first = baseRepo.Where(a=>a.Id==Convert.ToInt64(userId)).First();
        return new BaseIdTranStruVo{Id = first.Id.ToString(),Name = first.Name};
    }

    public  ResultAndPage<BaseIdTranStruVo> SelectUserPageList(Page<BaseIdTranStruVo> page, TaskMgmtVO taskMgmtVo)
    {
        Expression<Func<User, bool>> expression = a => 1 == 1;
        if (!string.IsNullOrEmpty(taskMgmtVo?.Description))
        {
            expression=expression.And(a=>a.Name.Contains(taskMgmtVo.Description));
        }

        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        
        List<User> users = baseRepo.Where(expression)
            .Page(basePagingInfo)
            .ToList();
        List<BaseIdTranStruVo> baseIdTranStruVos = users.Select(a=>a.ToBaseIdTranStruVo()).ToList();
        return PageUtils.GetResultAndPage(page.Of(baseIdTranStruVos, basePagingInfo));
    }

    public List<BaseIdTranStruVo> SelectAll()
    {
        List<BaseIdTranStruVo> results = baseRepo.Where(a=>1==1)
            .ToList<BaseIdTranStruVo>(a=>new BaseIdTranStruVo(a.Id.ToString(),a.Name));
        return results;
    }
}