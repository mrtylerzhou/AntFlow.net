namespace AntFlowCore.Core.factory;

using AntFlowCore.Core.adaptor.processoperation;

/// <summary>
/// Factory interface for creating process operation adaptor proxies.
/// Implementations can add cross-cutting concerns like message sending.
/// </summary>
public interface IProcessOperationAdaptorProxyFactory
{
    /// <summary>
    /// Creates a proxy for the given process operation adaptor.
    /// </summary>
    /// <param name="adaptor">The original adaptor to wrap.</param>
    /// <returns>A proxied adaptor or the original if no proxy is needed.</returns>
    IProcessOperationAdaptor CreateProxy(IProcessOperationAdaptor adaptor);
}