namespace AntFlow.Core.Entity;

public class OrderItem
{
    private bool asc = true;
    private string column;

    public OrderItem() { }

    public OrderItem(string column, bool asc)
    {
        this.column = column;
        this.asc = asc;
    }

    public static OrderItem Asc(string column)
    {
        return Build(column, true);
    }

    public static OrderItem Desc(string column)
    {
        return Build(column, false);
    }

    public static List<OrderItem> Ascs(params string[] columns)
    {
        return columns.Select(Asc).ToList();
    }

    public static List<OrderItem> Descs(params string[] columns)
    {
        return columns.Select(Desc).ToList();
    }

    private static OrderItem Build(string column, bool asc)
    {
        return new OrderItem(column, asc);
    }

    public string GetColumn()
    {
        return column;
    }

    public bool IsAsc()
    {
        return asc;
    }

    public void SetColumn(string column)
    {
        this.column = column;
    }

    public void SetAsc(bool asc)
    {
        this.asc = asc;
    }

    public override bool Equals(object obj)
    {
        if (obj == this)
        {
            return true;
        }

        if (!(obj is OrderItem other))
        {
            return false;
        }

        if (!other.CanEqual(this))
        {
            return false;
        }

        if (IsAsc() != other.IsAsc())
        {
            return false;
        }

        return string.Equals(column, other.column);
    }

    protected bool CanEqual(object other)
    {
        return other is OrderItem;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int result = 1;
            result = (result * 59) + (IsAsc() ? 79 : 97);
            result = (result * 59) + (column == null ? 43 : column.GetHashCode());
            return result;
        }
    }

    public override string ToString()
    {
        return $"OrderItem(column={column}, asc={asc})";
    }
}