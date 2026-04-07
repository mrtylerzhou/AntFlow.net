using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class UserMessageStatusService: AFBaseCurdRepositoryService<UserMessageStatus>,IUserMessageStatusService
{
    public UserMessageStatusService(IFreeSql freeSql) : base(freeSql)
    {
    }
}