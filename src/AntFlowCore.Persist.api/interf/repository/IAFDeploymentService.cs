

using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFDeploymentService : IBaseRepositoryService<BpmAfDeployment>
{
    void UpdateNodeAssignee(string processNumber, List<BaseIdTranStruVo> userInfos, string nodeId, int actionType);
    List<BpmnConfCommonElementVo>? GetDeploymentByProcessNumber(string processNumber);
}
