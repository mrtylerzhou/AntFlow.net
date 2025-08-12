using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class UserMessageService :AFBaseCurdRepositoryService<UserMessage>,IUserMessageService
{
    public UserMessageService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
    public void ReadNode(string node)
    {
        List<UserMessage> userMessages = this.baseRepo.Where(a=>a.Node==node).ToList();
        foreach (UserMessage userMessage in userMessages)
        {
            userMessage.IsRead=true;
            this.baseRepo.Update(userMessage);
        }
    }

    public void InsertMessage(UserMessage message)
    {
        this.baseRepo.Insert(message);
    }

    public void SaveBatch(List<UserMessage> list)
    {
        this.baseRepo.Insert(list);
    }
}