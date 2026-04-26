using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IDefaultTemplateService : IAntFlowRepositoryMix<DefaultTemplate, IDefaultTemplateRepository>
{
    void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list);
}
