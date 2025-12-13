using Abp.Zero.EntityFrameworkCore;
using antflow.abp.Authorization.Roles;
using antflow.abp.Authorization.Users;
using antflow.abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace antflow.abp.EntityFrameworkCore;

public class flowerDbContext : AbpZeroDbContext<Tenant, Role, User, flowerDbContext>
{
    /* Define a DbSet for each entity of the application */

    public flowerDbContext(DbContextOptions<flowerDbContext> options)
        : base(options)
    {
    }
}
