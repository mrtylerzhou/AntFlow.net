using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.dto;

/// <summary>
/// 下一节点任务上下文 DTO,由 BpmnTaskListener 在任务到达时构建,
/// 传递给所有 <see cref="AntFlowCore.Engine.service.processor.IAntFlowOrderPostProcessor{T}"/> 实现处理.
/// 对应 Java BpmNextTaskDto.
/// </summary>
public class BpmNextTaskDto
{
    public string TaskId { get; set; }
    public string TaskName { get; set; }
    public string Assignee { get; set; }
    public string ProcessNumber { get; set; }
    public string ProcessInstanceId { get; set; }
    public string TaskDefKey { get; set; }
    public string BpmnCode { get; set; }
    public string BusinessId { get; set; }
    public string StartUser { get; set; }
    public string FormCode { get; set; }
    public string BpmnName { get; set; }
    public bool? IsOutSide { get; set; }
    public List<BpmnNodeLabelVO> NodeLabels { get; set; }
    public BusinessDataVo BusinessDataVo { get; set; }
    public BpmAfTask DelegateTask { get; set; }
}
