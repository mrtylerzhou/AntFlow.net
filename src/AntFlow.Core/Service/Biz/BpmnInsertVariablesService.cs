using AntFlow.Core.Adaptor.Variable;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class BpmnInsertVariablesService
{
    private readonly BpmVariableButtonService _bpmVariableButtonService;
    private readonly BpmVariableMessageService _bpmVariableMessageService;
    private readonly BpmVariableSequenceFlowService _bpmVariableSequenceFlowService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly BpmVariableViewPageButtonService _bpmVariableViewPageButtonService;
    private readonly ILogger<BpmnInsertVariablesService> _logger;

    public BpmnInsertVariablesService(
        BpmVariableService bpmVariableService,
        BpmVariableViewPageButtonService bpmVariableViewPageButtonService,
        BpmVariableButtonService bpmVariableButtonService,
        BpmVariableSequenceFlowService bpmVariableSequenceFlowService,
        BpmVariableSignUpService bpmVariableSignUpService,
        BpmVariableMessageService bpmVariableMessageService,
        ILogger<BpmnInsertVariablesService> logger
    )
    {
        _bpmVariableService = bpmVariableService;
        _bpmVariableViewPageButtonService = bpmVariableViewPageButtonService;
        _bpmVariableButtonService = bpmVariableButtonService;
        _bpmVariableSequenceFlowService = bpmVariableSequenceFlowService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _bpmVariableMessageService = bpmVariableMessageService;
        _logger = logger;
    }

    public void InsertVariables(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        BpmVariable? bpmVariable = new()
        {
            BpmnCode = bpmnConfCommonVo.BpmnCode,
            ProcessNum = bpmnConfCommonVo.ProcessNum,
            ProcessName = bpmnConfCommonVo.ProcessName,
            ProcessDesc = bpmnConfCommonVo.ProcessDesc ?? "",
            ProcessStartConditions = JsonSerializer.Serialize(bpmnStartConditions),
            CreateUser = SecurityUtils.GetLogInEmpIdSafe(),
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };

        _bpmVariableService.baseRepo.Insert(bpmVariable);

        long variableId = bpmVariable.Id;

        InsertViewPageButton(bpmnConfCommonVo, variableId);

        Dictionary<string, List<BpmnConfCommonElementVo>>? signUpMultimap = new();
        List<BpmnConfCommonElementVo>? elementList = bpmnConfCommonVo.ElementList;

        foreach (BpmnConfCommonElementVo? elementVo in elementList)
        {
            int elementType = elementVo.ElementType;
            int elementProperty = elementVo.ElementProperty;

            if (elementType == ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code)
            {
                Type bpmnInsertVariableSubs = ElementPropertyEnum.GetVariableSubClassByCode(elementProperty);
                if (bpmnInsertVariableSubs != null)
                {
                    IBpmnInsertVariableSubs insertVariableSubsService =
                        (IBpmnInsertVariableSubs)ServiceProviderUtils.GetService(bpmnInsertVariableSubs);
                    insertVariableSubsService.InsertVariableSubs(elementVo, variableId);
                }

                if (elementVo.IsSignUp == 1)
                {
                    if (!signUpMultimap.ContainsKey(elementVo.ElementId))
                    {
                        signUpMultimap[elementVo.ElementId] = new List<BpmnConfCommonElementVo>();
                    }

                    signUpMultimap[elementVo.ElementId].Add(new BpmnConfCommonElementVo());
                }

                if (elementVo.IsSignUpSubElement == 1)
                {
                    if (!signUpMultimap.ContainsKey(elementVo.SignUpElementId))
                    {
                        signUpMultimap[elementVo.SignUpElementId] = new List<BpmnConfCommonElementVo>();
                    }

                    signUpMultimap[elementVo.SignUpElementId].Add(elementVo);
                }

                InsertElementButton(variableId, elementVo, elementVo.ElementId);
            }
            else if (elementType == ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code)
            {
                _bpmVariableSequenceFlowService.baseRepo.Insert(new BpmVariableSequenceFlow
                {
                    VariableId = variableId,
                    ElementId = elementVo.ElementId,
                    ElementName = elementVo.ElementName,
                    ElementFromId = elementVo.FlowFrom,
                    ElementToId = elementVo.FlowTo,
                    SequenceFlowType = 1,
                    CreateTime = DateTime.Now
                });
            }
        }

        InsertSignUp(variableId, signUpMultimap, elementList);
        _bpmVariableMessageService.InsertVariableMessage(variableId, bpmnConfCommonVo);
    }

    private void InsertSignUp(long variableId, Dictionary<string, List<BpmnConfCommonElementVo>> signUpMultimap,
        List<BpmnConfCommonElementVo> elementList)
    {
        if (signUpMultimap.Count == 0)
        {
            return;
        }

        List<BpmVariableSignUp>? bpmVariableSignUps = new();

        foreach (string? key in signUpMultimap.Keys)
        {
            BpmnConfCommonElementVo? elementVo =
                elementList.FirstOrDefault(o => o.ElementId == key) ?? new BpmnConfCommonElementVo();

            List<BpmnConfCommonElementVo>? subElements = signUpMultimap[key]
                .Where(o => o != null && !string.IsNullOrEmpty(o.ElementId))
                .ToList();

            bpmVariableSignUps.Add(new BpmVariableSignUp
            {
                VariableId = variableId,
                AfterSignUpWay = elementVo.AfterSignUpWay,
                ElementId = key,
                NodeId = elementVo.NodeId,
                Remark = "",
                SubElements = JsonSerializer.Serialize(subElements)
            });
        }

        _bpmVariableSignUpService.baseRepo.Insert(bpmVariableSignUps);
    }

    private void InsertElementButton(long variableId, BpmnConfCommonElementVo elementVo, string elementId)
    {
        if (elementVo.Buttons?.StartPage != null)
        {
            List<BpmVariableButton>? startPageButtons = elementVo.Buttons.StartPage.Select(o => new BpmVariableButton
            {
                VariableId = variableId,
                ElementId = elementId,
                ButtonPageType = (int)ButtonPageTypeEnum.INITIATE,
                ButtonType = o.ButtonType,
                ButtonName = o.ButtonName,
                CreateTime = DateTime.Now
            }).ToList();

            _bpmVariableButtonService.baseRepo.Insert(startPageButtons);
        }

        if (elementVo.Buttons?.ApprovalPage != null)
        {
            List<BpmVariableButton>? approvalPageButtons = elementVo.Buttons.ApprovalPage.Select(o =>
                new BpmVariableButton
                {
                    VariableId = variableId,
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.AUDIT,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName,
                    CreateTime = DateTime.Now
                }).ToList();

            _bpmVariableButtonService.baseRepo.Insert(approvalPageButtons);
        }
    }

    private void InsertViewPageButton(BpmnConfCommonVo bpmnConfCommonVo, long variableId)
    {
        if (bpmnConfCommonVo.ViewPageButtons?.ViewPageStart != null)
        {
            List<BpmVariableViewPageButton>? startPageButtons = bpmnConfCommonVo.ViewPageButtons.ViewPageStart
                .Select(o => new BpmVariableViewPageButton
                {
                    VariableId = variableId,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                }).ToList();

            _bpmVariableViewPageButtonService.baseRepo.Insert(startPageButtons);
        }

        if (bpmnConfCommonVo.ViewPageButtons?.ViewPageOther != null)
        {
            List<BpmVariableViewPageButton>? otherPageButtons = bpmnConfCommonVo.ViewPageButtons.ViewPageOther
                .Select(o => new BpmVariableViewPageButton
                {
                    VariableId = variableId,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                }).ToList();

            _bpmVariableViewPageButtonService.baseRepo.Insert(otherPageButtons);
        }
    }
}