using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class DefaultTemplateService : AFBaseCurdRepositoryService<DefaultTemplate>
{
    public DefaultTemplateService(IFreeSql freeSql) : base(freeSql)
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
            this.baseRepo.Update(updates);
        }

        if (inserts.Count > 0)
        {
            this.baseRepo.Insert(inserts);
        }
    }
}