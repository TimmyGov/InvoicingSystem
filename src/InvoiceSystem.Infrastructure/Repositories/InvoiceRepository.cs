using Microsoft.EntityFrameworkCore;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Domain.Enums;
using InvoiceSystem.Domain.Interfaces;
using InvoiceSystem.Infrastructure.Data;

namespace InvoiceSystem.Infrastructure.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        var today = DateTime.UtcNow.Date;
        
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .Include(i => i.Notifications)
            .Where(i => i.DueDate.Date < today 
                && i.Status != InvoiceStatus.Paid 
                && i.Status != InvoiceStatus.Cancelled)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public new async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Items)
            .Include(i => i.Notifications)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}
