using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeConditionsConfService : AFBaseCurdRepositoryService<BpmnNodeConditionsConf>,
    IBpmnNodeConditionsConfService
{
    public BpmnNodeConditionsConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}