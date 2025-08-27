namespace AntFlow.Core.Service.Processor;

public interface IAntFlowOrderPreProcessor<in T> : IOrderedService
{
    void PreWriteProcess(T t);
    void PreReadProcess(T t);
}