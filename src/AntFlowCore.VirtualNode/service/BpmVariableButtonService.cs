using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

/// <summary>
/// 
/// </summary>
public class BpmVariableButtonService : IBpmVariableButtonService
{
    public BpmVariableButtonService(IBpmVariableButtonRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableButtonRepository _repository { get; }

    public List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds)
    {
        return _repository.GetButtonsByProcessNumber(processNum, elementIds);
    }
}
