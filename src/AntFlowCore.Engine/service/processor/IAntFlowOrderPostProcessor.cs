using AntFlowCore.Base.interf;

namespace AntFlowCore.Engine.service.processor;

public interface IAntFlowOrderPostProcessor<T>: IOrderedService
{
    void PostProcess(T t);
}