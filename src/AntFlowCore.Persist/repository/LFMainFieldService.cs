using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class LFMainFieldService: AFBaseCurdRepositoryService<LFMainField>,ILFMainFieldService
{
    public LFMainFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}