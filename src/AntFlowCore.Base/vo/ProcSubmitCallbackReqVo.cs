namespace AntFlowCore.Base.vo;

public class ProcSubmitCallbackReqVo: CallbackReqVo
{
    public ProcSubmitCallbackReqVo(){}

    public ProcSubmitCallbackReqVo(string formData)
    {
        this.FormData=formData;
    }

    public string FormData { get; set; }
}