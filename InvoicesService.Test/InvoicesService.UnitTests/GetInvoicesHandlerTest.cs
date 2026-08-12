using Application.Common;
using Application.Features.GetInvoices;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class GetInvoicesHandlerTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRespositoryMock;
    private readonly Mock<HybridCache> _hybridCacheMock;
    private readonly GetInvoicesQueryValidator _validator;

    private readonly GetInvoicesHandler _sut;

    public GetInvoicesHandlerTest()
    {
        _validator = new GetInvoicesQueryValidator();
        _invoiceRespositoryMock = new Mock<IInvoiceRepository>();
        _hybridCacheMock = new Mock<HybridCache>();
        
        _sut = new GetInvoicesHandler(
            _invoiceRespositoryMock.Object, 
            _hybridCacheMock.Object,
            _validator,
            NullLogger<GetInvoicesHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ShouleReturnPagedList_WhenRulesPassed()
    {
        var query = new GetInvoicesQuery(1, 10);
        var invoices = GenerateInvoice().ToResponses();

        _hybridCacheMock
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<PagedList<GetInvoicesResponse>>>>(),
                It.IsAny<HybridCacheEntryOptions>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);
        
        var result = await _sut.Handle(query, CancellationToken.None);

        result.Should().BeOfType(typeof(PagedList<GetInvoicesResponse>));
    }
    
    
    
    private List<Invoice> GenerateInvoice()
    {
        return new List<Invoice>
        {
            Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR)
        };
    }
}