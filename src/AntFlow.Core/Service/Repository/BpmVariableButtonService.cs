using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableButtonService : AFBaseCurdRepositoryService<BpmVariableButton>, IBpmVariableButtonService
{
    public BpmVariableButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmVariableButton> GetButtonsByProcessNumber(string processNum, string elementId)
    {
        List<BpmVariableButton> bpmVariableButtons = Frsql.Select<BpmVariableButton, BpmVariable>()
            .LeftJoin((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum && a.ElementId == elementId)
            .ToList();
        return bpmVariableButtons;
    }
}