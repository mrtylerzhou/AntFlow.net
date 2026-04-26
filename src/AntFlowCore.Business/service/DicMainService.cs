using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class DicMainService : AFBaseCurdRepositoryService<DictMain>,IDicMainService
{
    public DicMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}