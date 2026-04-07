using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeLfFormdataFieldControlService : IBaseRepositoryService<BpmnNodeLfFormdataFieldControl>
{
    List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey);
}
