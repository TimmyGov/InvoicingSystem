using InvoiceSystem.Application.Services;

namespace InvoiceSystem.Infrastructure.Jobs;

public class NotificationJob
{
    private readonly IInvoiceService _invoiceService;

    public NotificationJob(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task SendOverdueNotifications()
    {
        await _invoiceService.SendOverdueNotificationsAsync();
    }
}
