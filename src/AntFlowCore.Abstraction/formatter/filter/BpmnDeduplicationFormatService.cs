using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.formatter.filter;

 public class BpmnDeduplicationFormatService : IBpmnDeduplicationFormat
    {
        public BpmnConfVo ForwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
        {
            string startNodeId = null;
            Dictionary<string, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();
            foreach (var vo in bpmnConfVo.Nodes)
            {
                mapNodes[vo.NodeId] = vo;
                if (vo.NodeType!=0 && vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
                {
                    startNodeId = vo.NodeId;
                }
            }

            string initiator = mapNodes[startNodeId].Params.Assignee.Assignee;
            BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];
            List<BpmnNodeVo> nodeVoList = new List<BpmnNodeVo>();

            while (!string.IsNullOrEmpty(bpmnNodeVo.Params.NodeTo))
            {
                bpmnNodeVo = mapNodes[bpmnNodeVo.Params.NodeTo];

                if (bpmnNodeVo.Params.ParamType == 1)
                {
                    SinglePlayerNodeDeduplication(bpmnNodeVo, new HashSet<string>(),new List<string> { initiator }, bpmnStartConditions);
                    nodeVoList.Add(bpmnNodeVo);
                    continue;
                }

                if (bpmnNodeVo.Params.ParamType == 2)
                {
                    MultiPlayerNodeDeduplication(bpmnNodeVo, new HashSet<string>(),new List<string> { initiator }, false, bpmnStartConditions);
                    nodeVoList.Add(bpmnNodeVo);
                }
            }

            nodeVoList.Reverse();

            List<string> approverList = new List<string>();
            foreach (var bpmnNode in nodeVoList)
            {
                if (bpmnNode.Params.ParamType == 1)
                {
                    SinglePlayerNodeDeduplication(bpmnNode,new HashSet<string>(), approverList, bpmnStartConditions);
                    continue;
                }

                if (bpmnNode.Params.ParamType == 2)
                {
                    bpmnNode.Params.AssigneeList.Reverse();
                    MultiPlayerNodeDeduplication(bpmnNode,new HashSet<string>(), approverList, true, bpmnStartConditions);
                    bpmnNode.Params.AssigneeList.Reverse();
                }
            }

            return bpmnConfVo;
        }

        public BpmnConfVo BackwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
        {
            List<string> approverList = new List<string>();
            string startNodeId = null;
            Dictionary<string, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();
            foreach (var vo in bpmnConfVo.Nodes)
            {
                mapNodes[vo.NodeId] = vo;
                if (vo.NodeType!=0 && vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
                {
                    startNodeId = vo.NodeId;
                }
            }

            string initiator = mapNodes[startNodeId].Params.Assignee.Assignee;
            approverList.Add(initiator);
            BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];

            // 使用递归处理并行网关
            ProcessNodeRecursively(bpmnNodeVo,new HashSet<string>(), mapNodes, approverList, bpmnStartConditions);

            return bpmnConfVo;
        }

        private void SinglePlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo,HashSet<String> alreadyProcessedNods, List<string> approverList, BpmnStartConditionsVo bpmnStartConditions)
        {
            if (bpmnNodeVo.DeduplicationExclude || alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
            {
                return;
            }
            if (bpmnNodeVo.Params.IsNodeDeduplication == 1)
            {
                return;
            }

            BpmnNodeParamsAssigneeVo assignee = bpmnNodeVo.Params.Assignee;
            bool isSkipNext = bpmnStartConditions.DeduplicationType == (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_SKIP_NEXT;
            if (approverList.Contains(assignee.Assignee))
            {
                assignee.IsDeduplication = 1;
                bpmnNodeVo.Params.IsNodeDeduplication = 1;
                if (isSkipNext)
                {
                    approverList.Clear();
                    approverList.Add(assignee.Assignee);
                }
            }
            else
            {
                if (isSkipNext)
                {
                    approverList.Clear();
                    approverList.Add(assignee.Assignee);
                }
                else
                {
                    approverList.Add(assignee.Assignee);
                }
            }
            alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
        }

        private void MultiPlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo,HashSet<String> alreadyProcessedNods,List<string> approverList, bool flag, BpmnStartConditionsVo bpmnStartConditions)
        {
            if (bpmnNodeVo.DeduplicationExclude||
                bpmnNodeVo.Params.IsNodeDeduplication == 1
                ||alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
            {
                return;
            }

            bool isSkipNext = bpmnStartConditions.DeduplicationType == (int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_SKIP_NEXT;
            List<BpmnNodeParamsAssigneeVo> assigneeList = bpmnNodeVo.Params.AssigneeList;
            int isNodeDeduplication = 1;
            foreach (var assignee in assigneeList)
            {
                if (assignee.IsDeduplication == 1)
                {
                    continue;
                }

                if (approverList.Contains(assignee.Assignee))
                {
                    assignee.IsDeduplication = 1;
                    if (isSkipNext)
                    {
                        approverList.Clear();
                        approverList.Add(assignee.Assignee);
                    }
                }
                else
                {
                    if (flag)
                    {
                        if (isSkipNext)
                        {
                            approverList.Clear();
                            approverList.Add(assignee.Assignee);
                        }
                        else
                        {
                            approverList.Add(assignee.Assignee);
                        }
                    }
                    isNodeDeduplication = 0;
                }
            }
            bpmnNodeVo.Params.IsNodeDeduplication = isNodeDeduplication;
            alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
        }
        private void ProcessNodeRecursively(BpmnNodeVo bpmnNodeVo,HashSet<String> alreadyProcessedNodes, Dictionary<String, BpmnNodeVo> mapNodes, List<String> approverList, BpmnStartConditionsVo bpmnStartConditions) {

            String nextId=null;
            do {


                if((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY==bpmnNodeVo.NodeType){
                    List<String> parallelNodeToIds = bpmnNodeVo.NodeTo;
                    foreach (String parallelNodeToId in parallelNodeToIds) {
                        if (mapNodes.TryGetValue(parallelNodeToId, out var parallelNodeTo))
                        {
                            ProcessNodeRecursively(parallelNodeTo,alreadyProcessedNodes, mapNodes, approverList, bpmnStartConditions);
                        }

                    }

                }


                // 处理单节点去重
                if (bpmnNodeVo.Params.ParamType==1) {
                    SinglePlayerNodeDeduplication(bpmnNodeVo,alreadyProcessedNodes, approverList, bpmnStartConditions);
                }else if (bpmnNodeVo.Params.ParamType==2) {
                    MultiPlayerNodeDeduplication(bpmnNodeVo,alreadyProcessedNodes, approverList, true, bpmnStartConditions);
                }

                String nodeTo = GetNodeTo(bpmnNodeVo);

                if (string.IsNullOrEmpty(nodeTo)) {
                    return;
                }
                bpmnNodeVo= GetNextNodeVo(mapNodes.Values, nodeTo);
                nextId=bpmnNodeVo.NodeId;
            }while (!string.IsNullOrEmpty(nextId));

        }
        private BpmnNodeVo GetNextNodeVo(ICollection<BpmnNodeVo> nodes, String nodeTo)
        {
            List<BpmnNodeVo> nextNodeVo = nodes
                .Where(o => o.NodeId == nodeTo)
                .ToList();

            if (nextNodeVo.Count == 0) {
                throw new AFBizException("未找到下一节点流程发起失败");
            }
            return nextNodeVo[0];
        }
        private String GetNodeTo(BpmnNodeVo nodeVo)
        {
            BpmnNodeParamsVo bpmnNodeParamsVo = nodeVo.Params;
            if (bpmnNodeParamsVo == null)
            {
                return null;
            }

            string nodeTo = bpmnNodeParamsVo.NodeTo;
            return nodeTo;
        }
    }