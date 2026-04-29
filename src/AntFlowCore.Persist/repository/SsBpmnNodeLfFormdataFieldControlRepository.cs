using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeLfFormdataFieldControlRepository : RepositoryBase<BpmnNodeLfFormdataFieldControl>, IBpmnNodeLfFormdataFieldControlRepository
{
    public SsBpmnNodeLfFormdataFieldControlRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey)
    {
        List<string> singleNodeIds = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableSingle>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
            .Select((a, b) => b.NodeId)
            .ToList();

        List<string> multiplayerNodeIds = Db.Queryable<BpmVariable>()
            .LeftJoin<BpmVariableMultiplayer>((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
            .Select((a, b) => b.NodeId)
            .ToList();

        List<string> nodeIds = singleNodeIds.Union(multiplayerNodeIds).ToList();

        List<LFFieldControlVO> lfFieldControlVos = GetQueryable()
            .Where(a => nodeIds.Contains(a.NodeId.ToString()))
            .ToList()
            .Select(a => new LFFieldControlVO
            {
                FieldId = a.FieldId,
                FieldName = a.FieldName,
                Perm = a.Perm,
            }).ToList();
        return lfFieldControlVos;
    }
}
