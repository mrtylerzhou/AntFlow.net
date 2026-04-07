using AntFlowCore.Vo;

namespace AntFlowCore.Core.vo;

public class ProcSubmitCallbackReqVo: CallbackReqVo
{
    public ProcSubmitCallbackReqVo(){}

    public ProcSubmitCallbackReqVo(string formData)
    {
        this.FormData=formData;
    }

    public string FormData { get; set; }
}