using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeLfFormdataFieldControlService : AFBaseCurdRepositoryService<BpmnNodeLfFormdataFieldControl>,
    IBpmnNodeLfFormdataFieldControlService
{
    public BpmnNodeLfFormdataFieldControlService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey)
    {
        List<string> nodeIds = Frsql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
            .WithTempQuery((a, b) => b.NodeId)
            .UnionAll(
                Frsql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
                    .WithTempQuery((a, b) => b.NodeId)
            )
            .ToList();
        List<LFFieldControlVO> lfFieldControlVos = baseRepo
            .Where(a => nodeIds.Contains(a.NodeId.ToString()))
            .ToList<LFFieldControlVO>(a => new LFFieldControlVO
            {
                FieldId = a.FieldId, FieldName = a.FieldName, Perm = a.Perm
            });
        return lfFieldControlVos;
    }
}