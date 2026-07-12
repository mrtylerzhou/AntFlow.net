using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service;
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

public class ProcessConstantsService : IProcessConstantsService
{
    private readonly IAfTaskInstService _afTaskInstService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly IUserMessageService _userMessageService;
    private readonly IBpmProcessForwardService _processForwardService;
    private readonly IAFTaskService _afTaskService;
    private readonly ILogger<ProcessConstantsService> _logger;

    public ProcessConstantsService(IAfTaskInstService afTaskInstService, 
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnEmployeeInfoProviderService employeeInfoProviderService,
        TaskMgmtService taskMgmtService,
        IUserMessageService userMessageService,
        IBpmProcessForwardService processForwardService,
        IAFTaskService afTaskService,
        ILogger<ProcessConstantsService> logger)
    {
        _afTaskInstService = afTaskInstService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _employeeInfoProviderService = employeeInfoProviderService;
        _taskMgmtService = taskMgmtService;
        _userMessageService = userMessageService;
        _processForwardService = processForwardService;
        _afTaskService = afTaskService;
        _logger = logger;
    }

    public BpmAfTaskInst GetPrevTask(String taskDefKey, String procInstId)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            throw new ArgumentNullException(nameof(taskDefKey));
        }

        if (string.IsNullOrEmpty(procInstId))
        {
            throw new ArgumentNullException(nameof(procInstId));
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService._repository
            .Find(a => a.ProcInstId == procInstId && a.TaskDefKey == taskDefKey&&a.EndTime!=null);
        BpmAfTaskInst? bpmAfTaskInst = bpmAfTaskInsts.Where(a => a.TaskDefKey != taskDefKey).FirstOrDefault();
        return bpmAfTaskInst;
    }

    public ProcessRecordInfoVo ProcessInfo(BpmBusinessProcess bpmBusinessProcess)
    {
        var processInfoVo = new ProcessRecordInfoVo();
        if (bpmBusinessProcess == null)
        {
            return processInfoVo;
        }

        // 检查权限
        if (!ShowProcessData(bpmBusinessProcess.BusinessNumber))
        {
            throw new AFBizException("00", "current user has no access right！");
        }

        // 设置任务状态
        processInfoVo.TaskState = ProcessStateEnumExtensions.GetDescByCode(bpmBusinessProcess.ProcessState);

        // 设置审核信息
        processInfoVo.VerifyInfoList = ServiceProviderUtils.GetService<IBpmVerifyInfoService>().VerifyInfoList(bpmBusinessProcess.BusinessNumber,bpmBusinessProcess.ProcInstId);

        // 设置流程描述
        processInfoVo.ProcessTitle = bpmBusinessProcess.Description;

        // 获取员工信息
        var employee = _employeeInfoProviderService.QryLiteEmployeeInfoById(bpmBusinessProcess.CreateUser);
        processInfoVo.Employee = employee;
        processInfoVo.CreateTime = bpmBusinessProcess.CreateTime;

        // 设置发起人 ID
        processInfoVo.StartUserId = bpmBusinessProcess.CreateUser;

        // 设置流程编号
        processInfoVo.ProcessNumber = bpmBusinessProcess.BusinessNumber;

        string processInstanceId = bpmBusinessProcess.ProcInstId;

        // 修改转发数据
        _processForwardService.UpdateProcessForward(new BpmProcessForward
        {
            ProcessInstanceId = processInstanceId,
            ForwardUserId = SecurityUtils.GetLogInEmpIdStr()
        });

        // 修改通知
        _userMessageService.ReadNode(processInstanceId);

        // 查询当前用户的任务
        var tasks = _afTaskService
            ._repository
            .Find(a=>a.ProcInstId== processInstanceId);
        string taskDefKey = "";
        List<String> viewNodeIds=null;
        if (tasks.Any())
        {
            List<BpmAfTask> currentAssigneeTasks = tasks.Where(a => a.Assignee == SecurityUtils.GetLogInEmpIdStr()).ToList();
           if (!currentAssigneeTasks.IsEmpty())
           {
               taskDefKey=currentAssigneeTasks.First().TaskDefKey;
               viewNodeIds=currentAssigneeTasks.Select(a=>a.TaskDefKey).ToList();
           }
           else
           {
               viewNodeIds=tasks.Select(a=>a.TaskDefKey).ToList();
           }
           
            processInfoVo.TaskId = tasks[0].Id;
            processInfoVo.ViewNodeIds=viewNodeIds;
            processInfoVo.NodeId = taskDefKey;
        }
        else if (bpmBusinessProcess.IsLowCodeFlow == 1)
        {
            List<BpmAfTaskInst> historicTasks = _afTaskInstService
                ._repository
                .Find(a => a.ProcInstId == processInstanceId && a.Assignee == SecurityUtils.GetLogInEmpId());

            if (historicTasks.Any())
            {
                taskDefKey = historicTasks.First().TaskDefKey;
            }
        }

        if (!string.IsNullOrEmpty(taskDefKey) && bpmBusinessProcess.IsLowCodeFlow == 1)
        {
            // 读取节点级低代码表单配置: 整表隐藏(formHidden) + 字段级权限(fieldControls)
            // taskDefKey 是 ACT 任务定义 key,需经 BpmVariableMultiplayer 解析为 BpmnNode 主键 id 后再读取节点配置
            try
            {
                var bpmVariableMultiplayerService = ServiceProviderUtils.GetService<IBpmVariableMultiplayerService>();
                var bpmnNodeService = ServiceProviderUtils.GetService<IBpmnNodeService>();
                if (bpmVariableMultiplayerService != null && bpmnNodeService != null)
                {
                    List<BpmVariableMultiplayer> multiplayers =
                        bpmVariableMultiplayerService._repository.QueryMultiplayersByProcessNumAndElementId(
                            bpmBusinessProcess.BusinessNumber, taskDefKey) ?? new List<BpmVariableMultiplayer>();
                    long nodeId = multiplayers
                        .Where(a => !string.IsNullOrEmpty(a.NodeId) && long.TryParse(a.NodeId, out _))
                        .Select(a => long.Parse(a.NodeId!))
                        .FirstOrDefault();
                    if (nodeId > 0)
                    {
                        BpmnNode? node = bpmnNodeService._repository
                            .FirstOrDefault(a => a.Id == nodeId && a.IsDel == 0);
                        if (node != null && !string.IsNullOrEmpty(node.NodeConfigJson))
                        {
                            BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
                            BpmnNodeLowCodeConfJson? lowCodeConf = nodeConfig?.LowCodeConf;
                            if (lowCodeConf != null)
                            {
                                if (lowCodeConf.FormHidden != null && lowCodeConf.FormHidden.Count > 0)
                                {
                                    processInfoVo.FormHidden = lowCodeConf.FormHidden;
                                }
                                if (lowCodeConf.FieldControls != null && lowCodeConf.FieldControls.Count > 0)
                                {
                                    processInfoVo.LfFieldControlVOs = lowCodeConf.FieldControls;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load low-code form config for process:{BusinessNumber}, node:{TaskDefKey}",
                    bpmBusinessProcess.BusinessNumber, taskDefKey);
            }
        }

        return processInfoVo;
    }
    public bool ShowProcessData(string processCode)
    {
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processCode);
    
        // 监控、查看、流程管理员、超级管理员、历史审批人和转发用户
        if (bpmBusinessProcess != null)
        {
            List<BpmAfTaskInst> taskInstanceList = _afTaskInstService
                ._repository
                .Find(a => a.ProcInstId == bpmBusinessProcess.ProcInstId);
                
            

            var assigneeList = taskInstanceList
                .Where(task => task != null)
                .Select(task => task.Assignee)
                .ToList();

            if (assigneeList.Contains(SecurityUtils.GetLogInEmpIdStr()))
            {
                return true;
            }

            // TODO: 重新设计逻辑
            return true;
        }

        return true;
    }

}