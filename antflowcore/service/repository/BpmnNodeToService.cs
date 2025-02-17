using System.Runtime.InteropServices.JavaScript;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service.repository;

public class BpmnNodeToService: AFBaseCurdRepositoryService<BpmnNodeTo>
{
    public BpmnNodeToService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
    public void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId) {
        
        if (bpmnNodeVo==null) {
            return;
        }
        List<String> nodeTo = bpmnNodeVo.NodeTo;

        //delete existing data
        int executeAffrows = Frsql.Delete<BpmnNodeTo>(new BpmnNodeTo { BpmnNodeId = bpmnNodeId }).ExecuteAffrows();
       

        string logInEmpName = SecurityUtils.GetLogInEmpNameSafe();
        DateTime nowTime = DateTime.Now;
        IEnumerable<BpmnNodeTo> bpmnNodeTos = nodeTo.Select(a=>new BpmnNodeTo
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