namespace antflowcore.constant.enums;

public class JumpUrlEnum
{
    // 定义枚举值
    public static readonly JumpUrlEnum PROCESS_APPROVE = new JumpUrlEnum(1, "流程审批页");

    public static readonly JumpUrlEnum PROCESS_VIEW = new JumpUrlEnum(2, "流程查看页");
    public static readonly JumpUrlEnum PROCESS_BACKLOG = new JumpUrlEnum(3, "流程待办页");

    // 属性
    public int Code { get; }

    public string Desc { get; }

    // 构造函数
    private JumpUrlEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }

    // 获取所有枚举值
    public static IEnumerable<JumpUrlEnum> Values =>
        new List<JumpUrlEnum>
        {
            PROCESS_APPROVE,
            PROCESS_VIEW,
            PROCESS_BACKLOG
        };

    // 根据code获取desc
    public static string GetDescByCode(int code)
    {
        return Values.FirstOrDefault(v => v.Code == code)?.Desc;
    }

    public override string ToString()
    {
        return Desc;
    }
}