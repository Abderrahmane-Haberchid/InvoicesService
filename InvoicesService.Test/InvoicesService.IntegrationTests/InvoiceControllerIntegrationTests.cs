namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class InvoiceControllerIntegrationTests : IClassFixture<InvoiceWebApplicationFactory>
{

    private readonly InvoiceWebApplicationFactory _factory;
    
    public InvoiceControllerIntegrationTests(InvoiceWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturn200_WhenInvoiceIsSaved()
    {
        var client = _factory.CreateClient(); 
        
    }
}