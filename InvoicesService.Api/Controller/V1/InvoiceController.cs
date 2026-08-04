using Application.Features.CreateInvoice.Dtos.requests;
using Application.Features.CreateInvoice.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    IInvoiceService invoiceService,
    ILogger<InvoiceController> logger) 
    : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] InvoiceRequest invoiceRequest, 
        CancellationToken cancellationToken)
    {
        var invoiceResponse = await invoiceService.CreateAsync(invoiceRequest, cancellationToken);
        
        return Created($"api/v1/invoices/{invoiceResponse.InvoiceId}", invoiceResponse);
    }
}