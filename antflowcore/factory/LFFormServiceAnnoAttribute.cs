namespace antflowcore.factory.tagparser;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class LFFormServiceAnnoAttribute: Attribute
{
    // 属性 svcName，默认值为空字符串
    public string SvcName { get; set; } = "";

    // 必须提供的描述属性
    public string Desc { get; set; }
}