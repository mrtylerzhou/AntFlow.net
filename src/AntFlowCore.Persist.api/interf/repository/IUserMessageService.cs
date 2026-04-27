using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserMessageService : IAntFlowRepositoryMix<UserMessage, IUserMessageRepository>
{
    void ReadNode(string node);
    void InsertMessage(UserMessage message);
    void SaveBatch(List<UserMessage> list);
}
