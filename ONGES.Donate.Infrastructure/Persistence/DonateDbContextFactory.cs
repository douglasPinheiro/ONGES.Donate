using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ONGES.Donate.Infrastructure.Persistence;

public sealed class DonateDbContextFactory : IDesignTimeDbContextFactory<DonateDbContext>
{
    public DonateDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A variavel de ambiente 'ConnectionStrings__DefaultConnection' e obrigatoria para criar o DonateDbContext em design time.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DonateDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DonateDbContext(optionsBuilder.Options);
    }
}
