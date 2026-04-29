using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeRepository : RepositoryBase<BpmnNode>, IBpmnNodeRepository
{
    public SsBpmnNodeRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property)
    {
        return Db.Queryable<BpmnConf>()
            .InnerJoin<BpmnNode>((a, b) => a.Id == b.ConfId)
            .Where((a, b) => a.FormCode == formCode && a.EffectiveStatus == 1 && b.NodeProperty == property)
            .OrderByDescending(a => a.CreateTime)
            .Select((a, b) => b)
            .ToList();
    }

    public int? GetCustomizeNodeSignType(long nodeId)
    {
        return Db.Queryable<BpmnNode>()
            .InnerJoin<BpmnNodeCustomizeConf>((a, b) => a.Id == b.BpmnNodeId)
            .Where((a, b) => a.Id == nodeId)
            .Select((a, b) => b.SignType)
            .First();
    }

    public int UpdateConfExtraFlags(long confId, int? extraFlags)
    {
        return Db.Updateable<BpmnConf>()
            .SetColumns(a => a.ExtraFlags == extraFlags)
            .Where(a => a.Id == confId)
            .ExecuteCommand();
    }
}
