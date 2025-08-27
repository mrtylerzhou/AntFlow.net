using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class DicDataSerivce : AFBaseCurdRepositoryService<DictData>, IDicDataSerivce
{
    public DicDataSerivce(IFreeSql freeSql) : base(freeSql)
    {
    }
}