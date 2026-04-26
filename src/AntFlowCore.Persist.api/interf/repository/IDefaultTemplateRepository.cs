using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IDefaultTemplateRepository : IBaseRepository<DefaultTemplate>
{
    void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list);
}
