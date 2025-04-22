using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public interface IProcessOperationAdaptor : IAdaptorService
{
    void DoProcessButton(BusinessDataVo vo);
}