using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace antflowcore.conf.ef;

public class FsOutSideBpmConditionsTemplateRepository : RepositoryBase<OutSideBpmConditionsTemplate>, IOutSideBpmConditionsTemplateRepository
{
    public FsOutSideBpmConditionsTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
