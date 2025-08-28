using FreeSql.Internal.Model;

namespace AntFlow.Core.Entity;

public class Page<T>
{
    public int Current = 1;
    public List<OrderItem> Orders = new();
    public int Size = 10;
    public int Total;
    public Page() { }

    public Page(int current, int size) : this(current, size, 0) { }

    public Page(int current, int size, int total) : this(current, size, total, true) { }

    public Page(int current, int size, bool searchCount) : this(current, size, 0, searchCount) { }

    public Page(int current, int size, int total, bool searchCount)
    {
        if (current > 1L)
        {
            Current = current;
        }

        Size = size;
        Total = total;
        SearchCount = searchCount;
    }

    public List<T> Records { get; set; } = new();
    public string CountId { get; set; }
    public long? MaxLimit { get; set; }
    public bool SearchCount { get; set; }

    public bool HasPrevious()
    {
        return Current > 1L;
    }

    public bool HasNext()
    {
        return Current < GetPages();
    }

    private string[] MapOrderToArray(Func<OrderItem, bool> filter)
    {
        return Orders.Where(filter).Select(i => i.GetColumn()).ToArray();
    }

    private void RemoveOrder(Predicate<OrderItem> filter)
    {
        Orders.RemoveAll(filter);
    }

    public Page<T> AddOrder(params OrderItem[] items)
    {
        Orders.AddRange(items);
        return this;
    }

    public Page<T> AddOrder(List<OrderItem> items)
    {
        Orders.AddRange(items);
        return this;
    }


    public static Page<T> Of(int current, int size, int total, bool searchCount)
    {
        return new Page<T>(current, size, total, searchCount);
    }


    public Page<T> SetSearchCount(bool searchCount)
    {
        SearchCount = searchCount;
        return this;
    }


    public int GetPages()
    {
        if (Size == 0)
        {
            return 0;
        }

        int pages = Total / Size;
        if (Total % Size != 0L)
        {
            ++pages;
        }

        return pages;
    }


    public long Offset()
    {
        long current = Current;
        return current <= 1L ? 0L : Math.Max((current - 1L) * Size, 0L);
    }

    public BasePagingInfo ToPagingInfo()
    {
        BasePagingInfo info = new() { PageSize = Size, PageNumber = Current };
        return info;
    }

    public Page<T> Of(ICollection<T> records, BasePagingInfo pagingInfo)
    {
        Records.AddRange(records);
        Total = (int)pagingInfo.Count;
        return this;
    }
}