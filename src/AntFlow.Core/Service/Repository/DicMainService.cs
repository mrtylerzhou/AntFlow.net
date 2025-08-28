using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class DicMainService : AFBaseCurdRepositoryService<DictMain>, IDicMainService
{
    public DicMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}