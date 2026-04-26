using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class AFExecutionService:IAFExecutionService
{
    public AFExecutionService(IBpmAfExecutionRepository repository)
    {
        _repository = repository;
    }
    public IBpmAfExecutionRepository _repository { get; }
}