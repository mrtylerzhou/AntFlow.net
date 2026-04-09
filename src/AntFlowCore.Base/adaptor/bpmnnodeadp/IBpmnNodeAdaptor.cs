using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.adaptor.bpmnnodeadp;

public interface  IBpmnNodeAdaptor: IAdaptorService
{
    /**
     * format BpmnNodeVo
     *
     * @param bpmnNodeVo
     * @return
     */
    public void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo);
    /**
     * edit bpmn node info
     *
     * @param bpmnNodeVo
     */
    public void EditBpmnNode(BpmnNodeVo bpmnNodeVo);

    public void SetSupportBusinessObjects();

}