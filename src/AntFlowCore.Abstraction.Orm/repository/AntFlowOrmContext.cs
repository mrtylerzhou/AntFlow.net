using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.repository;

public class AntFlowOrmContext
{
    public AntFlowOrmContext(ISqlSugarClient sqlSugar)
    {
        SqlSugar = sqlSugar;
    }

    public ISqlSugarClient SqlSugar { get; }
}
