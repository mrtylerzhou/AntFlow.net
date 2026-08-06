using AntFlowCore.Base.dto;
using AntFlowCore.Base.interf;

namespace AntFlowCore.Bpmn.service.processor;

/// <summary>
/// 下一节点任务处理器接口,由 BpmnTaskListener 在任务到达时按 Order 顺序调用.
/// 对应 Java AntFlowNextNodeBeforeWriteProcessor.
/// 定义在 Bpmn 层(而非 Engine 层)以避免循环依赖: BpmnTaskListener 位于 Bpmn 层,
/// 而 processor 实现位于 Engine 层,通过 ServiceProviderUtils 延迟解析实现解耦.
/// </summary>
public interface INextNodeTaskProcessor : IOrderedService
{
    void PostProcess(BpmNextTaskDto dto);
}
