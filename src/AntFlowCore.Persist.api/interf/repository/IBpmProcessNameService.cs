using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameService : IBaseRepositoryService<BpmProcessName>
{
    BpmProcessName GetBpmProcessName(string processKey);
    void EditProcessName(BpmnConf bpmnConfByCode);
    BpmProcessVo Get(string processKey);
}
