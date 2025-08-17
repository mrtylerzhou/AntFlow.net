using AntFlowCore.Entity;

namespace antflowcore.service.processor.lowcodeflow;

public interface IBpmnConfLfFormdataFieldService
{
    Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId);
}