using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class LFMainFieldService: AFBaseCurdRepositoryService<LFMainField>,ILFMainFieldService
{
    public LFMainFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}