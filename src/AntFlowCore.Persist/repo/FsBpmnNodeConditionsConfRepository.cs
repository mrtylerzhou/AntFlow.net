using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace antflowcore.conf.ef;

public class FsBpmnNodeConditionsConfRepository : RepositoryBase<BpmnNodeConditionsConf>, IBpmnNodeConditionsConfRepository
{
    public FsBpmnNodeConditionsConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo)
    {
        string processNumber = businessDataVo.ProcessNumber;
        List<string> result = null;
        if (!string.IsNullOrEmpty(processNumber))
        {
            result = _ormContext.FreeSql
                .Select<BpmnConf, BpmnNode, BpmnNodeConditionsConf, BpmnNodeConditionsParamConf, BpmBusinessProcess>()
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbn.ConfId == tbc.Id && tbn.NodeType == 3)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbncc.BpmnNodeId == tbn.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc, bpb) => bpb.Version == tbc.BpmnCode)
                .Where((tbc, tbn, tbncc, tbncpc, bpb) => bpb.BusinessNumber == processNumber)
                .ToList<string>(((tbc, tbn, tbncc, tbncpc, bpb) => tbncpc.ConditionParamName));
        }
        else
        {
            result = _ormContext.FreeSql
                .Select<BpmnConf, BpmnNode, BpmnNodeConditionsConf, BpmnNodeConditionsParamConf>()
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbn.ConfId == tbc.Id && tbn.NodeType == 3)
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbncc.BpmnNodeId == tbn.Id)
                .InnerJoin((tbc, tbn, tbncc, tbncpc) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .Where((tbc, tbn, tbncc, tbncpc) => tbc.FormCode == businessDataVo.FormCode && tbc.EffectiveStatus == 1)
                .ToList<string>(((tbc, tbn, tbncc, tbncpc) => tbncpc.ConditionParamName));
        }
        return result;
    }
}
