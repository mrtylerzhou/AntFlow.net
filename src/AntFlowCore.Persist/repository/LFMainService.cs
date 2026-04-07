using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class LFMainService: AFBaseCurdRepositoryService<LFMain>,ILFMainService
{
    public LFMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}