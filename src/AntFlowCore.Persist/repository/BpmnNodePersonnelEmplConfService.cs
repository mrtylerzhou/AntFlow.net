using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodePersonnelEmplConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelEmplConf>,IBpmnNodePersonnelEmplConfService
{
    public BpmnNodePersonnelEmplConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}