using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeCustomizeConfService : AFBaseCurdRepositoryService<BpmnNodeCustomizeConf>,
    IBpmnNodeCustomizeConfService
{
    public BpmnNodeCustomizeConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}