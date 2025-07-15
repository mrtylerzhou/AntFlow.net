using System.Net;
using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class ProcessBusinessContansService
{
    private readonly AFExecutionService _executionService;
    private readonly AfTaskInstService _taskInstService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
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
        _executionService.baseRepo.Delete(a=>a.ProcInstId==processInstanceId);
        _taskService.baseRepo.Delete(a=>a.ProcInstId==processInstanceId);
        _taskInstService.Frsql
            .Update<BpmAfTaskInst>()
            .Set(a => a.EndTime, DateTime.Now)
            .Set(a => a.DeleteReason, "process ending")
            .Where(a => a.ProcInstId == processInstanceId && a.EndTime == null &&
                        a.Assignee == SecurityUtils.GetLogInEmpId())
            .ExecuteAffrows();
    }
    public BpmAfTaskInst GetPrevTask(String taskDefKey,String procInstId){
        if(string.IsNullOrEmpty(taskDefKey)){
            return null;
        }
        if(string.IsNullOrEmpty(procInstId)){
            throw new AFBizException("taskId不为空,流程实例Id不存在!");
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _taskInstService
            .baseRepo
            .Where(a=>a.ProcInstId==procInstId)
            .OrderByDescending(a=>a.StartTime)
            .ToList();
        BpmAfTaskInst bpmAfTaskInst = bpmAfTaskInsts.First(a => a.EndTime!=null&&a.TaskDefKey!=taskDefKey);
        return bpmAfTaskInst;
    }

    public string GetRoute(int type, ProcessInforVo inforVo, bool isOutside)
    {
        string url = string.Empty;

        // email
        if (type == MessageSendTypeEnum.MAIL.Code)
        {
            url = PcApplyRoute(inforVo.ProcessinessKey, inforVo.FormCode, inforVo.BusinessNumber, inforVo.Type, isOutside);
        }
        else
        {
            url = DetailRoute(inforVo.FormCode, inforVo.BusinessNumber, inforVo.Type, isOutside);
        }

        return url;
    }

    public string PcApplyRoute(string processKey, string formCode, string processNumber, int type, bool isOutside)
    {
        // 如果是外部流程，返回外部 URL
        if (isOutside)
        {
            // 需要重设计第三方 URL
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
            // 其他类型，待设计
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
                var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
                appUrl = appUrl + "_";

                if (bpmBusinessProcess.Version != null)
                {
                    appUrl = appUrl + bpmBusinessProcess.Version.ToString();
                }
            }
        }

        try
        {
            string inParameter = WebUtility.UrlEncode("{\"formCode\":\"" + formCode + "\",\"processNumber\":\"" + processNumber + "\",\"type\":" + type + "}");

            // Placeholder for detail, adjust per your requirements
            detail = string.Empty;
            if (isOutside)
            {
                formCode = "OUTSIDE_WMA";
            }

            // Replace placeholders in the detail string
            detail = detail.Replace("{申请类型}", formCode)
                .Replace("{路由}", appUrl)
                .Replace("{工作流入参}", inParameter);

            detail = WebUtility.UrlEncode(detail);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }

        return detail;
    }
}