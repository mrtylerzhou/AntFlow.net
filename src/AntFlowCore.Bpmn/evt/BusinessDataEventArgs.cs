

using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.evt;

public class BusinessDataEventArgs : EventArgs
{
    public BusinessDataVo BusinessData { get; }

    public BusinessDataEventArgs(BusinessDataVo businessData)
    {
        BusinessData = businessData;
    }
}