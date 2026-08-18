using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// 流程对比(实例级): 流程监控-更多-流程对比, 与 Java 版 ProcessCompareBizService 对等.
/// 仅提供两个薄查询; 节点对齐(alignTrees)与审批人 diff 全部由前端完成。
/// 设计: .scratch/process-instance-compare-design.md §4
/// </summary>
public interface IProcessCompareBizService
{
    /// <summary>
    /// 候选实例搜索 (同 formCode, 全状态, 排除已删除), 按 create_time 倒序最多取 50 条
    /// </summary>
    List<ProcessCompareCandidateVo> CompareCandidates(string formCode, string keyword);

    /// <summary>
    /// 某实例全部加签/减签/转办记录 (bpm_flowrun_entrust, 自带 node_id)
    /// </summary>
    List<ProcessCompareEntrustVo> CompareEntrusts(string processNumber);
}
