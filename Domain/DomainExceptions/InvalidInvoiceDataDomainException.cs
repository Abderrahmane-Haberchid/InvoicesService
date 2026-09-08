namespace Domain.DomainExceptions;

public class InvalidInvoiceDataDomainException(string message) : DomainException(message);