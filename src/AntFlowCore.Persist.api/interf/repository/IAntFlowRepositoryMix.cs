using AntFlowCore.Abstraction.Orm.repository;

namespace antflowcore.service.interf.repository;

public interface IAntFlowRepositoryMix<T, TRepo> where T : class where TRepo : IBaseRepository<T>
{
    public TRepo _repository { get; }
}