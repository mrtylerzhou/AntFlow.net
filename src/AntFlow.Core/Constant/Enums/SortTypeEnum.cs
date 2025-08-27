namespace AntFlow.Core.Constant.Enums;

public class SortTypeEnum
{
    public static readonly SortTypeEnum ASC = new(1, " ASC", true);
    public static readonly SortTypeEnum DESC = new(2, " DESC", false);
    public static readonly SortTypeEnum FIELD = new(3, string.Empty, false);

    private SortTypeEnum(int code, string mark, bool isAsc)
    {
        Code = code;
        Mark = mark;
        IsAsc = isAsc;
    }

    public int Code { get; }
    public string Mark { get; }
    public bool IsAsc { get; }

    public static SortTypeEnum GetSortTypeEnumByCode(int code)
    {
        SortTypeEnum[] sortTypes = new[] { ASC, DESC, FIELD };
        return sortTypes.FirstOrDefault(st => st.Code == code);
    }
}