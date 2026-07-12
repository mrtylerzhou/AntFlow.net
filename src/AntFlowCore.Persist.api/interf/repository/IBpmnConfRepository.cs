using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
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
}
