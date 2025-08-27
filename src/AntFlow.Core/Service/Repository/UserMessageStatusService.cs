using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class UserMessageStatusService : AFBaseCurdRepositoryService<UserMessageStatus>, IUserMessageStatusService
{
    public UserMessageStatusService(IFreeSql freeSql) : base(freeSql)
    {
    }
}