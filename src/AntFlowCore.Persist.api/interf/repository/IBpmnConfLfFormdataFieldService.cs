using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataFieldService : IAntFlowRepositoryMix<BpmnConfLfFormdataField, IBpmnConfLfFormdataFieldRepository>
{
    Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId);
}
