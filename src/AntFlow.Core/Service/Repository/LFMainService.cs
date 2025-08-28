using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class LFMainService : AFBaseCurdRepositoryService<LFMain>, ILFMainService
{
    public LFMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}