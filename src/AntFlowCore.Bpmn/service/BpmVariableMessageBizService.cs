using System.Collections;
using System.Reflection;
using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.service;

/// <summary>
/// Variable message business service.
///
/// This is the JSON-first adaptation of the legacy implementation. Message templates are
/// read from <c>t_bpm_variable.variable_config_json</c> (field <c>messages[]</c>) instead of
/// the dropped <c>t_bpm_variable_message</c> table, and notice channel types are read from
/// <c>t_bpmn_conf.conf_config_json</c> (field <c>noticeChannelTypes</c>) instead of the dropped
/// <c>bpm_process_notice</c> table. Behaviour mirrors the Java
/// <c>BpmVariableMessageBizServiceImpl</c>.
/// </summary>
public class BpmVariableMessageBizService : IBpmVariableMessageBizService
{
    private readonly IBpmVariableService _variableService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IAFTaskService _taskService;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly IProcessBusinessContansService _processBusinessContansService;

    public BpmVariableMessageBizService(
        IBpmVariableService variableService,
        IBpmnConfService bpmnConfService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IAfTaskInstService afTaskInstService,
        IAFTaskService taskService,
        IRoleService roleService,
        IUserService userService,
        IBpmProcessForwardService bpmProcessForwardService,
        IProcessBusinessContansService processBusinessContansService)
    {
        _variableService = variableService;
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskInstService = afTaskInstService;
        _taskService = taskService;
        _roleService = roleService;
        _userService = userService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _processBusinessContansService = processBusinessContansService;
    }

    /// <summary>
    /// check whether to send messages by template
    /// </summary>
    public bool CheckIsSendByTemplate(BpmVariableMessageVo vo)
    {
        BpmVariable bpmVariable = _variableService._repository
            .Find(a => a.ProcessNum == vo.ProcessNumber)
            .FirstOrDefault();

        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
        {
            return false;
        }

        VariableConfigJson config = JsonConfUtil.ParseVariableConfig(bpmVariable.VariableConfigJson);
        if (config == null || config.Messages.IsEmpty())
        {
            return false;
        }

        int? messageType = vo.MessageType;
        if (messageType == null)
        {
            return false;
        }

        //如果节点存在自定义通知类型,则默认走自定义的,需要注意的是即便不设置只要开启了通知,流程仍然会通知,内部有一套默认通知机制.自定义通知主要是为了增加灵活性,慎用
        if (messageType == 2)
        {
            //in node messages
            return config.Messages.Any(m => m.MessageType != null && m.MessageType == 2
                                            && m.EventType != null && m.EventType == vo.EventType);
        }

        if (messageType == 1)
        {
            //out of node messages
            return config.Messages.Any(m => m.MessageType != null && m.MessageType == 1
                                            && m.EventType != null && m.EventType == vo.EventType);
        }

        return false;
    }

    /// <summary>
    /// build variable message vo for sending messages
    /// </summary>
    public BpmVariableMessageVo GetBpmVariableMessageVo(BpmVariableMessageVo vo)
    {
        if (vo == null)
        {
            return null;
        }

        BpmVariable bpmVariable = null;
        List<BpmVariable> bpmVariables =
            _variableService._repository.Find(a => a.ProcessNum.Equals(vo.ProcessNumber));

        if (!ObjectUtils.IsEmpty(bpmVariables))
        {
            bpmVariable = bpmVariables[0];
        }

        if (bpmVariable == null)
        {
            return null;
        }

        //set variable id
        vo.VariableId = bpmVariable.Id;

        //get bpmn conf
        BpmnConf bpmnConf = _bpmnConfService._repository.Find(a => a.BpmnCode.Equals(bpmVariable.BpmnCode)).FirstOrDefault();

        if (bpmnConf == null)
        {
            throw new AFBizException($"can not get bpmnConf by bpmncode:{bpmVariable.BpmnCode}");
        }

        //set bpmn code
        vo.BpmnCode = bpmnConf.BpmnCode;

        //set bpmn name
        vo.BpmnName = bpmnConf.BpmnName;

        //set form code
        vo.FormCode = bpmnConf.FormCode;

        //todo
        //process type info
        //vo.setProcessType(SysDicUtils.getDicNameByCode("DIC_LCLB", bpmnConf.getBpmnType()));

        //set process start variables
        if (!string.IsNullOrEmpty(bpmVariable.ProcessStartConditions))
        {
            BpmnStartConditionsVo bpmnStartConditionsVo =
                JsonSerializer.Deserialize<BpmnStartConditionsVo>(bpmVariable.ProcessStartConditions);
            vo.BpmnStartConditions = bpmnStartConditionsVo;
            //set approval employee id
            vo.ApprovalEmplId = bpmnStartConditionsVo.ApprovalEmplId ?? "0";
        }

        //query bpmn business process by process number
        BpmBusinessProcess businessProcess = _bpmBusinessProcessService._repository
            .FirstOrDefault(a => a.BusinessNumber.Equals(vo.ProcessNumber));

        if (businessProcess == null)
        {
            throw new AFBizException($"can not get BpmBusinessProcess by process Numbeer:{vo.ProcessNumber}");
        }

        vo.ProcessInsId = businessProcess.ProcInstId;
        vo.StartUser = businessProcess.CreateUser;
        vo.ApplyDate = businessProcess.CreateTime?.ToString("yyyy-MM-dd");
        vo.ApplyTime = businessProcess.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");

        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService._repository.Find(a => a.ProcInstId == vo.ProcessInsId);
        vo.Approveds = bpmAfTaskInsts.Where(a => !string.IsNullOrEmpty(a.Assignee)).Select(a => a.Assignee).ToList();
        //if the current node approver is empty, then get it from login user info
        if (string.IsNullOrEmpty(vo.Assignee))
        {
            vo.Assignee = SecurityUtils.GetLogInEmpId();
        }

        //if the event type is in node event, then get the node info from activiti process engine
        if (vo.EventTypeEnum.IsInNode())
        {
            //get current task list by process instance id
            List<BpmAfTask> tasks = _taskService._repository
                .Find(a => a.ProcInstId == vo.ProcessInsId);
            if (!tasks.IsEmpty())
            {
                //if node is empty then get from task's definition
                if (string.IsNullOrEmpty(vo.ElementId))
                {
                    vo.ElementId = tasks[0].TaskDefKey;
                }

                //if task id is empty then get it from current tasks
                if (string.IsNullOrEmpty(vo.TaskId))
                {
                    vo.TaskId = tasks[0].Id;
                }

                //if link type is empty then set it default to 1
                vo.Type ??= 1;
                List<BpmnConfCommonElementVo> elements = BpmnFlowUtil.GetElementVosByDeployId(tasks[0].ProcDefId);
                var (nextUserElement, nextFlowElement) =
                    BpmnFlowUtil.GetNextNodeAndFlowNode(elements, tasks[0].TaskDefKey);
                if (nextUserElement != null &&
                    nextUserElement.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
                {
                    //next element's id
                    vo.NextNodeApproveds = nextUserElement.AssigneeMap?.Select(a => a.Key).ToList();
                }
            }
        }

        return vo;
    }

    /// <summary>
    /// send templated messages in sync way
    /// </summary>
    public void SendTemplateMessages(BpmVariableMessageVo vo)
    {
        DoSendTemplateMessages(vo);
    }

    /// <summary>
    /// do send templated messages
    /// </summary>
    private void DoSendTemplateMessages(BpmVariableMessageVo vo)
    {
        //if next node's approvers is empty then query current tasks instead
        if (vo.NextNodeApproveds.IsEmpty())
        {
            List<BpmAfTask> tasks = _taskService._repository
                .Find(a => a.ProcInstId == vo.ProcessInsId);

            if (!tasks.IsEmpty())
            {
                vo.NextNodeApproveds = tasks.Select(a => a.Assignee).ToList();
            }
        }

        //read messages from variable config JSON
        BpmVariable bpmVariable = _variableService._repository
            .Find(a => a.Id == vo.VariableId).FirstOrDefault();
        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
        {
            return;
        }
        VariableConfigJson config = JsonConfUtil.ParseVariableConfig(bpmVariable.VariableConfigJson);
        if (config == null || config.Messages.IsEmpty())
        {
            return;
        }

        if (vo.MessageType == 1)
        {
            //out of node messages
            List<VariableMessageItem> messageItems = config.Messages
                .Where(m => m.MessageType != null && m.MessageType == 1 && m.EventType == vo.EventType)
                .ToList();
            foreach (VariableMessageItem messageItem in messageItems)
            {
                DoSendTemplateMessages(messageItem, vo);
            }
        }
        else if (vo.MessageType == 2)
        {
            //in node messages
            List<VariableMessageItem> messageItems = config.Messages
                .Where(m => m.EventType == vo.EventType)
                .ToList();
            if (!string.IsNullOrEmpty(vo.ElementId))
            {
                List<VariableMessageItem> currentNodeVariableMessages = messageItems
                    .Where(a => a.ElementId == vo.ElementId).ToList();
                if (!currentNodeVariableMessages.IsEmpty())
                {
                    //如果当前节点有节点内通知消息,则覆盖全局通用的,否则使用全局的
                    messageItems = currentNodeVariableMessages;
                }
            }
            foreach (VariableMessageItem messageItem in messageItems)
            {
                DoSendTemplateMessages(messageItem, vo);
            }
        }
    }

    /// <summary>
    /// do send templated messages
    /// </summary>
    private void DoSendTemplateMessages(VariableMessageItem messageItem, BpmVariableMessageVo vo)
    {
        BpmnTemplateVo bpmnTemplateVo = new BpmnTemplateVo();
        if (!string.IsNullOrEmpty(messageItem.Content))
        {
            bpmnTemplateVo = JsonConfUtil.ParseObject<BpmnTemplateVo>(messageItem.Content);
        }

        //query sender's info
        List<string> sendToUsers = GetSendToUsers(vo, bpmnTemplateVo);

        //if senders is empty then return
        if (sendToUsers.IsEmpty())
        {
            return;
        }

        List<DetailedUser> detailedUserDetailByIds = _userService.GetEmployeeDetailByIds(sendToUsers.Distinct().ToList());
        if (detailedUserDetailByIds.IsEmpty())
        {
            return;
        }

        //send messages
        SendMessage(vo, bpmnTemplateVo, detailedUserDetailByIds);
    }

    private List<string> GetSendToUsers(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo)
    {
        List<string> sendUsers = new List<string>();
        //specified assignees
        if (!bpmnTemplateVo.EmpIdList.IsEmpty())
        {
            sendUsers.AddRange(bpmnTemplateVo.EmpIdList);
        }

        //specified roles
        if (!bpmnTemplateVo.RoleIdList.IsEmpty())
        {
            List<BaseIdTranStruVo> users;
            if (vo.IsOutside && ConfigUtil.IsFullSassMode())
            {
                users = _roleService.QuerySassUserByRoleIds(bpmnTemplateVo.RoleIdList);
            }
            else
            {
                users = _roleService.QueryUserByRoleIds(bpmnTemplateVo.RoleIdList);
            }

            if (!users.IsEmpty())
            {
                sendUsers.AddRange(users.Select(u => u.Id));
            }
        }

        //todo functions
        //node sign up users
        if (!vo.SignUpUsers.IsEmpty())
        {
            sendUsers.AddRange(vo.SignUpUsers);
        }

        //forwarded
        List<string> forwardUsers = null;
        List<BpmProcessForward> bpmProcessForwards = _bpmProcessForwardService._repository
            .Find(a => a.ProcessInstanceId == vo.ProcessInsId);

        if (!vo.ForwardUsers.IsEmpty() && !bpmProcessForwards.IsEmpty())
        {
            forwardUsers = new List<string>();
            forwardUsers.AddRange(vo.ForwardUsers);
            forwardUsers.AddRange(bpmProcessForwards.Select(o => o.ForwardUserId).Distinct().ToList());
            forwardUsers = forwardUsers.Distinct().ToList();
        }
        else if (vo.ForwardUsers.IsEmpty() && !bpmProcessForwards.IsEmpty())
        {
            forwardUsers = new List<string>();
            forwardUsers.AddRange(bpmProcessForwards.Select(o => o.ForwardUserId).Distinct().ToList());
            forwardUsers = forwardUsers.Distinct().ToList();
        }
        else if (!vo.ForwardUsers.IsEmpty() && bpmProcessForwards.IsEmpty())
        {
            forwardUsers = new List<string>();
            forwardUsers.AddRange(vo.ForwardUsers);
            forwardUsers = forwardUsers.Distinct().ToList();
        }
        vo.ForwardUsers = forwardUsers;

        //inform users
        if (!bpmnTemplateVo.InformIdList.IsEmpty())
        {
            foreach (string informId in bpmnTemplateVo.InformIdList)
            {
                InformEnum? informEnum = InformEnumExtensions.GetEnumByCode(int.Parse(informId));
                if (informEnum == InformEnum.ASSIGNED_USER || informEnum == InformEnum.ASSIGNEED_ROLES)
                {
                    continue;
                }
                //todo check whether the result is valid
                string? fileName = informEnum?.GetFileName();
                object filObject = null;
                if (!string.IsNullOrEmpty(fileName))
                {
                    filObject = ReflectionUtils.GetPropertyValue(vo, fileName);
                }
                if (filObject is IEnumerable enumerable and not string)
                {
                    foreach (object o in enumerable)
                    {
                        if (o != null)
                        {
                            sendUsers.Add(o.ToString());
                        }
                    }
                }
                else if (filObject != null)
                {
                    sendUsers.Add(filObject.ToString());
                }
            }
        }
        return sendUsers;
    }

    private void SendMessage(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo, List<DetailedUser> employees)
    {
        //query all types of the messages from conf_config_json
        BpmnConf bpmnConf = _bpmnConfService._repository
            .Find(a => a.FormCode == vo.FormCode && a.EffectiveStatus == 1).FirstOrDefault();
        BpmnConfConfigJson? confConfig = bpmnConf != null ? JsonConfUtil.ParseConfConfig(bpmnConf.ConfConfigJson) : null;
        List<int>? noticeChannelTypes = confConfig?.NoticeChannelTypes;
        List<MessageSendTypeEnum> messageSendTypeEnums = (noticeChannelTypes == null || noticeChannelTypes.Count == 0)
            ? new List<MessageSendTypeEnum>()
            : noticeChannelTypes.Select(t => MessageSendTypeEnum.GetEnumByCode(t)).ToList();

        List<BaseNumIdStruVo> messageSendTypeList = bpmnTemplateVo.MessageSendTypeList;
        //如果有模板自身的通知方式,则使用模板自身的通知方式,前提是有默认通知,即默认通知关闭以后节点也不会再通知
        if (!messageSendTypeEnums.IsEmpty() && !messageSendTypeList.IsEmpty())
        {
            messageSendTypeEnums = messageSendTypeList.Select(o => MessageSendTypeEnum.GetEnumByCode((int)o.Id)).ToList();
        }

        Dictionary<int, string> wildcardCharacterMap = GetWildcardCharacterMap(vo);
        InformationTemplateVo templateVo = new InformationTemplateVo
        {
            Id = bpmnTemplateVo.TemplateId,
            WildcardCharacterMap = wildcardCharacterMap
        };
        InformationTemplateVo informationTemplateVo = InformationTemplateUtils.TranslateInformationTemplate(templateVo);

        //get message urls
        Dictionary<string, string> urlMap = GetUrlMap(vo, informationTemplateVo);
        urlMap.TryGetValue("emailUrl", out string? emailUrl);
        urlMap.TryGetValue("appUrl", out string? appUrl);

        foreach (MessageSendTypeEnum messageSendTypeEnum in messageSendTypeEnums)
        {
            if (messageSendTypeEnum == null)
            {
                continue;
            }

            List<UserMsgBatchVo> userMsgBatchVos = employees
                .Select(o => GetUserMsgBatchVo(o, informationTemplateVo.MailTitle,
                    informationTemplateVo.MailContent,
                    vo.TaskId, emailUrl, appUrl, messageSendTypeEnum))
                .ToList();
            UserMsgUtils.SendGeneralPurposeMessages(userMsgBatchVos);
        }
    }

    private UserMsgBatchVo GetUserMsgBatchVo(
        DetailedUser employee,
        string title,
        string content,
        string taskId,
        string emailUrl,
        string appUrl,
        MessageSendTypeEnum messageSendTypeEnum)
    {
        var userMsgVo = new UserMsgVo
        {
            UserId = employee.Id,
            Email = employee.Email,
            Mobile = employee.Mobile,
            Title = title,
            Content = content,
            EmailUrl = emailUrl,
            Url = emailUrl,
            AppPushUrl = appUrl,
            TaskId = taskId
        };

        return new UserMsgBatchVo
        {
            UserMsgVo = userMsgVo,
            MessageSendTypeEnums = new List<MessageSendTypeEnum> { messageSendTypeEnum }
        };
    }

    private Dictionary<int, string> GetWildcardCharacterMap(BpmVariableMessageVo vo)
    {
        var wildcardCharacterMap = new Dictionary<int, string>();

        foreach (var wildcardCharacterEnum in WildcardCharacterEnum.Values)
        {
            var filName = wildcardCharacterEnum.FilName;
            if (string.IsNullOrWhiteSpace(filName))
                continue;

            // 反射获取 vo 的属性值
            var propertyInfo = vo.GetType().GetProperty(filName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null) continue;

            var property = propertyInfo.GetValue(vo);
            if (property != null)
            {
                if (wildcardCharacterEnum.IsSearchEmpl)
                {
                    if (property is IEnumerable<string> list)
                    {
                        var propertyList = list.ToList();
                        if (!propertyList.Any())
                            continue;

                        var employees = _userService.QueryUserByIds(propertyList);
                        var emplNames = employees.Select(e => e.Name).ToList();
                        if (emplNames.Any())
                        {
                            wildcardCharacterMap[wildcardCharacterEnum.Code] = string.Join(",", emplNames);
                        }
                    }
                    else
                    {
                        var stringValue = property.ToString();
                        if (stringValue != "0")
                        {
                            var employee = _userService.GetById(stringValue);
                            if (employee != null)
                            {
                                wildcardCharacterMap[wildcardCharacterEnum.Code] = employee.Name;
                            }
                        }
                    }
                }
                else
                {
                    wildcardCharacterMap[wildcardCharacterEnum.Code] = property.ToString();
                }
            }
        }

        return wildcardCharacterMap;
    }

    public Dictionary<string, string> GetUrlMap(BpmVariableMessageVo vo, InformationTemplateVo informationTemplateVo)
    {
        var urlMap = new Dictionary<string, string>();

        string emailUrl = string.Empty;
        string appUrl = string.Empty;

        if (informationTemplateVo.JumpUrl != null &&
            (informationTemplateVo.JumpUrl == JumpUrlEnum.PROCESS_APPROVE.Code ||
             informationTemplateVo.JumpUrl == JumpUrlEnum.PROCESS_VIEW.Code))
        {
            int type = informationTemplateVo.JumpUrl == 1 ? 2 : 1;

            var processInfo = new ProcessInforVo
            {
                ProcessinessKey = vo.BpmnCode,
                BusinessNumber = vo.ProcessNumber,
                FormCode = vo.FormCode,
                Type = type
            };

            bool isOutside = vo.IsOutside;

            emailUrl = _processBusinessContansService.GetRoute(
                ProcessNoticeEnum.EMAIL_TYPE.Code,
                processInfo,
                isOutside
            );

            appUrl = _processBusinessContansService.GetRoute(
                ProcessNoticeEnum.APP_TYPE.Code,
                processInfo,
                isOutside
            );
        }
        else if (informationTemplateVo.JumpUrl != null &&
                 informationTemplateVo.JumpUrl == JumpUrlEnum.PROCESS_BACKLOG.Code)
        {
            emailUrl = "/user/workflow/upcoming?page=1&pageSize=10";
            appUrl = "";
        }

        urlMap["emailUrl"] = emailUrl;
        urlMap["appUrl"] = appUrl;

        return urlMap;
    }
}
