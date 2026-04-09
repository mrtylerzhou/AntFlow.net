using AntFlowCore.Base.interf;
using AntFlowCore.Engine.Engine.service;

namespace AntFlowCore.Engine.service.processor;

public interface IAntFlowOrderPreProcessor<in T> : IOrderedService
{
    void PreWriteProcess(T t);
    void PreReadProcess(T t);
}