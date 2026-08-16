using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// 兜底 element 适配器: 找不到具体节点类型适配器时由 BpmnNodeFormatService 使用.
/// 对等 Java BpmnGeneralPurposeElementAdaptor. varName="general" 时,
/// AbstractCommonBpmnElementAdaptor 会用流程名拼音首字母大写拼接 collection 名.
/// 注意: 不注册任何业务对象(nodeProperty), 仅作为兜底被显式调用.
/// </summary>
public class BpmnGeneralPurposeElementAdaptor : AbstractCommonBpmnElementAdaptor
{
    protected override string ProvideVarName()
    {
        return "general";
    }

    public override void SetSupportBusinessObjects()
    {
        // 兜底适配器不注册任何节点类型
    }
}