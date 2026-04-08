using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodePersonnelConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelConf>,IBpmnNodePersonnelConfService
{
    public BpmnNodePersonnelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}