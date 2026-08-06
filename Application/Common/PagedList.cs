namespace Application.Common;

public record PagedList<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    bool HasPrevious => PageNumber > 1;
    bool HasNext => PageNumber < TotalCount%PageSize;
}