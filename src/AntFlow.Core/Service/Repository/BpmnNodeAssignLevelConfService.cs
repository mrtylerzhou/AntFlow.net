using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeAssignLevelConfService : AFBaseCurdRepositoryService<BpmnNodeAssignLevelConf>,
    IBpmnNodeAssignLevelConfService
{
    public BpmnNodeAssignLevelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}