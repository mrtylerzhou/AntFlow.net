using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// 流程诊断 (流程管理-流程监控-更多-流程诊断), 与 Java 版 ProcessDiagnosisBizService 对等.
/// 前端与 Java 共享, .NET 仅实现后端.
/// </summary>
public interface IProcessDiagnosisBizService
{
    /// <summary>
    /// 诊断初始化: processNumber → confId/bpmnCode/发起人/当前表单值
    /// </summary>
    ProcessDiagnosisInitVo DiagnosisInit(string processNumber);

    /// <summary>
    /// 节点归因诊断, 短路矩阵见 .scratch/process-diagnosis-design.md §5
    /// </summary>
    NodeDiagnosisVo DiagnoseNode(NodeDiagnosisRequestVo request);
}
