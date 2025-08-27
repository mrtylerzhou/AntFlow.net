namespace AntFlow.Core.Constant.Enums;

public enum OrderNodeTypeEnum
{
    TEST_ORDERED_SIGN = 1, // ʾ��˳��ڵ�
    OUT_SIDE_NODE = 2, // �ⲿϵͳ����ڵ�
    LOOP_NODE = 3 // ѭ���ڵ�
}

public static class OrderNodeTypeEnumExtensions
{
    private static readonly Dictionary<OrderNodeTypeEnum, string> Descriptions = new()
    {
        { OrderNodeTypeEnum.TEST_ORDERED_SIGN, "ʾ��˳��ڵ�" },
        { OrderNodeTypeEnum.OUT_SIDE_NODE, "�ⲿϵͳ����ڵ�" },
        { OrderNodeTypeEnum.LOOP_NODE, "ѭ���ڵ�" }
    };

    /// <summary>
    ///     ��ȡö�ٶ�Ӧ��������Ϣ��
    /// </summary>
    /// <param name="enumValue">OrderNodeTypeEnum ö��ֵ��</param>
    /// <returns>������Ϣ�ַ�����</returns>
    public static string GetDescription(this OrderNodeTypeEnum enumValue)
    {
        return Descriptions.TryGetValue(enumValue, out string? description) ? description : "δ֪�ڵ�";
    }

    /// <summary>
    ///     ���� Code ��ȡö��ʵ����
    /// </summary>
    /// <param name="code">�ڵ� Code��</param>
    /// <returns>ƥ��� OrderNodeTypeEnum �� null��</returns>
    public static OrderNodeTypeEnum? GetByCode(int? code)
    {
        if (!code.HasValue)
        {
            return null;
        }

        foreach (OrderNodeTypeEnum value in Enum.GetValues<OrderNodeTypeEnum>())
        {
            if ((int)value == code.Value)
            {
                return value;
            }
        }

        return null;
    }

    /// <summary>
    ///     ��ȡ����ö��ֵ��
    /// </summary>
    /// <returns>ö��ֵ�б�</returns>
    public static IEnumerable<OrderNodeTypeEnum> GetAllValues()
    {
        return Enum.GetValues<OrderNodeTypeEnum>();
    }
}