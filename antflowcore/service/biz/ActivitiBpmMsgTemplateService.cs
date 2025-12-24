using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.entityj;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class ActivitiBpmMsgTemplateService
{
    private readonly IUserService _userService;
    private readonly BpmProcessNoticeService _bpmProcessNoticeService;
    private readonly IConfiguration _configuration;
    private readonly BpmnConfNoticeTemplateService _bpmnConfNoticeTemplateService;
    private readonly ILogger<ActivitiBpmMsgTemplateService> _logger;

    public ActivitiBpmMsgTemplateService(IUserService userService,
        BpmProcessNoticeService bpmProcessNoticeService,
        IConfiguration configuration,
        BpmnConfNoticeTemplateService bpmnConfNoticeTemplateService,
        ILogger<ActivitiBpmMsgTemplateService> logger)
    {
        _userService = userService;
        _bpmProcessNoticeService = bpmProcessNoticeService;
        _configuration = configuration;
        _bpmnConfNoticeTemplateService = bpmnConfNoticeTemplateService;
        _logger = logger;
    }
    private const  String baseTitle = "工作流通知";

    private const  String baseSpace = "。   ";
    public void SendBpmCustomMsg(ActivitiBpmMsgVo activitiBpmMsgVo, String content) {

        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmApprovalMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流流转通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}", 
                activitiBpmMsgVo.ProcessId, 
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_FLOW);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmApprovalMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null)
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("工作流流转通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.PROCESS_FLOW);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }
    public async Task SendBpmForwardedMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("收到转发工作流通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmForwardedMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("收到转发工作流通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.RECEIVE_FLOW_PROCESS);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }

    public async Task SendBpmFinishMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流完成通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_FINISH);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmFinishMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("工作流完成通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.PROCESS_FINISH);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }
    public async Task SendBpmRejectMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流流程审批不通过通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_REJECT);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmRejectMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("工作流流程审批不通过通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.PROCESS_REJECT);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }
    public async Task SendBpmTerminationMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流被终止通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        // 生成消息内容
        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_STOP);
    
        // 执行用户消息发送
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmTerminationMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("工作流被终止通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o,  (int)MsgNoticeTypeEnum.PROCESS_STOP);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }
    public async Task SendBpmGenerationApprovalMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流代审批通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }

    public async Task SendBpmGenerationApprovalMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        var messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("工作流代审批通知，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        System.Text.Json.JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.PROCESS_WAIR_VERIFY);
                return BuildUserMsgBatchVo(o, content);
            })
            .ToList();

        UserMsgUtils.SendMessageBatch(messages);
    }
    public async Task SendBpmChangePersonOrgiMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流变更处理人通知(原审批节点处理人)，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_CHANGE_ORIAL_TREATOR);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
    public async Task SendBpmChangePersonNewMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo == null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("工作流变更处理人通知(现审批节点处理人)，流程编号 {ProcessId}，入参 {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }

    private string GetContent(ActivitiBpmMsgVo activitiBpmMsgVo, int msgNoticeType)
    {
        _logger.LogInformation("content数据转换, activitiBpmMsgVo: {ActivitiBpmMsgVo}, msgNoticeType: {MsgNoticeType}", 
            System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo), msgNoticeType);

        BpmnConfNoticeTemplateDetail bpmnConfNoticeTemplateDetail = null;

        if (!string.IsNullOrWhiteSpace(activitiBpmMsgVo.BpmnCode))
        {
            bpmnConfNoticeTemplateDetail = 
                _bpmnConfNoticeTemplateService.GetDetailByCodeAndType(activitiBpmMsgVo.BpmnCode, msgNoticeType);
        }

        string content;
        if (bpmnConfNoticeTemplateDetail == null)
        {
            _logger.LogInformation("模板内容为空, bpmnCode: {BpmnCode}", activitiBpmMsgVo.BpmnCode);
            content = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode(msgNoticeType);
        }
        else
        {
            content = bpmnConfNoticeTemplateDetail.NoticeTemplateDetail;
        }

        string result = ReplaceTemplateDetail(activitiBpmMsgVo, content);

        _logger.LogInformation("转换后数据content: {Result}", result);

        return result;
    }

    private void DoUserMsgSend(ActivitiBpmMsgVo activitiBpmMsgVo, string content)
    {
       
        content = string.Join(" ", content, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

      
        UserMsgVo userMsgVo = BuildUserMsgVo(activitiBpmMsgVo, content);

       
        MessageSendTypeEnum[] messageSendTypeEnums = 
            GetMessageSendTypeEnums(activitiBpmMsgVo.ProcessId, activitiBpmMsgVo.FormCode, 1);

       
        UserMsgUtils.SendMessages(userMsgVo, messageSendTypeEnums);
    }
    private UserMsgVo BuildUserMsgVo(ActivitiBpmMsgVo activitiBpmMsgVo, string content)
    {
        DetailedUser employee = GetEmployee(activitiBpmMsgVo.UserId, activitiBpmMsgVo);

        return new UserMsgVo
        {
            UserId = activitiBpmMsgVo.UserId,
            Email = employee.Email,
            Mobile = employee.Mobile,
            Title = baseTitle,
            Content = content,
            EmailUrl = activitiBpmMsgVo.EmailUrl,
            Url = activitiBpmMsgVo.Url,
            AppPushUrl = activitiBpmMsgVo.AppPushUrl,
            TaskId = activitiBpmMsgVo.TaskId,
            SsoSessionDomain = _configuration["system.domain"]
        };
    }
    private MessageSendTypeEnum[] GetMessageSendTypeEnums(string processId, string formCode, int selectMack)
    {
        if (selectMack == 1)
        {
            var bpmProcessNotices = _bpmProcessNoticeService.ProcessNoticeList(formCode);
            if (bpmProcessNotices != null && bpmProcessNotices.Any())
            {
                return bpmProcessNotices
                    .Select(o => MessageSendTypeEnum.GetEnumByCode(o.Type))
                    .ToArray();
            }
        }
        else if (selectMack == 2)
        {
            BpmBusinessProcessService bpmBusinessProcessService = ServiceProviderUtils.GetService<BpmBusinessProcessService>();
            var bpmBusinessProcess = bpmBusinessProcessService.GetBpmBusinessProcess(processId) 
                                     ?? new BpmBusinessProcess();

            var processKey = bpmBusinessProcess.ProcessinessKey ?? processId;

            /*var bpmProcessNodeOvertimeVos = processNodeOvertimeService.SelectNoticeNodeName(processKey);
            if (bpmProcessNodeOvertimeVos != null && bpmProcessNodeOvertimeVos.Any())
            {
                return bpmProcessNodeOvertimeVos
                    .Select(o => MessageSendTypeEnum.GetEnumByCode(o.NoticeType))
                    .ToArray();
            }*/
        }

        return Array.Empty<MessageSendTypeEnum>();
    }

    private DetailedUser GetEmployee(string employeeId, ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        DetailedUser employee = _userService.GetEmployeeDetailById(employeeId);

        if (employee == null)
        {
            employee = new DetailedUser();
            _logger.LogError("流程消息查询员工信息失败，消息入参 {activitiBpmMsgVo}", 
                System.Text.Json.JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        return employee;
    }
    public string ReplaceTemplateDetail(ActivitiBpmMsgVo activitiBpmMsgVo, string content)
    {
        var noticeReplaceEnums = new List<NoticeReplaceEnum>();

        foreach (var noticeReplaceEnum in noticeReplaceEnums)
        {
            if (content.Contains("{" + noticeReplaceEnum.Desc + "}"))
            {
                noticeReplaceEnums.Add(noticeReplaceEnum);
            }
        }

        if (noticeReplaceEnums.Any())
        {
            DetailedUser employee = null;
            if (noticeReplaceEnums.Any(e => e.IsSelectEmpl))
            {
                employee = _userService.GetEmployeeDetailById(activitiBpmMsgVo.OtherUserId);
            }

            foreach (var noticeReplaceEnum in noticeReplaceEnums)
            {
                if (noticeReplaceEnum.IsSelectEmpl)
                {
                    var name = employee?.UserName ?? string.Empty;
                    content = content.Replace("{" + noticeReplaceEnum.Desc + "}", name);
                }
                else
                {
                    // 反射获取属性值
                    var propertyInfo = typeof(ActivitiBpmMsgVo).GetProperty(noticeReplaceEnum.FilName);
                    var propertyValue = propertyInfo?.GetValue(activitiBpmMsgVo)?.ToString() ?? string.Empty;

                    content = content.Replace("{" + noticeReplaceEnum.Desc + "}", propertyValue);
                }
            }
        }

        return content;
    }
    private UserMsgBatchVo BuildUserMsgBatchVo(ActivitiBpmMsgVo o, string content)
    {
        // 拼接内容 + 空格 + 当前时间
        content = string.Join(" ", content, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        return new UserMsgBatchVo
        {
            UserMsgVo = BuildUserMsgVo(o, content),
            MessageSendTypeEnums = GetMessageSendTypeEnums(o.ProcessId, o.FormCode, 1).ToList()
        };
    }

    public void SendBpmApprovalMsg(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        if (activitiBpmMsgVo==null) {
            return;
        }
        
        String content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_FLOW);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }
}