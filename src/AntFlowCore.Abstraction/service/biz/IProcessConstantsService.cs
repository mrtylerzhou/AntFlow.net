
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IProcessConstantsService
{
    BpmAfTaskInst? GetPrevTask(string taskDefKey, string procInstId);
    ProcessRecordInfoVo ProcessInfo(BpmBusinessProcess bpmBusinessProcess);
    bool ShowProcessData(string processCode);
}
