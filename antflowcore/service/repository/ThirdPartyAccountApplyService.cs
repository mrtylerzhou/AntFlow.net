using antflowcore.entity;

namespace antflowcore.service.repository;

public class ThirdPartyAccountApplyService: AFBaseCurdRepositoryService<ThirdPartyAccountApply>
{
    public ThirdPartyAccountApplyService(IFreeSql freeSql) : base(freeSql)
    {
    }
}