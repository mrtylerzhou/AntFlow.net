
using AntFlowCore.Core.entity;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IProcessConstantsService
{
    BpmAfTaskInst? GetPrevTask(string taskDefKey, string procInstId);
    ProcessRecordInfoVo ProcessInfo(BpmBusinessProcess bpmBusinessProcess);
    bool ShowProcessData(string processCode);
}
