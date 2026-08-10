namespace Domain.DomainExceptions;

public class InvoiceStatusAlreadyAssignedException(string message) : DomainException(message);