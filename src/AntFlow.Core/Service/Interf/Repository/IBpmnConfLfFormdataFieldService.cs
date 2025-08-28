using AntFlow.Core.Entity;

namespace AntFlow.Core.Service.Processor.LowCodeFlow;

public interface IBpmnConfLfFormdataFieldService
{
    Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId);
}