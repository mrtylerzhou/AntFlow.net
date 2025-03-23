namespace antflowcore.constant.enus;

public class SortTypeEnum
{
    public int Code { get; }
    public string Mark { get; }
    public bool IsAsc { get; }
 
    private SortTypeEnum(int code, string mark, bool isAsc)
    {
        Code = code;
        Mark = mark;
        IsAsc = isAsc;
    }
 
    public static readonly SortTypeEnum Asc = new SortTypeEnum(1, " ASC", true);
    public static readonly SortTypeEnum Desc = new SortTypeEnum(2, " DESC", false);
    public static readonly SortTypeEnum Field = new SortTypeEnum(3, string.Empty, false);
 
    public static SortTypeEnum GetSortTypeEnumByCode(int code)
    {
        var sortTypes = new[] { Asc, Desc, Field };
        return sortTypes.FirstOrDefault(st => st.Code == code);
    }
}