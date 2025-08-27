using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeOutSideAccessConfService : AFBaseCurdRepositoryService<BpmnNodeOutSideAccessConf>,
    IBpmnNodeOutSideAccessConfService
{
    public BpmnNodeOutSideAccessConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}