using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class UserMessageService : IUserMessageService
{
    public UserMessageService(IUserMessageRepository repository)
    {
        _repository = repository;
    }

    public IUserMessageRepository _repository { get; }

    public void ReadNode(string node)
    {
        List<UserMessage> userMessages = _repository.Find(a => a.Node == node);
        foreach (UserMessage userMessage in userMessages)
        {
            userMessage.IsRead = true;
            _repository.Update(userMessage);
        }
    }

    public void InsertMessage(UserMessage message)
    {
        _repository.Add(message);
    }

    public void SaveBatch(List<UserMessage> list)
    {
        _repository.AddRange(list);
    }
}
