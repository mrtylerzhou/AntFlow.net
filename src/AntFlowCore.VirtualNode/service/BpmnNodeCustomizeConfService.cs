using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeCustomizeConfService: AFBaseCurdRepositoryService<BpmnNodeCustomizeConf>,IBpmnNodeCustomizeConfService
{
    public BpmnNodeCustomizeConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}