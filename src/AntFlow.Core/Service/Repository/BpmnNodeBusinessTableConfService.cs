using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeBusinessTableConfService : AFBaseCurdRepositoryService<BpmnNodeBusinessTableConf>,
    IBpmnNodeBusinessTableConfService
{
    public BpmnNodeBusinessTableConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}