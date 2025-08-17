using AntFlowCore.Entities;
using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class UserMessageStatusService: AFBaseCurdRepositoryService<UserMessageStatus>,IUserMessageStatusService
{
    public UserMessageStatusService(IFreeSql freeSql) : base(freeSql)
    {
    }
}