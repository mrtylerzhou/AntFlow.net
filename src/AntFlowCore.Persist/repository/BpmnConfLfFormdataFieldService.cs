using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfLfFormdataFieldService: AFBaseCurdRepositoryService<BpmnConfLfFormdataField>,IBpmnConfLfFormdataFieldService
{
    public BpmnConfLfFormdataFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public Dictionary<string,BpmnConfLfFormdataField> QryFormDataFieldMap(long confId)
    {
        List<BpmnConfLfFormdataField> allFields = baseRepo.Where(x => x.BpmnConfId == confId).ToList();
        if (allFields == null || !allFields.Any())
        {
            throw new AFBizException("lowcode form data has no fields");
        }

        var id2SelfMap = new Dictionary<string, BpmnConfLfFormdataField>();
        foreach (var field in allFields)
        {
            id2SelfMap[field.FieldId] = field;
        }

        return id2SelfMap;
    }
    
}