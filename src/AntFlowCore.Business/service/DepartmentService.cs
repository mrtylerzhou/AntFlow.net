using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DepartmentService : IDepartmentService
{
    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public IDepartmentRepository _repository { get; }
}
