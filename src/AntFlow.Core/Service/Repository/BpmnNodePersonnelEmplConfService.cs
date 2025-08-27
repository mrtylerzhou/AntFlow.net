using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodePersonnelEmplConfService : AFBaseCurdRepositoryService<BpmnNodePersonnelEmplConf>,
    IBpmnNodePersonnelEmplConfService
{
    public BpmnNodePersonnelEmplConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}