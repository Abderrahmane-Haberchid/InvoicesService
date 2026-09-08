namespace Domain.DomainExceptions;

public class InvoiceObjectIsNullDomainException(string message) : DomainException(message);