using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IDefaultTemplateService : IAntFlowRepositoryMix<DefaultTemplate, IDefaultTemplateRepository>
{
    void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list);
}
