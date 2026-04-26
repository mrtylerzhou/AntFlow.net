using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeLfFormdataFieldControlService : IBpmnNodeLfFormdataFieldControlService
{
    public BpmnNodeLfFormdataFieldControlService(IBpmnNodeLfFormdataFieldControlRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeLfFormdataFieldControlRepository _repository { get; }

    public List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey)
    {
        return _repository.GetFieldControlByProcessNumberAndElementId(processNumber, taskDefKey);
    }
}
