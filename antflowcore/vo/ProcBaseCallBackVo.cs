using AntFlowCore.Vo;

namespace antflowcore.vo;

public class ProcBaseCallBackVo:CallbackReqVo
{
    public ProcBaseCallBackVo() {

    }
    public ProcBaseCallBackVo(String formData) {

        this.FormData = formData;
    }

    /**
     * 表单数据Json字符串
     */
    public string FormData { get; set; }
}