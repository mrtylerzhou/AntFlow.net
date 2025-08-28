using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeToService : AFBaseCurdRepositoryService<BpmnNodeTo>, IBpmnNodeToService
{
    public BpmnNodeToService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        if (bpmnNodeVo == null)
        {
            return;
        }

        List<string> nodeTo = bpmnNodeVo.NodeTo;

        //delete existing data
        int executeAffrows = Frsql.Delete<BpmnNodeTo>(new BpmnNodeTo { BpmnNodeId = bpmnNodeId }).ExecuteAffrows();


        string logInEmpName = SecurityUtils.GetLogInEmpNameSafe();
        DateTime nowTime = DateTime.Now;
        IEnumerable<BpmnNodeTo> bpmnNodeTos = nodeTo.Select(a => new BpmnNodeTo
        {
            BpmnNodeId = bpmnNodeId,
            NodeTo = a,
            CreateUser = logInEmpName,
            Remark = "",
            CreateTime = nowTime
        });

        baseRepo.Insert(bpmnNodeTos);
    }
}