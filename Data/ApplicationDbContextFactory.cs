using Microsoft.EntityFrameworkCore;

namespace FEALVES.AspNetMVCCore.Boilerpate.Data
{
    public class ApplicationDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApplicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var provider = _configuration.GetValue<string>("DatabaseProvider");

            switch (provider)
            {
                case "SqlServer":
                    optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlServerConnection"));
                    break;
                case "PostgreSQL":
                    optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSqlConnection"));
                    break;
                case "MySQL":
                    optionsBuilder.UseMySql(_configuration.GetConnectionString("MySqlConnection"), new MySqlServerVersion(new Version(8, 0, 21)));
                    break;
                case "SQLite":
                    optionsBuilder.UseSqlite(_configuration.GetConnectionString("SqliteConnection"));
                    break;
                case "InMemory":
                    optionsBuilder.UseInMemoryDatabase("InMemoryDb");
                    break;
                default:
                    throw new InvalidOperationException("Unknown database provider");
            }

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
