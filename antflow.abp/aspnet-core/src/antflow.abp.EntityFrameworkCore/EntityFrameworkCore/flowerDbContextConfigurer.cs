using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace antflow.abp.EntityFrameworkCore;

public static class flowerDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<flowerDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<flowerDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
