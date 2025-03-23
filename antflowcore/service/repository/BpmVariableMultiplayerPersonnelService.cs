using AntFlowCore.Entities;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmVariableMultiplayerPersonnelService: AFBaseCurdRepositoryService<BpmVariableMultiplayerPersonnel>
{
    public BpmVariableMultiplayerPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void Undertake(string processNumber, string taskTaskDefKey)
    {
        List<BpmVariableMultiplayer> bpmVariableMultiplayers = Frsql.Select<BpmVariableMultiplayer>()
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

            BpmVariableMultiplayerPersonnel dto = new BpmVariableMultiplayerPersonnel
            {
                UndertakeStatus = 1
            };
            this.Frsql.Update<BpmVariableMultiplayerPersonnel>()
                .SetDto(dto)
                .Where(a => a.VariableMultiplayerId == bpmVariableMultiplayers[0].Id);
        }
    }
}