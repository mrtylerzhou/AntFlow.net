using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeSignUpConfService : IBaseRepositoryService<BpmnNodeSignUpConf>
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
