using Application.Features.CreateInvoice;
using Application.Features.GetInvoiceByCutomerId;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    ISender sender,
    ILogger<InvoiceController> logger) 
    : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Command command, 
        CancellationToken cancellationToken)
    {
        var invoiceResponse = await sender.Send(command, cancellationToken);
        
        return Created($"api/v1/invoices/{invoiceResponse.InvoiceId}", invoiceResponse);
    }

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(Query request, CancellationToken cancellationToken)
    {
        var invoices = await mediator.Send(request, cancellationToken);
        return Ok(invoices);
    }
    
    [HttpGet("{invoiceId:guid}")]
    public async Task<IActionResult> GetByInvoiceId(Query request, CancellationToken cancellationToken)
    {
        var invoice = await getInvoicesHandler.GetInvoiceByInvoiceIdAsync(invoiceId, cancellationToken);
        return Ok(invoice);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int? page, 
        int? pageSize, 
        CancellationToken cancellationToken)
    {
        var invoices = await getInvoicesHandler.GetAllInvoicesAsync(page, pageSize, cancellationToken);
        return Ok(invoices);
    }
}