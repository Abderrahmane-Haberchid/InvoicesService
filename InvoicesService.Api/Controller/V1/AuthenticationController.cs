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
        
        Log.Information("Connecting to Duende Server to get Discovery Document");
        
        var discovery = await client.GetDiscoveryDocumentAsync(
            new DiscoveryDocumentRequest
            {
                Address = configuration["Duende:Authority"]!,
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = true
                }
            });
        
        if (discovery.IsError)
        {
            Log.Error("Error Occured: {DiscoveryError}", discovery.Error!);
            return Unauthorized();
        }
        
        Log.Information("Discovery Document received successfully...");
        Log.Information("Invoking Token Endpoint to get JWT...");
        
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discovery.TokenEndpoint,
            ClientId = "mf-invoices-api",
            ClientSecret = "top-secret"
        });

        if (tokenResponse.IsError)
        {
            
            Log.Error("Error Occured While Waiting for JWT {TokenResponseError}", tokenResponse.Error!);
            return BadRequest(tokenResponse.Error);
        }
        Log.Information("JWT received: {TokenResponseAccessToken}", tokenResponse.AccessToken);
        return Ok(tokenResponse.AccessToken);
    }
}