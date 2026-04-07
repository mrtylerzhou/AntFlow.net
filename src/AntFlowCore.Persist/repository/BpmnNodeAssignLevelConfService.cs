using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeAssignLevelConfService: AFBaseCurdRepositoryService<BpmnNodeAssignLevelConf>,IBpmnNodeAssignLevelConfService
{
    public BpmnNodeAssignLevelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}