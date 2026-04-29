using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeConditionsConfRepository : RepositoryBase<BpmnNodeConditionsConf>, IBpmnNodeConditionsConfRepository
{
    public SsBpmnNodeConditionsConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo)
    {
        string processNumber = businessDataVo.ProcessNumber;
        if (!string.IsNullOrEmpty(processNumber))
        {
            return Db.Queryable<BpmnNodeConditionsParamConf>()
                .InnerJoin<BpmnNodeConditionsConf>((tbncpc, tbncc) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .InnerJoin<BpmnNode>((tbncpc, tbncc, tbn) => tbncc.BpmnNodeId == tbn.Id && tbn.NodeType == 3)
                .InnerJoin<BpmnConf>((tbncpc, tbncc, tbn, tbc) => tbn.ConfId == tbc.Id)
                .InnerJoin<BpmBusinessProcess>((tbncpc, tbncc, tbn, tbc, bpb) => bpb.Version == tbc.BpmnCode)
                .Where((tbncpc, tbncc, tbn, tbc, bpb) => bpb.BusinessNumber == processNumber)
                .Select((tbncpc, tbncc, tbn, tbc, bpb) => tbncpc.ConditionParamName)
                .ToList();
        }
        else
        {
            return Db.Queryable<BpmnNodeConditionsParamConf>()
                .InnerJoin<BpmnNodeConditionsConf>((tbncpc, tbncc) => tbncpc.BpmnNodeConditionsId == tbncc.Id)
                .InnerJoin<BpmnNode>((tbncpc, tbncc, tbn) => tbncc.BpmnNodeId == tbn.Id && tbn.NodeType == 3)
                .InnerJoin<BpmnConf>((tbncpc, tbncc, tbn, tbc) => tbn.ConfId == tbc.Id)
                .Where((tbncpc, tbncc, tbn, tbc) => tbc.FormCode == businessDataVo.FormCode && tbc.EffectiveStatus == 1)
                .Select((tbncpc, tbncc, tbn, tbc) => tbncpc.ConditionParamName)
                .ToList();
        }
    }
}
