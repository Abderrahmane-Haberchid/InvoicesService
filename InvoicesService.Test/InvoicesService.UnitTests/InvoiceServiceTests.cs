using FluentValidation;
using InvoicesService.Enums;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Services;
using InvoicesService.Respository;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class InvoiceServiceTests
{
    private readonly IInvoiceService _invoiceService;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IValidator<CreateInvoiceDto>> _createInvoiceDtoValidatorMock;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _createInvoiceDtoValidatorMock = new Mock<IValidator<CreateInvoiceDto>>();
        _invoiceService = new InvoiceService(
            _invoiceRepositoryMock.Object,
            _createInvoiceDtoValidatorMock.Object,
            NullLogger<InvoiceService>.Instance);
    }
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCreateInvoiceDtoIsNull()
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _invoiceService.CreateAsync(null, CancellationToken.None));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCustomerIdIsZeroAndInvoiceItemsIsEmpty()
    {
        // Arrange
        var invoiceDto = 
            new CreateInvoiceDto(0, CurrencyType.EUR, new List<CreateInvoiceItemDto>());
        
        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _invoiceService.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCustomerIdIsZero()
    {
        var invoiceItemsDto = new List<CreateInvoiceItemDto>
        {
            new (123, 5, 400),
            new (123, 5, 400),
            new (123, 5, 400),
        };
        
        var invoiceDto = new CreateInvoiceDto(0, CurrencyType.EUR, invoiceItemsDto);
        
        await Assert.ThrowsAsync<ValidationException>(() => 
            _invoiceService.CreateAsync(invoiceDto, CancellationToken.None));
    }
}