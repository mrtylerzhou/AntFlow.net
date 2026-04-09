using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmvariableBizService
{
    NodeElementDto GetNodeIdByElementId(string processNumber, string elementId);
    List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds);
    NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId);
    void AddNodeAssignees(string processNumber, string elementId, List<BaseIdTranStruVo> assignees);
    List<BpmVariableMultiplayer> QuerymultiplayersByProcessNumAndElementId(string processNum, string elementId);
    void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle);
    BpmVariableMultiplayer GetCurrentMultiPlayerNode(string processNumber, string elementId, string nodeId = null);
    void ChangeVariableAssignees(IDictionary<BaseInfoTranStructVo, BaseIdTranStruVo> changedAssignees, bool isSingle);
}
