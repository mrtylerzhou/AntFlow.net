using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFDeploymentService : IAntFlowRepositoryMix<BpmAfDeployment, IBpmAfDeploymentRepository>
{
    void UpdateNodeAssignee(string processNumber, List<BaseIdTranStruVo> userInfos, string nodeId, int actionType);
    List<BpmnConfCommonElementVo>? GetDeploymentByProcessNumber(string processNumber);
}
