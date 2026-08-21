using Application.Features.CreateInvoice;
using Application.Features.GetInvoiceByCustomerId;
using Application.Features.GetInvoiceById;
using Application.Features.GetInvoices;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Authorize]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    ISender sender) 
    : ControllerBase
{
    
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceCommand createInvoiceCommand, 
        CancellationToken cancellationToken)
    {
        var invoiceResponse = await sender.Send(createInvoiceCommand, cancellationToken);
        
        return Created($"api/v1/invoices/{invoiceResponse.InvoiceId}", invoiceResponse);
    }

    [HttpGet("{customerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        
        var request = new GetInvoiceByCustomerIdQuery(customerId);
        var invoices = await sender.Send(request, cancellationToken);
        return Ok(invoices);
    }
    
    [HttpGet("{invoiceId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByInvoiceId(Guid invoiceId, CancellationToken cancellationToken)
    {
        var request = new GetInvoiceByIdQuery(invoiceId);
        var invoice = await sender.Send(request, cancellationToken);
        return Ok(invoice);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int? page, int? pageSize, CancellationToken cancellationToken)
    {
        var request = new GetInvoicesQuery(page ?? 1, pageSize ?? 50);
        var invoices = await sender.Send(request, cancellationToken);
        return Ok(invoices);
    }
}