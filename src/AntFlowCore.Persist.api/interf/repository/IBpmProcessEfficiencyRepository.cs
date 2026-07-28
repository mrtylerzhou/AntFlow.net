using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessEfficiencyRepository : IBaseRepository<BpmProcessEfficiency>
{
    /// <summary>
    /// 根据流程编号删除统计记录
    /// </summary>
    int DeleteByProcessNumber(string processNumber);

    /// <summary>
    /// 分页查询流程级效能数据
    /// </summary>
    (List<BpmProcessEfficiency> Data, int Total) PageProcessLevel(
        string tenantId, string formCode, string processNumber,
        int? processState, DateTime? startTimeBegin, DateTime? startTimeEnd,
        List<string> procInstIds, int page, int pageSize);
}
