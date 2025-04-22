using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public abstract class AbstractBpmnRemoveFormat : IBpmnRemoveFormat
    {
        public void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
        {
            string startNodeId = null;
            Dictionary<String, BpmnNodeVo> mapNodes =new Dictionary<String, BpmnNodeVo>();
            foreach (BpmnNodeVo bpmnNodeVo in bpmnConfVo.Nodes)
            {
                mapNodes.Add(bpmnNodeVo.NodeId, bpmnNodeVo);
                if (bpmnNodeVo.NodeType != null && bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
                {
                    startNodeId = bpmnNodeVo.NodeId;
                }
            }
            var preVo = mapNodes[startNodeId];
            var vo = mapNodes[startNodeId];

            // Parallel gateway node will be treated recursively
            while (!string.IsNullOrEmpty(vo.Params.NodeTo) || vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
            {
                if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
                {
                    var aggregationNode = BpmnUtils.GetAggregationNode(vo, mapNodes.Values);
                    TreatParallelGatewayRecursively(vo, aggregationNode, mapNodes, bpmnStartConditions);
                    vo = aggregationNode;
                }
                else
                {
                    vo = mapNodes[vo.Params.NodeTo];
                    // If the next node is also a parallel gateway node, let the while loop handle it
                    if (vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
                        continue;

                    var funcs = TrueFuncs(vo, bpmnStartConditions);
                    foreach (var func in funcs)
                    {
                        if (func())
                            preVo.Params.NodeTo = vo.Params.NodeTo;
                    }
                }

                preVo = vo;
            }
        }

        private void TreatParallelGatewayRecursively(BpmnNodeVo outerMostParallelGatewayNode, BpmnNodeVo itsAggregationNode, Dictionary<string, BpmnNodeVo> mapNodes, BpmnStartConditionsVo bpmnStartConditions)
        {
            if (itsAggregationNode == null)
                throw new AFBizException("There is a parallel gateway node, but cannot get its aggregation node!");

            var aggregationNodeNodeId = itsAggregationNode.NodeId;
            foreach (var nodeTo in outerMostParallelGatewayNode.NodeTo)
            {
                var currentNodeVo = mapNodes[nodeTo];
                // Treat all nodes between parallel gateway and its aggregation node (not include the latter)
                for (var nodeVo = currentNodeVo; nodeVo.NodeId != aggregationNodeNodeId; nodeVo = mapNodes[nodeVo.Params.NodeTo])
                {
                    if (nodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY)
                    {
                        var aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                        TreatParallelGatewayRecursively(nodeVo, aggregationNode, mapNodes, bpmnStartConditions);
                    }

                    var funcs = TrueFuncs(nodeVo, bpmnStartConditions);
                    foreach (var func in funcs)
                    {
                        if (func())
                            currentNodeVo.Params.NodeTo = nodeVo.Params.NodeTo;
                    }
                }
            }
        }

        public abstract List<Func<bool>> TrueFuncs(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditionsVo);
        public abstract int Order();
    }