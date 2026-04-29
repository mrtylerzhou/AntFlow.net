using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmAdminPersonnelRepository : RepositoryBase<OutSideBpmAdminPersonnel>, IOutSideBpmAdminPersonnelRepository
{
    public SsOutSideBpmAdminPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
