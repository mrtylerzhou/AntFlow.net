using AntFlowCore.Entities;
using AntFlowCore.Entity;

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
    }
}