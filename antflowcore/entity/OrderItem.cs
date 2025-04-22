namespace antflowcore.entity;

using System.Collections.Generic;
using System.Linq;

public class OrderItem
{
    private string column;
    private bool asc = true;

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
        return columns.Select(OrderItem.Asc).ToList();
    }

    public static List<OrderItem> Descs(params string[] columns)
    {
        return columns.Select(OrderItem.Desc).ToList();
    }

    private static OrderItem Build(string column, bool asc)
    {
        return new OrderItem(column, asc);
    }

    public string GetColumn() => this.column;

    public bool IsAsc() => this.asc;

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
        if (this.IsAsc() != other.IsAsc())
        {
            return false;
        }
        return string.Equals(this.column, other.column);
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
            result = result * 59 + (this.IsAsc() ? 79 : 97);
            result = result * 59 + (this.column == null ? 43 : this.column.GetHashCode());
            return result;
        }
    }

    public override string ToString()
    {
        return $"OrderItem(column={this.column}, asc={this.asc})";
    }

    public OrderItem()
    { }

    public OrderItem(string column, bool asc)
    {
        this.column = column;
        this.asc = asc;
    }
}