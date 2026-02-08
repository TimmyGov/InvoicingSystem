using InvoiceSystem.Domain.Enums;

namespace InvoiceSystem.Application.DTOs;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public CustomerDto Customer { get; set; } = null!;
    public List<InvoiceItemResponseDto> Items { get; set; } = new();
}
