namespace AntFlow.Core.Service.Processor;

public interface IAntFlowOrderPostProcessor<T> : IOrderedService
{
    void PostProcess(T t);
}