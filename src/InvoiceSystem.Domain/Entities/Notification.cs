namespace InvoiceSystem.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    
    // Foreign key
    public Guid InvoiceId { get; set; }
    
    // Navigation property
    public Invoice Invoice { get; set; } = null!;
}
