using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnViewPageButtonService: AFBaseCurdRepositoryService<BpmnViewPageButton>,IBpmnViewPageButtonService
{
    public BpmnViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public void DeleteByConfId(long confId)
    {
        this.baseRepo.Delete(a => a.ConfId == confId);
    }
}