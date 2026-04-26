
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnApproveRemindService: AFBaseCurdRepositoryService<BpmnApproveRemind>,IBpmnApproveRemindService
{
    public BpmnApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public void EditBpmnApproveRemind(BpmnNodeVo bpmnNodeVo) {
        BpmnApproveRemindVo o = bpmnNodeVo.ApproveRemindVo;
        if (o==null) {
            return;
        }

        BpmnApproveRemind bpmnApproveRemind = o.MapToEntity();
        bpmnApproveRemind.ConfId=bpmnNodeVo.ConfId;
        bpmnApproveRemind.NodeId=(bpmnNodeVo.Id);
        if (o.IsInuse!=null) {
            bpmnApproveRemind.Days=(string.Join(",",o.DayList));
        } else {
            bpmnApproveRemind.TemplateId=null;
        }
        bpmnApproveRemind.CreateUser=(SecurityUtils.GetLogInEmpNameSafe());
        baseRepo.Update(bpmnApproveRemind);
    }
}