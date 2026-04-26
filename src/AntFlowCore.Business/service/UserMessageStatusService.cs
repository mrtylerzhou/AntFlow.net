using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class UserMessageStatusService : IUserMessageStatusService
{
    public UserMessageStatusService(IUserMessageStatusRepository repository)
    {
        _repository = repository;
    }

    public IUserMessageStatusRepository _repository { get; }
}
