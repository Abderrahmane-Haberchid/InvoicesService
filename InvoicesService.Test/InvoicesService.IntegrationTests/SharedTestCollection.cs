using InvoicesServiceTest.InvoicesService.IntegrationTests.WebApplicationFactory;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

[CollectionDefinition(nameof(SharedTestCollection))]
public class SharedTestCollection : ICollectionFixture<InvoiceWebApplicationFactory>
{
    
}