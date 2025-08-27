namespace AntFlow.Core.Constant.Enums;

public enum CallbackTypeEnum
{
    CONF_CONDITION_CALL_BACK,
    PROC_CONDITION_CALL_BACK,
    PROC_SUBMIT_CALL_BACK,
    PROC_STARTED_CALL_BACK,
    PROC_COMMIT_CALL_BACK,
    PROC_END_CALL_BACK,
    PROC_FINISH_CALL_BACK
}

public static class CallbackTypeEnumExtensions
{
    public static string GetMark(this CallbackTypeEnum type)
    {
        return type.ToString(); // mark ����ö����
    }

    public static string GetDesc(this CallbackTypeEnum type)
    {
        return type switch
        {
            CallbackTypeEnum.CONF_CONDITION_CALL_BACK => "������֧�ص�",
            CallbackTypeEnum.PROC_CONDITION_CALL_BACK => "�����жϻص�",
            CallbackTypeEnum.PROC_SUBMIT_CALL_BACK => "�ύ�����ص�",
            CallbackTypeEnum.PROC_STARTED_CALL_BACK => "���̷�����ɻص�",
            CallbackTypeEnum.PROC_COMMIT_CALL_BACK => "��ת�����ص�",
            CallbackTypeEnum.PROC_END_CALL_BACK => "���������ص�",
            CallbackTypeEnum.PROC_FINISH_CALL_BACK => "��ɲ����ص�",
            _ => string.Empty
        };
    }
}