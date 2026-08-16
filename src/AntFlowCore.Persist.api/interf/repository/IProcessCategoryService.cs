using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 流程分类管理 业务服务. 对应 Java BpmProcessCategoryService.
/// </summary>
public interface IProcessCategoryService : IAntFlowRepositoryMix<BpmProcessCategory, IBpmProcessCategoryRepository>
{
    /// <summary>
    /// 新增/编辑分类(id 为空=新增, 非空=编辑)
    /// </summary>
    bool EditProcessCategory(BpmProcessCategoryVo vo);

    /// <summary>
    /// 分类操作: 2 上移 / 3 下移 / 4 删除
    /// </summary>
    bool CategoryOperation(int type, long id);

    /// <summary>
    /// 分类列表(is_del=0, sort asc)
    /// </summary>
    List<BpmProcessCategory> ProcessCategoryList(BpmProcessCategoryVo vo);

    /// <summary>
    /// 分页查询(PC 管理页)
    /// </summary>
    ResultAndPage<BpmProcessCategoryVo> SelectPage(PageDto pageDto, BpmProcessCategoryVo vo);

    /// <summary>
    /// 下拉选项(流程设计器-基础设置-流程类型): is_del=0, 不过滤内置 id/is_app
    /// </summary>
    List<BpmProcessCategoryVo> Options();
}
