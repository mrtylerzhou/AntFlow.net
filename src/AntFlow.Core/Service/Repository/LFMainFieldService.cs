using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class LFMainFieldService : AFBaseCurdRepositoryService<LFMainField>, ILFMainFieldService
{
    public LFMainFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}