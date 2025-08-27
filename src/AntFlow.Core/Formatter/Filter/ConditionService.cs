using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;
using System.Collections;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Service.Processor.Filter;

public class ConditionService : IConditionService
{
    private readonly ILogger<ConditionService> _logger;

    public ConditionService(ILogger<ConditionService> logger)
    {
        _logger = logger;
    }

    public bool CheckMatchCondition(BpmnNodeVo bpmnNodeVo, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, bool isDynamicConditionGateway)
    {
        string nodeId = bpmnNodeVo.NodeId;
        IDictionary<int, List<int>> groupedConditionParamTypes = conditionsConf.GroupedConditionParamTypes;
        if (groupedConditionParamTypes.IsEmpty())
        {
            return false;
        }


        bool result = true;
        int? groupRelation = conditionsConf.GroupRelation;
        foreach (KeyValuePair<int, List<int>> conditionTypeEntry in groupedConditionParamTypes)
        {
            int currentGroup = conditionTypeEntry.Key;
            bool currentGroupResult = true;
            if (!conditionsConf.GroupedCondRelations.TryGetValue(currentGroup, out int condRelation))
            {
                throw new AntFlowException.AFBizException("logic error,please contact the Administrator");
            }

            List<int> conditionParamTypeList = conditionTypeEntry.Value;
            if (conditionParamTypeList.IsEmpty())
            {
                result = false;
                break;
            }

            conditionParamTypeList = conditionParamTypeList.Distinct().ToList();

            foreach (int conditionParam in conditionParamTypeList)
            {
                ConditionTypeEnum? conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(conditionParam);
                if (conditionTypeEnum == null)
                {
                    _logger.LogInformation("condition type is null,type:{}", conditionParam);
                    throw new AntFlowException.AFBizException("logic error,please contact the Administrator");
                }

                ConditionTypeAttributes conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                Type conditionJudgeClassType = conditionTypeAttributes.ConditionJudgeClass;
                IEnumerable conditionJudgeServices = ServiceProviderUtils.GetServices(typeof(IConditionJudge));
                if (conditionJudgeServices == null)
                {
                    throw new AntFlowException.AFBizException(
                        $"未找到条件判断服务:{conditionJudgeClassType}，请检查服务注册，或者检查条件类型配置是否正确");
                }

                IConditionJudge conditionJudge = null;
                int count = 0;
                //in fact each time one can only get one
                foreach (object conditionJudgeService in conditionJudgeServices)
                {
                    if (count > 1)
                    {
                        throw new AntFlowException.AFBizException(
                            "there should be only one favorable condition judge service!");
                    }

                    if (conditionJudgeService.GetType() == conditionJudgeClassType)
                    {
                        conditionJudge = (IConditionJudge)conditionJudgeService;
                        count++;
                    }
                }

                if (conditionJudge == null)
                {
                    throw new AntFlowException.AFBizException(
                        $"can not find a condition judge service by provided type:{conditionJudgeClassType}");
                }

                try
                {
                    if (!conditionJudge.Judge(nodeId, conditionsConf, bpmnStartConditionsVo, currentGroup))
                    {
                        currentGroupResult = false;
                        //条件不满足，且关系为AND时返回false并跳出循环
                        if (condRelation == ConditionRelationShipEnum.AND.Code)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //条件满足，且关系为OR时返回true并跳出循环
                        currentGroupResult = true;
                        if (condRelation == ConditionRelationShipEnum.OR.Code)
                        {
                            break;
                        }
                    }
                }
                catch (AntFlowException.AFBizException e)
                {
                    _logger.LogInformation($"condiiton judge business exception:{e.Message}");
                    throw;
                }
                catch (System.Exception e)
                {
                    _logger.LogInformation("conditionjudge error:{}", e);
                    throw;
                }
            }

            result = currentGroupResult;
            if (groupRelation == ConditionRelationShipEnum.AND.Code && !result)
            {
                //?????????????????,???????????????????false,?????????false
                break;
            }

            if (groupRelation == ConditionRelationShipEnum.OR.Code && result)
            {
                //?????????????????,???????????????????true,?????????true
                break;
            }
        }

        return result;
    }
}