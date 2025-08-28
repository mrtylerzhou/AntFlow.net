using AntFlow.Core.Vo;

namespace AntFlow.Core.Event;

public class BusinessDataEventArgs : EventArgs
{
    public BusinessDataEventArgs(BusinessDataVo businessData)
    {
        BusinessData = businessData;
    }

    public BusinessDataVo BusinessData { get; }
}