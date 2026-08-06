using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

/// <summary>
/// Variable message business service.
/// Reads message configuration from <c>t_bpm_variable.variable_config_json</c>
/// (replacing the legacy <c>t_bpm_variable_message</c> table) and notice channel
/// types from <c>t_bpmn_conf.conf_config_json</c> (replacing <c>bpm_process_notice</c>).
/// </summary>
public interface IBpmVariableMessageBizService
{
    /// <summary>
    /// Checks whether template-based messages should be sent for the given event.
    /// </summary>
    bool CheckIsSendByTemplate(BpmVariableMessageVo vo);

    /// <summary>
    /// Builds the full variable message vo for sending messages.
    /// </summary>
    BpmVariableMessageVo GetBpmVariableMessageVo(BpmVariableMessageVo vo);

    /// <summary>
    /// Sends templated messages synchronously.
    /// </summary>
    void SendTemplateMessages(BpmVariableMessageVo vo);

    /// <summary>
    /// Builds a variable message vo from a business data vo, looking up event type
    /// from operation type and enriching with process/task metadata.
    /// </summary>
    BpmVariableMessageVo FromBusinessDataVo(BusinessDataVo businessDataVo);

    /// <summary>
    /// Sends templated messages asynchronously (fire-and-forget).
    /// </summary>
    void SendTemplateMessagesAsync(BpmVariableMessageVo vo);
}
