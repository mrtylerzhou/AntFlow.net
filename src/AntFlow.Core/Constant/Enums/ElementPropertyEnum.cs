using AntFlow.Core.Adaptor.BpmnElementAdp;
using AntFlow.Core.Adaptor.Variable;
using System.Reflection;

namespace AntFlow.Core.Constant.Enums;

public class ElementPropertyEnum
{
    // ?????????
    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SINGLE =
        new(1, "??????", typeof(BpmnAddFlowElementSingleAdaptor), typeof(BpmnInsertVariableSubsSingleAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN =
        new(2, "?????????", typeof(BpmnAddFlowElementMultSignAdaptor),
            typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_ORSIGN =
        new(3, "?????????", typeof(BpmnAddFlowElementMultOrSignAaptor),
            typeof(BpmnInsertVariableSubsMultiplayerOrSignAdaptor));

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_EXCLUSIVE_GATEWAY = new(4, "????????", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_PARALLEL_GATEWAY = new(5, "????????", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SEQUENCE_FLOW = new(6, "???????", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_PARAM_SEQUENCE_FLOW = new(7, "????????", null, null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_LOOP =
        new(8, "??????", typeof(BpmnAddFlowElementLoopAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_SERIAL =
        new(9, "????????-?????", typeof(BpmnAddFlowElementSignUpSerialAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_PARALLEL =
        new(10, "????????-??????", typeof(BpmnAddFlowElementMultSignAdaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_SIGN_UP_PARALLEL_OR =
        new(11, "???????§Ý??-???", typeof(BpmnAddFlowElementMultOrSignAaptor), null);

    public static readonly ElementPropertyEnum ELEMENT_PROPERTY_MULTIPLAYER_SIGN_IN_ORDER =
        new(21, "????????????", typeof(BpmnAddFlowElementSignUpSerialAdaptor),
            typeof(BpmnInsertVariableSubsMultiplayerSignAdaptor));

    // ??????????????
    private static readonly List<ElementPropertyEnum> _values = typeof(ElementPropertyEnum)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(ElementPropertyEnum))
        .Select(f => (ElementPropertyEnum)f.GetValue(null))
        .ToList();

    // ??§Û?????
    private ElementPropertyEnum(int code, string desc, Type adaptorClass, Type variableSubClass)
    {
        Code = code;
        Desc = desc;
        AdaptorClass = adaptorClass;
        VariableSubClass = variableSubClass;
    }

    // ???????
    public int Code { get; }
    public string Desc { get; private set; }
    public Type AdaptorClass { get; }
    public Type VariableSubClass { get; }

    // ???? Code ?????????
    public static Type GetAdaptorClassByCode(int code)
    {
        return _values.FirstOrDefault(item => item.Code == code)?.AdaptorClass;
    }

    // ???? Code ?????????????
    public static Type GetVariableSubClassByCode(int code)
    {
        return _values.FirstOrDefault(item => item.Code == code)?.VariableSubClass;
    }

    // ????????????
    public static IEnumerable<ElementPropertyEnum> Values()
    {
        return _values;
    }
}