using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsDefaultTemplateRepository : RepositoryBase<DefaultTemplate>, IDefaultTemplateRepository
{
    public SsDefaultTemplateRepository(AntFlowOrmContext context) : base(context)
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
            if (item.Id == null || item.Id == 0)
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
            Db.Updateable(updates).ExecuteCommand();
        }

        if (inserts.Count > 0)
        {
            Db.Insertable(inserts).ExecuteCommand();
        }
    }
}
