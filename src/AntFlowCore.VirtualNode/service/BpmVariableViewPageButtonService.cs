using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableViewPageButtonService : IBpmVariableViewPageButtonService
{
    public BpmVariableViewPageButtonService(IBpmVariableViewPageButtonRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableViewPageButtonRepository _repository { get; }

    public List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum)
    {
        return _repository.GetButtonsByProcessNumber(processNum);
    }
}
