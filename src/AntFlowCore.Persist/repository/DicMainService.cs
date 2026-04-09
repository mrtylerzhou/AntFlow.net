using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class DicMainService : AFBaseCurdRepositoryService<DictMain>,IDicMainService
{
    public DicMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}