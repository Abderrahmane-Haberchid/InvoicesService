using Application.Features.CreateInvoice.Dtos.requests;
using Application.Features.CreateInvoice.Services;
using Application.Features.GetInvoices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    IInvoiceService invoiceService,
    IGetInvoicesService getInvoicesService,
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

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var invoices = await getInvoicesService.GetInvoicesByCustomerIdAsync(customerId, cancellationToken);
        return Ok(invoices);
    }
    
    [HttpGet("{invoiceId:guid}")]
    public async Task<IActionResult> GetByInvoiceId(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await getInvoicesService.GetInvoiceByInvoiceIdAsync(invoiceId, cancellationToken);
        return Ok(invoice);
    }

    [HttpGet("size={take:int}&skip={skip:int}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int take, 
        [FromQuery] int skip, 
        CancellationToken cancellationToken)
    {
        var invoices = await getInvoicesService.GetAllInvoicesAsync(take, skip, cancellationToken);
        return Ok(invoices);
    }
}