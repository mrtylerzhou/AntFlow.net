using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

/// <summary>
/// 用户自动审批设置 服务接口. 对应 Java UserAutoApproveBizServiceImpl.
/// </summary>
public interface IBpmUserAutoApproveService : IAntFlowRepositoryMix<BpmUserAutoApprove, IBpmUserAutoApproveRepository>
{
    ResultAndPage<UserAutoApproveVo> ListPage(PageDto pageDto, string ownerUserName, string ownerUserId, string formCode);

    /// <summary>
    /// 活跃流程下拉(三类: DIY/LF/第三方)
    /// </summary>
    List<UserAutoApproveVo> ActiveConfList();

    void Save(UserAutoApproveVo vo);

    void Update(UserAutoApproveVo vo);

    void Toggle(long id, int enabled);

    void Delete(long id);

    /// <summary>
    /// 复制到最新活跃版本(含节点/表单校验)
    /// </summary>
    void Copy(long id);

    /// <summary>
    /// 运行时查询: 归属人+formCode+活跃bpmnCode+启用
    /// </summary>
    List<BpmUserAutoApprove> ListForRuntime(string ownerUserId, string formCode, string bpmnCode);
}
