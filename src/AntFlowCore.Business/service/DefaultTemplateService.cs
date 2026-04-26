using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DefaultTemplateService : IDefaultTemplateService
{
    public DefaultTemplateService(IDefaultTemplateRepository repository)
    {
        _repository = repository;
    }

    public IDefaultTemplateRepository _repository { get; }

    public void InsertOrUpdateAllColumnBatch(List<DefaultTemplate> list)
    {
        _repository.InsertOrUpdateAllColumnBatch(list);
    }
}
