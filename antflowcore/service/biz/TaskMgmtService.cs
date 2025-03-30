using System.Collections;
using System.Reflection;
using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class TaskMgmtService
{
    private readonly AFTaskService _taskService;
    private readonly AfTaskInstService _taskInstService;
    private readonly BpmnConfService _bpmnConfService;
    private IEnumerable services = ServiceProviderUtils.GetServicesByOpenGenericType(typeof(IFormOperationAdaptor<>));
    public TaskMgmtService(
        AFTaskService taskService,
        AfTaskInstService taskInstService,
        BpmnConfService bpmnConfService
        )
    {
        _taskService = taskService;
        _taskInstService = taskInstService;
        _bpmnConfService = bpmnConfService;
    }

    /// <summary>
    /// modify current node's history assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTaskInst(TaskMgmtVO taskMgmtVO)
    {
        int affrows = _taskInstService
            .Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.Assignee, taskMgmtVO.ApplyUser)
            .Set(a => a.AssigneeName, taskMgmtVO.ApplyUserName)
            .Set(a=>a.Description,"变更处理人")
            .Set(a=>a.UpdateUser,SecurityUtils.GetLogInEmpId())
            .Where(a => a.Id == taskMgmtVO.TaskId)
            .ExecuteAffrows();
        return affrows;
    }

    /// <summary>
    /// modify current assignee
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int UpdateTask(TaskMgmtVO taskMgmtVO)
    {
        int affrows = _taskService
            .Frsql
            .Update<BpmAfTask>()
            .Set(a => a.Assignee, taskMgmtVO.ApplyUser)
            .Set(a => a.AssigneeName, taskMgmtVO.ApplyUserName)
            .Where(a => a.Id == taskMgmtVO.TaskId)
            .ExecuteAffrows();
       
        return affrows;
    }

    public List<BpmAfTask> GetAgencyList(string taskId, int code, string taskProcInstId)
    {
        IEnumerable<string> taskDefKeys = _taskService.baseRepo.Where(a=>a.Id==taskId).ToList().Select(a=>a.TaskDefKey);
        List<BpmAfTask> bpmAfTasks = _taskService.baseRepo.Where(a=>taskDefKeys.Contains(a.TaskDefKey)&&a.ProcInstId==taskProcInstId).ToList();
        List<BpmAfTask> afTasks = bpmAfTasks.Where(a=>a.TaskDefKey!=taskId).ToList();
        return afTasks;
    }

    public void DeleteTask(string taskId)
    {
       _taskService.baseRepo.Delete(a=>a.Id==taskId);
    }

    public List<DIYProcessInfoDTO> ViewProcessInfo(string desc)
    {
        List<DIYProcessInfoDTO> diyProcessInfoDTOS = BaseFormInfo(desc);
        if (diyProcessInfoDTOS == null || diyProcessInfoDTOS.Count == 0)
        {
            return diyProcessInfoDTOS;
        }

        List<string> formCodes = diyProcessInfoDTOS.Select(dto => dto.Key).ToList();

        var bpmnConfs = _bpmnConfService.baseRepo
            .Where(b => formCodes.Contains(b.FormCode) && b.EffectiveStatus == 1 && b.ExtraFlags != null)
            .ToList(b => new { b.FormCode, b.ExtraFlags });

        if (bpmnConfs.Count > 0)
        {
            Dictionary<string, int?> formCode2Flags = bpmnConfs.ToDictionary(b => b.FormCode, x => x.ExtraFlags,StringComparer.Ordinal);

            foreach (var diyProcessInfoDTO in diyProcessInfoDTOS)
            {
                if (formCode2Flags.TryGetValue(diyProcessInfoDTO.Key, out int? flags))
                {
                    bool hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                    diyProcessInfoDTO.HasStarUserChooseModule = hasStartUserChooseModules;
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
           
            var annotation = service.GetType().GetCustomAttribute<AfFormServiceAnnoAttribute>();
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
}