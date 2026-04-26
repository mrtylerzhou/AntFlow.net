using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DicDataSerivce: AFBaseCurdRepositoryService<DictData>,IDicDataSerivce
{
    public DicDataSerivce(IFreeSql freeSql) : base(freeSql)
    {
    }
}