using AutoMapper;
using InvoiceSystem.Application.DTOs;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Domain.Enums;
using InvoiceSystem.Domain.Interfaces;

namespace InvoiceSystem.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IMapper _mapper;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IRepository<Customer> customerRepository,
        IRepository<Notification> notificationRepository,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _customerRepository = customerRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(Guid userId, CreateInvoiceDto dto)
    {
        // Verify customer exists and belongs to user
        var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
        if (customer == null || customer.UserId != userId)
        {
            throw new InvalidOperationException("Customer not found or does not belong to user");
        }

        // Create invoice entity
        var invoice = _mapper.Map<Invoice>(dto);
        invoice.Id = Guid.NewGuid();
        invoice.UserId = userId;
        invoice.InvoiceNumber = GenerateInvoiceNumber();
        invoice.Status = InvoiceStatus.Draft;
        invoice.CreatedAt = DateTime.UtcNow;

        // Calculate total amount
        invoice.TotalAmount = invoice.Items.Sum(item => item.Quantity * item.UnitPrice);

        // Set IDs for items
        foreach (var item in invoice.Items)
        {
            item.Id = Guid.NewGuid();
            item.InvoiceId = invoice.Id;
            item.Amount = item.Quantity * item.UnitPrice;
        }

        // Save invoice
        await _invoiceRepository.AddAsync(invoice);

        // Return response
        var result = await _invoiceRepository.GetByIdAsync(invoice.Id);
        return _mapper.Map<InvoiceResponseDto>(result);
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        return invoice == null ? null : _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesAsync(Guid userId)
    {
        var invoices = await _invoiceRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<InvoiceResponseDto>>(invoices);
    }

    public async Task SendOverdueNotificationsAsync()
    {
        var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync();

        foreach (var invoice in overdueInvoices)
        {
            // Check if notification already sent today
            var today = DateTime.UtcNow.Date;
            var hasNotificationToday = invoice.Notifications
                .Any(n => n.IsSent && n.SentAt.HasValue && n.SentAt.Value.Date == today);

            if (!hasNotificationToday)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    Message = $"Invoice {invoice.InvoiceNumber} is overdue. Due date was {invoice.DueDate:yyyy-MM-dd}",
                    IsSent = true,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow
                };

                await _notificationRepository.AddAsync(notification);

                // Update invoice status to Overdue if not already
                if (invoice.Status != InvoiceStatus.Overdue && invoice.Status != InvoiceStatus.Paid)
                {
                    invoice.Status = InvoiceStatus.Overdue;
                    invoice.UpdatedAt = DateTime.UtcNow;
                    await _invoiceRepository.UpdateAsync(invoice);
                }
            }
        }
    }

    private string GenerateInvoiceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"INV-{timestamp}-{random}";
    }
}
