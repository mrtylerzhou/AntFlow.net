namespace antflowcore.service.processor;

public interface IAntFlowOrderPreProcessor<in T> : IOrderedService
{
    void PreWriteProcess(T t);

    void PreReadProcess(T t);
}