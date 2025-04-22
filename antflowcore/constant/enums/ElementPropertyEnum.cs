using antflowcore.adaptor.bpmnelementadp;
using antflowcore.adaptor.variable;
using System.Reflection;

namespace antflowcore.constant.enums;

public class ElementPropertyEnum
{
    // 定义枚举项
    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SINGLE =
        new ElementPropertyEnum(1, "单人节点", typeof(BpmnAddFlowElementSingleAdaptor), typeof(BpmnInsertVariableSubsSingleAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN =
        new ElementPropertyEnum(2, "多人会签节点", typeof(BpmnAddFlowElementMultSignAdaptor), typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_ORSIGN =
        new ElementPropertyEnum(3, "多人或签节点", typeof(BpmnAddFlowElementMultOrSignAaptor), typeof(BpmnInsertVariableSubsMultiplayerOrSignAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_EXCLUSIVE_GATEWAY =
        new ElementPropertyEnum(4, "排他网关", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_PARALLEL_GATEWAY =
        new ElementPropertyEnum(5, "并行网关", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SEQUENCE_FLOW =
        new ElementPropertyEnum(6, "普通连线", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_PARAM_SEQUENCE_FLOW =
        new ElementPropertyEnum(7, "带参连线", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_LOOP =
        new ElementPropertyEnum(8, "循环节点", typeof(BpmnAddFlowElementLoopAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_SERIAL =
        new ElementPropertyEnum(9, "加批串行-顺序会签", typeof(BpmnAddFlowElementSignUpSerialAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_PARALLEL =
        new ElementPropertyEnum(10, "加批并行-无序会签", typeof(BpmnAddFlowElementMultSignAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_PARALLEL_OR =
        new ElementPropertyEnum(11, "加批并行或签-或签", typeof(BpmnAddFlowElementMultOrSignAaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN_IN_ORDER =
        new ElementPropertyEnum(21, "多人顺序会签节点", typeof(BpmnAddFlowElementSignUpSerialAdaptor), typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));

    // 属性定义
    public int Code { get; private set; }

    public string Desc { get; private set; }
    public Type AdaptorClass { get; private set; }
    public Type VariableSubClass { get; private set; }

    // 私有构造函数
    private ElementPropertyEnum(int code, string desc, Type adaptorClass, Type variableSubClass)
    {
        Code = code;
        Desc = desc;
        AdaptorClass = adaptorClass;
        VariableSubClass = variableSubClass;
    }

    // 动态收集所有枚举值
    private static readonly List<ElementPropertyEnum> _values = typeof(ElementPropertyEnum)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(ElementPropertyEnum))
        .Select(f => (ElementPropertyEnum)f.GetValue(null))
        .ToList();

    // 根据 Code 获取适配类
    public static Type GetAdaptorClassByCode(int code)
    {
        return _values.FirstOrDefault(item => item.Code == code)?.AdaptorClass;
    }

    // 根据 Code 获取记录子参数类
    public static Type GetVariableSubClassByCode(int code)
    {
        return _values.FirstOrDefault(item => item.Code == code)?.VariableSubClass;
    }

    // 获取所有枚举项
    public static IEnumerable<ElementPropertyEnum> Values()
    {
        return _values;
    }
}