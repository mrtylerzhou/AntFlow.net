using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

/// <summary>
/// Repository-mix service for <see cref="Base.entity.BpmBusinessDraft"/>.
/// Provides basic CRUD via the injected repository.
/// </summary>
public class BpmBusinessDraftService : IBpmBusinessDraftService
{
    public BpmBusinessDraftService(IBpmBusinessDraftRepository repository)
    {
        _repository = repository;
    }

    public IBpmBusinessDraftRepository _repository { get; }
}
