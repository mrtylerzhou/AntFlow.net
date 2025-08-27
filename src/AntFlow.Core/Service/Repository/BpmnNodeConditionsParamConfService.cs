using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeConditionsParamConfService : AFBaseCurdRepositoryService<BpmnNodeConditionsParamConf>,
    IBpmnNodeConditionsParamConfService
{
    public BpmnNodeConditionsParamConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}