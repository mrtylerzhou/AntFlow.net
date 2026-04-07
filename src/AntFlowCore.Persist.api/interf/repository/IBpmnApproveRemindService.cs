using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnApproveRemindService : IBaseRepositoryService<BpmnApproveRemind>
{
    void EditBpmnApproveRemind(BpmnNodeVo bpmnNodeVo);
}
