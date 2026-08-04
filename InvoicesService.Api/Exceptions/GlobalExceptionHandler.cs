using System.Data;
using System.Net;
using FluentValidation;
using InvoicesService.Domain.DomainExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InvoicesService.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation error"),

            ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid argument"),

            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            
            DataException => (HttpStatusCode.InternalServerError, "An Error Occured While Persisting Invoice to Database"),
            
            InvalidInvoiceItemDataException => (HttpStatusCode.BadRequest, "Domain Exception: Invalid invoice item data"),
            
            InvalidInvoiceDataException => (HttpStatusCode.BadRequest, "Domain Exception: Invalid invoice data"),
            
            _ => (HttpStatusCode.InternalServerError, "Internal server error")
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = (int)statusCode,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}