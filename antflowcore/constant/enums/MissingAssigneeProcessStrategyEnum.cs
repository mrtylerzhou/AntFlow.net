namespace antflowcore.constant.enums;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 缺失审批人处理策略
/// </summary>
public sealed class MissingAssigneeProcessStrategyEnum
{
    
    public static readonly MissingAssigneeProcessStrategyEnum NOTALLOWED = new MissingAssigneeProcessStrategyEnum(0, "不允许发起");

    public static readonly MissingAssigneeProcessStrategyEnum SKIP = new MissingAssigneeProcessStrategyEnum(1, "跳过"); // 注意: 这里的跳过指的是不生成审批任务节点,即流程图里没有当前缺失审批人的节点

    public static readonly MissingAssigneeProcessStrategyEnum TRANSFERTOADMIN = new MissingAssigneeProcessStrategyEnum(2, "转办给管理员"); // 转给管理员需实现 IBpmnProcessAdminProvider 接口
    
    private static readonly List<MissingAssigneeProcessStrategyEnum> AllValues = new List<MissingAssigneeProcessStrategyEnum>
    {
        NOTALLOWED,
        SKIP,
        TRANSFERTOADMIN
    };

   
    public int Code { get; }
    public string Desc { get; }

   
    private MissingAssigneeProcessStrategyEnum(int code, string desc)
    {
        Code = code;
        Desc = desc ?? throw new ArgumentNullException(nameof(desc));
    }

    /// <summary>
    /// 根据 code 获取对应的策略
    /// </summary>
    /// <param name="code">策略代码</param>
    /// <returns>匹配的策略，未找到则返回 null</returns>
    public static MissingAssigneeProcessStrategyEnum? GetByCode(int? code)
    {
        if (code == null) return null;
        return AllValues.FirstOrDefault(v => v.Code == code);
    }

   
    public override bool Equals(object? obj)
    {
        return obj is MissingAssigneeProcessStrategyEnum other && Code == other.Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

   
    public override string ToString()
    {
        return $"{nameof(Code)}: {Code}, {nameof(Desc)}: {Desc}";
    }
}