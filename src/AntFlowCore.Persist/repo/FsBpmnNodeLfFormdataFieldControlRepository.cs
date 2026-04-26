using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace antflowcore.conf.ef;

public class FsBpmnNodeLfFormdataFieldControlRepository : RepositoryBase<BpmnNodeLfFormdataFieldControl>, IBpmnNodeLfFormdataFieldControlRepository
{
    public FsBpmnNodeLfFormdataFieldControlRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey)
    {
        List<string> nodeIds = _ormContext.FreeSql.Select<BpmVariable, BpmVariableSingle>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
            .WithTempQuery((a, b) => b.NodeId)
            .UnionAll(
                _ormContext.FreeSql.Select<BpmVariable, BpmVariableMultiplayer>()
                    .InnerJoin((a, b) => a.Id == b.VariableId)
                    .Where((a, b) => a.ProcessNum == processNumber && b.ElementId == taskDefKey)
                    .WithTempQuery((a, b) => b.NodeId)
            )
            .ToList();

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
