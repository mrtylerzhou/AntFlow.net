using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataFieldRepository : IBaseRepository<BpmnConfLfFormdataField>
{
    int UpdateIsConditionField(long confId, string fieldId, int isConditionField);
}
