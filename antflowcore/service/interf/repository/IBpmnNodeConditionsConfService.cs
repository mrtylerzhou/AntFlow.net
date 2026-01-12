using AntFlowCore.Vo;

namespace antflowcore.service.interf.repository;

public interface IBpmnNodeConditionsConfService
{
    List<String> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}