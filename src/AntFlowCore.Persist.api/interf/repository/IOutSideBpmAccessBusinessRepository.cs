using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmAccessBusinessRepository : IBaseRepository<OutSideBpmAccessBusiness>
{
    List<BpmnConfVo> SelectOutSideFormCodePageList();
}
