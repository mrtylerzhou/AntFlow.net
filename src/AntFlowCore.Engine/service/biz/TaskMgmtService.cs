using System.Collections;
using System.Reflection;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.DependencyInjection;

namespace AntFlowCore.Engine.service.biz;

public class TaskMgmtService : ITaskMgmtService
{
    private readonly IAFTaskService _taskService;
    private readonly IAfTaskInstService _taskInstService;
    private readonly IAFExecutionService _executionService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IServiceProvider _serviceProvider;
    private IEnumerable services = ServiceProviderUtils.GetServicesByOpenGenericType(typeof(IFormOperationAdaptor<>));
    public TaskMgmtService(
        IAFTaskService taskService,
        IAfTaskInstService taskInstService,
        IAFExecutionService executionService,
        IBpmnConfService bpmnConfService,
        IServiceProvider serviceProvider
        )
    {
        _taskService = taskService;
        _taskInstService = taskInstService;
        _executionService = executionService;
        _bpmnConfService = bpmnConfService;
        _serviceProvider = serviceProvider;
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
            .Select(b => new { b.FormCode, b.ExtraFlags, b.ConfConfigJson })
            .ToList();

        if (bpmnConfs.Count > 0)
        {
            Dictionary<string, int?> formCode2Flags = bpmnConfs
                .Where(b => b.ExtraFlags != null)
                .ToDictionary(b => b.FormCode, x => x.ExtraFlags, StringComparer.Ordinal);

            // 解析每个流程配置的通知渠道类型
            Dictionary<string, List<int>> formCode2NoticeTypes = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            foreach (var conf in bpmnConfs)
            {
                var confConfig = JsonConfUtil.ParseConfConfig(conf.ConfConfigJson);
                if (confConfig?.NoticeChannelTypes != null && confConfig.NoticeChannelTypes.Count > 0)
                {
                    formCode2NoticeTypes[conf.FormCode] = confConfig.NoticeChannelTypes;
                }
            }

            foreach (var diyProcessInfoDTO in diyProcessInfoDTOS)
            {
                string formCode = diyProcessInfoDTO.Key;

                if (formCode2Flags.TryGetValue(formCode, out int? flags))
                {
                    bool hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                    diyProcessInfoDTO.HasStarUserChooseModule = hasStartUserChooseModules;
                }

                // 构建流程通知渠道列表(遍历所有渠道,active 标记是否启用)
                if (formCode2NoticeTypes.TryGetValue(formCode, out List<int> noticeChannelTypes) && !noticeChannelTypes.IsEmpty())
                {
                    List<BaseNumIdStruVo> processNotices = new List<BaseNumIdStruVo>();
                    foreach (var noticeEnum in ProcessNoticeEnum.Values)
                    {
                        processNotices.Add(new BaseNumIdStruVo
                        {
                            Id = noticeEnum.Code,
                            Name = noticeEnum.Desc,
                            Active = noticeChannelTypes.Contains(noticeEnum.Code)
                        });
                    }
                    diyProcessInfoDTO.ProcessNotices = processNotices;
                }

                // 填充通知模板配置列表
                BpmnConfVo confVo = new BpmnConfVo { FormCode = formCode };
                var bpmnConfBizService = _serviceProvider.GetRequiredService<IBpmnConfBizService>();
                bpmnConfBizService.SetBpmnTemplateVos(confVo);
                diyProcessInfoDTO.TemplateVos = confVo.TemplateVos;
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