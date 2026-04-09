using AntFlowCore.Base.entity;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmProcessNodeSubmitBizService
{
    void ProcessComplete(BpmAfTask task);
}