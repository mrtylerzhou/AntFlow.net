namespace antflowcore.service.processor;

public interface IAntFlowOrderPostProcessor<T>: IOrderedService
{
    void PostProcess(T t);
}