using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableApproveRemindService: IBpmVariableApproveRemindService
{
    public BpmVariableApproveRemindService(IBpmVariableApproveRemindRepository repository)
    {
        _repository = repository;
    }

    public IBpmVariableApproveRemindRepository _repository { get; }
}
