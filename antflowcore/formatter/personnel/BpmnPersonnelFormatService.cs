using antflowcore.adaptor;
using antflowcore.adaptor.personnel;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.processor.personnel;

public class BpmnPersonnelFormatService: IBpmnPersonnelFormat
{
    private readonly IAdaptorFactory _adaptorFactory;
    private readonly ILogger<BpmnPersonnelFormatService> _logger;

    public BpmnPersonnelFormatService(
        IAdaptorFactory adaptorFactory,
        ILogger<BpmnPersonnelFormatService> logger)
    {
        _adaptorFactory = adaptorFactory;
        _logger = logger;
    }
    public BpmnConfVo FormatPersonnelsConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        //params check
        if (bpmnConfVo==null || bpmnStartConditions==null) {
            throw new AFBizException("the process's conf is null or empty,can not go on!");
        }
        if (ObjectUtils.IsEmpty(bpmnConfVo.Nodes)) {
            throw new AFBizException("process's nodes is empty,can not go on!");
        }

        if (string.IsNullOrEmpty(bpmnStartConditions.StartUserId)) {
            throw new AFBizException("process's start userId is null or empty,can not go on");
        }
        //approve
        //Integer startEmployeeId = bpmnStartConditions.getEmplId().intValue();
        //to find out the start node's id
        String startNodeId = null;

        Dictionary<String, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();

        Dictionary<String, BpmnNodeVo> mapPreNodes = new Dictionary<string, BpmnNodeVo>();
        
        HashSet<BpmnNodeVo> setAddNodes = new HashSet<BpmnNodeVo>();
        
        //many approve
        List<String> manyEmployeeIds = bpmnStartConditions.EmplIdList;
        foreach (BpmnNodeVo vo in bpmnConfVo.Nodes) {
            mapNodes.Add(vo.NodeId, vo);
            if (vo.NodeType==(int)NodeTypeEnum.NODE_TYPE_START) {
                startNodeId = vo.NodeId;
            }

            //最后一个节点的nodeto为空,.net里字典添加null就会报错
            if (!string.IsNullOrEmpty(vo.Params.NodeTo))
            {
                mapPreNodes[vo.Params.NodeTo]=vo;
            }
        }
        if (string.IsNullOrEmpty(startNodeId)) {
            throw new AFBizException("config error,can not find the start node id");
        }

        if (!mapNodes.ContainsKey(startNodeId)) {
            throw new AFBizException("config error,the start node is not in the nodes list");
        }
        BpmnNodeVo baseStart = mapNodes[startNodeId];
        //a node's consignees info
        BpmnNodeParamsVo paramsStartVo = baseStart.Params;
        
        
        //start node is a single assignee node
        paramsStartVo.ParamType=(int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE;
        BpmnNodeParamsAssigneeVo startResult = new BpmnNodeParamsAssigneeVo();
        startResult.Assignee=bpmnStartConditions.StartUserId;
        startResult.ElementName="发起人";
        paramsStartVo.Assignee=startResult;
        mapNodes[startNodeId].Params=paramsStartVo;
        
        
        String nextId = paramsStartVo.NodeTo;
        //if the process only have one node,then return
        if (string.IsNullOrEmpty(nextId)) {
            _logger.LogWarning("the process only have one start node,maybe there is something wrong!");
            bpmnConfVo.Nodes=mapNodes.Values.ToList();
            return bpmnConfVo;
        }
        /*use a while loop to process assignees*/
        do
        {
            if (!mapNodes.ContainsKey(nextId)) {
                throw new AFBizException("the processed node isn't in the nodes list,can not go on");
            }
            //base is the next node of the start node
            BpmnNodeVo baseNodeVo = mapNodes[nextId];
            //node's type
            //only processing assignee nodes
            if((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY==baseNodeVo.NodeType){
                BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(baseNodeVo, mapNodes.Values);
                TreatParallelGateWayRecursively(baseNodeVo,aggregationNode,mapNodes,setAddNodes,bpmnStartConditions);
                nextId=aggregationNode.NodeId;
            }else{
                ProcessAssigneeNode(baseNodeVo,mapPreNodes,setAddNodes,nextId,bpmnStartConditions);
                nextId=baseNodeVo.Params.NodeTo;
            }
        } while (!String.IsNullOrEmpty(nextId));
        
        List<BpmnNodeVo> finallyNodes = new List<BpmnNodeVo>();
        finallyNodes.AddRange(mapNodes.Values);

        if (!ObjectUtils.IsEmpty(setAddNodes)) {
            finallyNodes.AddRange(setAddNodes);
        }

        //bpmnConfVo.setNodes(new ArrayList<>(mapNodes.values()));
        bpmnConfVo.Nodes=finallyNodes;
        //then return
        return bpmnConfVo;
    }
    
    private void TreatParallelGateWayRecursively(BpmnNodeVo outerMostParallelGatewayNode,BpmnNodeVo itsAggregationNode,Dictionary<String, BpmnNodeVo> mapNodes,HashSet<BpmnNodeVo> setAddNodes,BpmnStartConditionsVo bpmnStartConditions){
        if (itsAggregationNode == null) {
            throw new AFBizException("there is a parallel gateway node,but can not get its aggregation node!");
        }
        String aggregationNodeNodeId = itsAggregationNode.NodeId;
        List<String> nodeTos = outerMostParallelGatewayNode.NodeTo;
        foreach (String nodeTo in nodeTos) {
            BpmnNodeVo currentNodeVo = mapNodes[nodeTo];
            //treat all nodes between parallel gateway and its aggregation node(not include the latter)
            for (BpmnNodeVo nodeVo = currentNodeVo; nodeVo!=null&&!nodeVo.NodeId.Equals(aggregationNodeNodeId); nodeVo = mapNodes[nodeVo.Params.NodeTo]) {
                if (NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY==(NodeTypeEnum)nodeVo.NodeType) {
                    BpmnNodeVo aggregationNode = BpmnUtils.GetAggregationNode(nodeVo, mapNodes.Values);
                    TreatParallelGateWayRecursively(nodeVo, aggregationNode, mapNodes,setAddNodes, bpmnStartConditions);
                }
                String nextId = nodeVo.Params.NodeTo;
                ProcessAssigneeNode(nodeVo,mapNodes,setAddNodes,nextId,bpmnStartConditions);
            }
        }
    }
    private void ProcessAssigneeNode(BpmnNodeVo baseNodeVo, Dictionary<String, BpmnNodeVo> mapPreNodes, HashSet<BpmnNodeVo> setAddNodes,String nextId,BpmnStartConditionsVo bpmnStartConditions){

        if (baseNodeVo.NodeType==(int)NodeTypeEnum.NODE_TYPE_APPROVER||baseNodeVo.NodeType==(int)NodeTypeEnum.NODE_TYPE_COPY) {
            BpmnNodeParamsVo paramsVo = baseNodeVo.Params;
            if (baseNodeVo.NodeProperty==1) {
                throw new AFBizException("the assignee node has no property,can not go on");
            }
            int? nodeProperty =baseNodeVo.NodeProperty;
            NodePropertyEnum? nodePropertyEnum=NodePropertyEnumExtensions.GetByCode(nodeProperty);
            if(nodePropertyEnum==null){
                throw new AFBizException("node property is undefined!");
            }
            BpmnNodeParamTypeEnum paramTypeEnum = nodePropertyEnum.Value.GetParamTypeEnum();
            PersonnelEnum? personnelEnum = PersonnelEnumExtensions.FromNodePropertyEnum(nodePropertyEnum.Value);
            AbstractBpmnPersonnelAdaptor personnelAdaptor=null;
            IEnumerable<AbstractBpmnPersonnelAdaptor> abstractBpmnPersonnelAdaptors = ServiceProviderUtils.GetServices<AbstractBpmnPersonnelAdaptor>();
            foreach (AbstractBpmnPersonnelAdaptor abstractBpmnPersonnelAdaptor in abstractBpmnPersonnelAdaptors){
                if(((IAdaptorService)abstractBpmnPersonnelAdaptor).IsSupportBusinessObject(personnelEnum)){
                    personnelAdaptor=abstractBpmnPersonnelAdaptor;
                    break;
                }
            }
            if(personnelAdaptor==null){
                throw new AFBizException("can not find personnel adaptor by given node property");
            }
            personnelAdaptor.SetNodeParams(baseNodeVo,bpmnStartConditions,paramTypeEnum,nextId,mapPreNodes,setAddNodes);

        }
    }
}