using System.Collections.ObjectModel;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

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