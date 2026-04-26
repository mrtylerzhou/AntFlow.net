using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnViewPageButtonRepository: IBaseRepository<BpmnViewPageButton>
{
    void DeleteByConfId(long confId);
}
