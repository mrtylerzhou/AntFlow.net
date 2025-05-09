﻿using AntFlowCore.Entity;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service.repository;

public class BpmnApproveRemindService: AFBaseCurdRepositoryService<BpmnApproveRemind>
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