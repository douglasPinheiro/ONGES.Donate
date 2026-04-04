using ONGES.Donate.Domain.Entities;

namespace ONGES.Donate.Application.Interfaces;

public interface IDonationRepository
{
    Task<bool> ExistsAsync(Guid donationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DonationEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DonationEntity donation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
