namespace AntFlow.Core.Factory;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)] // ��Ӧ @Target
public sealed class AfFormServiceAnnoAttribute : Attribute
{
    // ���� svcName��Ĭ��ֵΪ���ַ���
    public string SvcName { get; set; } = "";

    // �����ṩ����������
    public string Desc { get; set; }
}