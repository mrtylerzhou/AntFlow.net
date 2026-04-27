using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeLfFormdataFieldControlService : IAntFlowRepositoryMix<BpmnNodeLfFormdataFieldControl, IBpmnNodeLfFormdataFieldControlRepository>
{
    List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey);
}
