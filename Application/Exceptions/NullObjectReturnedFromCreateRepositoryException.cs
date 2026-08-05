namespace Application.Exceptions;

public class NullObjectReturnedFromCreateRepositoryException(string? message) : Exception(message);