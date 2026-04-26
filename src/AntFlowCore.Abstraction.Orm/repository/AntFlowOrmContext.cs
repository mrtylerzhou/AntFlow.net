namespace AntFlowCore.Abstraction.Orm.repository;

public class AntFlowOrmContext
{
    public AntFlowOrmContext(IFreeSql freeSql)
    {
        FreeSql = freeSql;
    }
   public IFreeSql FreeSql { get; }
}