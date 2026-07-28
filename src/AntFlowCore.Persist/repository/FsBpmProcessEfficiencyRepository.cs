using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmProcessEfficiencyRepository : RepositoryBase<BpmProcessEfficiency>, IBpmProcessEfficiencyRepository
{
    public FsBpmProcessEfficiencyRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }

    public int DeleteByProcessNumber(string processNumber)
    {
        return _ormContext.FreeSql.Delete<BpmProcessEfficiency>()
            .Where(a => a.ProcessNumber == processNumber)
            .ExecuteAffrows();
    }

    public (List<BpmProcessEfficiency> Data, int Total) PageProcessLevel(
        string tenantId, string formCode, string processNumber,
        int? processState, DateTime? startTimeBegin, DateTime? startTimeEnd,
        List<string> procInstIds, int page, int pageSize)
    {
        var query = _ormContext.FreeSql.Select<BpmProcessEfficiency>()
            .Where(a => a.StaticType == BpmProcessEfficiency.TYPE_PROCESS);

        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(a => a.TenantId == tenantId);
        }
        if (!string.IsNullOrEmpty(formCode))
        {
            query = query.Where(a => a.FormCode == formCode);
        }
        if (!string.IsNullOrEmpty(processNumber))
        {
            query = query.Where(a => a.ProcessNumber.Contains(processNumber));
        }
        if (processState.HasValue)
        {
            query = query.Where(a => a.ProcessState == processState.Value);
        }
        if (startTimeBegin.HasValue)
        {
            query = query.Where(a => a.StartTime >= startTimeBegin.Value);
        }
        if (startTimeEnd.HasValue)
        {
            query = query.Where(a => a.StartTime <= startTimeEnd.Value);
        }
        if (procInstIds != null && procInstIds.Count > 0)
        {
            query = query.Where(a => procInstIds.Contains(a.ProcInstId));
        }

        query = query.OrderByDescending(a => a.StartTime);

        int total = (int)query.Count();
        var data = query.Page(page, pageSize).ToList();
        return (data, total);
    }
}
