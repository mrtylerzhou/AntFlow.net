
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using antflowcore.vo;

namespace antflowcore.service.repository;

public class BpmnNodeLfFormdataFieldControlService: AFBaseCurdRepositoryService<BpmnNodeLfFormdataFieldControl>,IBpmnNodeLfFormdataFieldControlService
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
        List<LFFieldControlVO> lfFieldControlVos = this
            .baseRepo
            .Where(a => nodeIds.Contains(a.NodeId.ToString()))
            .ToList<LFFieldControlVO>(a => new LFFieldControlVO
            {
                FieldId = a.FieldId,
                FieldName = a.FieldName,
                Perm = a.Perm,
            });
        return lfFieldControlVos;
    }
}