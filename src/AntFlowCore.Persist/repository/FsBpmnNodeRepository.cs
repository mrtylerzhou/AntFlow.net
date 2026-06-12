using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnNodeRepository : RepositoryBase<BpmnNode>, IBpmnNodeRepository
{
    public FsBpmnNodeRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmnNode> GetNodesByFormCodeAndProperty(string formCode, int property)
    {
        return _ormContext.FreeSql.Select<BpmnConf, BpmnNode>()
            .InnerJoin((a, b) => a.Id == b.ConfId)
            .Where((a, b) => a.FormCode == formCode && a.EffectiveStatus == 1 && b.NodeProperty == property)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList((a, b) => b);
    }

    public int UpdateConfExtraFlags(long confId, int? extraFlags)
    {
        return _ormContext.FreeSql.Update<BpmnConf>()
            .Set(a => a.ExtraFlags, extraFlags)
            .Where(a => a.Id == confId)
            .ExecuteAffrows();
    }
}
