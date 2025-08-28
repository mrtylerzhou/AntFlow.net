namespace AntFlow.Core.Dto;

public class ResultAndPage<T>
{
    // Constructor with data and pagination
    public ResultAndPage(List<T> data, PageDto pagination)
    {
        Data = data;
        Pagination = pagination;
        Code = 200;
    }

    // Constructor with data, pagination, and statistics
    public ResultAndPage(List<T> data, PageDto pagination, Dictionary<string, object> statistics)
    {
        Data = data;
        Pagination = pagination;
        Statistics = statistics;
        Code = 200;
    }

    // Constructor with data, pagination, statistics, and sort column map
    public ResultAndPage(List<T> data, PageDto pagination, Dictionary<string, object> statistics,
        Dictionary<string, string> sortColumnMap)
    {
        Data = data;
        Pagination = pagination;
        Statistics = statistics;
        SortColumnMap = sortColumnMap;
        Code = 200;
    }

    // Constructor with all parameters
    public ResultAndPage(List<T> data, PageDto pagination, Dictionary<string, object> statistics,
        Dictionary<string, string> sortColumnMap, int? flag)
    {
        Data = data;
        Pagination = pagination;
        Statistics = statistics;
        SortColumnMap = sortColumnMap;
        Flag = flag;
        Code = 200;
    }

    /// <summary>
    ///     Back data
    /// </summary>
    public List<T> Data { get; set; }

    /// <summary>
    ///     Pagination
    /// </summary>
    public PageDto Pagination { get; set; }

    /// <summary>
    ///     Statistics
    /// </summary>
    public Dictionary<string, object> Statistics { get; set; }

    /// <summary>
    ///     Sorting fields
    /// </summary>
    public Dictionary<string, string> SortColumnMap { get; set; }

    /// <summary>
    ///     Page flag
    /// </summary>
    public int? Flag { get; set; }

    /// <summary>
    ///     Request ID
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    ///     Response code
    /// </summary>
    public int Code { get; set; } = 200;
}