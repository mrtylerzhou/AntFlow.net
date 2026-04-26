using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataRepository : IBaseRepository<BpmnConfLfFormdata>
{
    BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode);
}
