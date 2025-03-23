using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;

namespace antflowcore.service.repository;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>
{
    private readonly BpmVariableService _variableService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmVariableApproveRemindService _bpmVariableApproveRemindService;

    public BpmVariableMessageService(
        IFreeSql freeSql,
        BpmVariableService variableService,
        BpmnConfService bpmnConfService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmVariableApproveRemindService bpmVariableApproveRemindService
    ) : base(freeSql)
    {
        _variableService = variableService;
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
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

        if (eventTypeEnum == null||eventTypeEnum==0)
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
            SignUpUsers = businessDataVo.SignUpUsers.Select(a=>a.Id).ToList(),
            MessageType =eventTypeEnum.IsInNode()?2:1,
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


        //todo 

        return vo;
    }
    public void InsertVariableMessage(long variableId, BpmnConfCommonVo bpmnConfCommonVo)
    {
        // Variable message list
        var bpmVariableMessages = new List<BpmVariableMessage>();

        // Process node approval remind list
        var bpmVariableApproveReminds = new List<BpmVariableApproveRemind>();

        // Add out-of-node variable message config
        if (bpmnConfCommonVo.TemplateVos != null && bpmnConfCommonVo.TemplateVos.Any())
        {
            bpmVariableMessages.AddRange(GetBpmVariableMessages(variableId, bpmnConfCommonVo.TemplateVos, string.Empty, 1));
        }

        // Add in-node message config
        if (bpmnConfCommonVo.ElementList != null && bpmnConfCommonVo.ElementList.Any())
        {
            foreach (var elementVo in bpmnConfCommonVo.ElementList)
            {
                if (elementVo.TemplateVos == null || !elementVo.TemplateVos.Any())
                {
                    continue;
                }

                bpmVariableMessages.AddRange(GetBpmVariableMessages(variableId, elementVo.TemplateVos, elementVo.ElementId, 2));

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

    private List<BpmVariableMessage> GetBpmVariableMessages(long variableId, List<BpmnTemplateVo> templateVos, string elementId, int messageType)
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

}