using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeHrbpConfService : AFBaseCurdRepositoryService<BpmnNodeHrbpConf>, IBpmnNodeHrbpConfService
{
    public BpmnNodeHrbpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}