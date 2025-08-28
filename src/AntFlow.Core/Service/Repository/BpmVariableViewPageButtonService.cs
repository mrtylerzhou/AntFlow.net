using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableViewPageButtonService : AFBaseCurdRepositoryService<BpmVariableViewPageButton>,
    IBpmVariableViewPageButtonService
{
    public BpmVariableViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum)
    {
        List<BpmVariableViewPageButton> bpmVariableViewPageButtons = Frsql
            .Select<BpmVariableViewPageButton, BpmVariable>()
            .LeftJoin((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum)
            .ToList();
        return bpmVariableViewPageButtons;
    }
}