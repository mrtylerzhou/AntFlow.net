using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class UserMessageStatusService: AFBaseCurdRepositoryService<UserMessageStatus>,IUserMessageStatusService
{
    public UserMessageStatusService(IFreeSql freeSql) : base(freeSql)
    {
    }
}