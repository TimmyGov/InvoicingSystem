using InvoiceSystem.Application.DTOs;

namespace InvoiceSystem.Application.Services;

public interface IInvoiceService
{
    Task<InvoiceResponseDto> CreateInvoiceAsync(Guid userId, CreateInvoiceDto dto);
    Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id);
    Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesAsync(Guid userId);
    Task SendOverdueNotificationsAsync();
}
