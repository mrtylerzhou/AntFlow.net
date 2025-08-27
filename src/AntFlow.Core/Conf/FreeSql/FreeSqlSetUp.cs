using FreeSql;

namespace AntFlow.Core.Conf.FreeSql;

public static class FreeSqlSetUp
{
    public static void FreeSqlSet(this IServiceCollection services, IConfiguration configuration)
    {
        Func<IServiceProvider, IFreeSql> fsqlFactory = _ =>
        {
            //MySqlConnection����appsetting�����ݿ����ӵ��ַ���
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, configuration.GetConnectionString("MySqlConnection"))
                .UseAdoConnectionPool(true)
                .UseMonitorCommand(cmd => Console.WriteLine($"Sql��{cmd.CommandText}")) //����SQL���
                .UseAutoSyncStructure(false) //�Զ�ͬ��ʵ��ṹ�����ݿ⣬FreeSql����ɨ����򼯣�ֻ��CRUDʱ�Ż����ɱ���
                .Build();
            return fsql;
        };
        services.AddSingleton<IFreeSql>(fsqlFactory);
    }
}