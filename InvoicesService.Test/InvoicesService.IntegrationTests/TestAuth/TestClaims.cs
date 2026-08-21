namespace InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;

public static class TestClaims
{
    public static readonly Guid CompanyId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid UserId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public const string Email = "test@example.com";
}