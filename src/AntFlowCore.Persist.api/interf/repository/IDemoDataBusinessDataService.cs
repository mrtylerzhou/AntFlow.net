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
}