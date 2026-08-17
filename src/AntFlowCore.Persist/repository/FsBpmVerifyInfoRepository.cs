using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVerifyInfoRepository : RepositoryBase<BpmVerifyInfo>, IBpmVerifyInfoRepository
{
    public FsBpmVerifyInfoRepository(AntFlowOrmContext context) : base(context)
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
            expression.And(a => vo.ProcessCodeList.Contains(a.ProcessCode));
        }

        if (!string.IsNullOrEmpty(vo.BusinessId))
        {
            expression = expression.And(a => a.BusinessId == vo.BusinessId);
        }

        // 流程诊断: 历史审批记录按 task_id 关联 bpm_af_taskinst 取 node_id (两段查询, 避免多表 lambda 重载问题)
        List<BpmVerifyInfo> infos = _ormContext.FreeSql.GetRepository<BpmVerifyInfo>()
            .Select
            .Where(expression)
            .OrderByDescending(a => a.VerifyDate)
            .ToList();
        List<string> taskIds = infos.Select(i => i.TaskId)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();
        Dictionary<string, string> nodeIdByTaskId = new();
        if (taskIds.Count > 0)
        {
            nodeIdByTaskId = _ormContext.FreeSql.GetRepository<BpmAfTaskInst>()
                .Select
                .Where(a => taskIds.Contains(a.Id))
                .ToList(a => new { a.Id, a.NodeId })
                .Where(x => !string.IsNullOrEmpty(x.NodeId))
                .ToDictionary(x => x.Id, x => x.NodeId!);
        }

        List<BpmVerifyInfoVo> bpmVerifyInfoVos = infos.Select(w => new BpmVerifyInfoVo
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
            ElementId = w.TaskDefKey,
            NodeId = !string.IsNullOrEmpty(w.TaskId) && nodeIdByTaskId.TryGetValue(w.TaskId, out var nid) ? nid : null,
            AttachmentsJson = w.AttachmentsJson
        }).ToList();
        return bpmVerifyInfoVos;
    }

    public BpmVerifyInfo? FindByProcessCodeAndVerifyUserId(string processNumber, string assignee)
    {
        return _ormContext.FreeSql.GetRepository<BpmVerifyInfo>()
            .Select.Where(a => a.ProcessCode == processNumber && a.VerifyUserId == assignee)
            .First();
    }

    public List<BpmVerifyInfo> FindByRunInfoIdAndTaskDefKey(string runInfoId, string taskDefKey)
    {
        return _ormContext.FreeSql.GetRepository<BpmVerifyInfo>()
            .Select.Where(a => a.RunInfoId == runInfoId && a.TaskDefKey == taskDefKey)
            .ToList();
    }
}
