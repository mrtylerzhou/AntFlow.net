using antflowcore.vo;

namespace antflowcore.service.interf.repository;

public interface IBpmnNodeLfFormdataFieldControlService
{
    List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey);
}