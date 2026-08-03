using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Report>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Report>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Remove(report);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
