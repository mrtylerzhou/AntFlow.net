using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfLfFormdataService: AFBaseCurdRepositoryService<BpmnConfLfFormdata>,IBpmnConfLfFormdataService
{
    public BpmnConfLfFormdataService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnConfLfFormdata> ListByConfId(long confId)
    {
        List<BpmnConfLfFormdata> lfFormdatas = baseRepo.Select.Where(a=>a.BpmnConfId == confId).ToList();
        return lfFormdatas;
    }
    
}