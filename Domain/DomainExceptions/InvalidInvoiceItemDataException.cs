namespace Domain.DomainExceptions;

public class InvalidInvoiceItemDataException(string message) : DomainException(message);