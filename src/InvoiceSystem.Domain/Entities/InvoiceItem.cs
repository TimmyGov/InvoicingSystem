namespace InvoiceSystem.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    
    // Foreign key
    public Guid InvoiceId { get; set; }
    
    // Navigation property
    public Invoice Invoice { get; set; } = null!;
}
