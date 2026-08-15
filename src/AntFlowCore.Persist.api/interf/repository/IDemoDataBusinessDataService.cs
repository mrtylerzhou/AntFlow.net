using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 演示数据-业务数据 动态列表服务. 对应 Java DemoDataBusinessDataBizServiceImpl.
/// </summary>
public interface IDemoDataBusinessDataService
{
    /// <summary>
    /// 分页查询低代码流程业务数据,动态拼接横向表
    /// </summary>
    BusinessDataListVo ListPage(BusinessDataListPageReq req);

    /// <summary>
    /// 校验当前登录用户是否有权查看流程详情
    /// </summary>
    bool CheckPermission(string processNumber);

    /// <summary>
    /// 人员管理分页列表(姓名/手机号模糊搜索,关联部门名称/直属领导姓名/HRBP姓名)
    /// </summary>
    ResultAndPage<DemoDataUserVo> UserListPage(DemoDataMgmtPageReq req);

    /// <summary>
    /// 部门管理分页列表(名称模糊搜索,关联上级部门名称/负责人姓名)
    /// </summary>
    ResultAndPage<DemoDataDepartmentVo> DepartmentListPage(DemoDataMgmtPageReq req);

    /// <summary>
    /// 角色管理分页列表(名称模糊搜索,含关联人数)
    /// </summary>
    ResultAndPage<DemoDataRoleVo> RoleListPage(DemoDataMgmtPageReq req);

    /// <summary>
    /// 角色详情:角色下人员分页列表
    /// </summary>
    ResultAndPage<DemoDataRoleUserVo> RoleUsers(DemoDataMgmtPageReq req);
}
