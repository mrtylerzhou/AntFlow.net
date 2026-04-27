using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsDefaultTemplateRepository : RepositoryBase<DefaultTemplate>, IDefaultTemplateRepository
{
    public FsDefaultTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list)
    {
        if (list == null || list.Count == 0)
            return;

        var updates = new List<DefaultTemplate>();
        var inserts = new List<DefaultTemplate>();

        foreach (var item in list)
        {
            if (item.Id == 0)
            {
                inserts.Add(item);
            }
            else
            {
                updates.Add(item);
            }
        }

        if (updates.Count > 0)
        {
            _ormContext.FreeSql.Update<DefaultTemplate>().SetSource(updates).ExecuteAffrows();
        }

        if (inserts.Count > 0)
        {
            _ormContext.FreeSql.Insert(inserts).ExecuteAffrows();
        }
    }
}
