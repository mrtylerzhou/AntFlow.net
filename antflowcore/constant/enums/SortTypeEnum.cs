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
 
    public static readonly SortTypeEnum ASC = new SortTypeEnum(1, " ASC", true);
    public static readonly SortTypeEnum DESC = new SortTypeEnum(2, " DESC", false);
    public static readonly SortTypeEnum FIELD = new SortTypeEnum(3, string.Empty, false);
 
    public static SortTypeEnum GetSortTypeEnumByCode(int code)
    {
        SortTypeEnum[] sortTypes = new SortTypeEnum[] { ASC, DESC, FIELD };
        return sortTypes.FirstOrDefault(st => st.Code == code);
    }
}