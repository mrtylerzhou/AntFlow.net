namespace antflowcore.conf.freesql;

public static class FreeSqlSetUp
{
    public static void FreeSqlSet(this IServiceCollection services, IConfiguration configuration)
    {
        Func<IServiceProvider, IFreeSql> fsqlFactory = _ =>
        {
            //OracleConnection这是appsetting中数据库连接的字符串
            IFreeSql fsql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(FreeSql.DataType.Oracle, configuration.GetConnectionString("OracleConnection"))
                .UseQuoteSqlName(false) 
                .UseAdoConnectionPool(true)
                .UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}")) //监听SQL语句
                .UseAutoSyncStructure(false) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
                .Build();
            
            return fsql;
        };
        services.AddSingleton<IFreeSql>(fsqlFactory);
    }
}