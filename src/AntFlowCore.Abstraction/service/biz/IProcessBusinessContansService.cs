using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IProcessBusinessContansService
{
    void DeleteProcessInstance(string processInstanceId);
    BpmAfTaskInst GetPrevTask(string taskDefKey, string procInstId);
    string GetRoute(int type, ProcessInforVo inforVo, bool isOutside);
    string PcApplyRoute(string processKey, string formCode, string processNumber, int type, bool isOutside);
    string DetailRoute(string formCode, string processNumber, int type, bool isOutside);
}
