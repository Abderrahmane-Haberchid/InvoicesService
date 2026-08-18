using Asp.Versioning;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class AuthenticationController(
    IHttpClientFactory httpClient,
    IConfiguration configuration) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetToken()
    {
        var client = httpClient.CreateClient();
        var discovery = await client.GetDiscoveryDocumentAsync(configuration["Duende:Authority"]);
        
        if (discovery.IsError)
        {
            Log.Error(discovery.Error!);
            return Unauthorized();
        }

        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discovery.TokenEndpoint,
            ClientId = "mf-invoices-api",
            ClientSecret = "top-secret"
        });

        if (tokenResponse.IsError)
        {
            Log.Error(tokenResponse.Error!);
            return BadRequest(tokenResponse.Error);
        }
        return Ok(tokenResponse.AccessToken);
    }
}