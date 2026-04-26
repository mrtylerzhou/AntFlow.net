using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmnApproveRemindService: IBpmnApproveRemindService
{
    public IBpmnApproveRemindRepository _repository { get; }

    public BpmnApproveRemindService(IBpmnApproveRemindRepository repository)
    {
        _repository = repository;
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
        _repository.Update(bpmnApproveRemind);
    }
}