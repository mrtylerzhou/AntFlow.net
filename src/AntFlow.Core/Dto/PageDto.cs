namespace AntFlow.Core.Dto;

public class PageDto
{
    // Default constructor
    public PageDto()
    {
    }

    // Parameterized constructor
    public PageDto(int page, int pageSize, string orderColumn, int? orderType)
    {
        Page = page;
        PageSize = pageSize;
        OrderColumn = orderColumn;
        OrderType = orderType;
    }

    /// <summary>
    ///     Current page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    ///     How many records in a page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     Total records count
    /// </summary>
    public int? TotalCount { get; set; }

    /// <summary>
    ///     Total page count
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    ///     Start index (calculated property, not stored directly)
    /// </summary>
    public int StartIndex => (Page - 1) * PageSize;

    /// <summary>
    ///     Columns for sorting
    /// </summary>
    public string OrderColumn { get; set; }

    /// <summary>
    ///     Sort type: 1 ascending, 2 descending
    /// </summary>
    public int? OrderType { get; set; }

    /// <summary>
    ///     Returns a PageDto instance with default first page settings
    /// </summary>
    public static PageDto First()
    {
        return new PageDto { Page = 1, PageSize = 10 };
    }

    /// <summary>
    ///     Builds a PageDto instance with total count and calculates page count
    /// </summary>
    public static PageDto BuildCountedPage(PageDto pageDto, int totalCount)
    {
        return new PageDto
        {
            Page = pageDto.Page,
            PageSize = pageDto.PageSize,
            TotalCount = totalCount,
            PageCount = ((totalCount - 1) / pageDto.PageSize) + 1,
            OrderColumn = pageDto.OrderColumn,
            OrderType = pageDto.OrderType
        };
    }
}