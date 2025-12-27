using System.Collections;
using System.Reflection;
using System.Text.Json;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using AntFlowCore.Entities;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.entityj;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.interf.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>,IBpmVariableMessageService
{
    private readonly BpmVariableService _variableService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFTaskService _taskService;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly BpmProcessNoticeService _bpmProcessNoticeService;
    private readonly BpmProcessForwardService _bpmProcessForwardService;
    private readonly ProcessBusinessContansService _processBusinessContansService;
    private readonly BpmVariableApproveRemindService _bpmVariableApproveRemindService;

    public BpmVariableMessageService(
        IFreeSql freeSql,
        BpmVariableService variableService,
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        AfTaskInstService afTaskInstService,
        AFTaskService taskService,
        IRoleService roleService,
        IUserService userService,
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
            throw new AFBizException("未知操作类型");
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

        BpmVariableMessageVo vo = new BpmVariableMessageVo
        {
            ProcessNumber = businessDataVo.ProcessNumber,
            FormCode = businessDataVo.FormCode,
            EventType = (int)eventTypeEnum,
            ForwardUsers = businessDataVo.UserIds,
            SignUpUsers = businessDataVo.SignUpUsers.Select(a => a.Id).ToList(),
            MessageType = eventTypeEnum.IsInNode() ? 2 : 1,
            EventTypeEnum = eventTypeEnum.Value,
            Type = type,
        };
        return GetBpmVariableMessageVo(vo);
    }

    /**
    * build variable message vo for sending messages
    *
    * @param vo
    */
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
                var (nextUserElement, nextFlowElement) =
                    BpmnFlowUtil.GetNextNodeAndFlowNode(elements, tasks[0].TaskDefKey);
                if (nextUserElement != null &&
                    nextUserElement.ElementType == ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code)
                {
                    //next element's id
                    String nextElementId = nextUserElement.ElementId;
                    vo.NextNodeApproveds = nextUserElement.AssigneeMap?.Select(a => a.Key).ToList();
                }
            }
        }

        return vo;
    }

    public void InsertVariableMessage(long variableId, BpmnConfCommonVo bpmnConfCommonVo)
    {
        // Variable message list
        List<BpmVariableMessage> bpmVariableMessages = new List<BpmVariableMessage>();

        // Process node approval remind list
        List<BpmVariableApproveRemind> bpmVariableApproveReminds = new List<BpmVariableApproveRemind>();

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
                MessageType = GetMessageSendType(o.Event,messageType),
                EventType = o.Event,
                Content = JsonSerializer.Serialize(o)
                ,CreateTime = DateTime.Now
            })
            .ToList();
    }

    
    private int GetMessageSendType(int messageEvent,int defaultMessageSendType){
        if(messageEvent==null){
            return defaultMessageSendType;
        }

        EventTypeEnum eventTypeEnum = (EventTypeEnum)messageEvent;
        if(eventTypeEnum==null){
            return defaultMessageSendType;
        }

        EventTypeEnumExtensions.EventTypeMappings.TryGetValue(eventTypeEnum, out var eventTypeMapping);
        bool isInNode = eventTypeMapping?.IsInNode??false;
        return isInNode?2:1;
    }
    /**

    * check whether to to send messages by template
    *
    * @param vo
    * @return
    */
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
    
        //如果节点存在自定义通知类型,则默认走自定义的,需要注意的是即便不设置只要开启了通知,流程仍然会通知,内部有一套默认通知机制.自定义通知主要是为了增加灵活性,慎用
        if (messageType == 2)
        {
            long count = this.baseRepo
                .Where(a=> 
                             a.VariableId == bpmVariable.Id
                           //&&a.ElementId == vo.ElementId
                           && a.MessageType == messageType
                           && a.EventType == vo.EventType).Count();
            return count > 0;
        }else if (messageType == 1)
        {
            long count = this.baseRepo
                .Where(a=> 
                              a.VariableId == bpmVariable.Id
                           && a.MessageType == messageType
                           && a.EventType == vo.EventType).Count();
            return count > 0;
        }

        return false;
    }

    /// <summary>
    /// end templated messages in sync way
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    public void SendTemplateMessages(BpmVariableMessageVo vo)
    {
        DoSendTemplateMessages(vo);
    }

    /**
   * do send templated messages
   *
   * @param vo
   */
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

        List<BpmVariableMessage> bpmVariableMessages = null;
        if (vo.MessageType == 1)
        {
            //out of node messages
            bpmVariableMessages = this.baseRepo
                .Where(a =>
                    a.VariableId == vo.VariableId
                    && a.MessageType == 1
                    && a.EventType == vo.EventType).ToList();
            
        }
        else if (vo.MessageType == 2)
        {
            //in node messages
             bpmVariableMessages = this.baseRepo
                .Where(a =>
                    a.VariableId == vo.VariableId
                    && a.EventType == vo.EventType)
                .ToList();
            if (!string.IsNullOrEmpty(vo.ElementId))
            {
                List<BpmVariableMessage> currentNodeVariableMessages = bpmVariableMessages.Where(a => a.ElementId == vo.ElementId).ToList();
                if(!currentNodeVariableMessages.IsEmpty())
                {
                    //如果当前节点有节点内通知消息,则覆盖全局通用的,否则使用全局的
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
    /**
    * do send templated messages
    *
    * @param bpmVariableMessage
    */
    private void DoSendTemplateMessages(BpmVariableMessage bpmVariableMessage, BpmVariableMessageVo vo) {

        BpmnTemplateVo bpmnTemplateVo = new BpmnTemplateVo();
        if (!string.IsNullOrEmpty(bpmVariableMessage.Content)) {
            bpmnTemplateVo = JsonSerializer.Deserialize<BpmnTemplateVo>(bpmVariableMessage.Content);
        }


        //query sender's info
        List<String> sendToUsers = GetSendToUsers(vo, bpmnTemplateVo);


        //if senders is empty then return
        if (sendToUsers.IsEmpty()) {
            return;
        }

        List<DetailedUser> detailedUserDetailByIds = _userService.GetEmployeeDetailByIds(sendToUsers.Distinct().ToList());
        if(detailedUserDetailByIds.IsEmpty()){
            return;
        }

        //send messages
        SendMessage(vo, bpmnTemplateVo, detailedUserDetailByIds);

    }
     private List<String> GetSendToUsers(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo)
     {
         List<String> sendUsers = new List<string>();
        //specified assignees
        if (!bpmnTemplateVo.EmpIdList.IsEmpty()) {
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
                sendUsers.AddRange(users.Select(u => u.Id.ToString()));
            }
        }

        //todo functions
        //node sign up users
        if (!vo.SignUpUsers.IsEmpty()) {
            sendUsers.AddRange(vo.SignUpUsers);
        }

        //forwarded
        List<String> forwardUsers = null;
        List<BpmProcessForward> bpmProcessForwards = _bpmProcessForwardService.baseRepo
            .Where(a=>a.ProcessInstanceId==vo.ProcessInsId).ToList();
            
        if (!vo.ForwardUsers.IsEmpty() && !bpmProcessForwards.IsEmpty()) {
            forwardUsers =new List<String>();
            forwardUsers.AddRange(vo.ForwardUsers);
            forwardUsers.AddRange(bpmProcessForwards.Select(o => o.ForwardUserId).Distinct().ToList());
            forwardUsers = forwardUsers.Distinct().ToList();
        } else if (vo.ForwardUsers.IsEmpty() && !bpmProcessForwards.IsEmpty())
        {
            forwardUsers = new List<string>();
            forwardUsers.AddRange(bpmProcessForwards.Select(o => o.ForwardUserId).Distinct().ToList());
            forwardUsers = forwardUsers.Distinct().ToList();
        } else if (!vo.ForwardUsers.IsEmpty() && bpmProcessForwards.IsEmpty())
        {
            forwardUsers = new List<string>();
            forwardUsers.AddRange(vo.ForwardUsers);
            forwardUsers = forwardUsers.Distinct().ToList();
        }
        vo.ForwardUsers=forwardUsers;

        //inform users
        if (!bpmnTemplateVo.InformIdList.IsEmpty()) {
            foreach (String informId in bpmnTemplateVo.InformIdList) {
                InformEnum? informEnum = InformEnumExtensions.GetEnumByCode(int.Parse(informId));
                if(informEnum==InformEnum.ASSIGNED_USER||informEnum==InformEnum.ASSIGNEED_ROLES){
                    continue;
                }
                //todo check whether the result is valid
                string? fileName = informEnum?.GetFileName();
                Object filObject = null;
                if (!string.IsNullOrEmpty(fileName))
                {

                    filObject = ReflectionUtils.GetPropertyValue(vo, fileName);
                }
                if (filObject  is IEnumerable enumerable and not string)
                {
                    foreach (object o in enumerable)
                    {
                        if (o != null)
                        {
                            sendUsers.Add(o.ToString());
                        }
                    }
                } else if (filObject!=null) {
                    sendUsers.Add(filObject.ToString());
                }
            }
        }
        return sendUsers;
    }
     private void SendMessage(BpmVariableMessageVo vo, BpmnTemplateVo bpmnTemplateVo, List<DetailedUser> employees) {
        //query all types of the messages
        List<MessageSendTypeEnum> messageSendTypeEnums = _bpmProcessNoticeService.ProcessNoticeList(vo.FormCode)
            .Select(o => MessageSendTypeEnum.GetEnumByCode(o.Type)).ToList();

        List<BaseNumIdStruVo> messageSendTypeList = bpmnTemplateVo.MessageSendTypeList;
        if(!messageSendTypeEnums.IsEmpty()&&!messageSendTypeList.IsEmpty())//如果有模板自身的通知方式,则使用模板自身的通知方式,前提是有默认通知,即默认通知关闭以后节点也不会再通知
        {
            messageSendTypeEnums=messageSendTypeList.Select(o => MessageSendTypeEnum.GetEnumByCode((int)o.Id)).ToList();
        }
        Dictionary<int, String> wildcardCharacterMap = GetWildcardCharacterMap(vo);
        InformationTemplateVo templateVo = new InformationTemplateVo
        {
            Id = bpmnTemplateVo.TemplateId,
            WildcardCharacterMap =wildcardCharacterMap
        };
        InformationTemplateVo informationTemplateVo = InformationTemplateUtils.TranslateInformationTemplate(templateVo);

        //get message urls
        Dictionary<String, String> urlMap = GetUrlMap(vo, informationTemplateVo);
       urlMap.TryGetValue("emailUrl", out string? emailUrl);
       urlMap.TryGetValue("appUrl", out string? appUrl);

        foreach (MessageSendTypeEnum messageSendTypeEnum in messageSendTypeEnums) {
            if (messageSendTypeEnum==null) {
                continue;
            }

            List<UserMsgBatchVo> userMsgBatchVos = employees
                .Select(o => GetUserMsgBatchVo(o, informationTemplateVo.MailTitle,
                    informationTemplateVo.MailContent,
                    vo.TaskId, emailUrl, appUrl, MessageSendTypeEnum.MAIL))
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