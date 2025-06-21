using AntFlowCore.Entities;

namespace antflowcore.service.repository;

public class UserMessageStatusService: AFBaseCurdRepositoryService<UserMessageStatus>
{
    public UserMessageStatusService(IFreeSql freeSql) : base(freeSql)
    {
    }
}