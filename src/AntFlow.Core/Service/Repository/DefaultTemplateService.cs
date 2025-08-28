using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class DefaultTemplateService : AFBaseCurdRepositoryService<DefaultTemplate>, IDefaultTemplateService
{
    public DefaultTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }

        List<DefaultTemplate>? updates = new();
        List<DefaultTemplate>? inserts = new();

        foreach (DefaultTemplate? item in list)
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
            baseRepo.Update(updates);
        }

        if (inserts.Count > 0)
        {
            baseRepo.Insert(inserts);
        }
    }
}