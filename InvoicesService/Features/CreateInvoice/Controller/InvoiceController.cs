using InvoicesService.Features.CreateInvoice.Dtos;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Features.CreateInvoice.Controller;

[ApiController]
[Route("api/v1/invoices")]
public class InvoiceController(
    IInvoiceService invoiceService,
    ILogger<InvoiceController> logger) 
    : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceDto invoiceDto, 
        CancellationToken cancellationToken)
    {
        var invoiceResponse = await invoiceService.CreateAsync(invoiceDto, cancellationToken);
        return Created($"api/v1/invoices/{invoiceResponse.InvoiceId}", invoiceResponse);
    }
}