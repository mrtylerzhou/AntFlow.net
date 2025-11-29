using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.service.processor;
using antflowcore.service.processor.filter;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace antflowcore.service.repository;

public class BpmnConfCommonService
{
    private readonly BpmnConfBizService _bpmnConfBizService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmnStartFormatFactory _bpmnStartFormatFactory;
    private readonly IBpmnPersonnelFormat _personnelFormat;
    private readonly IBpmnDeduplicationFormat _deduplicationFormat;
    private readonly IBpmnOptionalDuplicatesAdaptor _optionalDuplicatesAdaptor;
    private readonly BpmnRemoveConfFormatFactory _bpmnRemoveConfFormatFactory;
    private readonly BpmnNodeFormatService _bpmnNodeFormatService;
    private readonly BpmnInsertVariablesService _bpmnInsertVariablesService;
    private readonly BpmnCreateAndStartService _bpmnCreateAndStartService;
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly BpmVariableService _bpmVariableService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly FormFactory _formFactory;
    private readonly IFreeSql _freeSql;
    private readonly ILogger<BpmnConfCommonService> _logger;

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
        BpmFlowrunEntrustService flowrunEntrustService,
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
        _flowrunEntrustService = flowrunEntrustService;
        _formFactory = formFactory;
        _freeSql = freeSql;
        _logger = logger;
    }

    public BpmnConf GetBpmnConfByFormCode(String formCode)
    {
        BpmnConf bpmnConf = _freeSql
            .Select<BpmnConf>()
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToOne() ?? new BpmnConf();
        return bpmnConf;
    }

    public void StartProcess(String bpmnCode, BpmnStartConditionsVo bpmnStartConditions)
    {
        //to query the process's config information
        BpmnConfVo bpmnConfVo = _bpmnConfBizService.Detail(bpmnCode);
        // format process's floating direction,set assignees,assignees deduplication and remove some nodes on conditions
        BpmnConfVo confVo = GetBpmnConfVo(bpmnStartConditions, bpmnConfVo);

        //to convert the process element information
        //set some basic information
        BpmnConfCommonVo bpmnConfCommonVo = new BpmnConfCommonVo
        {
            BpmnCode = confVo.BpmnCode,
            FormCode = confVo.FormCode,
            BpmnName = confVo.BpmnName,
            ProcessNum = bpmnStartConditions.ProcessNum,
            ProcessName = bpmnConfVo.BpmnName,
            ProcessDesc = bpmnStartConditions.ProcessDesc,
            TemplateVos = bpmnConfVo.TemplateVos,
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


        //2、set consignees information and finally determine the flow's direction
        _personnelFormat.FormatPersonnelsConf(bpmnConfVo, bpmnStartConditions);
        //3. to determine whether it is necessary to deduplication
        if (bpmnConfVo.DeduplicationType != (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_NULL)
        {
            if (bpmnConfVo.DeduplicationType == (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_FORWARD)
            {
                //deduplication forward
                _deduplicationFormat.ForwardDeduplication(bpmnConfVo, bpmnStartConditions);
            }
            else if (bpmnConfVo.DeduplicationType == (int)(DeduplicationTypeEnum.DEDUPLICATION_TYPE_BACKWARD))
            {
                //deduplication backword
                _deduplicationFormat.BackwardDeduplication(bpmnConfVo, bpmnStartConditions);
            }
        }

        _optionalDuplicatesAdaptor.OptionalDuplicate(bpmnConfVo, bpmnStartConditions);
        //4、format the nodes by pipelines
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
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList(),

            ViewPageOther = bpmnConfVo.ViewPageButtons.ViewPageOther
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList()
        };
    }

    public PreviewNode PreviewNode(String parameters)
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
        var vo = _formFactory.DataFormConversion(JsonSerializer.Serialize(jobj), null);
        vo.IsOutSideAccessProc = detail.IsOutSideProcess == 1;
        vo.IsLowCodeFlow = detail.IsLowCodeFlow;
        vo.BpmnConfVo = detail;
        vo.IsStartPagePreview = isStartPagePreview;

        var bpmnStartConditionsExtendVo = new BpmnStartConditionsVo { IsLowCodeFlow = true };

        // 设置发起人信息
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
        PreviewNode previewNode = new PreviewNode
        {
            BpmnName = detail.BpmnName,
            FormCode = detail.FormCode,
            BpmnNodeList = SetNodeFromV2(bpmnConfVo.Nodes),
            DeduplicationType = bpmnConfVo.DeduplicationType,
            DeduplicationTypeName = DeduplicationTypeEnumExtensions.GetDescByCode(bpmnConfVo.DeduplicationType.Value)
        };
        ReTreatNodeAssignee(previewNode.BpmnNodeList,vo.ProcessNumber);
        string currentNodeIdStr = _bpmVerifyInfoBizService.FindCurrentNodeIds(vo.ProcessNumber);
        previewNode.CurrentNodeId = currentNodeIdStr;

        List<string> currentNodeIds = currentNodeIdStr.Split(',').ToList();
        var bpmnNodeVoMap = previewNode.BpmnNodeList.ToDictionary(n => n.NodeId, n => n);

        List<string> nodeToResults = new List<string>();
        ProcessNodeToRecursively(currentNodeIds, bpmnNodeVoMap, nodeToResults);
        previewNode.AfterNodeIds = nodeToResults;

        List<string> nodeFromResults = new List<string>();
        HashSet<string> allNodeIds = new HashSet<string>(bpmnNodeVoMap.Keys);
        nodeFromResults.AddRange(allNodeIds.Where(o => !nodeToResults.Contains(o) && !currentNodeIds.Contains(o)));

        previewNode.BeforeNodeIds = nodeFromResults;

        return previewNode;
    }

    public List<BpmnNodeVo> SetNodeFromV2(List<BpmnNodeVo> nodeList)
    {
        var map = nodeList.ToDictionary(node => node.NodeId);
        BpmnNodeVo startNode = GetNodeMapAndStartNode(nodeList, map);
        List<BpmnNodeVo> resultList = new List<BpmnNodeVo>();
        BpmnNodeVo lastNode = new BpmnNodeVo { NodeId = "" };
        BpmnNodeVo nowNode = startNode;

        if (nowNode != null)
        {
            while (true)
            {
                if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == nowNode.NodeType)
                {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nowNode, nodeList);
                    TreatParallelGateWayRecursively(nowNode, lastNode, aggregationNode, map, resultList,
                        new HashSet<string>());

                    nowNode = map.TryGetValue(aggregationNode.Params.NodeTo??"", out var nextNode) ? nextNode : null;
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
                        throw new AFBizException("999", "nodeId数据错误");
                    }

                    nowNode.NodeFrom = lastNode.NodeId;
                    resultList.Add(nowNode);
                    lastNode = nowNode;
                    nowNode = map.TryGetValue(nowNode.Params.NodeTo, out var nextNode) ? nextNode : null;
                }
            }
        }

        return resultList;
    }

    private static BpmnNodeVo GetNodeMapAndStartNode(List<BpmnNodeVo> nodeList, Dictionary<string, BpmnNodeVo> nodeIdMapNode)
    {
        BpmnNodeVo startNode = null;
        bool existEnd = false;

        foreach (var bpmnNodeVo in nodeList)
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
                    throw new AFBizException("999", "has more than 1 start up node");
                }
            }

            if (bpmnNodeVo.Params == null || string.IsNullOrWhiteSpace(bpmnNodeVo.Params.NodeTo))
            {
                existEnd = true;
            }
        }

        if (!existEnd)
        {
           
            throw new AFBizException("has not end node while previewing the process");
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
            if (!mapNodes.TryGetValue(nodeTo, out var currentNodeVo)) continue;

            for (BpmnNodeVo nodeVo = currentNodeVo;
                 nodeVo != null && nodeVo.NodeId != aggregationNodeNodeId;
                 nodeVo = mapNodes.TryGetValue(nodeVo.Params.NodeTo, out var nextNode) ? nextNode : null)
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
                    throw new AFBizException("999", "nodeId数据错误");
                }

                nodeVo.NodeFrom = prevNode.NodeId;
                results.Add(nodeVo);
                alreadyProcessNodeIds.Add(nodeVo.NodeId);
                prevNode = nodeVo;
            }
        }
    }
    private void ProcessNodeToRecursively(List<string> currentNodeIds, Dictionary<string, BpmnNodeVo> bpmnNodeVoMap, List<string> results)
    {
        if (currentNodeIds.Count == 0)
        {
            return;
        }

        foreach (var currentNodeId in currentNodeIds)
        {
            if (bpmnNodeVoMap.TryGetValue(currentNodeId, out var bpmnNodeVo))
            {
                var nodeTo = bpmnNodeVo.NodeTo;
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
        // 解析 JSON 请求参数
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
            throw new Exception("BpmVariable not found or processStartConditions is empty");
        }

        // 解析 processStartConditions 并添加额外字段
        JsonNode? objectStart = JsonNode.Parse(bpmnVariable.ProcessStartConditions);

        objectStart["bpmnCode"] = bpmnVariable.BpmnCode;
        objectStart["isLowCodeFlow"] = isLowCodeFlow;
        objectStart["processNumber"] = processNumber;
        return GetPreviewNode(objectStart.ToJsonString(), false);
            
    }

    public List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes)
    {
        List<BpmnConf> bpmnConfs = _bpmnConfService
            .baseRepo
            .Where(a=>formCodes.Contains(a.FormCode)&&a.EffectiveStatus==1)
            .ToList();
        return bpmnConfs;
    }

    private void ReTreatNodeAssignee(List<BpmnNodeVo> nodeVos,string processNumber)
    {
        if (string.IsNullOrEmpty(processNumber))
        {
            return;
        }

        List<BpmFlowrunEntrust> bpmFlowrunEntrusts = _flowrunEntrustService
            .Frsql
            .Select<BpmFlowrunEntrust, BpmBusinessProcess>()
            .InnerJoin((a, b) => a.RunInfoId == b.ProcInstId)
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList<BpmFlowrunEntrust>();
         
         Dictionary<string, List<BpmFlowrunEntrust>> nodeId2entrustDict= bpmFlowrunEntrusts.Where(a=>!string.IsNullOrEmpty(a.NodeId)).GroupBy(a => a.NodeId)
            .ToDictionary(g => g.Key, g => g.ToList());
        foreach (BpmnNodeVo bpmnNodeVo in nodeVos)
        {
            int nodeType = bpmnNodeVo.NodeType;
            if (nodeType == (int)NodeTypeEnum.NODE_TYPE_START || nodeType == (int)NodeTypeEnum.NODE_TYPE_GATEWAY ||
                nodeType == (int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY ||
                nodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITIONS)
            {
                continue;
            }

            BpmnNodePropertysVo bpmnNodePropertysVo = bpmnNodeVo.Property;
            if (bpmnNodePropertysVo == null)
            {
                continue;
            }

            List<BpmFlowrunEntrust> flowrunEntrusts = nodeId2entrustDict.GetValueOrDefault(bpmnNodeVo.Id.ToString(), null);
            if (flowrunEntrusts.IsEmpty())
            {
                continue;
            }
            List<BaseIdTranStruVo> emplList = bpmnNodePropertysVo.EmplList;
            if (emplList.IsEmpty())
            {
                continue;
            }
            IEnumerable<IGrouping<int?,BpmFlowrunEntrust>> groupBy = flowrunEntrusts.GroupBy(a=>a.ActionType);
            foreach (IGrouping<int?,BpmFlowrunEntrust> entrustse in groupBy)
            {
                int? actionType = entrustse.Key;
                if (actionType == null)
                {
                    continue;
                }
                foreach (BpmFlowrunEntrust bpmFlowrunEntrust in entrustse)
                {
                    BaseIdTranStruVo? matchEmp = emplList.FirstOrDefault(a => a.Id==bpmFlowrunEntrust.Original);
                    if (matchEmp == null)
                    {
                        continue;
                    }
                    if (actionType == 0 || actionType == 1)//change assignee
                    {
                        matchEmp.Id = bpmFlowrunEntrust.Actual;
                        matchEmp.Name = bpmFlowrunEntrust.ActualName + "*";
                    }else if (actionType == 2)//add asignee
                    {
                        BaseIdTranStruVo addEmp =
                            new BaseIdTranStruVo(bpmFlowrunEntrust.Actual, bpmFlowrunEntrust.ActualName+"+");
                        emplList.Add(addEmp);
                    }else if (actionType == 3)//remove assignee
                    {
                        matchEmp.Name = matchEmp.Name + "-";
                    }
                }
            }

            if (emplList.Count == 0)
            {
                emplList.Add(new BaseIdTranStruVo("0","*"));
            }

            List<string> emplIds = emplList.Select(a=>a.Id).ToList();
            bpmnNodePropertysVo.EmplIds = emplIds;
            bpmnNodePropertysVo.EmplList = emplList;
        }
       
    }
}