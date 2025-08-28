namespace AntFlow.Core.Constant.Enums;

public class ProcessButtonEnum
{
    // ������������ʵ��
    public static readonly ProcessButtonEnum DISAGREE_TYPE = new(1, "��ͬ��");
    public static readonly ProcessButtonEnum AGREE_TYPE = new(2, "ͬ��");
    public static readonly ProcessButtonEnum WITHDRAW_TYPE = new(3, "����");
    public static readonly ProcessButtonEnum END_TYPE = new(4, "��ֹ");
    public static readonly ProcessButtonEnum DELETE_TYPE = new(5, "����");
    public static readonly ProcessButtonEnum CHANGE_TYPE = new(6, "���������");
    public static readonly ProcessButtonEnum HANDLE_TYPE = new(7, "������");
    public static readonly ProcessButtonEnum PRINTRING_TYPE = new(8, "��ӡ");
    public static readonly ProcessButtonEnum CEO_TYPE = new(9, "�ύCEO����");
    public static readonly ProcessButtonEnum FORWARD_TYPE = new(10, "ת��");
    public static readonly ProcessButtonEnum ACTING_TYPE = new(11, "��������ͬ��");
    public static readonly ProcessButtonEnum UNDERTAKE_TYPE = new(12, "�а�");
    public static readonly ProcessButtonEnum JOINTLY_SIGN = new(14, "��ǩ");
    public static readonly ProcessButtonEnum GET_BACK = new(15, "����");
    public static readonly ProcessButtonEnum ADD_BATCH = new(16, "����������");
    public static readonly ProcessButtonEnum STAFF_CONFIRM_TYPE = new(17, "��Ա��ȷ��");
    public static readonly ProcessButtonEnum VIEW_TYPE = new(1, "�鿴����");
    public static readonly ProcessButtonEnum DEAL_WITH_TYPE = new(2, "��������");
    public static readonly ProcessButtonEnum MAIN_COLOR = new(1, "primary");
    public static readonly ProcessButtonEnum DEFAULT_COLOR = new(2, "default");

    // �������ͼ��ϣ����ڲ�ѯ��
    private static readonly List<ProcessButtonEnum> AllTypes = new()
    {
        DISAGREE_TYPE,
        AGREE_TYPE,
        WITHDRAW_TYPE,
        END_TYPE,
        DELETE_TYPE,
        CHANGE_TYPE,
        HANDLE_TYPE,
        PRINTRING_TYPE,
        CEO_TYPE,
        FORWARD_TYPE,
        ACTING_TYPE,
        UNDERTAKE_TYPE,
        JOINTLY_SIGN,
        GET_BACK,
        ADD_BATCH,
        STAFF_CONFIRM_TYPE,
        VIEW_TYPE,
        DEAL_WITH_TYPE,
        MAIN_COLOR,
        DEFAULT_COLOR
    };

    // ˽���ֶ�

    // ���캯��
    private ProcessButtonEnum(int code, string desc)
    {
        this.Code = code;
        this.Desc = desc;
    }

    // ���Է�����
    public int Code { get; }

    public string Desc { get; }

    // ͨ��Code��ȡDesc
    public static string GetDescByCode(int code)
    {
        return AllTypes.FirstOrDefault(t => t.Code == code)?.Desc;
    }

    // ͨ��Desc��ȡCode
    public static int? GetCodeByDesc(string desc)
    {
        return AllTypes.FirstOrDefault(t => t.Desc == desc)?.Code;
    }
}