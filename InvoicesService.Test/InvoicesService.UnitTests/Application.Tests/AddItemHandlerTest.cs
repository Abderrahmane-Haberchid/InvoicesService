using Application.Features.AddItem;
using Domain.Enums;
using Domain.Models.Invoice;
using Domain.Respository;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests.Application.Tests;

public class AddItemHandlerTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<ILogger<AddItemHandler>> _loggerMock;
    private readonly AddItemCommandValidator _validator;
    private readonly AddItemHandler _sut;

    public AddItemHandlerTest()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _loggerMock = new Mock<ILogger<AddItemHandler>>();
        _validator = new AddItemCommandValidator();

        _sut = new AddItemHandler(
            _validator,
            _loggerMock.Object,
            _invoiceRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_GivenValidCommand_WhenInvoiceExists_ShouldAddItemAndSave()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = Invoice.Create(Guid.NewGuid(), 1, CurrencyType.EUR);
        var command = new AddItemCommand(invoiceId, 1, 5, 10.5m);
        var ct = CancellationToken.None;

        _invoiceRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId, ct))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.Handle(command, ct);

        // Assert
        result.Should().NotBeNull();
        result.InvoiceId.Should().Be(invoiceId);
        result.ProductId.Should().Be(command.ProductId);
        result.Quantity.Should().Be(command.Quantity);
        result.Total.Should().Be(command.Quantity * command.UnitPrice);

        invoice.Items.Should().ContainSingle(i => i.ProductId == command.ProductId);
        _invoiceRepositoryMock.Verify(x => 
            x.SaveChangeAsync(ct), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_GivenInvalidCommand_ShouldThrowValidationExceptionAndLog()
    {
        // Arrange
        var command = new AddItemCommand(Guid.Empty, 0, 0, 0);
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _sut.Handle(command, ct);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation Failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
            
        _invoiceRepositoryMock.Verify(x => 
            x.SaveChangeAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInvoiceDoesNotExist_ShouldThrowKeyNotFoundExceptionAndLogWarning()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var command = new AddItemCommand(invoiceId, 1, 1, 10m);
        var ct = CancellationToken.None;

        _invoiceRepositoryMock.Setup(x => 
                x.GetInvoiceByIdAsync(invoiceId, ct))
            .ReturnsAsync((Invoice?)null);

        // Act
        Func<Task> act = () => _sut.Handle(command, ct);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invoice not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _invoiceRepositoryMock.Verify(x => 
            x.SaveChangeAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAddingDuplicateProduct_ShouldIncreaseQuantity()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = Invoice.Create(Guid.NewGuid(), 1, CurrencyType.EUR);
        // Add initial item
        var initialItem = InvoiceItem.Create(invoiceId, invoice, 1, 2, 10m);
        invoice.AddInvoiceItem(initialItem);
        
        var command = new AddItemCommand(invoiceId, 1, 3, 10m);
        var ct = CancellationToken.None;

        _invoiceRepositoryMock.Setup(x => x.GetInvoiceByIdAsync(invoiceId, ct))
            .ReturnsAsync(invoice);

        // Act
        var result = await _sut.Handle(command, ct);

        // Assert
        result.Quantity.Should().Be(5); // 2 + 3
        result.Total.Should().Be(50m); // 5 * 10
        invoice.Items.Should().ContainSingle();
        _invoiceRepositoryMock.Verify(x => x.SaveChangeAsync(ct), Times.Once);
    }
}