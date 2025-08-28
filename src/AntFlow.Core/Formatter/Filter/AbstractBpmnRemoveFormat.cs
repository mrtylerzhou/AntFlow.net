using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Exception;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public abstract class AbstractBpmnRemoveFormat : IBpmnRemoveFormat
{
    /// <summary>
    ///     reviewed
    /// </summary>
    /// <param name="bpmnConfVo"></param>
    /// <param name="bpmnStartConditions"></param>
    public void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        string startNodeId = null;
        Dictionary<string, BpmnNodeVo> mapNodes = new();
        foreach (BpmnNodeVo bpmnNodeVo in bpmnConfVo.Nodes)
        {
            mapNodes.Add(bpmnNodeVo.NodeId, bpmnNodeVo);
            if (bpmnNodeVo.NodeType != null && bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
            {
                startNodeId = bpmnNodeVo.NodeId;
            }
        }

        BpmnNodeVo? preVo = mapNodes[startNodeId];
        BpmnNodeVo? vo = mapNodes[startNodeId];

        // Parallel gateway node will be treated recursively
        while (!string.IsNullOrEmpty(vo.Params.NodeTo) || vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
        {
            if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
            {
                BpmnNodeVo? aggregationNode = BpmnUtils.GetAggregationNode(vo, mapNodes.Values);
                TreatParallelGatewayRecursively(vo, aggregationNode, mapNodes, bpmnStartConditions);
                vo = aggregationNode;
            }
            else
            {
                vo = mapNodes[vo.Params.NodeTo];
                // If the next node is also a parallel gateway node, let the while loop handle it
                if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
                {
                    continue;
                }

                List<Func<bool>>? funcs = TrueFuncs(vo, bpmnStartConditions);
                foreach (Func<bool>? func in funcs)
                {
                    if (func())
                    {
                        preVo.Params.NodeTo = vo.Params.NodeTo;
                    }
                }
            }

            preVo = vo;
        }
    }

    public abstract List<Func<bool>> TrueFuncs(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditionsVo);
    public abstract int Order();

    /// <summary>
    ///     reviewed
    /// </summary>
    /// <param name="outerMostParallelGatewayNode"></param>
    /// <param name="itsAggregationNode"></param>
    /// <param name="mapNodes"></param>
    /// <param name="bpmnStartConditions"></param>
    /// <exception cref="AFBizException"></exception>
    private void TreatParallelGatewayRecursively(BpmnNodeVo outerMostParallelGatewayNode, BpmnNodeVo itsAggregationNode,
        Dictionary<string, BpmnNodeVo> mapNodes, BpmnStartConditionsVo bpmnStartConditions)
    {
        if (itsAggregationNode == null)
        {
            throw new AFBizException("There is a parallel gateway node, but cannot get its aggregation node!");
        }

        string? aggregationNodeNodeId = itsAggregationNode.NodeId;
        foreach (string? nodeTo in outerMostParallelGatewayNode.NodeTo)
        {
            BpmnNodeVo currentNodeVo = mapNodes[nodeTo];
            BpmnNodeVo prevNode = mapNodes[currentNodeVo.NodeId];
            // Treat all nodes between parallel gateway and its aggregation node (not include the latter)
            for (BpmnNodeVo? nodeVo = currentNodeVo;
                 nodeVo.NodeId != aggregationNodeNodeId;
                 nodeVo = mapNodes[nodeVo.Params.NodeTo])
            {
                if (nodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                    TreatParallelGatewayRecursively(nodeVo, aggregationNode, mapNodes, bpmnStartConditions);
                }

                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nodeVo.NodeType)
                {
                    continue;
                }

                List<Func<bool>>? funcs = TrueFuncs(nodeVo, bpmnStartConditions);
                foreach (Func<bool>? func in funcs)
                {
                    if (func())
                    {
                        currentNodeVo.Params.NodeTo = nodeVo.Params.NodeTo;
                    }
                }

                prevNode = nodeVo;
            }
        }
    }
}