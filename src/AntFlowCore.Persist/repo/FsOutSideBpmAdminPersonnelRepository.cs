using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsOutSideBpmAdminPersonnelRepository : RepositoryBase<OutSideBpmAdminPersonnel>, IOutSideBpmAdminPersonnelRepository
{
    public FsOutSideBpmAdminPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
