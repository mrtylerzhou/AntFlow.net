using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeToRepository : RepositoryBase<BpmnNodeTo>, IBpmnNodeToRepository
{
    public SsBpmnNodeToRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        if (bpmnNodeVo == null)
        {
            return;
        }

        List<String> nodeTo = bpmnNodeVo.NodeTo;

        Db.Deleteable<BpmnNodeTo>()
            .Where(a => a.BpmnNodeId == bpmnNodeId)
            .ExecuteCommand();

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

        AddRange(bpmnNodeTos);
    }
}
