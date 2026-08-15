using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service;

/// <summary>
/// 流程管理员信息提供者(接口契约).
/// 默认实现 ProcessAddminProvider 在 AntFlowCore.Bpmn 中,可自定义实现替换.
/// </summary>
public interface IBpmnProcessAdminProvider
{
    BaseIdTranStruVo ProvideProcessAdminInfo();
}