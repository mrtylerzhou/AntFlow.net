using antflowcore.adaptor.bpmnelementadp;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor;

public class BpmnNodeFormatService
{
    private readonly IAdaptorFactory _adaptorFactory;

    public BpmnNodeFormatService(IAdaptorFactory adaptorFactory)
    {
        _adaptorFactory = adaptorFactory;
    }
    public List<BpmnConfCommonElementVo> GetBpmnConfCommonElementVoList(
        BpmnConfCommonVo bpmnConfCommonVo,
        List<BpmnNodeVo> nodes,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        if (nodes == null || !nodes.Any())
        {
            return new List<BpmnConfCommonElementVo>();
        }

        int sequenceFlowNum = 1; // Start sequence flow number
        string startElementId = ProcessNodeEnum.START_TASK_KEY.Description; // Start element

        // Process diagram element list
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos = new List<BpmnConfCommonElementVo>();

        // Set start element
        string startEventElementId = $"{bpmnConfCommonVo.ProcessNum}_{ElementTypeEnum.ELEMENT_TYPE_START_EVENT.Desc}";
        var startEventElement = BpmnElementUtils.GetStartEventElement(startEventElementId);
        bpmnConfCommonElementVos.Add(startEventElement);

        // Rebuild nodes recursively
        List<BpmnNodeVo> rebuildNodesList = new List<BpmnNodeVo>();
        BpmnNodeVo startNode = GetStartUserNode(nodes);
        rebuildNodesList.Add(startNode); // Set start node
        TreatNodesRecursively(rebuildNodesList, nodes, startNode);

        // Set start user node
        BpmnNodeVo startUserNode = GetStartUserNode(rebuildNodesList);
        string startUserId = bpmnStartConditions.StartUserId?.ToString() ?? "";
        Dictionary<string, string> singleAssigneeMap = new Dictionary<string, string>
        {
            { startUserId, bpmnStartConditions.StartUserName }
        };
        BpmnConfCommonElementVo startNodeElement =
            BpmnElementUtils.GetSingleElement(startElementId, "发起人", "startUser", startUserId, singleAssigneeMap);

        // Set start user node buttons
        SetStartNodeElementButtons(startUserNode, startNodeElement);

        // Set start user node template
        startNodeElement.TemplateVos = startUserNode.TemplateVos;

        // Set start user approval reminder
        startNodeElement.ApproveRemindVo = startUserNode.ApproveRemindVo;

        // Add start node to elements list
        bpmnConfCommonElementVos.Add(startNodeElement);

        // Set start user node sequence flow
        bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(
            sequenceFlowNum, startEventElement.ElementId, startNodeElement.ElementId));

        // Format BpmnNodeVo to BpmnConfCommonElementVo list recursively
        int? nodeCode = ProcessNodeEnum.GetCodeByDesc(startElementId);
        if (nodeCode == null)
        {
            throw new AFBizException("nodeCode can not be null");
        }

        var numMap = new Dictionary<string, int>
        {
            { "nodeCode", nodeCode.Value },
            { "sequenceFlowNum", sequenceFlowNum }
        };
        string startUserNodeTo = GetNodeTo(startUserNode);

        // If the target node of the start user node is empty, no need to continue assembling the node element
        if (!string.IsNullOrEmpty(startUserNodeTo))
        {
            FormatNodes(bpmnConfCommonElementVos, rebuildNodesList, startUserNodeTo, nodeCode.Value, sequenceFlowNum, numMap);
        }

        // The last node's ID
        string lastNodeId = ProcessNodeEnum.GetDescByCode(numMap["nodeCode"]);

        // Last sequence flow number
        int lastSequenceFlowNum = numMap["sequenceFlowNum"] + 1;

        // Set end element
        string endEventElementId = $"{bpmnConfCommonVo.ProcessNum}_{ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Desc}";
        var endEventElement = BpmnElementUtils.GetEndEventElement(endEventElementId);
        bpmnConfCommonElementVos.Add(endEventElement);

        // Set last node to end node sequence flow
        var lastSequenceFlow =
            BpmnElementUtils.GetSequenceFlow(lastSequenceFlowNum, lastNodeId, endEventElement.ElementId);
        lastSequenceFlow.IsLastSequenceFlow = 1;
        bpmnConfCommonElementVos.Add(lastSequenceFlow);

        return bpmnConfCommonElementVos;
    }

    private BpmnNodeVo GetStartUserNode(List<BpmnNodeVo> nodes)
    {
        var startNodes = nodes
            .Where(o => o.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
            .ToList();

        if (startNodes == null || !startNodes.Any())
        {
            throw new AFBizException("未找到开始节点流程发起失败");
        }

        return startNodes[0];
    }

    private String GetNodeTo(BpmnNodeVo nodeVo)
    {
        string paramsNodeTo = nodeVo.Params?.NodeTo;
        return paramsNodeTo;
    }

    private Dictionary<string, int> FormatNodes(
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        List<BpmnNodeVo> rebuildNodesList,
        string nodeTo,
        int nodeCode,
        int sequenceFlowNum,
        Dictionary<string, int> numMap)
    {
        rebuildNodesList = rebuildNodesList.Distinct(new NodeVoEqualityComparer()).ToList();
        var mapNodes = rebuildNodesList.ToDictionary(
            node => node.NodeId,
            node => node,
            StringComparer.OrdinalIgnoreCase);

        BpmnNodeVo aggregationNode = null;

        do
        {
            BpmnNodeVo nextNodeVo = GetNextNodeVo(rebuildNodesList, nodeTo);

            if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nextNodeVo.NodeType)
            {
                aggregationNode = BpmnUtils.GetAggregationNode(nextNodeVo, mapNodes.Values);
                int parallelGatewayNodeCode = numMap["nodeCode"] + 1;
                int parallelGatewaySequenceFlowNum = numMap["sequenceFlowNum"] + 1;

                numMap["nodeCode"] = parallelGatewayNodeCode;
                numMap["sequenceFlowNum"] = parallelGatewaySequenceFlowNum;

                // 如果节点是并行网关，则生成并行网关元素并添加到元素列表
                var parallelGatewayElement = BpmnElementUtils.GetParallelGateWayElement(parallelGatewaySequenceFlowNum);
                bpmnConfCommonElementVos.Add(parallelGatewayElement);

                // 添加从前一个元素到网关节点的序列流
                bpmnConfCommonElementVos.Add(
                    BpmnElementUtils.GetSequenceFlow(
                        parallelGatewaySequenceFlowNum,
                        ProcessNodeEnum.GetDescByCode(parallelGatewayNodeCode - 1),
                        parallelGatewayElement.ElementId
                    )
                );

                // 递归处理并行网关
                TreatParallelGatewayRecursively(
                    nextNodeVo,
                    aggregationNode,
                    mapNodes,
                    bpmnConfCommonElementVos,
                    rebuildNodesList,
                    nextNodeVo.NodeId,
                    parallelGatewayNodeCode,
                    parallelGatewaySequenceFlowNum,
                    numMap,
                    new BpmnNodeVo(),
                    new HashSet<string>()
                );

                nodeTo = aggregationNode.NodeId;
            }
            else
            {
                // 如果是聚合节点，生成聚合并行网关节点
                if (nextNodeVo.Equals(aggregationNode))
                {
                    var nodeFroms = GetNodeFroms(rebuildNodesList, nextNodeVo);
                    nextNodeVo.FromNodes = nodeFroms;
                }

                FormatNodesToElements(
                    bpmnConfCommonElementVos,
                    rebuildNodesList,
                    nextNodeVo.NodeId,
                    numMap["nodeCode"],
                    numMap["sequenceFlowNum"],
                    numMap
                );

                nodeTo = nextNodeVo.Params.NodeTo;
            }
        } while (!string.IsNullOrEmpty(nodeTo));

        return numMap;
    }
    /// <summary>
    /// reviewed
    /// </summary>
    /// <param name="rebuildNodesList"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeVo"></param>
    private void TreatNodesRecursively(List<BpmnNodeVo> rebuildNodesList, List<BpmnNodeVo> nodes, BpmnNodeVo nodeVo)
    {
        var mapNodes = nodes.ToDictionary(node => node.NodeId, node => node);
        string nextId = null;

        do
        {
            string nodeTo = GetNodeTo(nodeVo);

            if (string.IsNullOrEmpty(nodeTo))
            {
                return;
            }

            BpmnNodeVo nextNodeVo = GetNextNodeVo(nodes, nodeTo);
            BpmnNodeParamsVo nextParams = nextNodeVo.Params;
            if(nextParams==null||string.IsNullOrEmpty(nodeTo)&&(int)NodeTypeEnum.NODE_TYPE_COPY==nextNodeVo.NodeType){
                return;
            }
            if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nextNodeVo.NodeType)
            {
                BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nextNodeVo, mapNodes.Values);
                
                TreatParallelGatewayRecursively(nextNodeVo, aggregationNode, mapNodes, nodes, rebuildNodesList);
                nodeVo = aggregationNode;
            }
            else
            {
                RebuildNodes(rebuildNodesList, nodes, nextNodeVo);
                nodeVo = nextNodeVo;
            }

            nextId = nodeVo.Params.NodeTo;
        } while (!string.IsNullOrEmpty(nextId));
    }
    /// <summary>
    /// reviewed
    /// </summary>
    /// <param name="outerMostParallelGatewayNode"></param>
    /// <param name="itsAggregationNode"></param>
    /// <param name="mapNodes"></param>
    /// <param name="nodes"></param>
    /// <param name="rebuildNodesList"></param>
    /// <exception cref="AFBizException"></exception>
    private void TreatParallelGatewayRecursively(
        BpmnNodeVo outerMostParallelGatewayNode,
        BpmnNodeVo itsAggregationNode,
        Dictionary<string, BpmnNodeVo> mapNodes,
        List<BpmnNodeVo> nodes,
        List<BpmnNodeVo> rebuildNodesList)
    {
        if (itsAggregationNode == null)
        {
            throw new AFBizException("There is a parallel gateway node, but cannot get its aggregation node!");
        }

        string aggregationNodeNodeId = itsAggregationNode.NodeId;
        var nodeTos = outerMostParallelGatewayNode.NodeTo;

        RebuildNodes(rebuildNodesList, nodes, outerMostParallelGatewayNode);
        RebuildNodes(rebuildNodesList, nodes, itsAggregationNode);

        foreach (var nodeTo in nodeTos)
        {
            if (!mapNodes.TryGetValue(nodeTo, out var currentNodeVo))
            {
                continue;
            }

            // Treat all nodes between parallel gateway and its aggregation node (not including the latter)
            for (var nodeVo = currentNodeVo;
                 nodeVo != null && !nodeVo.NodeId.Equals(aggregationNodeNodeId);
                 nodeVo = mapNodes.TryGetValue(nodeVo.Params.NodeTo, out var nextNode) ? nextNode : null)
            {
                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nodeVo.NodeType)
                {
                    var aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                    TreatParallelGatewayRecursively(nodeVo, aggregationNode, mapNodes, nodes, rebuildNodesList);
                }
                else
                {
                    RebuildNodes(rebuildNodesList, nodes, nodeVo);
                }
            }
        }
    }
    /// <summary>
    /// reviewed
    /// </summary>
    /// <param name="rebuildNodesList"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeVo"></param>
    /// <exception cref="AFBizException"></exception>
    private void RebuildNodes(List<BpmnNodeVo> rebuildNodesList, List<BpmnNodeVo> nodes, BpmnNodeVo nodeVo)
    {
        BpmnNodeParamsVo nodeParamsVo = nodeVo.Params;

        if (nodeParamsVo.IsNodeDeduplication == 1)
        {
            // Skip deduplicated node and rebuild nodeTo
            var nextNodeTo = GetNodeTo(nodeVo);
            List<BpmnNodeVo> nodeVos = rebuildNodesList.Where(a => nodeVo.NodeId == a.Params.NodeTo).ToList();
            if(nodeVos.Count > 1)
            {
                BpmnNodeVo vo = nodeVos[0];
                if((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY==vo.NodeType)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(vo,nodes);
                    if(aggregationNode==null){
                        throw new AFBizException("there is a parallel gateway node,but can not get its aggregation node");
                    }
                    if(aggregationNode.NodeId==nextNodeTo){
                        nodeVo.Params.IsNodeDeduplication=0;
                        nodeVo.DeduplicationExclude=true;
                        rebuildNodesList.Add(nodeVo);
                        return;
                    }
                }

                vo.Params.NodeTo = nextNodeTo;
                List<String> nodeTo = vo.NodeTo;
                List<String> newNodeTos=new List<string>();
                foreach (String s in nodeTo) {
                    if(s==nodeVo.NodeId){
                        newNodeTos.Add(nextNodeTo);
                    }else{
                        newNodeTos.Add(s);
                    }
                }
                vo.NodeTo=newNodeTos;
            }
            else
            {
                BpmnNodeVo? maybeParallelGateway = rebuildNodesList
                    .FirstOrDefault(a => a.NodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY);
                if (maybeParallelGateway != null)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(maybeParallelGateway, nodes);
                    if(aggregationNode==null){
                        throw new AFBizException("there is a parallel gateway node,but can not get its aggregation node");
                    }
                    nodeVo.Params.IsNodeDeduplication=0;
                    nodeVo.DeduplicationExclude=true;
                    rebuildNodesList.Add(nodeVo);
                }
            }
        }
        else
        {
            rebuildNodesList.Add(nodeVo);
           
        }
    }

    /// <summary>
    /// reviewed
    /// </summary>
    /// <param name="outerMostParallelGatewayNode"></param>
    /// <param name="itsAggregationNode"></param>
    /// <param name="mapNodes"></param>
    /// <param name="bpmnConfCommonElementVos"></param>
    /// <param name="rebuildNodesList"></param>
    /// <param name="nextNodeTo"></param>
    /// <param name="nodeCode"></param>
    /// <param name="sequenceFlowNum"></param>
    /// <param name="numMap"></param>
    /// <param name="lastAggNode"></param>
    /// <param name="alreadyProcessNodeIds"></param>
    /// <exception cref="AFBizException"></exception>
    private void TreatParallelGatewayRecursively(BpmnNodeVo outerMostParallelGatewayNode,
        BpmnNodeVo itsAggregationNode,
        Dictionary<string, BpmnNodeVo> mapNodes,
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        List<BpmnNodeVo> rebuildNodesList,
        string nextNodeTo,
        int nodeCode, int sequenceFlowNum,
        Dictionary<string, int> numMap,
        BpmnNodeVo lastAggNode,
        HashSet<String> alreadyProcessNodeIds)
    {
        if (itsAggregationNode == null)
        {
            throw new AFBizException("There is a parallel gateway node, but cannot get its aggregation node!");
        }

        string aggregationNodeNodeId = itsAggregationNode.NodeId;
        List<string> nodeTos = outerMostParallelGatewayNode.NodeTo;
        string parallelGatewayNodeNodeId = outerMostParallelGatewayNode.NodeId;
        int gateWaySF = numMap["sequenceFlowNum"];

        foreach (string nodeTo in nodeTos)
        {
            BpmnNodeVo currentNodeVo = mapNodes[nodeTo];
            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(
                numMap["sequenceFlowNum"] + 1,
                $"gateWay{gateWaySF}",
                ProcessNodeEnum.GetDescByCode(numMap["nodeCode"] + 1)));

            // Process nodes between parallel gateway and its aggregation node (excluding the latter)
            for (BpmnNodeVo nodeVo = currentNodeVo;
                 nodeVo != null && nodeVo.NodeId != aggregationNodeNodeId;
                 nodeVo = mapNodes.TryGetValue(nodeVo.Params.NodeTo, out var nextNode) ? nextNode : null)
            {
                if(alreadyProcessNodeIds.Contains(nodeVo.NodeId)){
                    continue;
                }
                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nodeVo.NodeType)
                {
                    alreadyProcessNodeIds.Add(nodeVo.NodeId);
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                    lastAggNode = aggregationNode;
                    int parallelGatewayNodeCode = numMap["nodeCode"] + 1;
                    int parallelGatewaySequenceFlowNum = numMap["sequenceFlowNum"] + 1;
                    numMap["nodeCode"] = parallelGatewayNodeCode;
                    numMap["sequenceFlowNum"] = parallelGatewaySequenceFlowNum;

                    BpmnConfCommonElementVo parallelGatewayElement =
                        BpmnElementUtils.GetParallelGateWayElement(parallelGatewaySequenceFlowNum);
                    bpmnConfCommonElementVos.Add(parallelGatewayElement);
                    bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(
                        parallelGatewaySequenceFlowNum,
                        ProcessNodeEnum.GetDescByCode(parallelGatewayNodeCode - 1),
                        parallelGatewayElement.ElementId));

                    TreatParallelGatewayRecursively(nodeVo, aggregationNode, mapNodes,
                        bpmnConfCommonElementVos, rebuildNodesList, nodeVo.NodeId,
                        numMap["nodeCode"], numMap["sequenceFlowNum"], numMap,aggregationNode,alreadyProcessNodeIds);
                }
                else
                {
                    alreadyProcessNodeIds.Add(nodeVo.NodeId);
                    if (nodeVo == itsAggregationNode)
                    {
                        List<BpmnNodeVo> nodeFroms = GetNodeFroms(rebuildNodesList, nodeVo);
                        nodeVo.FromNodes = nodeFroms;
                    }

                    FormatNodesToElements(bpmnConfCommonElementVos, rebuildNodesList,
                        nodeVo.NodeId, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
                }
            }
        }
    }
    /// <summary>
    /// Recursively format BpmnNodeVo to BpmnConfCommonElementVo.
    /// </summary>
    /// <param name="bpmnConfCommonElementVos">List of BpmnConfCommonElementVo elements.</param>
    /// <param name="rebuildNodesList">Rebuild nodes list.</param>
    /// <param name="nodeTo">Target node.</param>
    /// <param name="nodeCode">Current node code.</param>
    /// <param name="sequenceFlowNum">Sequence flow number.</param>
    /// <param name="numMap">Map to store incremental values.</param>
    private Dictionary<string, int> FormatNodesToElements(
        List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        List<BpmnNodeVo> rebuildNodesList,
        string nodeTo,
        int nodeCode,
        int sequenceFlowNum,
        Dictionary<string, int> numMap)
    {
        // Get next node according to nodeTo
        BpmnNodeVo nextNodeVo = GetNextNodeVo(rebuildNodesList, nodeTo);

        // Use node property to get the adaptor and format nodeVo to elementVo
        NodePropertyEnum? nodePropertyEnum = NodePropertyEnumExtensions.GetByCode(nextNodeVo.NodeProperty);
        if (nodePropertyEnum == null)
        {
            throw new AFBizException($"can not get node property enum by value:{nextNodeVo.NodeProperty}!");
        }
        BpmnElementAdaptor elementAdaptor = _adaptorFactory.GetBpmnElementAdaptor(nodePropertyEnum.Value);
        if (elementAdaptor == null)
        {
            throw new AFBizException($"can not get element adaptor by node property:{nodePropertyEnum}");
        }
        elementAdaptor.DoFormatNodesToElements(bpmnConfCommonElementVos,nextNodeVo,nodeCode, sequenceFlowNum, numMap);
        // If there is still a next node, then continue formatting
        string nextNodeTo = GetNodeTo(nextNodeVo);
        
        return numMap;
    }

    private List<BpmnNodeVo> GetNodeFroms(List<BpmnNodeVo> nodes, BpmnNodeVo currentNode)
    {
        if (currentNode == null)
        {
            throw new AFBizException("Cannot set null to current node");
        }

        List<BpmnNodeVo> results = new List<BpmnNodeVo>();
        foreach (BpmnNodeVo node in nodes)
        {
            if (node.Params?.NodeTo != null && node.Params.NodeTo.Equals(currentNode.NodeId))
            {
                results.Add(node);
            }
        }

        return results;
    }

    private BpmnNodeVo GetNextNodeVo(List<BpmnNodeVo> nodes, string nodeTo)
    {
        List<BpmnNodeVo> nextNodeVo = nodes
            .Where(o => o.NodeId == nodeTo)
            .ToList();

        if (nextNodeVo == null || !nextNodeVo.Any())
        {
            throw new AFBizException("未找到下一节点流程发起失败");
        }

        return nextNodeVo[0];
    }
    private void SetStartNodeElementButtons(BpmnNodeVo startUserNode, BpmnConfCommonElementVo startNodeElement)
    {
        startNodeElement.Buttons = new BpmnConfCommonButtonsVo
        {
            StartPage = startUserNode.Buttons.StartPage
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList(),
            ApprovalPage = startUserNode.Buttons.ApprovalPage
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList()
        };
    }


}