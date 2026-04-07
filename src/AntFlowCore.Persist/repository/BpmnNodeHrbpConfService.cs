using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeHrbpConfService: AFBaseCurdRepositoryService<BpmnNodeHrbpConf>,IBpmnNodeHrbpConfService
{
    public BpmnNodeHrbpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}