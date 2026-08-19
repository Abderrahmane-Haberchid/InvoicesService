using Application.Features.GetInvoiceByCustomerId;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using FluentAssertions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class GetInvoiceByCustomerIdHandlerTest
{
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
    private readonly GetInvoiceByCustomerIdQueryValidator _validator;
    private readonly GetInvoiceByCustomerIdHandler _sut;

    public GetInvoiceByCustomerIdHandlerTest()
    {
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _validator = new GetInvoiceByCustomerIdQueryValidator();
        
        _sut = new GetInvoiceByCustomerIdHandler(_mockInvoiceRepository.Object, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnGetInvoiceByCustomerIdResponse_WhenAllRulesPasse()
    {
        var query = new GetInvoiceByCustomerIdQuery(123);
        var invoices = GenerateInvoice();
        
        _mockInvoiceRepository
            .Setup(x =>
                x.GetInvoiceByCustomerIdAsync(query.CustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int _, CancellationToken _) => invoices);
        
        var result = await _sut.Handle(query, CancellationToken.None);
        
        result.Should().BeOfType<List<GetInvoiceByCustomerIdResponse>>();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnKeyNotFoundException_WhenInvoiceNotFound()
    {
        var query = new GetInvoiceByCustomerIdQuery(123);

        _mockInvoiceRepository
            .Setup(x => x.GetInvoiceByCustomerIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>  _sut.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnArgumentNullException_WhenInvoiceQueryIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>  _sut.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnInvoiceNotFoundException_WhenInvoiceNotFound()
    {
        var query = new GetInvoiceByCustomerIdQuery(123);

        _mockInvoiceRepository
            .Setup(x => x.GetInvoiceByCustomerIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Invoice>());
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.Handle(query, CancellationToken.None));
    }
    
    
    
    
    
    
    
    
    

    private List<GetInvoiceByCustomerIdResponse> GenerateInvoiceResponse()
    {
        return new List<GetInvoiceByCustomerIdResponse>
        {
            new GetInvoiceByCustomerIdResponse(
                Guid.NewGuid(),
                Guid.NewGuid(),
                InvoiceStatus.CREATED,
                Decimal.One,
                CurrencyType.EUR,
                DateTime.UtcNow,
                new List<ItemResponse>
                {
                    new (1, 10, 100.99m),
                    new (1, 10, 100.99m),
                    new (1, 10, 100.99m)
                })
        };
    }

    private List<Invoice> GenerateInvoice()
    {
        return new List<Invoice>
        {
            Invoice.Create(Guid.NewGuid(), Guid.NewGuid(), "test@test.com", 123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), Guid.NewGuid(), "test@test.com", 123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), Guid.NewGuid(), "test@test.com", 123, CurrencyType.EUR)
        };
    }
}