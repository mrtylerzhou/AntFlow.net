using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmVariableMultiplayerRepository : RepositoryBase<BpmVariableMultiplayer>, IBpmVariableMultiplayerRepository
{
    public FsBpmVariableMultiplayerRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmVariableMultiplayer> QueryMultiplayersByProcessNumAndElementId(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = _ormContext.FreeSql.Select<BpmVariable, BpmVariableMultiplayer>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNum && b.ElementId == elementId)
            .ToList<BpmVariableMultiplayer>();
        return bpmVariableMultiplayers;
    }

    /// <summary>
    /// 查询多人节点及其人员记录(LEFT JOIN personnel),返回的每条记录对应一个 personnel,
    /// UnderTakeStatus 字段承载 personnel.undertake_status。
    /// 对应 Java 版 BpmVariableMultiplayerMapper.isMoreNode 的 SQL。
    /// </summary>
    public List<BpmVariableMultiplayer> IsMoreNode(string processNum, string elementId)
    {
        List<BpmVariableMultiplayer> list = _ormContext.FreeSql.Select<BpmVariableMultiplayer, BpmVariable, BpmVariableMultiplayerPersonnel>()
            .LeftJoin((a, b, c) => a.VariableId == b.Id)
            .LeftJoin((a, b, c) => a.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => a.ElementId == elementId && b.ProcessNum == processNum)
            .ToList((a, b, c) => new BpmVariableMultiplayer
            {
                Id = a.Id,
                VariableId = a.VariableId,
                ElementId = a.ElementId,
                NodeId = a.NodeId,
                ElementName = a.ElementName,
                CollectionName = a.CollectionName,
                SignType = a.SignType,
                Remark = a.Remark,
                IsDel = a.IsDel,
                TenantId = a.TenantId,
                CreateUser = a.CreateUser,
                CreateTime = a.CreateTime,
                UpdateUser = a.UpdateUser,
                UpdateTime = a.UpdateTime,
                UnderTakeStatus = c.UndertakeStatus,
            });
        return list;
    }
}
