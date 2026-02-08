using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using InvoiceSystem.Application.Services;
using InvoiceSystem.Application.DTOs;
using InvoiceSystem.Application.Mappings;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Domain.Interfaces;
using InvoiceSystem.Domain.Enums;

namespace InvoiceSystem.UnitTests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
    private readonly Mock<IRepository<Customer>> _mockCustomerRepository;
    private readonly Mock<IRepository<Notification>> _mockNotificationRepository;
    private readonly IMapper _mapper;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _mockCustomerRepository = new Mock<IRepository<Customer>>();
        _mockNotificationRepository = new Mock<IRepository<Notification>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _invoiceService = new InvoiceService(
            _mockInvoiceRepository.Object,
            _mockCustomerRepository.Object,
            _mockNotificationRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidData_ReturnsInvoiceResponseDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        
        var customer = new Customer
        {
            Id = customerId,
            UserId = userId,
            Name = "Test Customer",
            Email = "test@example.com"
        };

        var createDto = new CreateInvoiceDto
        {
            CustomerId = customerId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Items = new List<CreateInvoiceItemDto>
            {
                new CreateInvoiceItemDto
                {
                    Description = "Test Item",
                    Quantity = 2,
                    UnitPrice = 100.00m
                }
            }
        };

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _mockInvoiceRepository.Setup(x => x.AddAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => i);

        _mockInvoiceRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Invoice
            {
                Id = id,
                UserId = userId,
                CustomerId = customerId,
                Customer = customer,
                InvoiceNumber = "INV-TEST",
                IssueDate = createDto.IssueDate,
                DueDate = createDto.DueDate,
                Status = InvoiceStatus.Draft,
                TotalAmount = 200.00m,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        Description = "Test Item",
                        Quantity = 2,
                        UnitPrice = 100.00m,
                        Amount = 200.00m
                    }
                }
            });

        // Act
        var result = await _invoiceService.CreateInvoiceAsync(userId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Customer.Id.Should().Be(customerId);
        result.TotalAmount.Should().Be(200.00m);
        result.Status.Should().Be(InvoiceStatus.Draft);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithNonExistentCustomer_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var createDto = new CreateInvoiceDto
        {
            CustomerId = customerId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Items = new List<CreateInvoiceItemDto>
            {
                new CreateInvoiceItemDto
                {
                    Description = "Test Item",
                    Quantity = 1,
                    UnitPrice = 100.00m
                }
            }
        };

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _invoiceService.CreateInvoiceAsync(userId, createDto));
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        _mockInvoiceRepository.Setup(x => x.GetByIdAsync(invoiceId))
            .ReturnsAsync((Invoice?)null);

        // Act
        var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserInvoicesAsync_ReturnsInvoicesList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        var invoices = new List<Invoice>
        {
            new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CustomerId = customerId,
                Customer = new Customer { Id = customerId, Name = "Test Customer", Email = "test@test.com", UserId = userId },
                InvoiceNumber = "INV-001",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = InvoiceStatus.Sent,
                TotalAmount = 100.00m,
                Items = new List<InvoiceItem>()
            }
        };

        _mockInvoiceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(invoices);

        // Act
        var result = await _invoiceService.GetUserInvoicesAsync(userId);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(1);
        result.First().InvoiceNumber.Should().Be("INV-001");
    }
}
