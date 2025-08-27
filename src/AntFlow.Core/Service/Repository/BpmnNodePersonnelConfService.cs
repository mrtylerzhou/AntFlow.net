using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodePersonnelConfService : AFBaseCurdRepositoryService<BpmnNodePersonnelConf>,
    IBpmnNodePersonnelConfService
{
    public BpmnNodePersonnelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}