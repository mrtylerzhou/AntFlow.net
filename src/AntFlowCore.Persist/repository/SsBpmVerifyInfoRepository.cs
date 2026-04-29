using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVerifyInfoRepository : RepositoryBase<BpmVerifyInfo>, IBpmVerifyInfoRepository
{
    public SsBpmVerifyInfoRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVerifyInfoVo> GetVerifyInfo(BpmVerifyInfoVo vo)
    {
        Expression<Func<BpmVerifyInfo, bool>> expression = a => 1 == 1;
        if (!string.IsNullOrEmpty(vo.ProcessCode))
        {
            expression = expression.And(a => a.ProcessCode == vo.ProcessCode);
        }

        if (vo.ProcessCodeList != null && vo.ProcessCodeList.Count > 0)
        {
           
            expression = expression.And(a => vo.ProcessCodeList.Contains(a.ProcessCode));
        }

        if (!string.IsNullOrEmpty(vo.BusinessId))
        {
            expression = expression.And(a => a.BusinessId == vo.BusinessId);
        }

        List<BpmVerifyInfoVo> bpmVerifyInfoVos = Db.Queryable<BpmVerifyInfo>()
            .Where(expression)
            .Select<BpmVerifyInfoVo>(w => new BpmVerifyInfoVo
            {
                Id = w.Id.ToString(),
                VerifyUserId = w.VerifyUserId,
                VerifyUserName = w.VerifyUserName,
                TaskName = w.TaskName,
                VerifyStatus = w.VerifyStatus,
                VerifyStatusName =
                    w.VerifyStatus == 1 ? "提交" :
                    w.VerifyStatus == 2 ? "同意" :
                    w.VerifyStatus == 3 ? "不同意" :
                    w.VerifyStatus == 4 ? "撤回" :
                    w.VerifyStatus == 5 ? "作废" :
                    w.VerifyStatus == 6 ? "终止" :
                    w.VerifyStatus == 8 ? "退回修改" :
                    w.VerifyStatus == 9 ? "加批" :
                    w.VerifyStatus == 10 ? "转交" :
                    "",
                VerifyDate = w.VerifyDate,
                VerifyDesc = w.VerifyDesc,
                OriginalId = w.OriginalId,
                ElementId = w.TaskDefKey
            }).OrderByDescending(a => a.VerifyDate)
            .ToList();
        return bpmVerifyInfoVos;
    }

    public BpmVerifyInfo? FindByProcessCodeAndVerifyUserId(string processNumber, string assignee)
    {
        return Db.Queryable<BpmVerifyInfo>()
            .Where(a => a.ProcessCode == processNumber && a.VerifyUserId == assignee)
            .First();
    }

    public List<BpmVerifyInfo> FindByRunInfoIdAndTaskDefKey(string runInfoId, string taskDefKey)
    {
        return Db.Queryable<BpmVerifyInfo>()
            .Where(a => a.RunInfoId == runInfoId && a.TaskDefKey == taskDefKey)
            .ToList();
    }
}
