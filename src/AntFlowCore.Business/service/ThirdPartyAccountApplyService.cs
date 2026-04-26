using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class ThirdPartyAccountApplyService: AFBaseCurdRepositoryService<ThirdPartyAccountApply>, IThirdPartyAccountApplyService
{
    public ThirdPartyAccountApplyService(IFreeSql freeSql) : base(freeSql)
    {
    }
}