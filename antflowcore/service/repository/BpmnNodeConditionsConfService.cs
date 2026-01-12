using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmnNodeConditionsConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsConf>,IBpmnNodeConditionsConfService
{
    public BpmnNodeConditionsConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
    
    public List<String> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo) {
        String processNumber=businessDataVo.ProcessNumber;//流程查看页预览真使用流程编号
        List<String> result=null;
        if(!string.IsNullOrEmpty(processNumber))
        {
          result=  this.Frsql
                .Select<BpmnConf, BpmnNode, BpmnNodeConditionsConf, BpmnNodeConditionsParamConf, BpmBusinessProcess>()
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbn.ConfId == tbc.Id && tbn.NodeType == 3)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbncc.BpmnNodeId == tbn.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => bpb.Version == tbc.BpmnCode)
                .Where((tbc, tbn, tbncc, tbncpc, bpb) => bpb.BusinessNumber == processNumber)
                .ToList<string>(((tbc, tbn, tbncc, tbncpc, bpb) => tbncpc.ConditionParamName));

        }else{//流程发起和在发起页预览时还没有流程编号，使用表单编号查询
            result=  this.Frsql
                .Select<BpmnConf, BpmnNode, BpmnNodeConditionsConf, BpmnNodeConditionsParamConf>()
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbn.ConfId == tbc.Id && tbn.NodeType == 3)
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbncc.BpmnNodeId == tbn.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .Where((tbc, tbn, tbncc, tbncpc) => tbc.FormCode == businessDataVo.FormCode)
                .ToList<string>(((tbc, tbn, tbncc, tbncpc) => tbncpc.ConditionParamName));
        }
        return result;
    }
}