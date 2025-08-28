using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;
using System.Diagnostics;
using System.Text.Json;

namespace AntFlow.Core.Service.Business;

public class ConfigFlowButtonContantService
{
    private readonly AFDeploymentService _afDeploymentService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly BpmBusinessProcessService _bpmbusinessProcessService;
    private readonly BpmVariableButtonService _bpmVariableButtonService;
    private readonly BpmVariableSignUpService _bpmVariableSignUpService;
    private readonly BpmVariableViewPageButtonService _bpmVariableViewPageButtonService;
    private readonly ILogger<ConfigFlowButtonContantService> _logger;

    public ConfigFlowButtonContantService(BpmBusinessProcessService bpmbusinessProcessService,
        BpmVariableButtonService bpmVariableButtonService,
        BpmVariableViewPageButtonService bpmVariableViewPageButtonService,
        AFDeploymentService afDeploymentService,
        AfTaskInstService afTaskInstService,
        BpmVariableSignUpService bpmVariableSignUpService,
        ILogger<ConfigFlowButtonContantService> logger)
    {
        _bpmbusinessProcessService = bpmbusinessProcessService;
        _bpmVariableButtonService = bpmVariableButtonService;
        _bpmVariableViewPageButtonService = bpmVariableViewPageButtonService;
        _afDeploymentService = afDeploymentService;
        _afTaskInstService = afTaskInstService;
        _bpmVariableSignUpService = bpmVariableSignUpService;
        _logger = logger;
    }

    public Dictionary<string, List<ProcessActionButtonVo>> GetButtons(string processNum, string elementId,
        bool? isJurisdiction, bool? isInitiate)
    {
        Dictionary<string, List<ProcessActionButtonVo>>? buttonMap = new();

        List<ProcessActionButtonVo> initiateButtons = new();
        List<ProcessActionButtonVo> auditButtons = new();
        List<ProcessActionButtonVo> toViewButtons = new();

        BpmBusinessProcess bpmBusinessProcess = _bpmbusinessProcessService.GetBpmBusinessProcess(processNum);

        if (bpmBusinessProcess == null || bpmBusinessProcess.ProcessState == null
                                       || bpmBusinessProcess.ProcessState ==
                                       (int)ProcessStateEnum.HANDLING_STATE) // 处理中状态
        {
            if (!string.IsNullOrEmpty(processNum) && !string.IsNullOrEmpty(elementId))
            {
                List<BpmVariableButton> bpmVariableButtons = _bpmVariableButtonService
                    .GetButtonsByProcessNumber(processNum, elementId);

                initiateButtons = GetButtons(bpmVariableButtons, ButtonPageTypeEnum.INITIATE);
                auditButtons = GetButtons(bpmVariableButtons, ButtonPageTypeEnum.AUDIT);
            }

            if (!string.IsNullOrEmpty(processNum))
            {
                List<BpmVariableViewPageButton> bpmVariableViewPageButtons = _bpmVariableViewPageButtonService
                    .GetButtonsByProcessNumber(processNum);

                toViewButtons = ToViewButtons(bpmVariableViewPageButtons, isInitiate.HasValue && isInitiate.Value);
            }

            if (isJurisdiction == true)
            {
                // 转办按钮
                ProcessActionButtonVo? change = new()
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_CHANGE_ASSIGNEE,
                    Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_CHANGE_ASSIGNEE),
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                };

                ProcessActionButtonVo? end = new()
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_STOP,
                    Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_STOP),
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                };

                toViewButtons.Add(end);
                toViewButtons.Add(change);
            }

            string? procInstId = bpmBusinessProcess?.ProcInstId ?? string.Empty;

            if (_bpmVariableSignUpService.IsMoreNode(processNum, elementId))
            {
                // 签收按钮
                ProcessActionButtonVo? undertake = new()
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_UNDERTAKE,
                    Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_UNDERTAKE),
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                };

                auditButtons.Clear();
                auditButtons.Add(undertake);
            }
        }
        else if (bpmBusinessProcess.ProcessState == (int)ProcessStateEnum.HANDLE_STATE
                 || bpmBusinessProcess.ProcessState == (int)ProcessStateEnum.REJECT_STATE
                 || bpmBusinessProcess.ProcessState == (int)ProcessStateEnum.END_STATE)
        {
            // 流程已结束或其他状态
            if (!string.IsNullOrEmpty(processNum))
            {
                List<BpmVariableViewPageButton>? bpmVariableViewPageButtons = _bpmVariableViewPageButtonService
                    .GetButtonsByProcessNumber(processNum);

                toViewButtons = ToViewButtons(bpmVariableViewPageButtons, isInitiate.HasValue && isInitiate.Value);

                // 过滤掉作废按钮
                List<ProcessActionButtonVo>? toViewButtonsComplete = toViewButtons
                    .Where(btn => btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_ABANDONED)
                    .ToList();

                initiateButtons.AddRange(toViewButtonsComplete);
                auditButtons.AddRange(toViewButtonsComplete);
                toViewButtons = toViewButtonsComplete;
            }
        }

        // 按钮去重并排序后返回
        buttonMap[ButtonPageTypeEnum.INITIATE.GetName()] = ButtonsSort(RepeatFilter(initiateButtons));
        buttonMap[ButtonPageTypeEnum.AUDIT.GetName()] = ButtonsSort(RepeatFilter(auditButtons));
        buttonMap[ButtonPageTypeEnum.TOVIEW.GetName()] = ButtonsSort(RepeatFilter(toViewButtons));

        return buttonMap;
    }

    private bool IsMoreNode(string procInstId, string elementId)
    {
        if (string.IsNullOrEmpty(elementId))
        {
            return false;
        }

        BpmAfTaskInst bpmAfTaskInst = _afTaskInstService.baseRepo.Where(a => a.ProcInstId == procInstId).First();
        BpmAfDeployment bpmAfDeployment =
            _afDeploymentService.baseRepo.Where(a => a.Id == bpmAfTaskInst.ProcDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procInstId} not found");
        }

        string content = bpmAfDeployment.Content;
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        if (elements == null || elements.Count == 0)
        {
            throw new AFBizException($"deployment with id {procInstId} not found");
        }

        BpmnConfCommonElementVo element = elements.Where(a => a.ElementId == elementId).FirstOrDefault();
        if (element == null)
        {
            throw new AFBizException($"element with id {elementId} not found");
        }

        return element.SignType == 2;
    }


    private List<ProcessActionButtonVo> GetButtons(List<BpmVariableButton> bpmVariableButtons,
        ButtonPageTypeEnum buttonPageTypeEnum)
    {
        List<ProcessActionButtonVo> buttonList = new();

        foreach (BpmVariableButton button in bpmVariableButtons)
        {
            // ???? ButtonPageTypeEnum ?? Code ?????????????????
            if (button.ButtonPageType == (int)buttonPageTypeEnum)
            {
                buttonList.Add(new ProcessActionButtonVo
                {
                    ButtonType = button.ButtonType,
                    Name = button.ButtonName,
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                });
            }
        }

        return buttonList;
    }

    private List<ProcessActionButtonVo> ToViewButtons(List<BpmVariableViewPageButton> btnVarList, bool isInitiate)
    {
        List<ProcessActionButtonVo> buttonList = new();

        foreach (BpmVariableViewPageButton? item in btnVarList)
        {
            if (isInitiate)
            {
                if (item.ViewType == 1)
                {
                    buttonList.Add(new ProcessActionButtonVo
                    {
                        ButtonType = item.ButtonType,
                        Name = item.ButtonName,
                        Show = ProcessButtonEnum.VIEW_TYPE.Code,
                        Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                    });
                }
            }
            else
            {
                if (item.ViewType == 2)
                {
                    buttonList.Add(new ProcessActionButtonVo
                    {
                        ButtonType = item.ButtonType,
                        Name = item.ButtonName,
                        Show = ProcessButtonEnum.VIEW_TYPE.Code,
                        Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                    });
                }
            }
        }

        return buttonList;
    }

    private List<ProcessActionButtonVo> ButtonsSort(List<ProcessActionButtonVo> buttons)
    {
        buttons.Sort((o1, o2) =>
        {
            ConfigFlowButtonSortEnum? sort1 = ConfigFlowButtonSortEnum.GetEnumByCode(o1.ButtonType);
            ConfigFlowButtonSortEnum? sort2 = ConfigFlowButtonSortEnum.GetEnumByCode(o2.ButtonType);

            Debug.Assert(sort1 != null, "sort1 should not be null");
            Debug.Assert(sort2 != null, "sort2 should not be null");

            return sort1.Sort - sort2.Sort;
        });

        return buttons;
    }

    private List<ProcessActionButtonVo> RepeatFilter(List<ProcessActionButtonVo> initiateButtons)
    {
        if (initiateButtons == null || !initiateButtons.Any())
        {
            return new List<ProcessActionButtonVo>();
        }

        List<ProcessActionButtonVo>? lists = initiateButtons
            .DistinctBy(a => a.ButtonType)
            .ToList();

        return lists;
    }
}