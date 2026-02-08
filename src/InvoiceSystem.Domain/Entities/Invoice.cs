using InvoiceSystem.Domain.Enums;

namespace InvoiceSystem.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign keys
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}