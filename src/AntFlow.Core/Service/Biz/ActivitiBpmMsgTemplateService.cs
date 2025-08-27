using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Reflection;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class ActivitiBpmMsgTemplateService
{
    private const string baseTitle = "????????";

    private const string baseSpace = "??   ";
    private readonly BpmnConfNoticeTemplateService _bpmnConfNoticeTemplateService;
    private readonly BpmProcessNoticeService _bpmProcessNoticeService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ActivitiBpmMsgTemplateService> _logger;
    private readonly UserService _userService;

    public ActivitiBpmMsgTemplateService(UserService userService,
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

    public void SendBpmCustomMsg(ActivitiBpmMsgVo activitiBpmMsgVo, string content)
    {
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
            _logger.LogDebug("??????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("??????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
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
            _logger.LogDebug("?????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("?????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
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
            _logger.LogDebug("??????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("??????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
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
            _logger.LogDebug("????????????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("????????????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
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
            _logger.LogDebug("????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        // ???????????
        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_STOP);

        // ?????????????
        DoUserMsgSend(activitiBpmMsgVo, content);
    }

    public async Task SendBpmTerminationMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        if (activitiBpmMsgVos == null || !activitiBpmMsgVos.Any())
        {
            return;
        }

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
                }

                string content = GetContent(o, (int)MsgNoticeTypeEnum.PROCESS_STOP);
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
            _logger.LogDebug("?????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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

        List<UserMsgBatchVo>? messages = activitiBpmMsgVos
            .Where(o => o != null)
            .Select(o =>
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("?????????????????????? {ProcessId}????? {ActivitiBpmMsgVo}",
                        o.ProcessId,
                        JsonSerializer.Serialize(o));
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
            _logger.LogDebug("?????????????????(????????????)???????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
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
            _logger.LogDebug("?????????????????(?????????????)???????? {ProcessId}????? {ActivitiBpmMsgVo}",
                activitiBpmMsgVo.ProcessId,
                JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        string content = GetContent(activitiBpmMsgVo, (int)MsgNoticeTypeEnum.PROCESS_CHANGE_NOW_TREATOR);
        DoUserMsgSend(activitiBpmMsgVo, content);
    }

    private string GetContent(ActivitiBpmMsgVo activitiBpmMsgVo, int msgNoticeType)
    {
        _logger.LogInformation("content???????, activitiBpmMsgVo: {ActivitiBpmMsgVo}, msgNoticeType: {MsgNoticeType}",
            JsonSerializer.Serialize(activitiBpmMsgVo), msgNoticeType);

        BpmnConfNoticeTemplateDetail bpmnConfNoticeTemplateDetail = null;

        if (!string.IsNullOrWhiteSpace(activitiBpmMsgVo.BpmnCode))
        {
            bpmnConfNoticeTemplateDetail =
                _bpmnConfNoticeTemplateService.GetDetailByCodeAndType(activitiBpmMsgVo.BpmnCode, msgNoticeType);
        }

        string content;
        if (bpmnConfNoticeTemplateDetail == null)
        {
            _logger.LogInformation("??????????, bpmnCode: {BpmnCode}", activitiBpmMsgVo.BpmnCode);
            content = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode(msgNoticeType);
        }
        else
        {
            content = bpmnConfNoticeTemplateDetail.NoticeTemplateDetail;
        }

        string result = ReplaceTemplateDetail(activitiBpmMsgVo, content);

        _logger.LogInformation("?????????content: {Result}", result);

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
        Employee employee = GetEmployee(activitiBpmMsgVo.UserId, activitiBpmMsgVo);

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
            List<BpmProcessNotice>? bpmProcessNotices = _bpmProcessNoticeService.ProcessNoticeList(formCode);
            if (bpmProcessNotices != null && bpmProcessNotices.Any())
            {
                return bpmProcessNotices
                    .Select(o => MessageSendTypeEnum.GetEnumByCode(o.Type))
                    .ToArray();
            }
        }
        else if (selectMack == 2)
        {
            BpmBusinessProcessService bpmBusinessProcessService =
                ServiceProviderUtils.GetService<BpmBusinessProcessService>();
            BpmBusinessProcess? bpmBusinessProcess = bpmBusinessProcessService.GetBpmBusinessProcess(processId)
                                                     ?? new BpmBusinessProcess();

            string? processKey = bpmBusinessProcess.ProcessinessKey ?? processId;

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

    private Employee GetEmployee(string employeeId, ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        Employee employee = _userService.GetEmployeeDetailById(employeeId);

        if (employee == null)
        {
            employee = new Employee();
            _logger.LogError("?????????????????????????? {activitiBpmMsgVo}",
                JsonSerializer.Serialize(activitiBpmMsgVo));
        }

        return employee;
    }

    public string ReplaceTemplateDetail(ActivitiBpmMsgVo activitiBpmMsgVo, string content)
    {
        List<NoticeReplaceEnum>? noticeReplaceEnums = new();

        foreach (NoticeReplaceEnum? noticeReplaceEnum in noticeReplaceEnums)
        {
            if (content.Contains("{" + noticeReplaceEnum.Desc + "}"))
            {
                noticeReplaceEnums.Add(noticeReplaceEnum);
            }
        }

        if (noticeReplaceEnums.Any())
        {
            Employee employee = null;
            if (noticeReplaceEnums.Any(e => e.IsSelectEmpl))
            {
                employee = _userService.GetEmployeeDetailById(activitiBpmMsgVo.OtherUserId);
            }

            foreach (NoticeReplaceEnum? noticeReplaceEnum in noticeReplaceEnums)
            {
                if (noticeReplaceEnum.IsSelectEmpl)
                {
                    string? name = employee?.Username ?? string.Empty;
                    content = content.Replace("{" + noticeReplaceEnum.Desc + "}", name);
                }
                else
                {
                    // ???????????
                    PropertyInfo? propertyInfo = typeof(ActivitiBpmMsgVo).GetProperty(noticeReplaceEnum.FilName);
                    string? propertyValue = propertyInfo?.GetValue(activitiBpmMsgVo)?.ToString() ?? string.Empty;

                    content = content.Replace("{" + noticeReplaceEnum.Desc + "}", propertyValue);
                }
            }
        }

        return content;
    }

    private UserMsgBatchVo BuildUserMsgBatchVo(ActivitiBpmMsgVo o, string content)
    {
        // ??????? + ??? + ??????
        content = string.Join(" ", content, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        return new UserMsgBatchVo
        {
            UserMsgVo = BuildUserMsgVo(o, content),
            MessageSendTypeEnums = GetMessageSendTypeEnums(o.ProcessId, o.FormCode, 1).ToList()
        };
    }
}