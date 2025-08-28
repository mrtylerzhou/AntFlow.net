namespace AntFlow.Core.Vo;

public class ProcBaseCallBackVo : CallbackReqVo
{
    public ProcBaseCallBackVo()
    {
    }

    public ProcBaseCallBackVo(string formData)
    {
        FormData = formData;
    }

    /**
     * 表单数据Json字符串
     */
    public string FormData { get; set; }
}