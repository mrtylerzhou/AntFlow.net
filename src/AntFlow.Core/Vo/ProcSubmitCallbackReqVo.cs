namespace AntFlow.Core.Vo;

public class ProcSubmitCallbackReqVo : CallbackReqVo
{
    public ProcSubmitCallbackReqVo() { }

    public ProcSubmitCallbackReqVo(string formData)
    {
        FormData = formData;
    }

    public string FormData { get; set; }
}