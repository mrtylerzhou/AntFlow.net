using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Net;

namespace AntFlow.Core.Service.Business;

public class ProcessBusinessContansService
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly AFExecutionService _executionService;
    private readonly AfTaskInstService _taskInstService;
    private readonly AFTaskService _taskService;

    public ProcessBusinessContansService(
        AFExecutionService executionService,
        AfTaskInstService taskInstService,
        BpmBusinessProcessService bpmBusinessProcessService,
        AFTaskService taskService
    )
    {
        _executionService = executionService;
        _taskInstService = taskInstService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
    }

    public void DeleteProcessInstance(string processInstanceId)
    {
        _executionService.baseRepo.Delete(a => a.ProcInstId == processInstanceId);
        _taskService.baseRepo.Delete(a => a.ProcInstId == processInstanceId);
        _taskInstService.Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.EndTime, DateTime.Now)
            .Set(a => a.DeleteReason, "process ending")
            .Where(a => a.ProcInstId == processInstanceId && a.EndTime == null &&
                        a.Assignee == SecurityUtils.GetLogInEmpId())
            .ExecuteAffrows();
    }

    public BpmAfTaskInst GetPrevTask(string taskDefKey, string procInstId)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            return null;
        }

        if (string.IsNullOrEmpty(procInstId))
        {
            throw new AFBizException("taskId不能为空,流程实例Id不能为空!");
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _taskInstService
            .baseRepo
            .Where(a => a.ProcInstId == procInstId)
            .OrderByDescending(a => a.StartTime)
            .ToList();
        BpmAfTaskInst bpmAfTaskInst = bpmAfTaskInsts.First(a => a.EndTime != null && a.TaskDefKey != taskDefKey);
        return bpmAfTaskInst;
    }

    public string GetRoute(int type, ProcessInforVo inforVo, bool isOutside)
    {
        string url = string.Empty;

        // email
        if (type == MessageSendTypeEnum.MAIL.Code)
        {
            url = PcApplyRoute(inforVo.ProcessinessKey, inforVo.FormCode, inforVo.BusinessNumber, inforVo.Type,
                isOutside);
        }
        else
        {
            url = DetailRoute(inforVo.FormCode, inforVo.BusinessNumber, inforVo.Type, isOutside);
        }

        return url;
    }

    public string PcApplyRoute(string processKey, string formCode, string processNumber, int type, bool isOutside)
    {
        // 根据是否外部系统生成不同的 URL
        if (isOutside)
        {
            // 外部系统访问的 URL
            return $"/user/workflow/detail/third-party/{formCode}/{processNumber}";
        }

        string pcUrl;

        if (type == ProcessJurisdictionEnum.VIEW_TYPE.Code)
        {
            pcUrl = $"/user/workflow/detail/{formCode}/{processKey}/{processNumber}";
        }
        else if (type == ProcessJurisdictionEnum.CREATE_TYPE.Code)
        {
            pcUrl = $"/user/workflow/Upcoming/check/{formCode}/{processKey}/{processNumber}";
        }
        else
        {
            // 其他类型的处理
            pcUrl = $"/user/workflow/Upcoming/apply/{formCode}/{processKey}/{processNumber}";
        }

        return pcUrl;
    }

    public string DetailRoute(string formCode, string processNumber, int type, bool isOutside)
    {
        string detail = string.Empty;
        string appUrl = string.Empty;

        if (type == ProcessJurisdictionEnum.CONTROL_TYPE.Code)
        {
            type = ProcessJurisdictionEnum.CREATE_TYPE.Code;
            appUrl = "apply";
        }
        else
        {
            appUrl = "approval";

            if (!string.IsNullOrEmpty(processNumber))
            {
                BpmBusinessProcess? bpmBusinessProcess =
                    _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
                appUrl = appUrl + "_";

                if (bpmBusinessProcess.Version != null)
                {
                    appUrl = appUrl + bpmBusinessProcess.Version;
                }
            }
        }

        try
        {
            string inParameter = WebUtility.UrlEncode("{\"formCode\":\"" + formCode + "\",\"processNumber\":\"" +
                                                      processNumber + "\",\"type\":" + type + "}");

            // Placeholder for detail, adjust per your requirements
            detail = string.Empty;
            if (isOutside)
            {
                formCode = "OUTSIDE_WMA";
            }

            // Replace placeholders in the detail string
            detail = detail.Replace("{表单编码}", formCode)
                .Replace("{应用}", appUrl)
                .Replace("{输入参数}", inParameter);

            detail = WebUtility.UrlEncode(detail);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }

        return detail;
    }
}