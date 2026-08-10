namespace Domain.DomainExceptions;

public class InvalidInvoiceDataException(string message) : DomainException(message);