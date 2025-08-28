using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class UserMessageService : AFBaseCurdRepositoryService<UserMessage>, IUserMessageService
{
    public UserMessageService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void ReadNode(string node)
    {
        List<UserMessage> userMessages = baseRepo.Where(a => a.Node == node).ToList();
        foreach (UserMessage userMessage in userMessages)
        {
            userMessage.IsRead = true;
            baseRepo.Update(userMessage);
        }
    }

    public void InsertMessage(UserMessage message)
    {
        baseRepo.Insert(message);
    }

    public void SaveBatch(List<UserMessage> list)
    {
        baseRepo.Insert(list);
    }
}