using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableMultiplayerPersonnelService : AFBaseCurdRepositoryService<BpmVariableMultiplayerPersonnel>,
    IBpmVariableMultiplayerPersonnelService
{
    public BpmVariableMultiplayerPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void Undertake(string processNumber, string taskTaskDefKey)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Frsql.Select<BpmVariableMultiplayer>()
            .From<BpmVariable, BpmVariableMultiplayerPersonnel>(
                (a, b, c) =>
                    a.LeftJoin(x => x.VariableId == b.Id)
                        .LeftJoin(x => x.Id == c.VariableMultiplayerId)
            )
            .Where(a => a.t1.ElementId == taskTaskDefKey && a.t2.ProcessNum == processNumber &&
                        a.t3.UndertakeStatus == 0)
            .ToList();
        if (bpmVariableMultiplayers != null && bpmVariableMultiplayers.Count > 0 &&
            bpmVariableMultiplayers[0].SignType == 2)
        {
            string logInEmpId = SecurityUtils.GetLogInEmpId();
            if (string.IsNullOrEmpty(logInEmpId))
            {
                throw new AFBizException("current user is not login");
            }


            Frsql.Update<BpmVariableMultiplayerPersonnel>()
                .Set(a => a.UndertakeStatus, 1)
                .Where(a => a.VariableMultiplayerId == bpmVariableMultiplayers[0].Id)
                .ExecuteAffrows();
        }
    }
}