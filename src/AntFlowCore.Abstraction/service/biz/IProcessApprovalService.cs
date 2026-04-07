using AntFlowCore.Core.dto;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IProcessApprovalService
{
    BusinessDataVo ButtonsOperation(string parameters, string formCode);
    dynamic GetBusinessInfo(string parameters, string formCode);
    ResultAndPage<TaskMgmtVO> FindPcProcessList(PageDto pageDto, TaskMgmtVO vo);
    TaskMgmtVO ProcessStatistics();
}
