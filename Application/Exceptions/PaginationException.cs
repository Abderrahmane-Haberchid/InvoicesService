namespace Application.Exceptions;

public class PaginationException : Exception
{
    public PaginationException(string message) : base(message)
    {
        
    }
}