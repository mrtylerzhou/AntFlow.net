using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnApproveRemindService : IBaseRepositoryService<BpmnApproveRemind>
{
    void EditBpmnApproveRemind(BpmnNodeVo bpmnNodeVo);
}
