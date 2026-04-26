using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class ThirdPartyAccountApplyService : IThirdPartyAccountApplyService
{
    public ThirdPartyAccountApplyService(IThirdPartyAccountApplyRepository repository)
    {
        _repository = repository;
    }

    public IThirdPartyAccountApplyRepository _repository { get; }
}
