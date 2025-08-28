using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Interface.Repository;

public interface IBpmnNodeLfFormdataFieldControlService
{
    List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(string processNumber, string taskDefKey);
}