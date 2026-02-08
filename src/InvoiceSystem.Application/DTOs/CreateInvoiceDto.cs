namespace InvoiceSystem.Application.DTOs;

public class CreateInvoiceDto
{
    public CustomerDto Customer { get; set; } = new();
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}
