using System.Diagnostics;
using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

public class ConfigFlowButtonContantService : IConfigFlowButtonContantService
{
    private readonly IBpmBusinessProcessService _bpmbusinessProcessService;
    private readonly IAFDeploymentService _afDeploymentService;
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IBpmvariableBizService _bpmvariableBizService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly ILogger<ConfigFlowButtonContantService> _logger;

    public ConfigFlowButtonContantService(IBpmBusinessProcessService bpmbusinessProcessService,
       IAFDeploymentService afDeploymentService,
       IAfTaskInstService afTaskInstService,
        BpmvariableBizService bpmvariableBizService,
        IBpmVariableService bpmVariableService,
        IBpmnNodeService bpmnNodeService,
        IBpmnConfService bpmnConfService,
        ILogger<ConfigFlowButtonContantService> logger)
    {
        _bpmbusinessProcessService = bpmbusinessProcessService;
        _afDeploymentService = afDeploymentService;
        _afTaskInstService = afTaskInstService;
        _bpmvariableBizService = bpmvariableBizService;
        _bpmVariableService = bpmVariableService;
        _bpmnNodeService = bpmnNodeService;
        _bpmnConfService = bpmnConfService;
        _logger = logger;
    }
    public Dictionary<string, List<ProcessActionButtonVo>> GetButtons(string processNum, string elementId,List<String> viewNodeIds,
        bool? isJurisdiction, bool? isInitiate)
    {
        var buttonMap = new Dictionary<string, List<ProcessActionButtonVo>>();

        List<ProcessActionButtonVo> initiateButtons = new List<ProcessActionButtonVo>();
        List<ProcessActionButtonVo> auditButtons = new List<ProcessActionButtonVo>();
        List<ProcessActionButtonVo> toViewButtons = new List<ProcessActionButtonVo>();

        BpmBusinessProcess bpmBusinessProcess = _bpmbusinessProcessService.GetBpmBusinessProcess(processNum);

        if (bpmBusinessProcess == null || bpmBusinessProcess.ProcessState == null
                                       || bpmBusinessProcess.ProcessState ==
                                       (int)ProcessStateEnum.HANDLING_STATE) // 审批中
        {
            if (!string.IsNullOrEmpty(processNum) && !string.IsNullOrEmpty(elementId))
            {
                // Read buttons from variable_config_json
                var variableConfig = GetVariableConfig(processNum);
                List<VariableButtonItem>? buttons = variableConfig?.Buttons;

                initiateButtons = GetButtonsFromJson(buttons, elementId, ButtonPageTypeEnum.INITIATE);
                auditButtons = GetButtonsFromJson(buttons, elementId, ButtonPageTypeEnum.AUDIT);
            }

            if (!string.IsNullOrEmpty(processNum))
            {
                if(!viewNodeIds.IsEmpty()){
                    // Read to-view buttons from variable_config_json
                    var variableConfig = GetVariableConfig(processNum);
                    List<VariableButtonItem>? buttons = variableConfig?.Buttons;
                    toViewButtons = GetButtonsFromJsonByNodeIds(buttons, viewNodeIds, ButtonPageTypeEnum.TOVIEW);
                }
                
                // Read global view page buttons from variable_config_json (ViewType-based buttons)
                var globalViewConfig = GetVariableConfig(processNum);
                List<ProcessActionButtonVo> globalViewButtons = ToViewButtonsFromJson(globalViewConfig?.Buttons, isInitiate.HasValue && isInitiate.Value);
                if (!globalViewButtons.IsEmpty())
                {
                    toViewButtons.AddRange(globalViewButtons);
                }
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

            if (IsMoreNode(processNum, elementId))
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
                // Read global view page buttons from variable_config_json
                var variableConfig = GetVariableConfig(processNum);
                toViewButtons = ToViewButtonsFromJson(variableConfig?.Buttons, isInitiate.HasValue && isInitiate.Value);
                List<ProcessActionButtonVo> nodeConfButtons = getNodeConfButtons(bpmBusinessProcess,isInitiate.HasValue&&isInitiate.Value);
                if (!nodeConfButtons.IsEmpty())
                {
                    toViewButtons=nodeConfButtons;
                }
                // 过滤无效按钮
                var toViewButtonsComplete = toViewButtons
                    .Where(btn => btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_ABANDONED
                    && btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_PROCESS_DRAW_BACK
                    && btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT
                    && btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT
                    && btn.ButtonType != (int)ButtonTypeEnum.BUTTON_TYPE_STOP)
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

    private VariableConfigJson? GetVariableConfig(string processNum)
    {
        var bpmVariable = _bpmVariableService._repository.GetQueryable()
            .Where(a => a.ProcessNum == processNum)
            .First();
        return JsonConfUtil.ParseVariableConfig(bpmVariable?.VariableConfigJson);
    }

    private NodeConfigResult? GetNodeConfig(long? nodeId)
    {
        if (nodeId == null) return null;
        var bpmnNode = _bpmnNodeService._repository.GetQueryable()
            .Where(a => a.Id == nodeId.Value)
            .First();
        return new NodeConfigResult
        {
            NodeId = nodeId.Value,
            Config = JsonConfUtil.ParseNodeConfig(bpmnNode?.NodeConfigJson)
        };
    }

    private class NodeConfigResult
    {
        public long NodeId { get; set; }
        public BpmnNodeConfigJson? Config { get; set; }
    }

    private bool IsMoreNode(string processNum, string elementId)
    {
        if (string.IsNullOrEmpty(elementId))
        {
            return false;
        }
        // 查询 multiplayer LEFT JOIN personnel,过滤 undertake_status==0 的记录,
        // 未被承办的人数 > 1 且 signType==2(或签)时返回 true。
        // 承办后所有 personnel 的 undertake_status 被置为 1,过滤后为空,返回 false,不再显示承办按钮。
        return _bpmvariableBizService.IsMoreNode(processNum, elementId);
    }


    private List<ProcessActionButtonVo> GetButtonsFromJson(List<VariableButtonItem>? buttons, string elementId, ButtonPageTypeEnum buttonPageTypeEnum)
    {
        List<ProcessActionButtonVo> buttonList = new List<ProcessActionButtonVo>();
    
        if (buttons == null) return buttonList;

        foreach (VariableButtonItem button in buttons)
        {
            if (button.ButtonPageType == (int)buttonPageTypeEnum && button.ElementId == elementId)
            {
                buttonList.Add(new ProcessActionButtonVo
                {
                    ButtonType = button.ButtonType ?? 0,
                    Name = button.ButtonName,
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                });
            }
        }
    
        return buttonList;
    }

    private List<ProcessActionButtonVo> GetButtonsFromJsonByNodeIds(List<VariableButtonItem>? buttons, List<string> elementIds, ButtonPageTypeEnum buttonPageTypeEnum)
    {
        List<ProcessActionButtonVo> buttonList = new List<ProcessActionButtonVo>();
    
        if (buttons == null) return buttonList;

        foreach (VariableButtonItem button in buttons)
        {
            if (button.ButtonPageType == (int)buttonPageTypeEnum && !string.IsNullOrEmpty(button.ElementId) && elementIds.Contains(button.ElementId))
            {
                buttonList.Add(new ProcessActionButtonVo
                {
                    ButtonType = button.ButtonType ?? 0,
                    Name = button.ButtonName,
                    Show = ProcessButtonEnum.DEAL_WITH_TYPE.Code,
                    Type = ProcessButtonEnum.DEFAULT_COLOR.Desc
                });
            }
        }
    
        return buttonList;
    }

    private List<ProcessActionButtonVo> ToViewButtonsFromJson(List<VariableButtonItem>? btnVarList, bool isInitiate)
    {
        List<ProcessActionButtonVo> buttonList = new List<ProcessActionButtonVo>();
    
        if (btnVarList == null) return buttonList;

        foreach (var item in btnVarList)
        {
            if (item.ViewType == null) continue;

            if (isInitiate)
            {
                if (item.ViewType == 1)
                {
                    buttonList.Add(new ProcessActionButtonVo
                    {
                        ButtonType = item.ButtonType ?? 0,
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
                        ButtonType = item.ButtonType ?? 0,
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
            ConfigFlowButtonSortEnum sort1 = ConfigFlowButtonSortEnum.GetEnumByCode(o1.ButtonType);
            ConfigFlowButtonSortEnum sort2 = ConfigFlowButtonSortEnum.GetEnumByCode(o2.ButtonType);

            sort1 ??= ConfigFlowButtonSortEnum.NOTHING;
            sort2 ??= ConfigFlowButtonSortEnum.NOTHING;
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
      private List<ProcessActionButtonVo> getNodeConfButtons(BpmBusinessProcess bpmBusinessProcess,Boolean isInitiate){
        List<ButtonSignButtonConf>? buttonConfList = null;
        if(isInitiate){
            // For initiator page, read from node_config_json of all nodes with matching bpmnCode
            var bpmnConf = _bpmnConfService._repository.GetQueryable()
                .Where(a => a.BpmnCode == bpmBusinessProcess.Version && a.EffectiveStatus == 1)
                .First();
            if (bpmnConf == null)
            {
                return new List<ProcessActionButtonVo>();
            }
            var bpmnNodes = _bpmnNodeService._repository.GetQueryable()
                .Where(a => a.ConfId == bpmnConf.Id && a.IsDel == 0 && a.NodeConfigJson != null)
                .ToList();
            foreach (var node in bpmnNodes)
            {
                var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
                var confs = nodeConfig?.ButtonSignConf?.ButtonConfList;
                if (confs != null && confs.Any())
                {
                    buttonConfList = buttonConfList ?? new List<ButtonSignButtonConf>();
                    buttonConfList.AddRange(confs);
                }
            }
        }else
        {
            List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService
                ._repository
                .Find(a=>a.ProcInstId==bpmBusinessProcess.ProcInstId);

            List<String> hisTaskDefKeys = bpmAfTaskInsts
                .Where(a => a.EndTime.HasValue && SecurityUtils.GetLogInEmpIdSafe() == a.Assignee)
                .Select(a => a.TaskDefKey).ToList();
           
            if(!hisTaskDefKeys.IsEmpty())
            {
                List<String> nodeIdsByElementIds = _bpmvariableBizService.GetNodeIdByElementIds(bpmBusinessProcess.BusinessNumber, hisTaskDefKeys);
                    
                if (!nodeIdsByElementIds.IsEmpty())
                {
                    // Read from node_config_json
                    foreach (var nodeIdStr in nodeIdsByElementIds)
                    {
                        if (long.TryParse(nodeIdStr, out long nodeId))
                        {
                            var nodeConfigResult = GetNodeConfig(nodeId);
                            var confs = nodeConfigResult?.Config?.ButtonSignConf?.ButtonConfList
                                ?.Where(a => a.ButtonPageType == (int)ButtonPageTypeEnum.TOVIEW)
                                .ToList();
                            if (confs != null && confs.Any())
                            {
                                buttonConfList = buttonConfList ?? new List<ButtonSignButtonConf>();
                                buttonConfList.AddRange(confs);
                            }
                        }
                    }
                }
                //只能显示在发起人页的按钮不应显示在其它页面
                if(isInitiate&&!buttonConfList.IsEmpty()){
                    buttonConfList = buttonConfList.Where(a=>a.ButtonPageType==(int)ButtonPageTypeEnum.INITIATE).ToList();
                }
            }

        }

        if(!buttonConfList.IsEmpty())
        {
            List<ProcessActionButtonVo> processActionButtonVos =
                buttonConfList
                    .Select(item => new ProcessActionButtonVo
                    {
                        ButtonType = item.ButtonType ?? 0,
                        Name = item.ButtonName,
                        Show = ProcessButtonEnum.VIEW_TYPE.Code,
                    })
                    .ToList();
            
            return processActionButtonVos;
        }
       return new List<ProcessActionButtonVo>();
    }
}
