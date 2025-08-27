using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

public abstract class BpmnNodeAdaptor : IAdaptorService
{
    public abstract void SetSupportBusinessObjects();

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
}