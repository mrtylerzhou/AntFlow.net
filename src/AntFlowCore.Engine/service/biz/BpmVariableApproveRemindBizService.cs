using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;
using System.Text.Json;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// Process node timeout reminder job.
/// IMPORTANT: must be scheduled by an external scheduler at most once per day,
/// otherwise the same-day reminder would be sent repeatedly.
/// </summary>
public class BpmVariableApproveRemindBizService
{
    private readonly IAFTaskService _afTaskService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IUserService _userService;

    public BpmVariableApproveRemindBizService(IAFTaskService afTaskService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmVariableService bpmVariableService,
        IUserService userService)
    {
        _afTaskService = afTaskService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmVariableService = bpmVariableService;
        _userService = userService;
    }

    public void DoTimeoutReminder()
    {
        Dictionary<string, List<BpmnTimeoutReminderTaskVo>> tasksMultimap = GetTimeoutReminderTaskVoMultimap();
        if (tasksMultimap.Count == 0)
        {
            return;
        }
        Dictionary<string, BpmnTimeoutReminderVariableVo> variableVoMap = GetTimeoutReminderVariableVoMap(tasksMultimap);
        CheckAndSendMessage(tasksMultimap, variableVoMap);
    }

    private Dictionary<string, List<BpmnTimeoutReminderTaskVo>> GetTimeoutReminderTaskVoMultimap()
    {
        var map = new Dictionary<string, List<BpmnTimeoutReminderTaskVo>>();
        List<BpmAfTask> tasks = _afTaskService._repository.Find(a => !string.IsNullOrEmpty(a.Assignee));
        foreach (BpmAfTask task in tasks)
        {
            var taskVo = new BpmnTimeoutReminderTaskVo
            {
                ProcInstId = task.ProcInstId,
                TaskId = task.Id,
                ElementId = task.TaskDefKey,
                Assignee = task.Assignee,
                CreateTime = task.CreateTime
            };
            if (!map.TryGetValue(task.ProcInstId, out List<BpmnTimeoutReminderTaskVo> list))
            {
                list = new List<BpmnTimeoutReminderTaskVo>();
                map[task.ProcInstId] = list;
            }
            list.Add(taskVo);
        }
        return map;
    }

    private Dictionary<string, BpmnTimeoutReminderVariableVo> GetTimeoutReminderVariableVoMap(
        Dictionary<string, List<BpmnTimeoutReminderTaskVo>> tasksMultimap)
    {
        List<string> procInstIds = tasksMultimap.Keys.ToList();
        List<BpmBusinessProcess> bbps = _bpmBusinessProcessService._repository
            .Find(a => procInstIds.Contains(a.ProcInstId));
        List<string> businessNumbers = bbps.Select(a => a.BusinessNumber).Distinct().ToList();
        List<BpmVariable> bpmVariables = _bpmVariableService._repository
            .Find(a => businessNumbers.Contains(a.ProcessNum));

        var result = new Dictionary<string, BpmnTimeoutReminderVariableVo>();
        foreach (string procInstId in procInstIds)
        {
            BpmBusinessProcess bbp = bbps.FirstOrDefault(a => procInstId.Equals(a.ProcInstId));
            if (bbp == null)
            {
                continue;
            }
            BpmVariable bpmVariable = bpmVariables.FirstOrDefault(o => bbp.BusinessNumber.Equals(o.ProcessNum));
            if (bpmVariable == null)
            {
                continue;
            }

            var vo = new BpmnTimeoutReminderVariableVo
            {
                ProcessinessKey = bbp.ProcessinessKey,
                BusinessId = bbp.BusinessId,
                EntryId = bbp.EntryId,
                ProcessName = bpmVariable.ProcessName,
                ProcessNum = bpmVariable.ProcessNum,
                BpmnName = bpmVariable.ProcessName,
                ProcessNumber = bpmVariable.ProcessNum
            };

            if (!string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
            {
                VariableConfigJson varConfig = JsonSerializer.Deserialize<VariableConfigJson>(bpmVariable.VariableConfigJson, JsonConfUtil.Options);
                if (varConfig != null && !varConfig.ApproveReminds.IsEmpty())
                {
                    vo.BpmVariableApproveReminds = varConfig.ApproveReminds;
                }
            }

            DetailedUser startUser = _userService.GetEmployeeDetailById(bbp.CreateUser);
            vo.StartUser = startUser?.UserName;
            vo.ApplyDate = bbp.CreateTime?.ToString("yyyy-MM-dd");
            vo.ApplyTime = bbp.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");
            result[procInstId] = vo;
        }
        return result;
    }

    private void CheckAndSendMessage(Dictionary<string, List<BpmnTimeoutReminderTaskVo>> tasksMultimap,
        Dictionary<string, BpmnTimeoutReminderVariableVo> variableVoMap)
    {
        foreach (var kvp in tasksMultimap)
        {
            List<BpmnTimeoutReminderTaskVo> taskVos = kvp.Value;
            if (taskVos.IsEmpty())
            {
                continue;
            }
            if (!variableVoMap.TryGetValue(kvp.Key, out BpmnTimeoutReminderVariableVo variableVo) || variableVo == null)
            {
                continue;
            }
            List<VariableApproveRemindItem> approveRemindItems = variableVo.BpmVariableApproveReminds;
            if (approveRemindItems.IsEmpty())
            {
                continue;
            }

            foreach (BpmnTimeoutReminderTaskVo taskVo in taskVos)
            {
                if (taskVo.CreateTime == null)
                {
                    continue;
                }
                VariableApproveRemindItem item = approveRemindItems
                    .FirstOrDefault(o => taskVo.ElementId != null && taskVo.ElementId.Equals(o.ElementId));
                if (item == null || string.IsNullOrEmpty(item.Content))
                {
                    continue;
                }

                BpmnApproveRemindVo remindVo = JsonSerializer.Deserialize<BpmnApproveRemindVo>(item.Content, JsonConfUtil.Options);
                if (remindVo == null || remindVo.Days.IsEmpty() || remindVo.StandardMinutes == null)
                {
                    continue;
                }

                long elapsedMinutes = (long)(DateTime.Now - taskVo.CreateTime.Value).TotalMinutes;
                if (elapsedMinutes < remindVo.StandardMinutes)
                {
                    continue;
                }

                // day N after timeout, first 24h after timeout = day 1
                int overdueDay = (int)((elapsedMinutes - remindVo.StandardMinutes.Value) / (24 * 60)) + 1;
                if (remindVo.Days.Contains(overdueDay))
                {
                    DoSendMessage(variableVo, taskVo, remindVo);
                }
            }
        }
    }

    private void DoSendMessage(BpmnTimeoutReminderVariableVo variableVo, BpmnTimeoutReminderTaskVo taskVo, BpmnApproveRemindVo remindVo)
    {
        string emailUrl = "";
        string appUrl = "";
        string emplId = taskVo.Assignee;
        DetailedUser detailedUser = _userService.GetEmployeeDetailById(emplId);
        if (detailedUser == null)
        {
            return;
        }

        InformationTemplateVo informationTemplateVo = GetInformationTemplateVo(variableVo, remindVo, detailedUser);

        // resolve channels: empty -> in-site message only
        List<MessageSendTypeEnum> channels = remindVo.NoticeTypes.IsEmpty()
            ? new List<MessageSendTypeEnum> { MessageSendTypeEnum.IN_SITE }
            : remindVo.NoticeTypes.Select(MessageSendTypeEnum.GetEnumByCode)
                .Where(x => x != null).ToList();

        // unified dispatch: mail/sms/push/wechat/ding/feishu/in-site all go through adaptor dispatch,
        // content uses MailTitle/MailContent same as the regular notice path
        UserMsgUtils.SendGeneralPurposeMessages(GetUserMsgVo(taskVo, emailUrl, appUrl, emplId, detailedUser,
            informationTemplateVo.MailTitle, informationTemplateVo.MailContent), channels.ToArray());
    }

    private InformationTemplateVo GetInformationTemplateVo(BpmnTimeoutReminderVariableVo variableVo,
        BpmnApproveRemindVo remindVo, DetailedUser detailedUser)
    {
        // no template configured: fall back to PROCESS_TIME_OUT default text
        if (remindVo.TemplateId == null)
        {
            string content = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode((int)MsgNoticeTypeEnum.PROCESS_TIME_OUT)
                .Replace("{流程类型}", "")
                .Replace("{流程名称}", variableVo.ProcessName ?? "")
                .Replace("{流程编号}", variableVo.ProcessNumber ?? "");
            string title = MsgNoticeTypeEnumExtensions.GetDescByCode((int)MsgNoticeTypeEnum.PROCESS_TIME_OUT);
            return new InformationTemplateVo
            {
                SystemTitle = title,
                SystemContent = content,
                MailTitle = title,
                MailContent = content,
                NoteContent = content
            };
        }

        var wildcardCharacterMap = new Dictionary<int, string>
        {
            { WildcardCharacterEnum.ONE_CHARACTER.Code, variableVo.ProcessName ?? "" },
            { WildcardCharacterEnum.TWO_CHARACTER.Code, variableVo.ProcessNum ?? "" },
            { WildcardCharacterEnum.THREE_CHARACTER.Code, variableVo.StartUser ?? "" },
            { WildcardCharacterEnum.FOUR_CHARACTER.Code, variableVo.ApprovalEmpl ?? "" },
            { WildcardCharacterEnum.FIVE_CHARACTER.Code, variableVo.ApplyDate ?? "" },
            { WildcardCharacterEnum.SIX_CHARACTER.Code, variableVo.ApplyTime ?? "" },
            { WildcardCharacterEnum.EIGHT_CHARACTER.Code, detailedUser.UserName ?? "" }
        };
        return InformationTemplateUtils.TranslateInformationTemplate(new InformationTemplateVo
        {
            Id = remindVo.TemplateId,
            WildcardCharacterMap = wildcardCharacterMap
        });
    }

    private UserMsgVo GetUserMsgVo(BpmnTimeoutReminderTaskVo taskVo, string emailUrl, string appUrl, string emplId,
        DetailedUser detailedUser, string title, string content)
    {
        return new UserMsgVo
        {
            UserId = emplId,
            Email = detailedUser.Email,
            Mobile = detailedUser.Mobile,
            Title = title,
            Content = content,
            EmailUrl = emailUrl,
            Url = emailUrl,
            AppPushUrl = appUrl,
            TaskId = taskVo.TaskId
        };
    }
}