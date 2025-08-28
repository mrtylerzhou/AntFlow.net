using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;
using System.Reflection;
using System.Text.Json;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>, IBpmVariableMessageService
{
    private readonly AfTaskInstService _afTaskInstService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmProcessForwardService _bpmProcessForwardService;
    private readonly BpmProcessNoticeService _bpmProcessNoticeService;
    private readonly BpmVariableApproveRemindService _bpmVariableApproveRemindService;
    private readonly ProcessBusinessContansService _processBusinessContansService;
    private readonly RoleService _roleService;
    private readonly AFTaskService _taskService;
    private readonly UserService _userService;
    private readonly BpmVariableService _variableService;

    public BpmVariableMessageService(
        IFreeSql freeSql,
        BpmVariableService variableService,
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        AfTaskInstService afTaskInstService,
        AFTaskService taskService,
        RoleService roleService,
        UserService userService,
        BpmProcessNoticeService bpmProcessNoticeService,
        BpmProcessForwardService bpmProcessForwardService,
        ProcessBusinessContansService processBusinessContansService,
        BpmVariableApproveRemindService bpmVariableApproveRemindService
    ) : base(freeSql)
    {
        _variableService = variableService;
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskInstService = afTaskInstService;
        _taskService = taskService;
        _roleService = roleService;
        _userService = userService;
        _bpmProcessNoticeService = bpmProcessNoticeService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _processBusinessContansService = processBusinessContansService;
        _bpmVariableApproveRemindService = bpmVariableApproveRemindService;
    }

    public BpmVariableMessageVo GetBpmVariableMessageVo(BusinessDataVo businessDataVo)
    {
        if (businessDataVo == null)
        {
            return null;
        }

        if (businessDataVo.OperationType == null)
        {
            throw new AFBizException("δ֪��������");
        }

        //get event type by operation type
        EventTypeEnum? eventTypeEnum =
            EventTypeEnumExtensions.GetEnumByOperationType(businessDataVo.OperationType.Value);

        if (eventTypeEnum == null || eventTypeEnum == 0)
        {
            return null;
        }

        //default link type is process type
        int type = 2;

        //if event type is cancel operation then link type is view type
        if (eventTypeEnum == EventTypeEnum.PROCESS_CANCELLATION)
        {
            type = 1;
        }

        BpmVariableMessageVo vo = new()
        {
            ProcessNumber = businessDataVo.ProcessNumber,
            FormCode = businessDataVo.FormCode,
            EventType = (int)eventTypeEnum,
            ForwardUsers = businessDataVo.UserIds,
            SignUpUsers = businessDataVo.SignUpUsers.Select(a => a.Id).ToList(),
            MessageType = eventTypeEnum.IsInNode() ? 2 : 1,
            EventTypeEnum = eventTypeEnum.Value,
            Type = type
        };
        return GetBpmVariableMessageVo(vo);
    }

    public BpmVariableMessageVo GetBpmVariableMessageVo(BpmVariableMessageVo vo)
    {
        if (vo == null)
        {
            return null;
        }

        BpmVariable bpmVariable = null;
        List<BpmVariable> bpmVariables =
            _variableService.baseRepo.Where(a => a.ProcessNum.Equals(vo.ProcessNumber)).ToList();

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
        BpmnConf bpmnConf = _bpmnConfService.baseRepo.Where(a => a.BpmnCode.Equals(bpmVariable.BpmnCode)).ToOne();

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
        BpmBusinessProcess businessProcess = _bpmBusinessProcessService.baseRepo
            .Where(a => a.BusinessNumber.Equals(vo.ProcessNumber)).ToOne();

        if (businessProcess == null)
        {
            throw new AFBizException($"can not get BpmBusinessProcess by process Numbeer:{vo.ProcessNumber}");
        }

        vo.ProcessInsId = businessProcess.ProcInstId;
        vo.StartUser = businessProcess.CreateUser;
        vo.ApplyDate = businessProcess.CreateTime?.ToString("yyyy-MM-dd");
        vo.ApplyTime = businessProcess.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");

        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo.Where(a => a.ProcInstId == vo.ProcessInsId)
            .ToList();
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
            List<BpmAfTask> tasks = _taskService.baseRepo
                .Where(a => a.ProcInstId == vo.ProcessInsId).ToList();
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
                (BpmnConfCommonElementVo? nextUserElement, BpmnConfCommonElementVo? nextFlowElement) =
                    BpmnFlowUtil.GetNextNodeAndFlowNode(elements, tasks[0].TaskDefKey);
                if (nextUserElement != null &&
                    nextUserElement.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
                {
                    //next element's id
                    string nextElementId = nextUserElement.ElementId;
                    vo.NextNodeApproveds = nextUserElement.AssigneeMap?.Select(a => a.Key).ToList();
                }
            }
        }

        return vo;
    }

    public void InsertVariableMessage(long variableId, BpmnConfCommonVo bpmnConfCommonVo)
    {
        // Variable message list
        List<BpmVariableMessage> bpmVariableMessages = new();

        // Process node approval remind list
        List<BpmVariableApproveRemind> bpmVariableApproveReminds = new();

        // Add out-of-node variable message config
        if (bpmnConfCommonVo.TemplateVos != null && bpmnConfCommonVo.TemplateVos.Any())
        {
            bpmVariableMessages.AddRange(GetBpmVariableMessages(variableId, bpmnConfCommonVo.TemplateVos, string.Empty,
                1));
        }

        // Add in-node message config
        if (bpmnConfCommonVo.ElementList != null && bpmnConfCommonVo.ElementList.Any())
        {
            foreach (BpmnConfCommonElementVo elementVo in bpmnConfCommonVo.ElementList)
            {
                if (elementVo.TemplateVos == null || !elementVo.TemplateVos.Any())
                {
                    continue;
                }

                bpmVariableMessages.AddRange(GetBpmVariableMessages(variableId, elementVo.TemplateVos,
                    elementVo.ElementId, 2));

                // Add process node approval remind list
                if (elementVo.ApproveRemindVo != null && elementVo.ApproveRemindVo.Days != null)
                {
                    bpmVariableApproveReminds.Add(new BpmVariableApproveRemind
                    {
                        VariableId = variableId,
                        ElementId = elementVo.ElementId,
                        Content = JsonSerializer.Serialize(elementVo.ApproveRemindVo)
                    });
                }
            }
        }

        // Save variable messages in batch if not empty
        if (bpmVariableMessages.Any())
        {
            baseRepo.Insert(bpmVariableMessages);
        }

        // Save approval reminds in batch if not empty
        if (bpmVariableApproveReminds.Any())
        {
            _bpmVariableApproveRemindService.baseRepo.Insert(bpmVariableApproveReminds);
        }
    }

    private List<BpmVariableMessage> GetBpmVariableMessages(long variableId, List<BpmnTemplateVo> templateVos,
        string elementId, int messageType)
    {
        return templateVos
            .Select(o => new BpmVariableMessage
            {
                VariableId = variableId,
                ElementId = elementId,
                MessageType = messageType,
                EventType = o.Event,
                Content = JsonSerializer.Serialize(o)
            })
            .ToList();
    }

    public bool CheckIsSendByTemplate(BpmVariableMessageVo vo)
    {
        BpmVariable bpmVariable = _variableService.baseRepo
            .Where(a => a.ProcessNum == vo.ProcessNumber)
            .ToOne();

        if (bpmVariable == null)
        {
            return false;
        }

        int? messageType = vo.MessageType;
        if (messageType == null)
        {
            return false;
        }

        if (messageType == 2)
        {
            long count = baseRepo
                .Where(a =>
                    a.VariableId == bpmVariable.Id
                    && a.MessageType == messageType
                    && a.EventType == vo.EventType).Count();
            return count > 0;
        }

        if (messageType == 1)
        {
            long count = baseRepo
                .Where(a =>
                    a.VariableId == bpmVariable.Id
                    && a.MessageType == messageType
                    && a.EventType == vo.EventType).Count();
            return count > 0;
        }

        return false;
    }

    public void SendTemplateMessages(BpmVariableMessageVo vo)
    {
        DoSendTemplateMessages(vo);
    }

    private void DoSendTemplateMessages(BpmVariableMessageVo vo)
    {
        //if next node's approvers is empty then query current tasks instead
        if (vo.NextNodeApproveds.IsEmpty())
        {
            List<BpmAfTask> tasks = _taskService.baseRepo
                .Where(a => a.ProcInstId == vo.ProcessInsId).ToList();

            if (!tasks.IsEmpty())
            {
                vo.NextNodeApproveds = tasks.Select(a => a.Assignee).ToList();
            }
        }

        if (vo.MessageType == 1)
        {
            //out of node messages
            List<BpmVariableMessage> bpmVariableMessages = baseRepo
                .Where(a =>
                    a.VariableId == vo.VariableId
                    && a.MessageType == 1
                    && a.EventType == vo.EventType).ToList();

            if (!bpmVariableMessages.IsEmpty())
            {
                foreach (BpmVariableMessage bpmVariableMessage in bpmVariableMessages)
                {
                    DoSendTemplateMessages(bpmVariableMessage, vo);
                }
            }
        }
        else if (vo.MessageType == 2)
        {
            //in node messages
            List<BpmVariableMessage> bpmVariableMessages = baseRepo
                .Where(a =>
                    a.VariableId == vo.VariableId
                    && a.EventType == vo.EventType)
                .ToList();
            if (!string.IsNullOrEmpty(vo.ElementId))
            {
                List<BpmVariableMessage> currentNodeVariableMessages =
                    bpmVariableMessages.Where(a => a.ElementId == vo.ElementId).ToList();
                if (!currentNodeVariableMessages.IsEmpty())
                {
                    bpmVariableMessages = currentNodeVariableMessages;
                }
            }

            if (!bpmVariableMessages.IsEmpty())
            {
                foreach (BpmVariableMessage bpmVariableMessage in bpmVariableMessages)
                {
                    DoSendTemplateMessages(bpmVariableMessage, vo);
                }
            }
        }
    }

    private void DoSendTemplateMessages(BpmVariableMessage bpmVariableMessage, BpmVariableMessageVo vo)
    {
        BpmnTemplateVo bpmnTemplateVo = new();
        if (!string.IsNullOrEmpty(bpmVariableMessage.Content))
        {
            bpmnTemplateVo = JsonSerializer.Deserialize<BpmnTemplateVo>(bpmVariableMessage.Content);
        }

        //query sender's info
        List<string> sendToUsers = GetSendToUsers(vo, bpmnTemplateVo);

        //if senders is empty then return
        if (sendToUsers.IsEmpty())
        {
            return;
        }

        List<Employee> employeeDetailByIds = _userService.GetEmployeeDetailByIds(sendToUsers.Distinct().ToList());
        if (employeeDetailByIds.IsEmpty())
        {
            return;
        }

        //send messages
        SendMessage(vo, bpmnTemplateVo, employeeDetailByIds);
    }

    private List<string> GetSendToUsers(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo)
    {
        List<string> sendUsers = new();
        //specified assignees
        if (!bpmnTemplateVo.EmpIdList.IsEmpty())
        {
            sendUsers.AddRange(bpmnTemplateVo.EmpIdList);
        }

        //specified roles
        if (!bpmnTemplateVo.RoleIdList.IsEmpty())
        {
            List<User> users = _roleService.QueryUserByRoleIds(bpmnTemplateVo.RoleIdList);
            if (!users.IsEmpty())
            {
                sendUsers.AddRange(users.Select(u => u.Id.ToString()));
            }
        }

        //node sign up users
        if (!vo.SignUpUsers.IsEmpty())
        {
            sendUsers.AddRange(vo.SignUpUsers);
        }

        //forwarded
        List<string> forwardUsers = null;
        List<BpmProcessForward> bpmProcessForwards = _bpmProcessForwardService.baseRepo
            .Where(a => a.ProcessInstanceId == vo.ProcessInsId).ToList();

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
                string? fileName = informEnum?.GetFileName();
                object filObject = null;
                if (!string.IsNullOrEmpty(fileName))
                {
                    filObject = vo.GetType().GetProperty(fileName);
                }

                if (filObject is IList<string>)
                {
                    sendUsers.AddRange((List<string>)filObject);
                }
                else if (filObject != null)
                {
                    sendUsers.Add(filObject.ToString());
                }
            }
        }

        return sendUsers;
    }

    private void SendMessage(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo, List<Employee> employees)
    {
        //query all types of the messages
        List<MessageSendTypeEnum> messageSendTypeEnums = _bpmProcessNoticeService.ProcessNoticeList(vo.FormCode)
            .Select(o => MessageSendTypeEnum.GetEnumByCode(o.Type)).ToList();

        List<BaseNumIdStruVo> messageSendTypeList = bpmnTemplateVo.MessageSendTypeList;
        if (!messageSendTypeEnums.IsEmpty() && !messageSendTypeList.IsEmpty())
        {
            messageSendTypeEnums =
                messageSendTypeList.Select(o => MessageSendTypeEnum.GetEnumByCode((int)o.Id)).ToList();
        }

        Dictionary<int, string> wildcardCharacterMap = GetWildcardCharacterMap(vo);
        InformationTemplateVo templateVo = new()
        {
            Id = bpmnTemplateVo.TemplateId, WildcardCharacterMap = wildcardCharacterMap
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

            List<UserMsgBatchVo> userMsgBathVos = employees
                .Select(o => GetUserMsgBathVo(o, informationTemplateVo.MailTitle,
                    informationTemplateVo.MailContent,
                    vo.TaskId, emailUrl, appUrl, MessageSendTypeEnum.MAIL))
                .ToList();
            if (messageSendTypeEnum == MessageSendTypeEnum.MAIL)
            {
                UserMsgUtils.SendMessageBathNoUserMessage(userMsgBathVos);
            }
            else if (messageSendTypeEnum == MessageSendTypeEnum.MESSAGE)
            {
                userMsgBathVos.ForEach(a =>
                    a.MessageSendTypeEnums = new List<MessageSendTypeEnum> { MessageSendTypeEnum.MESSAGE });
                UserMsgUtils.SendMessageBathNoUserMessage(userMsgBathVos);
            }
            else if (messageSendTypeEnum == MessageSendTypeEnum.PUSH)
            {
                userMsgBathVos.ForEach(a =>
                    a.MessageSendTypeEnums = new List<MessageSendTypeEnum> { MessageSendTypeEnum.PUSH });
                UserMsgUtils.SendMessageBathNoUserMessage(userMsgBathVos);
            }
        }
    }

    private UserMsgBatchVo GetUserMsgBathVo(
        Employee employee,
        string title,
        string content,
        string taskId,
        string emailUrl,
        string appUrl,
        MessageSendTypeEnum messageSendTypeEnum)
    {
        UserMsgVo? userMsgVo = new()
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
            UserMsgVo = userMsgVo, MessageSendTypeEnums = new List<MessageSendTypeEnum> { messageSendTypeEnum }
        };
    }

    private Dictionary<int, string> GetWildcardCharacterMap(BpmVariableMessageVo vo)
    {
        Dictionary<int, string>? wildcardCharacterMap = new();

        foreach (WildcardCharacterEnum? wildcardCharacterEnum in Enum.GetValues(typeof(WildcardCharacterEnum))
                     .Cast<WildcardCharacterEnum>())
        {
            string filName = wildcardCharacterEnum.FilName;
            if (string.IsNullOrWhiteSpace(filName))
            {
                continue;
            }

            PropertyInfo? propertyInfo = vo.GetType().GetProperty(filName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                continue;
            }

            object? property = propertyInfo.GetValue(vo);
            if (property != null)
            {
                if (wildcardCharacterEnum.IsSearchEmpl)
                {
                    if (property is IEnumerable<string> list)
                    {
                        List<string> propertyList = list.ToList();
                        if (!propertyList.Any())
                        {
                            continue;
                        }

                        List<BaseIdTranStruVo> employees = _userService.QueryUserByIds(propertyList);
                        List<string> emplNames = employees.Select(e => e.Name).ToList();
                        if (emplNames.Any())
                        {
                            wildcardCharacterMap[wildcardCharacterEnum.Code] = string.Join(",", emplNames);
                        }
                    }
                    else
                    {
                        string? stringValue = property.ToString();
                        if (stringValue != "0")
                        {
                            BaseIdTranStruVo employee = _userService.GetById(stringValue);
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
        Dictionary<string, string>? urlMap = new();

        string emailUrl = string.Empty;
        string appUrl = string.Empty;

        if (informationTemplateVo.JumpUrl != null &&
            (informationTemplateVo.JumpUrl == JumpUrlEnum.PROCESS_APPROVE.Code ||
             informationTemplateVo.JumpUrl == JumpUrlEnum.PROCESS_VIEW.Code))
        {
            int type = informationTemplateVo.JumpUrl == 1 ? 2 : 1;

            ProcessInforVo? processInfo = new()
            {
                ProcessinessKey = vo.BpmnCode,
                BusinessNumber = vo.ProcessNumber,
                FormCode = vo.FormCode,
                Type = type
            };

            bool isOutside = vo.IsOutside;

            emailUrl = _processBusinessContansService.GetRoute(
                MessageSendTypeEnum.MAIL.Code,
                processInfo,
                isOutside
            );

            appUrl = _processBusinessContansService.GetRoute(
                MessageSendTypeEnum.PUSH.Code,
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