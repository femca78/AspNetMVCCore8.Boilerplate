using Microsoft.EntityFrameworkCore;

namespace FEALVES.AspNetMVCCore.Boilerpate.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<MyEntity> MyEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entities here
    }
}

public class MyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}