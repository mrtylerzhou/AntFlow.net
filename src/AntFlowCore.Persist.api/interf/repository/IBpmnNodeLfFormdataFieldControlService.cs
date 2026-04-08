using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeLfFormdataFieldControlService : IBaseRepositoryService<BpmnNodeLfFormdataFieldControl>
{
    List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey);
}
