using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.factory;

namespace AntFlowCore.Engine.Abstraction.aop;

/// <summary>
/// Implementation of proxy factory that uses BpmnSendMessageAspect for cross-cutting concerns.
/// </summary>
public class ProcessOperationAdaptorProxyFactory : IProcessOperationAdaptorProxyFactory
{
    public IProcessOperationAdaptor CreateProxy(IProcessOperationAdaptor adaptor)
    {
        return BpmnSendMessageAspect<IProcessOperationAdaptor>.Create(adaptor);
    }
}