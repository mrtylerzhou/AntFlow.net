using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

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