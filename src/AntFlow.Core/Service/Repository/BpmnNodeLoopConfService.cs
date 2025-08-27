using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeLoopConfService : AFBaseCurdRepositoryService<BpmnNodeLoopConf>, IBpmnNodeLoopConfService
{
    public BpmnNodeLoopConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}