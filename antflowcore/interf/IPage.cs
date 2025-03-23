using antflowcore.entity;

namespace antflowcore.interf;

public interface IPage<T>
{
    List<OrderItem> Orders();
    List<T> GetRecords();
    IPage<T> SetRecords(List<T> records);
    long GetTotal();
    IPage<T> SetTotal(long total);
    long GetSize();
    IPage<T> SetSize(long size);
    long GetCurrent();
    IPage<T> SetCurrent(long current);

    // 默认接口方法
    bool OptimizeCountSql() => true;
    bool OptimizeJoinOfCountSql() => true;
    bool SearchCount() => true;

    long Offset()
    {
        long current = GetCurrent();
        return current <= 1L ? 0L : Math.Max((current - 1L) * GetSize(), 0L);
    }

    long? MaxLimit() => null;

    long GetPages()
    {
        if (GetSize() == 0L)
        {
            return 0L;
        }
        else
        {
            long pages = GetTotal() / GetSize();
            if (GetTotal() % GetSize() != 0L)
            {
                ++pages;
            }

            return pages;
        }
    }

    IPage<T> SetPages(long pages) => this; // 假设用于链式调用，实际可能需要具体实现
    string CountId() => null;

}