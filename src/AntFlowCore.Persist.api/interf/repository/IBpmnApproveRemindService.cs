using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnApproveRemindService : IAntFlowRepositoryMix<BpmnApproveRemind, IBpmnApproveRemindRepository>
{
    void EditBpmnApproveRemind(BpmnNodeVo bpmnNodeVo);
}
