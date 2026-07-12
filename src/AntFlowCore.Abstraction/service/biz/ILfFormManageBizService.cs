using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// 独立表单管理业务接口
/// </summary>
public interface ILfFormManageBizService
{
    /// <summary>
    /// 分页查询独立表单（家族分组，每族一行生效版本）
    /// </summary>
    ResultAndPage<LfFormManageVo> ListPage(PageDto pageDto, LfFormManageVo vo);

    /// <summary>
    /// 按 id 查询表单版本（编辑回显 / 审批按 id 取 formdata）
    /// </summary>
    LfFormManageVo GetById(long id);

    /// <summary>
    /// 保存表单：无 formCode => 新建家族+首版本(默认生效)；有 formCode => 新建版本(默认不生效)
    /// </summary>
    /// <returns>新版本的 id</returns>
    long Save(LfFormManageVo vo);

    /// <summary>
    /// 生效指定版本：同 formCode 的其他生效版本自动置为非生效（互斥）。
    /// </summary>
    void Effective(long id);

    /// <summary>
    /// 软删除单个版本。被生效流程引用时拒绝。
    /// 删生效版本后族进入"无生效版本"状态（不自动提升）。
    /// </summary>
    void Delete(long id);

    /// <summary>
    /// 查询某家族所有版本（历史版本查看）
    /// </summary>
    List<LfFormManageVo> ListHistory(string formCode);

    /// <summary>
    /// 列出所有生效独立表单（流程设计多选下拉框）
    /// </summary>
    List<LfFormManageVo> ListEffectiveForSelect();

    /// <summary>
    /// 查询引用了指定表单版本的所有流程配置（查看引用/表单血缘）
    /// </summary>
    List<BpmnConfVo> ListReferencingConfs(long formdataId);
}
