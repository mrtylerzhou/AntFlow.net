namespace antflowcore.constant.enums;

/// <summary>
/// Process node static class
/// </summary>
public static class ProcessNodeEnum
{
    public static readonly ProcessNode START_TASK_KEY = new ProcessNode(1, "task1418018332271");
    public static readonly ProcessNode TOW_TASK_KEY = new ProcessNode(2, "task1418018332272");
    public static readonly ProcessNode THREE_TASK_KEY = new ProcessNode(3, "task1418018332273");
    public static readonly ProcessNode FOUR_TASK_KEY = new ProcessNode(4, "task1418018332274");
    public static readonly ProcessNode FIVE_TASK_KEY = new ProcessNode(5, "task1418018332275");
    public static readonly ProcessNode SIX_TASK_KEY = new ProcessNode(6, "task1418018332276");
    public static readonly ProcessNode SEVEN_TASK_KEY = new ProcessNode(7, "task1418018332277");
    public static readonly ProcessNode EIGHT_TASK_KEY = new ProcessNode(8, "task1418018332278");
    public static readonly ProcessNode NINE_TASK_KEY = new ProcessNode(9, "task1418018332279");
    public static readonly ProcessNode TEN_TASK_KEY = new ProcessNode(10, "task1418018332280");
    public static readonly ProcessNode THIRTEEN_TASK_KEY = new ProcessNode(11, "task1418018332281");
    public static readonly ProcessNode FOURTTEEN_TASK_KEY = new ProcessNode(12, "task1418018332282");
    public static readonly ProcessNode FIFTTEEN_TASK_KEY = new ProcessNode(13, "task1418018332283");
    public static readonly ProcessNode FOURTEEN_TASK_KEY = new ProcessNode(14, "task1418018332284");
    public static readonly ProcessNode SEX_TASK_KEY = new ProcessNode(15, "task1418018332285");
    public static readonly ProcessNode SEXTEEN_TASK_KEY = new ProcessNode(16, "task1418018332286");
    public static readonly ProcessNode SEVENTEEN_TASK_KEY = new ProcessNode(17, "task1418018332287");
    public static readonly ProcessNode EIGHTEEN_TASK_KEY = new ProcessNode(18, "task1418018332288");
    public static readonly ProcessNode NIGHTEEN_TASK_KEY = new ProcessNode(19, "task1418018332289");
    public static readonly ProcessNode TWENTY_TASK_KEY = new ProcessNode(20, "task1418018332290");
    public static readonly ProcessNode TWENTYONE_TASK_KEY = new ProcessNode(21, "task1418018332291");
    public static readonly ProcessNode TWENTYTWO_TASK_KEY = new ProcessNode(22, "task1418018332292");
    public static readonly ProcessNode TWENTYTHREE_TASK_KEY = new ProcessNode(23, "task1418018332293");
    public static readonly ProcessNode TWENTYFOUR_TASK_KEY = new ProcessNode(24, "task1418018332294");
    public static readonly ProcessNode TWENTYFIVE_TASK_KEY = new ProcessNode(25, "task1418018332295");
    public static readonly ProcessNode TWENTYSIX_TASK_KEY = new ProcessNode(26, "task1418018332296");
    public static readonly ProcessNode TWENTYSEVEN_TASK_KEY = new ProcessNode(27, "task1418018332297");
    public static readonly ProcessNode TWENTYEIGHT_TASK_KEY = new ProcessNode(28, "task1418018332298");
    public static readonly ProcessNode TWENTYNINE_TASK_KEY = new ProcessNode(29, "task1418018332299");
    public static readonly ProcessNode THIRTY_TASK_KEY = new ProcessNode(30, "task1418018332300");
    public static readonly ProcessNode THIRTYONE_TASK_KEY = new ProcessNode(31, "task1418018332301");
    public static readonly ProcessNode THIRTYTWO_TASK_KEY = new ProcessNode(32, "task1418018332302");
    public static readonly ProcessNode THIRTYTHREE_TASK_KEY = new ProcessNode(33, "task1418018332303");
    public static readonly ProcessNode THIRTYFOUR_TASK_KEY = new ProcessNode(34, "task1418018332304");
    public static readonly ProcessNode THIRTYFIVE_TASK_KEY = new ProcessNode(35, "task1418018332305");

    private static readonly List<ProcessNode> _nodes = new List<ProcessNode>
        {
            START_TASK_KEY, TOW_TASK_KEY, THREE_TASK_KEY, FOUR_TASK_KEY, FIVE_TASK_KEY,
            SIX_TASK_KEY, SEVEN_TASK_KEY, EIGHT_TASK_KEY, NINE_TASK_KEY, TEN_TASK_KEY,
            THIRTEEN_TASK_KEY, FOURTTEEN_TASK_KEY, FIFTTEEN_TASK_KEY, FOURTEEN_TASK_KEY,
            SEX_TASK_KEY, SEXTEEN_TASK_KEY, SEVENTEEN_TASK_KEY, EIGHTEEN_TASK_KEY, NIGHTEEN_TASK_KEY,
            TWENTY_TASK_KEY, TWENTYONE_TASK_KEY, TWENTYTWO_TASK_KEY, TWENTYTHREE_TASK_KEY,
            TWENTYFOUR_TASK_KEY, TWENTYFIVE_TASK_KEY, TWENTYSIX_TASK_KEY, TWENTYSEVEN_TASK_KEY,
            TWENTYEIGHT_TASK_KEY, TWENTYNINE_TASK_KEY, THIRTY_TASK_KEY, THIRTYONE_TASK_KEY,
            THIRTYTWO_TASK_KEY, THIRTYTHREE_TASK_KEY, THIRTYFOUR_TASK_KEY, THIRTYFIVE_TASK_KEY
        };

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