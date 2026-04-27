using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsThirdPartyAccountApplyRepository : RepositoryBase<ThirdPartyAccountApply>, IThirdPartyAccountApplyRepository
{
    public FsThirdPartyAccountApplyRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
