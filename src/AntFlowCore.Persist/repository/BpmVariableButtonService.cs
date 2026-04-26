using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableButtonService: AFBaseCurdRepositoryService<BpmVariableButton>,IBpmVariableButtonService
{
    public BpmVariableButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, List<string> elementIds)
    {
        List<BpmVariableButton> bpmVariableButtons = Frsql.Select<BpmVariableButton,BpmVariable>()
            .LeftJoin((a,b)=>a.VariableId==b.Id)
            .Where((a,b)=>b.ProcessNum==processNum&&elementIds.Contains(a.ElementId))
            .ToList();
        return bpmVariableButtons;
    }
}