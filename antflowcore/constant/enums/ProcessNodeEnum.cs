using antflowcore.exception;

namespace antflowcore.constant.enus;

/// <summary>
/// Process node static class
/// </summary>
public static class ProcessNodeEnum
{
    public static readonly ProcessNode START_TASK_KEY = new ProcessNode(1, "task1418018332271");
    public static readonly ProcessNode TWO_TASK_KEY = new ProcessNode(2, "task1418018332272");

    private const int MaxSupportedNodeCode = 100;
    private const long TaskDescriptionBaseValue = 1418018332270;

    private static readonly List<ProcessNode> _nodes = CreateNodes();

    private static List<ProcessNode> CreateNodes()
    {
        var nodes = new List<ProcessNode>
            {
                START_TASK_KEY, TWO_TASK_KEY
            };
        //3-100  the description is generated based on the code
        for (var code = TWO_TASK_KEY.Code + 1; code <= MaxSupportedNodeCode; code++)
        {
            nodes.Add(new ProcessNode(code, $"task{TaskDescriptionBaseValue + code}"));
        }

        return nodes;
    }

    public static string GetDescByCode(int code)
    {
        var node = _nodes.Find(n => n.Code == code);
        return node?.Description;
    }

    public static int? GetCodeByDesc(string description)
    {
        var node = _nodes.Find(n => n.Description == description);
        return node?.Code;
    }

    public static int Compare(String taskDefKey1, String taskDefKey2)
    {
        if (string.IsNullOrEmpty(taskDefKey1) || string.IsNullOrEmpty(taskDefKey2))
        {
            throw new AFBizException(BusinessError.PARAMS_IS_NULL, "请传入taskDefKey");
        }

        long longTaskDefKey1 = long.Parse(taskDefKey1.Replace("task", ""));
        long longTaskDefkey2 = long.Parse(taskDefKey2.Replace("task", ""));
        return longTaskDefKey1.CompareTo(longTaskDefkey2);
    }
}

public class ProcessNode
{
    public int Code { get; }
    public string Description { get; }

    public ProcessNode(int code, string description)
    {
        Code = code;
        Description = description;
    }
}