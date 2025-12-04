using InvoiceSystem.Domain.Entities;

namespace InvoiceSystem.Domain.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
    Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
}
