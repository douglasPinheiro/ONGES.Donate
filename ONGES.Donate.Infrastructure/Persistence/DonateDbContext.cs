using Microsoft.EntityFrameworkCore;
using ONGES.Donate.Domain.Entities;

namespace ONGES.Donate.Infrastructure.Persistence;

public sealed class DonateDbContext(DbContextOptions<DonateDbContext> options) : DbContext(options)
{
    public DbSet<DonationEntity> Donations => Set<DonationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DonateDbContext).Assembly);
    }
}
