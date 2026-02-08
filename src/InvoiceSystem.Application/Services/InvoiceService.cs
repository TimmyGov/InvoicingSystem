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
        // Create or find customer
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Customer.Name,
            Email = dto.Customer.Email,
            Phone = dto.Customer.Phone,
            Address = dto.Customer.Address,
            CreatedAt = DateTime.UtcNow
        };

        // Save customer
        await _customerRepository.AddAsync(customer);

        // Create invoice entity
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CustomerId = customer.Id,
            InvoiceNumber = GenerateInvoiceNumber(),
            IssueDate = DateTime.SpecifyKind(dto.IssueDate, DateTimeKind.Utc),
            DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),
            Status = InvoiceStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            Items = dto.Items.Select(itemDto => new InvoiceItem
            {
                Id = Guid.NewGuid(),
                Description = itemDto.Description,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                Amount = itemDto.Quantity * itemDto.UnitPrice
            }).ToList()
        };

        // Set invoice ID for items and calculate total amount
        foreach (var item in invoice.Items)
        {
            item.InvoiceId = invoice.Id;
        }

        invoice.TotalAmount = invoice.Items.Sum(item => item.Amount);

        // Save invoice
        await _invoiceRepository.AddAsync(invoice);

        // Return response
        var result = await _invoiceRepository.GetByIdAsync(invoice.Id);
        return _mapper.Map<InvoiceResponseDto>(result);
    }

    public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null) return null;
        
        // Update status if needed
        await UpdateInvoiceStatusIfNeeded(invoice);
        
        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<IEnumerable<InvoiceResponseDto>> GetUserInvoicesAsync(Guid userId)
    {
        var invoices = await _invoiceRepository.GetByUserIdAsync(userId);
        
        // Update status for all invoices if needed
        foreach (var invoice in invoices)
        {
            await UpdateInvoiceStatusIfNeeded(invoice);
        }
        
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

    private async Task UpdateInvoiceStatusIfNeeded(Invoice invoice)
    {
        var currentStatus = CalculateCurrentStatus(invoice);
        
        if (currentStatus != invoice.Status)
        {
            // Use the specialized status update method for better tracking
            await _invoiceRepository.UpdateInvoiceStatusAsync(invoice.Id, currentStatus);
            // Also update the in-memory object so the returned data is correct
            invoice.Status = currentStatus;
            invoice.UpdatedAt = DateTime.UtcNow;
        }
    }

    private InvoiceStatus CalculateCurrentStatus(Invoice invoice)
    {
        // Don't change Paid or Cancelled status
        if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
        {
            return invoice.Status;
        }

        // Check if invoice is overdue
        var today = DateTime.UtcNow.Date;
        var dueDate = invoice.DueDate.Date;
        
        if (dueDate < today)
        {
            return InvoiceStatus.Overdue;
        }

        // Keep current status if not overdue
        return invoice.Status;
    }

    private string GenerateInvoiceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"INV-{timestamp}-{random}";
    }
}
