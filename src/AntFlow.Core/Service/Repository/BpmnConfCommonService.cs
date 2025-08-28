using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Factory;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Processor;
using AntFlow.Core.Service.Processor.Filter;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;
using System.Text.Json.Nodes;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfCommonService
{
    private readonly BpmnConfBizService _bpmnConfBizService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmnCreateAndStartService _bpmnCreateAndStartService;
    private readonly BpmnInsertVariablesService _bpmnInsertVariablesService;
    private readonly BpmnNodeFormatService _bpmnNodeFormatService;
    private readonly BpmnRemoveConfFormatFactory _bpmnRemoveConfFormatFactory;
    private readonly BpmnStartFormatFactory _bpmnStartFormatFactory;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly IBpmnDeduplicationFormat _deduplicationFormat;
    private readonly FormFactory _formFactory;
    private readonly IFreeSql _freeSql;
    private readonly ILogger<BpmnConfCommonService> _logger;
    private readonly IBpmnOptionalDuplicatesAdaptor _optionalDuplicatesAdaptor;
    private readonly IBpmnPersonnelFormat _personnelFormat;

    public BpmnConfCommonService(
        BpmnConfBizService bpmnConfBizService,
        BpmnConfService bpmnConfService,
        BpmnStartFormatFactory bpmnStartFormatFactory,
        IBpmnPersonnelFormat personnelFormat,
        IBpmnDeduplicationFormat deduplicationFormat,
        IBpmnOptionalDuplicatesAdaptor optionalDuplicatesAdaptor,
        BpmnRemoveConfFormatFactory bpmnRemoveConfFormatFactory,
        BpmnNodeFormatService bpmnNodeFormatService,
        BpmnInsertVariablesService bpmnInsertVariablesService,
        BpmnCreateAndStartService bpmnCreateAndStartService,
        BpmVerifyInfoBizService bpmVerifyInfoBizService,
        BpmVariableService bpmVariableService,
        FormFactory formFactory,
        IFreeSql freeSql,
        ILogger<BpmnConfCommonService> logger)
    {
        _bpmnConfBizService = bpmnConfBizService;
        _bpmnConfService = bpmnConfService;
        _bpmnStartFormatFactory = bpmnStartFormatFactory;
        _personnelFormat = personnelFormat;
        _deduplicationFormat = deduplicationFormat;
        _optionalDuplicatesAdaptor = optionalDuplicatesAdaptor;
        _bpmnRemoveConfFormatFactory = bpmnRemoveConfFormatFactory;
        _bpmnNodeFormatService = bpmnNodeFormatService;
        _bpmnInsertVariablesService = bpmnInsertVariablesService;
        _bpmnCreateAndStartService = bpmnCreateAndStartService;
        _bpmVerifyInfoBizService = bpmVerifyInfoBizService;
        _bpmVariableService = bpmVariableService;
        _formFactory = formFactory;
        _freeSql = freeSql;
        _logger = logger;
    }

    public BpmnConf GetBpmnConfByFormCode(string formCode)
    {
        BpmnConf bpmnConf = _freeSql
            .Select<BpmnConf>()
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToOne() ?? new BpmnConf();
        return bpmnConf;
    }

    public void StartProcess(string bpmnCode, BpmnStartConditionsVo bpmnStartConditions)
    {
        //to query the process's config information
        BpmnConfVo bpmnConfVo = _bpmnConfBizService.Detail(bpmnCode);
        // format process's floating direction,set assignees,assignees deduplication and remove some nodes on conditions
        BpmnConfVo confVo = GetBpmnConfVo(bpmnStartConditions, bpmnConfVo);

        //to convert the process element information
        //set some basic information
        BpmnConfCommonVo bpmnConfCommonVo = new()
        {
            BpmnCode = confVo.BpmnCode,
            FormCode = confVo.FormCode,
            BpmnName = confVo.BpmnName,
            ProcessNum = bpmnStartConditions.ProcessNum,
            ProcessName = bpmnConfVo.BpmnName,
            ProcessDesc = bpmnStartConditions.ProcessDesc,
            TemplateVos = bpmnConfVo.TemplateVos
        };
        //set view page's buttons information
        SetViewPageButtons(bpmnConfVo, bpmnConfCommonVo);

        List<BpmnConfCommonElementVo> bpmnConfCommonElementVoList =
            _bpmnNodeFormatService.GetBpmnConfCommonElementVoList(bpmnConfCommonVo, bpmnConfVo.Nodes,
                bpmnStartConditions);
        bpmnConfCommonVo.ElementList = bpmnConfCommonElementVoList;
        _bpmnInsertVariablesService.InsertVariables(bpmnConfCommonVo, bpmnStartConditions);
        _bpmnCreateAndStartService.CreateBpmnAndStart(bpmnConfCommonVo, bpmnStartConditions);
    }

    private BpmnConfVo GetBpmnConfVo(BpmnStartConditionsVo bpmnStartConditions, BpmnConfVo bpmnConfVo)
    {
        //1. Format the process,filter it by condition
        _bpmnStartFormatFactory.formatBpmnConf(bpmnConfVo, bpmnStartConditions);


        //2��set consignees information and finally determine the flow's direction
        _personnelFormat.FormatPersonnelsConf(bpmnConfVo, bpmnStartConditions);
        //3. to determine whether it is necessary to deduplication
        if (bpmnConfVo.DeduplicationType != (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_NULL)
        {
            if (bpmnConfVo.DeduplicationType == (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_FORWARD)
            {
                //deduplication forward
                _deduplicationFormat.ForwardDeduplication(bpmnConfVo, bpmnStartConditions);
            }
            else if (bpmnConfVo.DeduplicationType == (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_BACKWARD)
            {
                //deduplication backword
                _deduplicationFormat.BackwardDeduplication(bpmnConfVo, bpmnStartConditions);
            }
        }

        _optionalDuplicatesAdaptor.OptionalDuplicate(bpmnConfVo, bpmnStartConditions);
        //4��format the nodes by pipelines
        _bpmnRemoveConfFormatFactory.RemoveBpmnConf(bpmnConfVo, bpmnStartConditions);
        return bpmnConfVo;
    }

    private void SetViewPageButtons(BpmnConfVo bpmnConfVo, BpmnConfCommonVo bpmnConfCommonVo)
    {
        bpmnConfCommonVo.ViewPageButtons = new BpmnConfViewPageButtonVo
        {
            ViewPageStart = bpmnConfVo.ViewPageButtons.ViewPageStart
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o, ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList(),
            ViewPageOther = bpmnConfVo.ViewPageButtons.ViewPageOther
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o, ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList()
        };
    }

    public PreviewNode PreviewNode(string parameters)
    {
        return GetPreviewNode(parameters, false);
    }

    public PreviewNode GetPreviewNode(string parameters, bool isStartPagePreview)
    {
        BusinessDataVo dataVo = JsonSerializer.Deserialize<BusinessDataVo>(parameters);

        BpmnConfVo detail = isStartPagePreview
            ? _bpmnConfBizService.DetailByFormCode(dataVo.FormCode)
            : _bpmnConfBizService.Detail(dataVo.BpmnCode);
        dataVo.FormCode = detail.FormCode;
        JsonNode jobj = JsonSerializer.Deserialize<JsonNode>(parameters);
        jobj["formCode"] = detail.FormCode;
        BusinessDataVo? vo = _formFactory.DataFormConversion(JsonSerializer.Serialize(jobj), null);
        vo.IsOutSideAccessProc = detail.IsOutSideProcess == 1;
        vo.IsLowCodeFlow = detail.IsLowCodeFlow;
        vo.BpmnConfVo = detail;
        vo.IsStartPagePreview = isStartPagePreview;

        BpmnStartConditionsVo? bpmnStartConditionsExtendVo = new() { IsLowCodeFlow = true };

        // ���÷�������Ϣ
        string startUserId;
        if (isStartPagePreview)
        {
            startUserId = dataVo.IsOutSideAccessProc != null && dataVo.IsOutSideAccessProc.Value
                ? dataVo.StartUserId
                : SecurityUtils.GetLogInEmpIdStr();
            vo.StartUserId = startUserId;
        }
        else
        {
            startUserId = vo.StartUserId;
            if (string.IsNullOrEmpty(startUserId))
            {
                vo.StartUserId = SecurityUtils.GetLogInEmpIdSafe();
            }
        }

        if (!string.IsNullOrEmpty(startUserId))
        {
            bpmnStartConditionsExtendVo.StartUserId = startUserId;
            // TODO: Set start condition
        }

        BpmnStartConditionsVo bpmnStartConditionsVo;
        if (dataVo.IsOutSideAccessProc != null && dataVo.IsOutSideAccessProc.Value)
        {
            bpmnStartConditionsVo = new BpmnStartConditionsVo
            {
                TemplateMarkIds = dataVo.TemplateMarkIds,
                OutSideType = dataVo.OutSideType,
                EmbedNodes = dataVo.EmbedNodes,
                IsOutSideAccessProc = true,
                StartUserId = dataVo.StartUserId
            };
        }
        else
        {
            bpmnStartConditionsVo = _formFactory.GetFormAdaptor(vo).PreviewSetCondition(vo);
        }

        bpmnStartConditionsVo.ApproversList = dataVo.ApproversList;
        bpmnStartConditionsVo.StartUserId = vo.StartUserId;
        bpmnStartConditionsVo.IsPreview = true;

        BpmnConfVo bpmnConfVo = GetBpmnConfVo(bpmnStartConditionsVo, detail);
        PreviewNode previewNode = new()
        {
            BpmnName = detail.BpmnName,
            FormCode = detail.FormCode,
            BpmnNodeList = SetNodeFromV2(bpmnConfVo.Nodes),
            DeduplicationType = bpmnConfVo.DeduplicationType,
            DeduplicationTypeName =
                DeduplicationTypeEnumExtensions.GetDescByCode(bpmnConfVo.DeduplicationType.Value)
        };

        string currentNodeIdStr = _bpmVerifyInfoBizService.FindCurrentNodeIds(vo.ProcessNumber);
        previewNode.CurrentNodeId = currentNodeIdStr;

        List<string> currentNodeIds = currentNodeIdStr.Split(',').ToList();
        Dictionary<string, BpmnNodeVo>? bpmnNodeVoMap = previewNode.BpmnNodeList.ToDictionary(n => n.NodeId, n => n);

        List<string> nodeToResults = new();
        ProcessNodeToRecursively(currentNodeIds, bpmnNodeVoMap, nodeToResults);
        previewNode.AfterNodeIds = nodeToResults;

        List<string> nodeFromResults = new();
        HashSet<string> allNodeIds = new(bpmnNodeVoMap.Keys);
        nodeFromResults.AddRange(allNodeIds.Where(o => !nodeToResults.Contains(o) && !currentNodeIds.Contains(o)));

        previewNode.BeforeNodeIds = nodeFromResults;

        return previewNode;
    }

    public List<BpmnNodeVo> SetNodeFromV2(List<BpmnNodeVo> nodeList)
    {
        Dictionary<string, BpmnNodeVo>? map = nodeList.ToDictionary(node => node.NodeId);
        BpmnNodeVo startNode = GetNodeMapAndStartNode(nodeList, map);
        List<BpmnNodeVo>? resultList = new();
        BpmnNodeVo? lastNode = new() { NodeId = "" };
        BpmnNodeVo? nowNode = startNode;

        if (nowNode != null)
        {
            while (true)
            {
                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nowNode.NodeType)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nowNode, nodeList);
                    TreatParallelGateWayRecursively(nowNode, lastNode, aggregationNode, map, resultList,
                        new HashSet<string>());

                    nowNode = map.TryGetValue(aggregationNode.Params.NodeTo ?? "", out BpmnNodeVo? nextNode)
                        ? nextNode
                        : null;
                    lastNode = aggregationNode;
                }

                if (nowNode == null)
                {
                    break;
                }

                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY != nowNode.NodeType)
                {
                    if ((int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE == nowNode.Params.ParamType)
                    {
                        nowNode.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { nowNode.Params.Assignee };
                    }

                    nowNode.NodePropertyName = NodePropertyEnumExtensions.GetDescByCode(nowNode.NodeProperty);

                    if (string.IsNullOrWhiteSpace(nowNode.Params.NodeTo))
                    {
                        nowNode.NodeFrom = lastNode.NodeId;
                        resultList.Add(nowNode);
                        break;
                    }

                    if (resultList.Count > nodeList.Count)
                    {
                        _logger.LogInformation(
                            $"error occur while set nodeFrom info, nodeList: {JsonSerializer.Serialize(nodeList)}");
                        throw new AntFlowException.AFBizException("999", "nodeId���ݴ���");
                    }

                    nowNode.NodeFrom = lastNode.NodeId;
                    resultList.Add(nowNode);
                    lastNode = nowNode;
                    nowNode = map.TryGetValue(nowNode.Params.NodeTo, out BpmnNodeVo? nextNode) ? nextNode : null;
                }
            }
        }

        return resultList;
    }

    private static BpmnNodeVo GetNodeMapAndStartNode(List<BpmnNodeVo> nodeList,
        Dictionary<string, BpmnNodeVo> nodeIdMapNode)
    {
        BpmnNodeVo startNode = null;
        bool existEnd = false;

        foreach (BpmnNodeVo? bpmnNodeVo in nodeList)
        {
            nodeIdMapNode[bpmnNodeVo.NodeId] = bpmnNodeVo;

            if ((int)NodeTypeEnum.NODE_TYPE_START == bpmnNodeVo.NodeType)
            {
                if (startNode == null)
                {
                    startNode = bpmnNodeVo;
                }
                else
                {
                    throw new AntFlowException.AFBizException("999", "has more than 1 start up node");
                }
            }

            if (bpmnNodeVo.Params == null || string.IsNullOrWhiteSpace(bpmnNodeVo.Params.NodeTo))
            {
                existEnd = true;
            }
        }

        if (!existEnd)
        {
            throw new AntFlowException.AFBizException("has not end node while previewing the process");
        }

        return startNode;
    }

    private void TreatParallelGateWayRecursively(
        BpmnNodeVo outerMostParallelGatewayNode,
        BpmnNodeVo itsPrevNode,
        BpmnNodeVo itsAggregationNode,
        Dictionary<string, BpmnNodeVo> mapNodes,
        List<BpmnNodeVo> results,
        HashSet<string> alreadyProcessNodeIds)
    {
        string aggregationNodeNodeId = itsAggregationNode.NodeId;
        List<string> nodeTos = outerMostParallelGatewayNode.NodeTo;
        outerMostParallelGatewayNode.NodeFrom = itsPrevNode.NodeId;
        itsAggregationNode.NodeFrom = outerMostParallelGatewayNode.NodeId;
        results.Add(outerMostParallelGatewayNode);
        results.Add(itsAggregationNode);
        alreadyProcessNodeIds.Add(outerMostParallelGatewayNode.NodeId);
        alreadyProcessNodeIds.Add(itsAggregationNode.NodeId);

        foreach (string nodeTo in nodeTos)
        {
            BpmnNodeVo prevNode = outerMostParallelGatewayNode;
            if (!mapNodes.TryGetValue(nodeTo, out BpmnNodeVo? currentNodeVo))
            {
                continue;
            }

            for (BpmnNodeVo nodeVo = currentNodeVo;
                 nodeVo != null && nodeVo.NodeId != aggregationNodeNodeId;
                 nodeVo = mapNodes.TryGetValue(nodeVo.Params.NodeTo, out BpmnNodeVo? nextNode) ? nextNode : null)
            {
                if (alreadyProcessNodeIds.Contains(nodeVo.NodeId))
                {
                    continue;
                }

                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nodeVo.NodeType)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                    TreatParallelGateWayRecursively(nodeVo, prevNode, aggregationNode, mapNodes, results,
                        alreadyProcessNodeIds);
                }

                if ((int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE == nodeVo.Params.ParamType)
                {
                    nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { nodeVo.Params.Assignee };
                }

                nodeVo.NodePropertyName = NodePropertyEnumExtensions.GetDescByCode(nodeVo.NodeProperty);

                if (string.IsNullOrWhiteSpace(nodeVo.Params.NodeTo))
                {
                    nodeVo.NodeFrom = prevNode.NodeId;
                    results.Add(nodeVo);
                    alreadyProcessNodeIds.Add(nodeVo.NodeId);
                    break;
                }

                if (results.Count > mapNodes.Values.Count)
                {
                    _logger.LogInformation(
                        $"error occur while set nodeFrom info, nodeList: {JsonSerializer.Serialize(mapNodes.Values)}");
                    throw new AntFlowException.AFBizException("999", "nodeId���ݴ���");
                }

                nodeVo.NodeFrom = prevNode.NodeId;
                results.Add(nodeVo);
                alreadyProcessNodeIds.Add(nodeVo.NodeId);
                prevNode = nodeVo;
            }
        }
    }

    private void ProcessNodeToRecursively(List<string> currentNodeIds, Dictionary<string, BpmnNodeVo> bpmnNodeVoMap,
        List<string> results)
    {
        if (currentNodeIds.Count == 0)
        {
            return;
        }

        foreach (string? currentNodeId in currentNodeIds)
        {
            if (bpmnNodeVoMap.TryGetValue(currentNodeId, out BpmnNodeVo? bpmnNodeVo))
            {
                List<string>? nodeTo = bpmnNodeVo.NodeTo;
                if (nodeTo != null && nodeTo.Count > 0)
                {
                    results.AddRange(nodeTo);
                    ProcessNodeToRecursively(nodeTo, bpmnNodeVoMap, results);
                }
            }
        }
    }

    public PreviewNode StartPagePreviewNode(string paramsJson)
    {
        return GetPreviewNode(paramsJson, true);
    }

    public PreviewNode TaskPagePreviewNode(string paramsJson)
    {
        // ���� JSON �������
        JsonNode? jsonObject = JsonNode.Parse(paramsJson);
        string? processNumber = jsonObject?["processNumber"]?.GetValue<string>();
        string? isLowcodeStr = jsonObject?["isLowCodeFlow"]?.ToString();
        bool isLowCodeFlow = !string.IsNullOrEmpty(isLowcodeStr) && Convert.ToBoolean(isLowcodeStr);

        if (string.IsNullOrEmpty(processNumber))
        {
            throw new ArgumentException("processNumber cannot be null or empty");
        }


        BpmVariable? bpmnVariable = _bpmVariableService.baseRepo.Where(a => a.ProcessNum == processNumber).First();


        if (bpmnVariable == null || string.IsNullOrEmpty(bpmnVariable.ProcessStartConditions))
        {
            throw new AntFlowException.AFBizException("BpmVariable not found or processStartConditions is empty");
        }

        // ���� processStartConditions ����Ӷ����ֶ�
        JsonNode? objectStart = JsonNode.Parse(bpmnVariable.ProcessStartConditions);

        if (objectStart != null)
        {
            objectStart["bpmnCode"] = bpmnVariable.BpmnCode;
            objectStart["isLowCodeFlow"] = isLowCodeFlow;
            objectStart["processNumber"] = processNumber;
            return GetPreviewNode(objectStart.ToJsonString(), false);
        }

        throw new AntFlowException.AFBizException("Failed to parse processStartConditions JSON");
    }

    public List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes)
    {
        List<BpmnConf> bpmnConfs = _bpmnConfService
            .baseRepo
            .Where(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1)
            .ToList();
        return bpmnConfs;
    }
}