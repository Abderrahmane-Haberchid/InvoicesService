using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;

[Trait("Category", "Integration")]
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger,
        UrlEncoder encoder, 
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("mf:cid", TestClaims.CompanyId.ToString()),
            new Claim("mf:uid", TestClaims.UserId.ToString()),
            new Claim("email", TestClaims.Email),
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "TestScheme");

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}