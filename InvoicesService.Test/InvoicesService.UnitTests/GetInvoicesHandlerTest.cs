using Application.Abstractions;
using Application.Common;
using Application.Features.GetInvoices;
using Domain.Enums;
using Domain.Models.Invoice;
using Domain.Respository;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class GetInvoicesHandlerTest
{
    private readonly Mock<IInvoiceRepository> _invoiceRespositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly GetInvoicesQueryValidator _validator;

    private readonly GetInvoicesHandler _sut;

    public GetInvoicesHandlerTest()
    {
        _validator = new GetInvoicesQueryValidator();
        _invoiceRespositoryMock = new Mock<IInvoiceRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        
        _sut = new GetInvoicesHandler(
            _invoiceRespositoryMock.Object, 
            _cacheServiceMock.Object,
            _validator,
            NullLogger<GetInvoicesHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedList_WhenRulesPassed()
    {
        var query = new GetInvoicesQuery(1, 10);
        var invoices = GenerateInvoices().ToResponses();

        _cacheServiceMock
            .Setup(x => x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<List<GetInvoicesResponse>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);
            
        
        var result = await _sut.Handle(query, CancellationToken.None);

        result.Should().BeOfType(typeof(PagedList<GetInvoicesResponse>));
    }

    [Fact]
    public async Task Handle_ShouldInvokeCache_WhenFetchingInvoices()
    {
        var query = new GetInvoicesQuery(1, 10);
        var invoices = GenerateInvoices().ToResponses();
        
        _cacheServiceMock.Setup(x => 
            x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<List<GetInvoicesResponse>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);
        
        await _sut.Handle(query, CancellationToken.None);
        
        _cacheServiceMock.Verify(x => 
            x.GetOrCreateAsync(
                It.Is<string>(x => x == $"cached-invoices-{query.Page}-{query.PageSize}"),
                It.IsAny<Func<CancellationToken, Task<List<GetInvoicesResponse>>>>(),
                It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _invoiceRespositoryMock.Verify(x => 
                x.GetAllInvoicesAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldInvokeDatabaseRepository_WhenFetchingInvoices()
    {
        var query = new GetInvoicesQuery(1, 10);
        var invoices = GenerateInvoices();
        
        _invoiceRespositoryMock.Setup(x =>
                x.GetAllInvoicesAsync(
                    It.Is<int>(x => x == query.Page),
                    It.Is<int>(x => x == query.PageSize),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);
        
        _cacheServiceMock.Setup(x => 
            x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<List<GetInvoicesResponse>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<
                string, 
                Func<CancellationToken, Task<List<GetInvoicesResponse>>>,
                CancellationToken>(
                async (_, factory, ct) => await factory(ct)
                );
        
        var result = await _sut.Handle(query, CancellationToken.None);
        result.Items.Should().HaveCount(3);
        result.PageSize.Should().Be(10);
        result.PageNumber.Should().Be(1);
        
        _invoiceRespositoryMock.Verify(x => 
            x.GetAllInvoicesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoInvoiceFound()
    {
        var query = new GetInvoicesQuery(1, 10);
        
        _cacheServiceMock.Setup(x => 
            x.GetOrCreateAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<List<GetInvoicesResponse>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetInvoicesResponse>());

        var result = await _sut.Handle(query, CancellationToken.None);
        result.Items.Should().BeEmpty();
    } 
    
    
    private List<Invoice> GenerateInvoices()
    {
        return new List<Invoice>
        {
            Invoice.Create(Guid.NewGuid(),123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR),
            Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR)
        };
    }
}