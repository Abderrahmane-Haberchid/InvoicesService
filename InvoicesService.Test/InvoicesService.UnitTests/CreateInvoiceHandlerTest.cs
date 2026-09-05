using Application.Abstractions;
using Application.Exceptions;
using Application.Features.CreateInvoice;
using Domain.DomainExceptions;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using FluentAssertions;
using FluentValidation;
using InvoicesService.Shared.Contracts.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class CreateInvoiceHandlerTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly CreateInvoiceValidator _validator;
    private readonly Mock<ITenantProvider>  _tenantProviderMock;
    private ILogger<CreateInvoiceHandlerTest> _logger;
    
    private CreateInvoiceHandler _sut;
    
    public CreateInvoiceHandlerTest()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _validator = new CreateInvoiceValidator();
        _tenantProviderMock = new Mock<ITenantProvider>();
        
        _invoiceRepositoryMock
            .Setup(x => 
                x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice invoice, CancellationToken _) => invoice);

        _tenantProviderMock.Setup(x => x.TenantId).Returns(Guid.NewGuid());
        
        _sut = new CreateInvoiceHandler(
            _invoiceRepositoryMock.Object,
            _validator,
            _eventPublisherMock.Object,
            _tenantProviderMock.Object,
            NullLogger<CreateInvoiceHandler>.Instance);
    }
    
    // Happy Path
    [Fact]
    public async Task Handle_ShouldReturnCreateInvoiceResponse_WhenInvoiceIsCreated()
    {
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        var result = await _sut.Handle(command, CancellationToken.None);
        
        _invoiceRepositoryMock
            .Verify(x => 
                x.CreateInvoiceAsync(It.Is<Invoice>(
                    i => i.CustomerId == command.CustomerId &&
                         i.Items.Count == command.Items.Count), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        
        var expectedTotal = command.Items.Sum(i => i.Quantity * i.UnitPrice);
        
        _eventPublisherMock.Verify(x => 
            x.PublishAsync(It.Is<InvoiceCreatedEvent>(
                i => i.CustomerId == command.CustomerId &&
                     i.Total == expectedTotal), 
                It.IsAny<CancellationToken>()), 
            Times.Once);

        result.InvoiceItems.Should().HaveCount(3);
        result.Status.Should().Be(InvoiceStatus.CREATED);
        result.Currency.Should().Be(command.Currency);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

    }
    
    [Fact]
    public async Task Handle_ShouldThrowNullObjectReturnedFromCreateRepositoryException_WhenNoInvoiceIsSavedByRepository()
    {
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        _invoiceRepositoryMock
            .Setup(x => x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice invoice, CancellationToken _) => null!);
        
        await Assert.ThrowsAsync<NullObjectReturnedFromCreateRepositoryException>(() => _sut.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnCreateInvoiceResponse_WhenInvoiceCreated()
    {
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);

        var result = await _sut.Handle(command, CancellationToken.None);
        
        result.GetType().Name.Should().Be("CreateInvoiceResponse");
    }

    [Fact]
    public async Task Handle_ShouldThrowNullArgumentException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.Handle(null!, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCustomerIdIsInvalid()
    {
        var command = new CreateInvoiceCommand(
            0,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        await Assert.ThrowsAsync<ValidationException>(async () => await _sut.Handle(command, CancellationToken.None));
        
        _invoiceRepositoryMock.Verify(x => 
                x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _eventPublisherMock.Verify(x => 
                x.PublishAsync(It.IsAny<InvoiceCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCompanyIdIsEmpty()
    {
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        _tenantProviderMock.Setup(x => x.TenantId).Returns(Guid.Empty);
        
        await Assert.ThrowsAsync<InvalidInvoiceDataException>(() =>  _sut.Handle(command, CancellationToken.None));
        
        _invoiceRepositoryMock.Verify(x => 
            x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _eventPublisherMock.Verify(x => 
            x.PublishAsync(It.IsAny<InvoiceCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenInvoiceItemsIsEmpty()
    {
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            []);
        
        await Assert.ThrowsAsync<ValidationException>(() =>  _sut.Handle(command, CancellationToken.None));
        
        _invoiceRepositoryMock.Verify(x => 
                x.CreateInvoiceAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _eventPublisherMock.Verify(x => 
                x.PublishAsync(It.IsAny<InvoiceCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}