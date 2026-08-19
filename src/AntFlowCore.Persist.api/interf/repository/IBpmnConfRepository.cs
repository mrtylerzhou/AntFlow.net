using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfRepository : IBaseRepository<BpmnConf>
{
    string? GetMaxBpmnCode(string bpmnCodeParts);
    string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode);
    List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo);
    void EffectiveBpmnConf(int id);
    BpmnConf GetBpmnConfByFormCode(string formCode);
    List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes);

    /// <summary>
    /// 统计有多少生效流程引用了指定表单版本id（删除保护）
    /// </summary>
    int CountEffectiveConfReferencingFormdata(long formdataId);

    /// <summary>
    /// 查询所有在 lf_formdata_ids 中引用了指定表单版本的流程配置（查看引用/表单血缘）
    /// </summary>
    List<BpmnConfVo> ListConfsReferencingFormdata(long formdataId);

    /// <summary>
    /// 发起流程页: 聚合所有可用流程(effective_status=1, is_del=0), 左连 bpm_process_app_application 取 applicationId.
    /// 对应 Java BpmnConfMapper.selectStartFlowList.
    /// </summary>
    List<StartFlowListRowVo> SelectStartFlowList();
}
