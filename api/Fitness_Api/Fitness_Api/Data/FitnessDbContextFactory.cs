using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fitness_Api.Data;

public class FitnessDbContextFactory : IDesignTimeDbContextFactory<FitnessDbContext>
{
    public FitnessDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Path.Combine(basePath, "api", "Fitness_Api", "Fitness_Api");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FitnessDbContext>();
        var connection = configuration.GetConnectionString("FitnessDbString")
            ?? "Host=localhost;Port=5432;Database=fitness_db;Username=postgres;Password=123";
        optionsBuilder.UseNpgsql(connection);

        return new FitnessDbContext(optionsBuilder.Options);
    }
}
