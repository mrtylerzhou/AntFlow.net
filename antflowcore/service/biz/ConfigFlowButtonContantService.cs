using System.Diagnostics;
using System.Text.Json;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ConfigFlowButtonContantService
{
    private readonly BpmBusinessProcessService _bpmbusinessProcessService;
    private readonly BpmVariableButtonService _bpmVariableButtonService;
    private readonly BpmVariableViewPageButtonService _bpmVariableViewPageButtonService;
    private readonly AFDeploymentService _afDeploymentService;
    private readonly AfTaskInstService _afTaskInstService;
    private readonly ILogger<ConfigFlowButtonContantService> _logger;

    public ConfigFlowButtonContantService(BpmBusinessProcessService bpmbusinessProcessService,
        BpmVariableButtonService bpmVariableButtonService,
        BpmVariableViewPageButtonService bpmVariableViewPageButtonService,
        AFDeploymentService afDeploymentService,
        AfTaskInstService afTaskInstService,
        ILogger<ConfigFlowButtonContantService> logger)
    {
        _bpmbusinessProcessService = bpmbusinessProcessService;
        _bpmVariableButtonService = bpmVariableButtonService;
        _bpmVariableViewPageButtonService = bpmVariableViewPageButtonService;
        _afDeploymentService = afDeploymentService;
        _afTaskInstService = afTaskInstService;
        _logger = logger;
    }
    public Dictionary<string, List<ProcessActionButtonVo>> GetButtons(string processNum, string elementId,
        bool? isJurisdiction, bool? isInitiate)
    {
        var buttonMap = new Dictionary<string, List<ProcessActionButtonVo>>();

        var initiateButtons = new List<ProcessActionButtonVo>();
        var auditButtons = new List<ProcessActionButtonVo>();
        var toViewButtons = new List<ProcessActionButtonVo>();

        var bpmBusinessProcess = _bpmbusinessProcessService.GetBpmBusinessProcess(processNum);

        if (bpmBusinessProcess == null || bpmBusinessProcess.ProcessState == null
                                       || bpmBusinessProcess.ProcessState ==
                                       (int)ProcessStateEnum.HANDLING_STATE) // 审批中
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
                // 添加监控权限按钮
                var change = new ProcessActionButtonVo
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_CHANGE_ASSIGNEE,
                    Name = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_CHANGE_ASSIGNEE),
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                };

                var end = new ProcessActionButtonVo
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_STOP,
                    Name =ButtonTypeEnumExtensions.GetDescByCode((int) ButtonTypeEnum.BUTTON_TYPE_STOP),
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                };

                toViewButtons.Add(end);
                toViewButtons.Add(change);
            }

            var procInstId = bpmBusinessProcess?.ProcInstId ?? string.Empty;

            if (IsMoreNode(procInstId, elementId))
            {
                // 添加承办按钮
                var undertake = new ProcessActionButtonVo
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_UNDERTAKE,
                    Name =ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_UNDERTAKE),
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
            // 流程完成状态处理
            if (!string.IsNullOrEmpty(processNum))
            {
                var bpmVariableViewPageButtons = _bpmVariableViewPageButtonService
                    .GetButtonsByProcessNumber(processNum);

                toViewButtons = ToViewButtons(bpmVariableViewPageButtons, isInitiate.HasValue && isInitiate.Value);

                // 过滤无效按钮
                var toViewButtonsComplete = toViewButtons
                    .Where(btn => btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_ABANDONED)
                    .ToList();

                initiateButtons.AddRange(toViewButtonsComplete);
                auditButtons.AddRange(toViewButtonsComplete);
                toViewButtons = toViewButtonsComplete;
            }
        }

        // 添加处理后的按钮列表到字典
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
        BpmAfTaskInst bpmAfTaskInst = _afTaskInstService.baseRepo.Where(a=>a.ProcInstId==procInstId).First();
        BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a=>a.Id==bpmAfTaskInst.ProcDefId).First();
        if (bpmAfDeployment == null)
        {
            throw new ApplicationException($"deployment with id {procInstId} not found");
        }
        string content=bpmAfDeployment.Content;
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


    private List<ProcessActionButtonVo> GetButtons(List<BpmVariableButton> bpmVariableButtons, ButtonPageTypeEnum buttonPageTypeEnum)
    {
        List<ProcessActionButtonVo> buttonList = new List<ProcessActionButtonVo>();
    
        foreach (BpmVariableButton button in bpmVariableButtons)
        {
            // 假设 ButtonPageTypeEnum 的 Code 属性对应按钮页面类型值
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
        List<ProcessActionButtonVo> buttonList = new List<ProcessActionButtonVo>();
    
        foreach (var item in btnVarList)
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
            var sort1 = ConfigFlowButtonSortEnum.GetEnumByCode(o1.ButtonType);
            var sort2 = ConfigFlowButtonSortEnum.GetEnumByCode(o2.ButtonType);
            
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
 
        var lists = initiateButtons
            .DistinctBy(a=>a.ButtonType)
            .ToList();
 
        return lists;
    }
}