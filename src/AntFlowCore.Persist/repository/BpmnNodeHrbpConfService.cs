using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeHrbpConfService: AFBaseCurdRepositoryService<BpmnNodeHrbpConf>,IBpmnNodeHrbpConfService
{
    public BpmnNodeHrbpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}