using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// 发起流程页(任务中心) 业务接口. 对应 Java StartFlowListBizService.
/// 前端与 Java 共享, .NET 仅实现后端.
/// </summary>
public interface IStartFlowListBizService
{
    /// <summary>
    /// 发起流程分页(页 = 最多 3 栏, 栏内按分类块)
    /// </summary>
    ResultAndPage<StartFlowCategoryVo> Page(StartFlowListPageReq? req);
}
