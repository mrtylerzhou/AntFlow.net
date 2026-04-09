using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class DicDataSerivce: AFBaseCurdRepositoryService<DictData>,IDicDataSerivce
{
    public DicDataSerivce(IFreeSql freeSql) : base(freeSql)
    {
    }
}