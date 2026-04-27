

using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IAFDeploymentService : IAntFlowRepositoryMix<BpmAfDeployment, IBpmAfDeploymentRepository>
{
    void UpdateNodeAssignee(string processNumber, List<BaseIdTranStruVo> userInfos, string nodeId, int actionType);
    List<BpmnConfCommonElementVo>? GetDeploymentByProcessNumber(string processNumber);
}
