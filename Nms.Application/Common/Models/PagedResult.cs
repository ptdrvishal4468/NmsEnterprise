using System.Text.Json.Serialization;

namespace Nms.Application.Common.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    [JsonConstructor]
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}