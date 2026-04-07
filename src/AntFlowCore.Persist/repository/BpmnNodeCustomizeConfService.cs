using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeCustomizeConfService: AFBaseCurdRepositoryService<BpmnNodeCustomizeConf>,IBpmnNodeCustomizeConfService
{
    public BpmnNodeCustomizeConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}