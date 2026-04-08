

using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;

public interface IBpmnProcessAdminProvider
{
    BaseIdTranStruVo ProvideProcessAdminInfo();
}