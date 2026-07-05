using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn;
using AntFlowCore.Bpmn.constants;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.service;

public class ConditionService : IConditionService
{
    private readonly ILogger<ConditionService> _logger;
    private readonly IBpmDynamicConditionChoosenService _dynamicConditionChoosenService;

    public ConditionService(ILogger<ConditionService> logger, IBpmDynamicConditionChoosenService dynamicConditionChoosenService)
    {
        _logger = logger;
        _dynamicConditionChoosenService = dynamicConditionChoosenService;
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
           
            for (var i = 0; i < conditionParamTypeList.Count; i++)
            {
                int conditionParam = conditionParamTypeList[i];
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
                    if (!conditionJudge.Judge(nodeId, conditionsConf, bpmnStartConditionsVo,currentGroup,i))
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
            }
            result = currentGroupResult;
            if(groupRelation==ConditionRelationShipEnum.AND.Code&&!result){//条件组之间如果为且关系,如果有一个条件组评估为false,则立刻返回false
                break;
            }
            if(groupRelation==ConditionRelationShipEnum.OR.Code&&result){//条件组之间如果为或关系,如果有一个条件组评估为true,则立刻返回true
                break;
            }
        }

        // 动态条件迁移预校验:检查条件是否发生变化
        // 关于默认条件,默认条件不记在表内,
        // 1.如果之前是默认条件,本次不是默认,则迁移预校验会查不到,也能说明条件发生了变化
        // 2.如果之前不是默认条件,本次变成了默认条件,库里也会查不到,说明条件发生了变化
        // 3.如果前后都是默认条件,则库里没记录,会跳过
        // 4.如果前后都不是默认条件,则正常逻辑,看看库里前后是否一样
        if (bpmnStartConditionsVo.IsMigration == true && bpmnStartConditionsVo.IsPreview && isDynamicConditionGateway)
        {
            var conditionChoosens = _dynamicConditionChoosenService._repository
                .Find(a => a.ProcessNumber == bpmnStartConditionsVo.ProcessNum
                        && a.NodeFrom == bpmnNodeVo.NodeFrom)
                .ToList();

            var nodeIdsEverUsed = conditionChoosens.Select(a => a.NodeId).ToList();
            // 如果当前节点没有使用过(曾经是false或者默认),现在变成了true,
            // 或者使用过(曾经是true),现在变成了false(或者默认),都说明条件发生了变化
            if ((!nodeIdsEverUsed.Contains(nodeId) && result)
                || (nodeIdsEverUsed.Contains(nodeId) && !result))
            {
                // 删除旧记录,插入新记录
                _dynamicConditionChoosenService._repository.RemoveRange(conditionChoosens);
                _dynamicConditionChoosenService._repository.Add(new BpmDynamicConditionChoosen
                {
                    ProcessNumber = bpmnStartConditionsVo.ProcessNum,
                    NodeId = bpmnNodeVo.NodeId
                });
                throw new AFBizException(StringConstants.CONDITION_CHANGED, "流程条件发生改变");
            }
        }

        // 如果是动态条件,将条件记录下来,后面比对是否发生了变化
        if (isDynamicConditionGateway && !bpmnStartConditionsVo.IsPreview && result)
        {
            _dynamicConditionChoosenService._repository.Add(new BpmDynamicConditionChoosen
            {
                ProcessNumber = bpmnStartConditionsVo.ProcessNum,
                NodeId = bpmnNodeVo.NodeId,
                NodeFrom = bpmnNodeVo.NodeFrom
            });
        }

        return result;
    }
}