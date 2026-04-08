using AntFlowCore.Base.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserMessageService : IBaseRepositoryService<UserMessage>
{
    void ReadNode(string node);
    void InsertMessage(UserMessage message);
    void SaveBatch(List<UserMessage> list);
}
