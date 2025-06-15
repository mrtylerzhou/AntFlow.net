using System.Collections;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.processor.filter;

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
        String nodeId = bpmnNodeVo.NodeId;
        IDictionary<int, List<int>> groupedConditionParamTypes = conditionsConf.GroupedConditionParamTypes;
        if (groupedConditionParamTypes.IsEmpty())
        {
            return false;
        }


        bool result = true;
        int? groupRelation = conditionsConf.GroupRelation;
        foreach (var conditionTypeEntry in groupedConditionParamTypes)
        {
            int currentGroup = conditionTypeEntry.Key;
            bool currentGroupResult = true;
            if (!conditionsConf.GroupedCondRelations.TryGetValue(currentGroup, out var condRelation))
            {
                throw new AFBizException("logic error,please contact the Administrator");
            }

            List<int> conditionParamTypeList = conditionTypeEntry.Value;
            if (conditionParamTypeList.IsEmpty())
            {
                result = false;
                break;
            }

            conditionParamTypeList = conditionParamTypeList.Distinct().ToList();
            int index = 0;
            foreach (int conditionParam in conditionParamTypeList)
            {
                ConditionTypeEnum? conditionTypeEnum = ConditionTypeEnumExtensions.GetEnumByCode(conditionParam);
                if (conditionTypeEnum == null)
                {
                    _logger.LogInformation("condition type is null,type:{}", conditionParam);
                    throw new AFBizException("logic error,please contact the Administrator");
                }

                ConditionTypeAttributes conditionTypeAttributes = conditionTypeEnum.Value.GetAttributes();
                Type conditionJudgeClassType = conditionTypeAttributes.ConditionJudgeClass;
                IEnumerable conditionJudgeServices = ServiceProviderUtils.GetServices(typeof(IConditionJudge));
                if (conditionJudgeServices == null)
                {
                    throw new AFBizException($"未能根据服务类型:{conditionJudgeClassType}找到对应服务,请检查是否存在或者是否已经注入");
                }

                IConditionJudge conditionJudge = null;
                int count = 0;
                //in fact each time one can only get one
                foreach (object conditionJudgeService in conditionJudgeServices)
                {
                    if (count > 1)
                    {
                        throw new AFBizException("there should be only one favorable condition judge service!");
                    }

                    if (conditionJudgeService.GetType() == conditionJudgeClassType)
                    {
                        conditionJudge = (IConditionJudge)conditionJudgeService;
                        count++;
                    }
                }

                if (conditionJudge == null)
                {
                    throw new AFBizException(
                        $"can not find a condition judge service by provided type:{conditionJudgeClassType}");
                }

                try
                {
                    if (!conditionJudge.Judge(nodeId, conditionsConf, bpmnStartConditionsVo, index,currentGroup))
                    {
                        currentGroupResult = false;
                        //如果是且关系,有一个条件判断为false则终止判断
                        if(condRelation==ConditionRelationShipEnum.AND.Code){
                            break;
                        }
                    }
                    else
                    {
                        //如果是或关系,有一个条件判断为true则终止判断
                        currentGroupResult=true;
                        if(condRelation==ConditionRelationShipEnum.OR.Code){
                            break;
                        }
                    }
                }
                catch (AFBizException e)
                {
                    _logger.LogInformation($"condiiton judge business exception:{e.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    _logger.LogInformation("conditionjudge error:{}", e);
                    throw;
                }

                index++;
            }
            result = currentGroupResult;
            if(groupRelation==ConditionRelationShipEnum.AND.Code&&!result){//条件组之间如果为且关系,如果有一个条件组评估为false,则立刻返回false
                break;
            }
            if(groupRelation==ConditionRelationShipEnum.OR.Code&&result){//条件组之间如果为或关系,如果有一个条件组评估为true,则立刻返回true
                break;
            }
        }

        return result;
    }
}