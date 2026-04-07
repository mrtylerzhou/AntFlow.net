using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeConditionsParamConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsParamConf>,IBpmnNodeConditionsParamConfService
{
    public BpmnNodeConditionsParamConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}