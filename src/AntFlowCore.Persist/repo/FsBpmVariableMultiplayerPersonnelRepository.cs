using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmVariableMultiplayerPersonnelRepository : RepositoryBase<BpmVariableMultiplayerPersonnel>, IBpmVariableMultiplayerPersonnelRepository
{
    public FsBpmVariableMultiplayerPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void Undertake(string processNumber, string taskTaskDefKey)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = _ormContext.FreeSql.Select<BpmVariableMultiplayer>()
            .From<BpmVariable, BpmVariableMultiplayerPersonnel>(
                (a,b,c)=>
                    a.LeftJoin(x=>x.VariableId==b.Id)
                        .LeftJoin(x=>x.Id==c.VariableMultiplayerId)
            )
            .Where(a=>(a.t1.ElementId==taskTaskDefKey)&&(a.t2.ProcessNum==processNumber)&&(a.t3.UndertakeStatus==0))
            .ToList();
        if (bpmVariableMultiplayers != null && bpmVariableMultiplayers.Count > 0 &&
            bpmVariableMultiplayers[0].SignType == 2)
        {
            String logInEmpId = SecurityUtils.GetLogInEmpId();
            if (string.IsNullOrEmpty(logInEmpId)) {
                throw new AFBizException("current user is not login");
            }


            _ormContext.FreeSql.Update<BpmVariableMultiplayerPersonnel>()
                .Set(a => a.UndertakeStatus, 1)
                .Where(a => a.VariableMultiplayerId == bpmVariableMultiplayers[0].Id)
                .ExecuteAffrows();
        }
    }

    public void UpdateAssignee(long id, string assignee, string assigneeName, string remark)
    {
        _ormContext.FreeSql.Update<BpmVariableMultiplayerPersonnel>()
            .Set(a => a.Assignee, assignee)
            .Set(a => a.AssigneeName, assigneeName)
            .Set(a => a.Remark, remark)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }
}
