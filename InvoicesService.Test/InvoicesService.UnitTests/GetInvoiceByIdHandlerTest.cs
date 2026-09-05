using Application.Features.GetInvoiceById;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class GetInvoiceByIdHandlerTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly GetInvoiceByIdQueryValidator _validator;

    private readonly GetInvoiceByIdHandler _sut;

    public GetInvoiceByIdHandlerTest()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _validator = new GetInvoiceByIdQueryValidator();
        
        _sut = new GetInvoiceByIdHandler(_invoiceRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnGetInvoiceByIdResponse_WhenRulesPasse()
    {
        var query = new GetInvoiceByIdQuery(Guid.NewGuid());
        var invoice = Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR);
        
        _invoiceRepositoryMock
            .Setup(x => x.GetInvoiceByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        
        var result = await _sut.Handle(query, default);
        
        _invoiceRepositoryMock.Verify(x => 
            x.GetInvoiceByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            , Times.Once);
        
        result.InvoiceId.Should().Be(invoice.Id);
        
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenGuidIsEmpty()
    {
        var query = new GetInvoiceByIdQuery(Guid.Empty);
        
        await Assert.ThrowsAsync<ValidationException>(() => _sut.Handle(query, default));
        
        _invoiceRepositoryMock.Verify(x => 
            x.GetInvoiceByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            , Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(null!, default));
        
        _invoiceRepositoryMock.Verify(x => 
            x.GetInvoiceByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            , Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnKeyNotFoundException_WhenInvoiceNotFound()
    {
        var query = new GetInvoiceByIdQuery(Guid.NewGuid());
        
        _invoiceRepositoryMock
            .Setup(x => x.GetInvoiceByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>  _sut.Handle(query, default));
    }
}