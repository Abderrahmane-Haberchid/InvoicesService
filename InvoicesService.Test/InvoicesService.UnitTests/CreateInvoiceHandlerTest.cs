using Application.Abstractions;
using Application.Features.CreateInvoice;
using Domain.Respository;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace InvoicesServiceTest.InvoicesService.UnitTests;

public class CreateInvoiceHandlerTest
{
    private Mock<IInvoiceRepository> invoiceRepositoryMock;
    private Mock<IEventPublisher> eventPublisherMock;
    private Mock<IValidator<CreateInvoiceCommand>> validatorMock;
    private ILogger<CreateInvoiceHandlerTest> logger;
    
    private CreateInvoiceHandler sut;
    
    public CreateInvoiceHandlerTest()
    {
        invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        eventPublisherMock = new Mock<IEventPublisher>();
        validatorMock = new Mock<IValidator<CreateInvoiceCommand>>();
        
        sut = new CreateInvoiceHandler(
            invoiceRepositoryMock.Object,
            validatorMock.Object,
            eventPublisherMock.Object,
            NullLogger<CreateInvoiceHandler>.Instance);
    }
}