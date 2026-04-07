using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class AFExecutionService: AFBaseCurdRepositoryService<BpmAfExecution>,IAFExecutionService
{
    public AFExecutionService(IFreeSql freeSql) : base(freeSql)
    {
        this.Frsql = freeSql;
    }

    public IFreeSql Frsql { get; }
}