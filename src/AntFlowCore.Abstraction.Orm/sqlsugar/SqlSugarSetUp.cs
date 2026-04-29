using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.sqlsugar;

public static class SqlSugarSetUp
{
    public static void SqlSugarSet(this IServiceCollection services, IConfiguration configuration)
    {
        SqlSugarScope sqlSugar = new SqlSugarScope(
            new ConnectionConfig
            {
                ConnectionString = configuration.GetConnectionString("MySqlConnection"),
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.SystemTable
            },
            db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine($"Sql：{sql}");
                };
                SqlSugarFluentConfiguration.ConfigureEntities(db);
            });

        services.AddSingleton<SqlSugarScope>(sqlSugar);
        services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }
}
