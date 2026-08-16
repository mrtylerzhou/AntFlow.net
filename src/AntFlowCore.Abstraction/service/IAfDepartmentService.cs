using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service;

/// <summary>
/// 部门服务抽象层(框架扩展点). 对等 Java org.openoa.base.service.AfDepartmentService.
/// 返回 BaseIdTranStruVo(id+name), 用户可提供自己的实现并注册覆盖.
/// 注意: 完整部门数据(树形懒加载/祖先链等)请使用 IDepartmentService 或默认实现.
/// </summary>
public interface IAfDepartmentService
{
    /// <summary>
    /// 根据部门id查询部门
    /// </summary>
    BaseIdTranStruVo GetDepartmentById(string id);

    /// <summary>
    /// 根据员工id查询其下级部门
    /// </summary>
    List<BaseIdTranStruVo> ListSubDepartmentByEmployeeId(string employeeId);

    /// <summary>
    /// 根据id集合批量查询部门
    /// </summary>
    List<BaseIdTranStruVo> GetByIds(List<string> ids);

    /// <summary>
    /// 根据部门名称模糊查询部门
    /// </summary>
    List<BaseIdTranStruVo> QueryByNameFuzzy(string name);

    /// <summary>
    /// 根据父级部门id查询直接子部门
    /// </summary>
    List<BaseIdTranStruVo> GetDepartmentsByParentId(string parentId);

    /// <summary>
    /// 根据企业id查询部门列表
    /// </summary>
    List<BaseIdTranStruVo> GetDepartmentByCompanyId(string companyId);

    /// <summary>
    /// 部门分页查询
    /// </summary>
    ResultAndPage<BaseIdTranStruVo> GetDepartmentPageList(int page, int pageSize, string name);
}