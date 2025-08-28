using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmnApproveRemindService : AFBaseCurdRepositoryService<BpmnApproveRemind>, IBpmnApproveRemindService
{
    public BpmnApproveRemindService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void EditBpmnApproveRemind(BpmnNodeVo bpmnNodeVo)
    {
        BpmnApproveRemindVo o = bpmnNodeVo.ApproveRemindVo;
        if (o == null)
        {
            return;
        }

        BpmnApproveRemind bpmnApproveRemind = o.MapToEntity();
        bpmnApproveRemind.ConfId = bpmnNodeVo.ConfId;
        bpmnApproveRemind.NodeId = bpmnNodeVo.Id;
        if (o.IsInuse != null)
        {
            bpmnApproveRemind.Days = string.Join(",", o.DayList);
        }
        else
        {
            bpmnApproveRemind.TemplateId = null;
        }

        bpmnApproveRemind.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
        baseRepo.Update(bpmnApproveRemind);
    }
}