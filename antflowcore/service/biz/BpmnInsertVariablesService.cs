using System.Text.Json;
using System.Text.Json.Serialization;
using antflowcore.adaptor.variable;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

 public class BpmnInsertVariablesService
    {
        private readonly BpmVariableService _bpmVariableService;
        private readonly BpmVariableViewPageButtonService _bpmVariableViewPageButtonService;
        private readonly BpmVariableButtonService _bpmVariableButtonService;
        private readonly BpmVariableSequenceFlowService _bpmVariableSequenceFlowService;
        private readonly BpmVariableSignUpService _bpmVariableSignUpService;
        private readonly BpmVariableMessageService _bpmVariableMessageService;
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
            var bpmVariable = new BpmVariable
            {
                BpmnCode = bpmnConfCommonVo.BpmnCode,
                ProcessNum = bpmnConfCommonVo.ProcessNum,
                ProcessName = bpmnConfCommonVo.ProcessName,
                ProcessDesc = bpmnConfCommonVo.ProcessDesc??"",
                ProcessStartConditions = JsonSerializer.Serialize(bpmnStartConditions),
                CreateUser = SecurityUtils.GetLogInEmpIdSafe(),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _bpmVariableService.baseRepo.Insert(bpmVariable);

            var variableId = bpmVariable.Id;

            InsertViewPageButton(bpmnConfCommonVo, variableId);

            var signUpMultimap = new Dictionary<string, List<BpmnConfCommonElementVo>>();
            var elementList = bpmnConfCommonVo.ElementList;

            foreach (var elementVo in elementList)
            {
                int elementType = elementVo.ElementType;
                int elementProperty = elementVo.ElementProperty;
                
                if (elementType == ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code)
                {
                    Type bpmnInsertVariableSubs = ElementPropertyEnum.GetVariableSubClassByCode(elementProperty);
                    if (bpmnInsertVariableSubs != null)
                    {
                        IBpmnInsertVariableSubs insertVariableSubsService = (IBpmnInsertVariableSubs)ServiceProviderUtils.GetService(bpmnInsertVariableSubs);
                        insertVariableSubsService.InsertVariableSubs(elementVo, variableId);
                    }

                    if (elementVo.IsSignUp == 1)
                    {
                        if (!signUpMultimap.ContainsKey(elementVo.ElementId))
                            signUpMultimap[elementVo.ElementId] = new List<BpmnConfCommonElementVo>();
                        signUpMultimap[elementVo.ElementId].Add(new BpmnConfCommonElementVo());
                    }

                    if (elementVo.IsSignUpSubElement == 1)
                    {
                        if (!signUpMultimap.ContainsKey(elementVo.SignUpElementId))
                            signUpMultimap[elementVo.SignUpElementId] = new List<BpmnConfCommonElementVo>();
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
                        CreateTime = DateTime.Now,
                    });
                }
            }

            InsertSignUp(variableId, signUpMultimap, elementList);
            _bpmVariableMessageService.InsertVariableMessage(variableId, bpmnConfCommonVo);
        }

        private void InsertSignUp(long variableId, Dictionary<string, List<BpmnConfCommonElementVo>> signUpMultimap, List<BpmnConfCommonElementVo> elementList)
        {
            if (signUpMultimap.Count == 0) return;

            var bpmVariableSignUps = new List<BpmVariableSignUp>();

            foreach (var key in signUpMultimap.Keys)
            {
                var elementVo = elementList.FirstOrDefault(o => o.ElementId == key) ?? new BpmnConfCommonElementVo();

                var subElements = signUpMultimap[key]
                    .Where(o => o != null && !string.IsNullOrEmpty(o.ElementId))
                    .ToList();

                bpmVariableSignUps.Add(new BpmVariableSignUp
                {
                    VariableId = variableId,
                    AfterSignUpWay = elementVo.AfterSignUpWay,
                    ElementId = key,
                    NodeId = elementVo.NodeId,
                    Remark = "",
                    SubElements = JsonSerializer.Serialize(subElements),
                    CreateTime = DateTime.Now,
                });
            }

            _bpmVariableSignUpService.baseRepo.Insert(bpmVariableSignUps);
        }

        private void InsertElementButton(long variableId, BpmnConfCommonElementVo elementVo, string elementId)
        {
            if (elementVo.Buttons?.StartPage != null)
            {
                var startPageButtons = elementVo.Buttons.StartPage.Select(o => new BpmVariableButton
                {
                    VariableId = variableId,
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.INITIATE,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName,
                    CreateTime = DateTime.Now,
                }).ToList();

                _bpmVariableButtonService.baseRepo.Insert(startPageButtons);
            }

            if (elementVo.Buttons?.ApprovalPage != null)
            {
                var approvalPageButtons = elementVo.Buttons.ApprovalPage.Select(o => new BpmVariableButton
                {
                    VariableId = variableId,
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.AUDIT,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName,
                    CreateTime = DateTime.Now,
                }).ToList();

                _bpmVariableButtonService.baseRepo.Insert(approvalPageButtons);
            }
        }

        private void InsertViewPageButton(BpmnConfCommonVo bpmnConfCommonVo, long variableId)
        {
            if (bpmnConfCommonVo.ViewPageButtons?.ViewPageStart != null)
            {
                var startPageButtons = bpmnConfCommonVo.ViewPageButtons.ViewPageStart.Select(o => new BpmVariableViewPageButton
                {
                    VariableId = variableId,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName,
                    Remark = StringConstants.BIG_WHITE_BLANK,
                }).ToList();

                _bpmVariableViewPageButtonService.baseRepo.Insert(startPageButtons);
            }

            if (bpmnConfCommonVo.ViewPageButtons?.ViewPageOther != null)
            {
                var otherPageButtons = bpmnConfCommonVo.ViewPageButtons.ViewPageOther.Select(o => new BpmVariableViewPageButton
                {
                    VariableId = variableId,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName,
                    Remark = StringConstants.BIG_WHITE_BLANK,
                }).ToList();

                _bpmVariableViewPageButtonService.baseRepo.Insert(otherPageButtons);
            }
        }
    }