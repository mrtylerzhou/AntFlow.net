using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataFieldService : IBaseRepositoryService<BpmnConfLfFormdataField>
{
    Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId);
}
