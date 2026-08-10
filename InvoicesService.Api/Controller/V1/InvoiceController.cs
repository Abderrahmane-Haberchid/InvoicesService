using Application.Abstractions;
using Application.Features.CreateInvoice;
using Application.Features.GetInvoiceByCustomerId;
using Application.Features.GetInvoiceById;
using Application.Features.GetInvoices;
using Asp.Versioning;
using InvoicesService.Shared.Contracts.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Controller.V1;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{apiVersion:apiVersion}/invoices")]
public class InvoiceController(
    ISender sender,
    IEventPublisher eventPublisher,
    ILogger<InvoiceController> logger) 
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

    [HttpPost("pay-invoice/{invoiceId:guid}")]
    public async Task<IActionResult> PayInvoice(Guid invoiceId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Invoice with Id {invoiceId} Event is under processing");
        
        var testEvent = new TestEvent(123);
        
        await eventPublisher.PublishAsync<TestEvent>(testEvent, cancellationToken);
        
        logger.LogInformation($"Event has been sent to payment service...{testEvent}");

        return Ok();
    }
}