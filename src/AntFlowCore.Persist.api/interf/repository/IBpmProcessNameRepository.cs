using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameRepository : IBaseRepository<BpmProcessName>
{
    List<BpmProcessVo> GetAllProcessVo();
}
