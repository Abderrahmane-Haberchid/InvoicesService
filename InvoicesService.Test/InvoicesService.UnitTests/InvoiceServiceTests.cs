using Application.Features.CreateInvoice.Dtos.requests;
using Application.Features.CreateInvoice.Services;
using Application.Features.CreateInvoice.Validators;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using FluentAssertions;
using FluentValidation;
using InvoicesService.Shared.Events;
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
        
        _invoiceRepositoryMock
            .Setup(x => x.SaveChangeAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);
        
        _invoiceRequestValidator = new InvoiceRequestValidator();
        
        _sut = new InvoiceService(
            _invoiceRepositoryMock.Object,
            _invoiceRequestValidator,
            _publishEndpointMock.Object,
            NullLogger<InvoiceService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_ShouldFindOneItem_WhenProductIdIsDuplicated()
    {
        var invoiceDto = new InvoiceRequest(
            123,
            Guid.NewGuid(),
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(123, 5, 400),
                new(123, 5, 400)
            ]);
        
        var result = await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        result.InvoiceItems.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldFindThreeItem_WhenProductIdIsDuplicated()
    {
        var invoiceDto = new InvoiceRequest(
            123,
            Guid.NewGuid(),
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(111, 5, 400),
                new(222, 5, 400),
                new(222, 5, 400),
                new(222, 5, 400)
            ]);
        
        var result = await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        result.InvoiceItems.Should().HaveCount(3);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenPublishEndpointIsInvoked()
    {
        var invoiceDto = new InvoiceRequest(
            123,
            Guid.NewGuid(),
            CurrencyType.EUR,
            [
                new(123, 5, 400),
                new(123, 5, 400),
                new(123, 5, 400)
            ]);
        
        await _sut.CreateAsync(invoiceDto, CancellationToken.None);
        
        var expectedTotal = invoiceDto.Items.Sum(item => item.Quantity *  item.UnitPrice);
        
        _publishEndpointMock.Verify(x =>
            x.Publish(
                It.Is<InvoiceCreatedEvent>(i => i.CustomerId == invoiceDto.CustomerId && i.Total == expectedTotal), 
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
    } 

    [Fact]
    public async Task CreateAsync_ShouldCreateInvoice_WhenInvoiceIsCreated()
    {
        
        var invoiceDto = new InvoiceRequest(
            123,
            Guid.NewGuid(),
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
                                        i.CompanyId == invoiceDto.CompanyId &&
                                        i.GetItems().Count() == invoiceDto.Items.Count &&
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

    [Fact] public async Task CreateAsync_ShouldReturnCreatedInvoice_WhenInvoiceMappingOfInvoiceItemDtoToInvoiceItemDone()
    {
        var invoiceDto = new InvoiceRequest(
            123, 
            Guid.NewGuid(),
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
                    i.GetItem(0).ProductId == invoiceDto.Items[0].ProductId &&
                    i.GetItem(0).Quantity ==  invoiceDto.Items[0].Quantity &&
                    i.GetItem(0).UnitPrice ==  invoiceDto.Items[0].UnitPrice),
                It.IsAny<CancellationToken>()
                ), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCreateInvoiceDtoIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.CreateAsync(null, CancellationToken.None));
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCustomerIdIsZeroAndInvoiceItemsIsEmpty()
    {
        // Arrange
        var invoiceDto = 
            new InvoiceRequest(
                0, 
                Guid.NewGuid(), 
                CurrencyType.EUR, 
                []);
        
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
        
        var invoiceDto = new InvoiceRequest(
            0,
            Guid.NewGuid(),
            CurrencyType.EUR, invoiceItemsDto);
        
        await Assert.ThrowsAsync<ValidationException>(() => 
            _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenInvoiceRequestItemIsEmpty()
    {
        var invoiceDto = new InvoiceRequest(
            10, Guid.NewGuid(),
            CurrencyType.EUR, 
            []);
        
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenCompanyIdIsEmpty()
    {
        var invoiceDto = new InvoiceRequest(
            10, 
            Guid.Empty, 
            CurrencyType.EUR,
            [new (123, 5, 400)]);
        
        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(invoiceDto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldInvokeIPublishEndpoint_WhenPublishEndpointIsCalled()
    {
        var invoiceDto = new InvoiceRequest(
            123,
            Guid.NewGuid(),
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