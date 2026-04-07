using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeOutSideAccessConfService: AFBaseCurdRepositoryService<BpmnNodeOutSideAccessConf>,IBpmnNodeOutSideAccessConfService
{
    public BpmnNodeOutSideAccessConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}