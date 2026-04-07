using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IButtonOperationService
{
    BusinessDataVo ButtonsOperationTransactional(BusinessDataVo vo);
}
