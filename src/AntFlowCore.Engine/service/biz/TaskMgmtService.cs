using System.Collections;
using System.Reflection;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class TaskMgmtService : ITaskMgmtService
{
    private readonly IAFTaskService _taskService;
    private readonly IAfTaskInstService _taskInstService;
    private readonly IBpmProcessNoticeService _bpmProcessNoticeService;
    private readonly IAFExecutionService _executionService;
    private readonly IBpmnConfService _bpmnConfService;
    private IEnumerable services = ServiceProviderUtils.GetServicesByOpenGenericType(typeof(IFormOperationAdaptor<>));
    public TaskMgmtService(
        IAFTaskService taskService,
        IAfTaskInstService taskInstService,
        IBpmProcessNoticeService bpmProcessNoticeService,
        IAFExecutionService executionService,
        IBpmnConfService bpmnConfService
        )
    {
        _taskService = taskService;
        _taskInstService = taskInstService;
        _bpmProcessNoticeService = bpmProcessNoticeService;
        _executionService = executionService;
        _bpmnConfService = bpmnConfService;
    }

    /// <summary>
    /// modify current node's history assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTaskInst(TaskMgmtVO taskMgmtVO)
    {
        int count = _taskInstService
            ._repository
            .UpdateTaskInstAssignee(taskMgmtVO.TaskId, taskMgmtVO.ApplyUser, taskMgmtVO.ApplyUserName, "变更处理人", SecurityUtils.GetLogInEmpId());
        return count;
    }

    /// <summary>
    /// modify current assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTask(TaskMgmtVO taskMgmtVO)
    {
        int updateAssignee = _taskService
            ._repository
            .UpdateAssignee(taskMgmtVO.TaskId, taskMgmtVO.ApplyUser, taskMgmtVO.ApplyUserName);

        return updateAssignee;
    }

    public List<BpmAfTask> GetAgencyList(string taskId, int code, string taskProcInstId)
    {
        IEnumerable<string> taskDefKeys = _taskService._repository.Find(a=>a.Id==taskId).Select(a=>a.TaskDefKey);
        List<BpmAfTask> bpmAfTasks = _taskService._repository.Find(a=>taskDefKeys.Contains(a.TaskDefKey)&&a.ProcInstId==taskProcInstId);
        List<BpmAfTask> afTasks = bpmAfTasks.Where(a=>a.Id!=taskId).ToList();
        return afTasks;
    }

    public void DeleteTask(string taskId)
    {
       _taskService._repository.DeleteByExpression(a=>a.Id==taskId);
    }

    public List<DIYProcessInfoDTO> ViewProcessInfo(string desc = "")
    {
        List<DIYProcessInfoDTO> diyProcessInfoDTOS = BaseFormInfo(desc);
        if (diyProcessInfoDTOS == null || diyProcessInfoDTOS.Count == 0)
        {
            return new List<DIYProcessInfoDTO>();
        }

        List<string> formCodes = diyProcessInfoDTOS.Select(dto => dto.Key).ToList();

        var bpmnConfs = _bpmnConfService._repository
            .Find(b => formCodes.Contains(b.FormCode) && b.EffectiveStatus == 1)
            .Select(b => new { b.FormCode, b.ExtraFlags })
            .ToList();

        if (bpmnConfs.Count > 0)
        {
            Dictionary<string, int?> formCode2Flags = bpmnConfs.ToDictionary(b => b.FormCode, x => x.ExtraFlags,StringComparer.Ordinal);
            IDictionary<String, List<BpmProcessNotice>> processNoticeMap = _bpmProcessNoticeService.ProcessNoticeMap(formCodes);
            foreach (var diyProcessInfoDTO in diyProcessInfoDTOS)
            {
                if (formCode2Flags.TryGetValue(diyProcessInfoDTO.Key, out int? flags))
                {
                    bool hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                    diyProcessInfoDTO.HasStarUserChooseModule = hasStartUserChooseModules;
                }

                string formCode = diyProcessInfoDTO.Key;
                if (processNoticeMap.TryGetValue(formCode, out var bpmProcessNotices) && bpmProcessNotices.Any())
                {
                    var processNotices = new List<BaseNumIdStruVo>();
                    foreach (ProcessNoticeEnum processNoticeEnum in ProcessNoticeEnum.Values)
                    {
                        var type = processNoticeEnum.Code;
                        var descByCode = processNoticeEnum.Desc;

                        var struVo = new BaseNumIdStruVo
                        {
                            Id = type,
                            Name = descByCode,
                            Active = bpmProcessNotices.Any(n => n.Type == type)
                        };

                        processNotices.Add(struVo);
                    }
                           
                    diyProcessInfoDTO.ProcessNotices = processNotices;
                }
            }
        }

        return diyProcessInfoDTOS;
    }
   
    private List<DIYProcessInfoDTO> BaseFormInfo(string desc)
    {
        List<DIYProcessInfoDTO> results = new List<DIYProcessInfoDTO>();
        foreach (object service in services)
        {
           
            var annotation = service.GetType().GetCustomAttribute<DIYFormServiceAnnoAttribute>();
            if (string.IsNullOrEmpty(annotation?.Desc))
            {
                continue;
            }
            if (!string.IsNullOrEmpty(desc))
            {
                if (annotation.Desc.Contains(desc))
                {
                    results.Add(new DIYProcessInfoDTO
                    {
                        Key = annotation.SvcName,
                        Value = annotation.Desc,
                        Type = "DIY"
                    });
                }
            }
            else
            {
                results.Add(new DIYProcessInfoDTO
                {
                    Key = annotation.SvcName,
                    Value = annotation.Desc,
                    Type = "DIY"
                });
            }
        }
        return results;
    }

  public  void DeleteExecutionById(String executionId)
    {
        if (string.IsNullOrEmpty(executionId))
        {
            throw new AFBizException("executionId不存在!");
        }
        _executionService._repository.DeleteByExpression(a => a.Id == executionId);
    }

    public void DeletTask(String taskId)
    {
        if (string.IsNullOrEmpty(taskId))
        {
            throw new AFBizException("taskId不存在!");
        }

        _taskService._repository.DeleteByExpression(a => a.Id == taskId);
    }
}