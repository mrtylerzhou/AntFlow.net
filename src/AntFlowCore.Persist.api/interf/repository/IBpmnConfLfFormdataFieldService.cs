using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataFieldService : IAntFlowRepositoryMix<BpmnConfLfFormdataField, IBpmnConfLfFormdataFieldRepository>
{
    Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId);

    /// <summary>
    /// 按表单版本ID(formdataId)查询字段配置Map
    /// 用于外部表单模式,每个表单版本有独立的字段配置
    /// </summary>
    Dictionary<string, BpmnConfLfFormdataField> QryFieldMapByFormdataId(long formdataId);
}
