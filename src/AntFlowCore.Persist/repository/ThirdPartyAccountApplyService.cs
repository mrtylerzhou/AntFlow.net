using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class ThirdPartyAccountApplyService: AFBaseCurdRepositoryService<ThirdPartyAccountApply>, IThirdPartyAccountApplyService
{
    public ThirdPartyAccountApplyService(IFreeSql freeSql) : base(freeSql)
    {
    }
}