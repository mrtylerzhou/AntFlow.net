using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnConfLfFormdataRepository : IBaseRepository<BpmnConfLfFormdata>
{
    BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode);

    /// <summary>
    /// 分页查询独立表单的当前生效版本（每族一行生效版本）
    /// </summary>
    List<LfFormManageVo> ListEffectiveFormPage(Page<LfFormManageVo> page, LfFormManageVo vo);

    /// <summary>
    /// 查询某家族所有版本（历史版本查看，排除已软删）
    /// </summary>
    List<LfFormManageVo> ListVersionsByFormCode(string formCode);

    /// <summary>
    /// 所有生效独立表单（流程设计多选下拉框，含formdata以供前端解析条件字段）
    /// </summary>
    List<LfFormManageVo> ListAllEffectiveForms();

    /// <summary>
    /// 按 id 列表批量查询（含已软删，供运行中流程实例读取）
    /// </summary>
    List<BpmnConfLfFormdata> ListByIdsIgnoreDeleted(List<long> ids);

    /// <summary>
    /// 生成新的家族 formCode（返回最大值）
    /// </summary>
    string? GetMaxFormCode(string prefix);
}
