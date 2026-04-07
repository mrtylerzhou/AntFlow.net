using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface ITaskMgmtService
{
    int UpdateTaskInst(TaskMgmtVO taskMgmtVO);
    int UpdateTask(TaskMgmtVO taskMgmtVO);
    List<BpmAfTask> GetAgencyList(string taskId, int code, string taskProcInstId);
    void DeleteTask(string taskId);
    List<DIYProcessInfoDTO> ViewProcessInfo(string desc = "");
    void DeleteExecutionById(string executionId);
    void DeletTask(string taskId);
}
