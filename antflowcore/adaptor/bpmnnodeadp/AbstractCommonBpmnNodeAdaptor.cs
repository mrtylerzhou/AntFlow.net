using antflowcore.service.interf;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor;

/// <summary>
/// 通用BPMN节点适配器抽象基类
/// 提供通用的实体查询、属性设置、实体构建和参数校验模板方法
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public abstract class AbstractCommonBpmnNodeAdaptor<TEntity> : AbstractAdditionSignNodeAdaptor where TEntity : class
{
    protected AbstractCommonBpmnNodeAdaptor(
        BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
    }

    /// <summary>
    /// 格式化节点数据
    /// </summary>
    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);
        TEntity entity = QueryEntity(bpmnNodeVo);
        SetNodeProperty(bpmnNodeVo, entity);
    }

    /// <summary>
    /// 编辑节点
    /// </summary>
    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        CheckParam(bpmnNodeVo);
        base.EditBpmnNode(bpmnNodeVo);
        List<TEntity> entities = BuildEntity(bpmnNodeVo);
        if (entities != null && entities.Count > 0)
        {
            SaveEntities(entities);
        }
    }

    /// <summary>
    /// 设置节点属性（子类实现）
    /// </summary>
    /// <param name="bpmnNodeVo">节点VO</param>
    /// <param name="entity">实体</param>
    protected abstract void SetNodeProperty(BpmnNodeVo bpmnNodeVo, TEntity entity);

    /// <summary>
    /// 构建实体列表（子类实现）
    /// </summary>
    /// <param name="nodeVo">节点VO</param>
    /// <returns>实体列表</returns>
    protected abstract List<TEntity> BuildEntity(BpmnNodeVo nodeVo);

    /// <summary>
    /// 校验参数（子类实现）
    /// </summary>
    /// <param name="nodeVo">节点VO</param>
    protected abstract void CheckParam(BpmnNodeVo nodeVo);

    /// <summary>
    /// 查询实体（子类可重写）
    /// </summary>
    /// <param name="nodeVo">节点VO</param>
    /// <returns>实体</returns>
    protected abstract TEntity QueryEntity(BpmnNodeVo nodeVo);

    /// <summary>
    /// 保存实体列表（子类可重写）
    /// </summary>
    /// <param name="entities">实体列表</param>
    protected abstract void SaveEntities(List<TEntity> entities);
}
