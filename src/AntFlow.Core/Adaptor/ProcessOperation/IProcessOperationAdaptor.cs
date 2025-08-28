using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

public interface IProcessOperationAdaptor : IAdaptorService
{
    void DoProcessButton(BusinessDataVo vo);
}