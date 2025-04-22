using antflowcore.vo;

namespace antflowcore.adaptor.bpmnnodeadp;

public abstract class BpmnNodeAdaptor : IAdaptorService
{
    /**
     * format BpmnNodeVo
     *
     * @param bpmnNodeVo
     * @return
     */

    public abstract BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo);

    /**
     * edit bpmn node info
     *
     * @param bpmnNodeVo
     */

    public abstract void EditBpmnNode(BpmnNodeVo bpmnNodeVo);

    public abstract void SetSupportBusinessObjects();
}