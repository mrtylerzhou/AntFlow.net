using AntFlowCore.Core.util;
using AntFlowCore.Engine.Engine.service;

namespace AntFlowCore.Engine.service.processor;

public interface IAntFlowOrderPostProcessor<T>: IOrderedService
{
    void PostProcess(T t);
}