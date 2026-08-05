using Application.Features.CreateInvoice;
using Application.Features.GetInvoices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    ICreateInvoiceHandler createInvoiceHandler,
    IGetInvoicesHandler getInvoicesHandler,
    ILogger<InvoiceController> logger) 
    : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Command command, 
        CancellationToken cancellationToken)
    {
        var invoiceResponse = await createInvoiceHandler.CreateAsync(command, cancellationToken);
        
        return Created($"api/v1/invoices/{invoiceResponse.InvoiceId}", invoiceResponse);
    }

    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var invoices = await getInvoicesHandler.GetInvoicesByCustomerIdAsync(customerId, cancellationToken);
        return Ok(invoices);
    }
    
    [HttpGet("{invoiceId:guid}")]
    public async Task<IActionResult> GetByInvoiceId(Guid invoiceId, CancellationToken cancellationToken)
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