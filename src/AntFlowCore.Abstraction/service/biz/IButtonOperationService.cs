using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IButtonOperationService
{
    BusinessDataVo ButtonsOperationTransactional(BusinessDataVo vo);
}
