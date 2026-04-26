using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodePersonnelEmplConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelEmplConf>,IBpmnNodePersonnelEmplConfService
{
    public BpmnNodePersonnelEmplConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}