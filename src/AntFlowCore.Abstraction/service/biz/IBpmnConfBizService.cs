using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnConfBizService
{
    void Edit(BpmnConfVo bpmnConfVo);
    ResultAndPage<BpmnConfVo> SelectPage(PageDto pageDto, BpmnConfVo vo);
    BpmnConfVo Detail(long id);
    BpmnConfVo Detail(string bpmnCode);
    BpmnConfVo DetailByFormCode(string formCode);
    int? GetCustomizeNodeSignType(long nodeId);
    
    /// <summary>
    /// Save process notice configuration (notice channel types and advanced
    /// notification templates) by partially updating conf_config_json.
    /// </summary>
    /// <param name="vo">process configuration vo carrying processKey, notifyTypeIds and templateVos</param>
    void SaveProcessNotices(ProcessConfVo vo);

    /// <summary>
    /// 根据 bpmnConfVo.FormCode 查询生效配置的 conf_config_json,
    /// 解析 confTemplates 并填充 bpmnConfVo.TemplateVos。
    /// 对应 Java 版 BpmnConfBizServiceImpl.setBpmnTemplateVos。
    /// </summary>
    void SetBpmnTemplateVos(BpmnConfVo bpmnConfVo);

    /// <summary>
    /// Check whether dynamic conditions have changed for the given process.
    /// Re-evaluates conditions with isMigration=true, isPreview=true.
    /// Returns true if conditions changed (CONDITION_CHANGED exception caught).
    /// </summary>
    bool MigrationCheckConditionsChange(BusinessDataVo vo);
}
