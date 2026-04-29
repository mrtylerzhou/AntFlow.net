using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableMultiplayerPersonnelRepository : RepositoryBase<BpmVariableMultiplayerPersonnel>, IBpmVariableMultiplayerPersonnelRepository
{
    public SsBpmVariableMultiplayerPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void Undertake(string processNumber, string taskTaskDefKey)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Db.Queryable<BpmVariableMultiplayer>()
            .LeftJoin<BpmVariable>((a, b) => a.VariableId == b.Id)
            .LeftJoin<BpmVariableMultiplayerPersonnel>((a, b, c) => a.Id == c.VariableMultiplayerId)
            .Where((a, b, c) => (a.ElementId == taskTaskDefKey) && (b.ProcessNum == processNumber) && (c.UndertakeStatus == 0))
            .Select((a, b, c) => a)
            .ToList();
        if (bpmVariableMultiplayers != null && bpmVariableMultiplayers.Count > 0 &&
            bpmVariableMultiplayers[0].SignType == 2)
        {
            String logInEmpId = SecurityUtils.GetLogInEmpId();
            if (string.IsNullOrEmpty(logInEmpId))
            {
                throw new AFBizException("current user is not login");
            }

            Db.Updateable<BpmVariableMultiplayerPersonnel>()
                .SetColumns(a => a.UndertakeStatus == 1)
                .Where(a => a.VariableMultiplayerId == bpmVariableMultiplayers[0].Id)
                .ExecuteCommand();
        }
    }

    public void UpdateAssignee(long id, string assignee, string assigneeName, string remark)
    {
        Db.Updateable<BpmVariableMultiplayerPersonnel>()
            .SetColumns(a => a.Assignee == assignee)
            .SetColumns(a => a.AssigneeName == assigneeName)
            .SetColumns(a => a.Remark == remark)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }
}
