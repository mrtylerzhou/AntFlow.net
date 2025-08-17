using AntFlowCore.Vo;

namespace antflowcore.evt;

public class BusinessDataEventArgs : EventArgs
{
    public BusinessDataVo BusinessData { get; }

    public BusinessDataEventArgs(BusinessDataVo businessData)
    {
        BusinessData = businessData;
    }
}