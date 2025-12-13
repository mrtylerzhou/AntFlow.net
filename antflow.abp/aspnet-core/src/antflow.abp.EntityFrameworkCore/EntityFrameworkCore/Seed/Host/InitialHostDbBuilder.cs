namespace antflow.abp.EntityFrameworkCore.Seed.Host;

public class InitialHostDbBuilder
{
    private readonly flowerDbContext _context;

    public InitialHostDbBuilder(flowerDbContext context)
    {
        _context = context;
    }

    public void Create()
    {
        new DefaultEditionCreator(_context).Create();
        new DefaultLanguagesCreator(_context).Create();
        new HostRoleAndUserCreator(_context).Create();
        new DefaultSettingsCreator(_context).Create();

        _context.SaveChanges();
    }
}
