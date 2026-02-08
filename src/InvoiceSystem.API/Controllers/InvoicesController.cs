using Microsoft.AspNetCore.Mvc;
using InvoiceSystem.Application.DTOs;
using InvoiceSystem.Application.Services;

namespace InvoiceSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new invoice
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InvoiceResponseDto>> CreateInvoice([FromBody] CreateInvoiceDto dto, [FromHeader(Name = "X-User-Id")] Guid? userId)
    {
        try
        {
            // In production, userId would come from JWT token
            // For now, we use a header or default value
            var actualUserId = userId ?? Guid.Parse("00000000-0000-0000-0000-000000000001");
            
            var result = await _invoiceService.CreateInvoiceAsync(actualUserId, dto);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating invoice");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, new { error = "An error occurred while creating the invoice" });
        }
    }

    /// <summary>
    /// Get invoice by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponseDto>> GetInvoiceById(Guid id)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            
            if (invoice == null)
            {
                return NotFound(new { error = "Invoice not found" });
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice {InvoiceId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the invoice" });
        }
    }

    /// <summary>
    /// Get all invoices for a user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetUserInvoices(Guid userId)
    {
        try
        {
            var invoices = await _invoiceService.GetUserInvoicesAsync(userId);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices for user {UserId}", userId);
            return StatusCode(500, new { error = "An error occurred while retrieving invoices" });
        }
    }
}