namespace antflowcore.constant.enums;

public class BpmnNodeFlagsEnum
{
    public static readonly BpmnNodeFlagsEnum NOTHING = new BpmnNodeFlagsEnum(0, "不存在");
    public static readonly BpmnNodeFlagsEnum HAS_ADDITIONAL_ASSIGNEE = new BpmnNodeFlagsEnum(0b1, "包含额外指定人员审批人");
    public static readonly BpmnNodeFlagsEnum HAS_ADDITIONAL_ASSIGNEE_ROLE = new BpmnNodeFlagsEnum(0b10, "包含额外指定角色审批人");
    public static readonly BpmnNodeFlagsEnum HAS_EXCLUDE_ASSIGNEE = new BpmnNodeFlagsEnum(0b100, "包含排除人员审批人");
    public static readonly BpmnNodeFlagsEnum HAS_EXCLUDE_ASSIGNEE_ROLE = new BpmnNodeFlagsEnum(0b1000, "包含排除角色审批人");
    
    private static readonly List<BpmnNodeFlagsEnum> _allFlags = new List<BpmnNodeFlagsEnum>
    {
        HAS_ADDITIONAL_ASSIGNEE,HAS_ADDITIONAL_ASSIGNEE_ROLE,HAS_EXCLUDE_ASSIGNEE,HAS_EXCLUDE_ASSIGNEE_ROLE
    };

    public int Code { get; }
    public string Desc { get; }

    private BpmnNodeFlagsEnum(int code, string desc)
    {
        Code = code;
        Desc = desc;
    }
    public static IReadOnlyList<BpmnNodeFlagsEnum> GetAllFlags() => _allFlags;
    
      /// <summary>
            /// 叠加 flag（位或运算）
            /// </summary>
            public static int BinaryOr(int alreadyFlags, BpmnNodeFlagsEnum newFlag)
            {
                return alreadyFlags | newFlag.Code;
            }
            public static int BinaryOr(int alreadyFlags, int newFlag)
            {
                return alreadyFlags | newFlag;
            }
            /// <summary>
            /// 从已有的标识中清除某个标识
            /// </summary>
            public static int BinaryAndNot(int alreadyFlags, BpmnNodeFlagsEnum flagToRemove)
            {
                return alreadyFlags & ~flagToRemove.Code;
            }
    
            /// <summary>
            /// 现有的标识拆分出的枚举值列表
            /// </summary>
            public static List<BpmnNodeFlagsEnum> FlagEnumsByCode(int? flags)
            {
                if (!flags.HasValue)
                {
                    flags = 0;
                }
                return _allFlags.Where(flag => (flags & flag.Code) != 0).ToList();
            }
    
            /// <summary>
            /// 判断是否包含某个 flag
            /// </summary>
            public static bool HasFlag(int? alreadyFlags, BpmnNodeFlagsEnum flag)
            {
                if (!alreadyFlags.HasValue)
                {
                    alreadyFlags = 0;
                }
                return (alreadyFlags & flag.Code) != 0;
            }
    
            public override string ToString() => $"{Desc} ({Code})";
}