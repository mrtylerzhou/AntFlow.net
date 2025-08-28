namespace AntFlow.Core.Constant.Enums;

public enum ProcessSubmitStateEnum
{
    /// <summary>
    ///     �����ύ״̬
    /// </summary>
    PROCESS_SUB_TYPE = 1,

    /// <summary>
    ///     ����ͬ��״̬
    /// </summary>
    PROCESS_AGRESS_TYPE = 2,

    /// <summary>
    ///     ���̲�ͬ��״̬
    /// </summary>
    PROCESS_NO_AGRESS_TYPE = 3,

    /// <summary>
    ///     ���̳���״̬
    /// </summary>
    WITHDRAW_AGRESS_TYPE = 4,

    /// <summary>
    ///     ��������״̬
    /// </summary>
    END_AGRESS_TYPE = 5,

    /// <summary>
    ///     ������ֹ״̬
    /// </summary>
    CRMCEL_AGRESS_TYPE = 6,

    /// <summary>
    ///     ���̳���״̬��back��
    /// </summary>
    WITHDRAW_DISAGREE_TYPE = 7,

    /// <summary>
    ///     ����޸�
    /// </summary>
    PROCESS_UPDATE_TYPE = 8,

    /// <summary>
    ///     ����
    /// </summary>
    PROCESS_SIGN_UP = 9
}

public static class ProcessSubmitStateEnumExtensions
{
    /// <summary>
    ///     ��ȡ������Ϣ
    /// </summary>
    private static readonly (ProcessSubmitStateEnum Code, string Desc)[] Descriptions = new[]
    {
        (ProcessSubmitStateEnum.PROCESS_SUB_TYPE, "�����ύ״̬"), (ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE, "����ͬ��״̬"),
        (ProcessSubmitStateEnum.PROCESS_NO_AGRESS_TYPE, "���̲�ͬ��״̬"),
        (ProcessSubmitStateEnum.WITHDRAW_AGRESS_TYPE, "���̳���״̬"), (ProcessSubmitStateEnum.END_AGRESS_TYPE, "��������״̬"),
        (ProcessSubmitStateEnum.CRMCEL_AGRESS_TYPE, "������ֹ״̬"),
        (ProcessSubmitStateEnum.WITHDRAW_DISAGREE_TYPE, "back"),
        (ProcessSubmitStateEnum.PROCESS_UPDATE_TYPE, "����޸�"), (ProcessSubmitStateEnum.PROCESS_SIGN_UP, "����")
    };

    /// <summary>
    ///     ���ݴ����ȡ����
    /// </summary>
    public static string GetDesc(this ProcessSubmitStateEnum code)
    {
        return Descriptions.FirstOrDefault(d => d.Code == code).Desc ?? string.Empty;
    }

    /// <summary>
    ///     ���ݴ���ֵ��ȡ����
    /// </summary>
    public static string GetDescByCode(int code)
    {
        return Descriptions.FirstOrDefault(d => (int)d.Code == code).Desc ?? null;
    }
}