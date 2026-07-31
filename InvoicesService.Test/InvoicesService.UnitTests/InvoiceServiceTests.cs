using FluentAssertions;
using FluentValidation;
using InvoicesService.Enums;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Services;
using InvoicesService.Features.CreateInvoice.Validators;
using InvoicesService.Models;
using InvoicesService.Respository;
using InvoicesService.Shared.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class InvoiceServiceTests
{
    private readonly InvoiceService _sut;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly InvoiceRequestValidator _invoiceRequestValidator;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        
        _invoiceRepositoryMock
            .Setup(x =>
                x.CreateInvoiceAsync(
                    It.IsAny<Invoice>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice invoice, CancellationToken ct) => invoice);
        
        _invoiceRequestValidator = new InvoiceRequestValidator();
        
        _sut = new InvoiceService(
            _invoiceRepositoryMock.Object,
            _invoiceRequestValidator,
            _publishEndpointMock.Object,
            NullLogger<InvoiceService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateInvoice_WhenInvoiceIsCreated()
    {
        
        var invoiceDto = new InvoiceRequest(
            123, 
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(123, 5, 400),
                new(123, 5, 400)
            ]);
        
        var result = await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        _invoiceRepositoryMock.Verify(x => 
                x.CreateInvoiceAsync(
                    It.Is<Invoice>(i => i.CustomerId ==  invoiceDto.CustomerId && 
                                        i.Items.Count == invoiceDto.Items.Count &&
                                        i.Status == InvoiceStatus.CREATED), 
                    It.IsAny<CancellationToken>()), 
            Times.Once);
        
        var expectedTotal = invoiceDto.Items.Sum(item => item.Quantity *  item.UnitPrice);
        
        Assert.NotNull(result);
        result.TotalAmount.Should().Be(expectedTotal);
        result.InvoiceId.Should().NotBeEmpty();
        result!.Currency.Should().Be(CurrencyType.EUR);
        result.Status.Should().Be(InvoiceStatus.CREATED);
    }

    [Fact] public async Task CreateAsync_ShouldReturnCreatedInvoice_WhenInvoiceMappingOfInvoiceItemDtotoInvoiceItemDone()
    {
        var invoiceDto = new InvoiceRequest(
            123, 
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(123, 5, 400),
                new(123, 5, 400)
            ]);
        
        await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        _invoiceRepositoryMock.Verify(x => 
            x.CreateInvoiceAsync(
                It.Is<Invoice>(i => 
                    i.Items[0].ProductId == invoiceDto.Items[0].ProductId &&
                    i.Items[0].Quantity ==  invoiceDto.Items[0].Quantity &&
                    i.Items[0].UnitPrice ==  invoiceDto.Items[0].UnitPrice),
                It.IsAny<CancellationToken>()
                ), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCreateInvoiceDtoIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.CreateAsync((InvoiceRequest)null, CancellationToken.None));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCustomerIdIsZeroAndInvoiceItemsIsEmpty()
    {
        // Arrange
        var invoiceDto = 
            new InvoiceRequest(0, CurrencyType.EUR, new List<InvoiceItemRequest>());
        
        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCustomerIdIsZero()
    {
        var invoiceItemsDto = new List<InvoiceItemRequest>
        {
            new (123, 5, 400),
            new (123, 5, 400),
            new (123, 5, 400),
        };
        
        var invoiceDto = new InvoiceRequest(0, CurrencyType.EUR, invoiceItemsDto);
        
        await Assert.ThrowsAsync<ValidationException>(() => 
            _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenInvoiceRequestItemIsEmpty()
    {
        var invoiceDto = new InvoiceRequest(10, CurrencyType.EUR, new List<InvoiceItemRequest>());
        
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldInvokeIPublishEndpoint_WhenPublishEndpointIsCalled()
    {
        var invoiceDto = new InvoiceRequest(
            123, 
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(123, 5, 400),
                new(123, 5, 400)
            ]);
        
        await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        _publishEndpointMock.Verify(x =>
            x.Publish(
                It.IsAny<InvoiceCreatedEvent>(), 
                It.IsAny<CancellationToken>())
            ,Times.Once);
    }
}