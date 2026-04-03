using Microsoft.EntityFrameworkCore;
using ONGES.Donate.Application.Interfaces;
using ONGES.Donate.Domain.Entities;
using ONGES.Donate.Infrastructure.Persistence;

namespace ONGES.Donate.Infrastructure.Repositories;

public sealed class DonationRepository(DonateDbContext context) : IDonationRepository
{
    public Task<bool> ExistsAsync(Guid donationId, CancellationToken cancellationToken = default)
        => context.Donations.AnyAsync(donation => donation.Id == donationId, cancellationToken);

    public async Task AddAsync(DonationEntity donation, CancellationToken cancellationToken = default)
        => await context.Donations.AddAsync(donation, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
