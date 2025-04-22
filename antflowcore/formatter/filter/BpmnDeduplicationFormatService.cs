using antflowcore.constant.enus;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

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
                    SinglePlayerNodeDeduplication(bpmnNodeVo, new List<string> { initiator });
                    nodeVoList.Add(bpmnNodeVo);
                    continue;
                }

                if (bpmnNodeVo.Params.ParamType == 2)
                {
                    MultiPlayerNodeDeduplication(bpmnNodeVo, new List<string> { initiator }, false);
                    nodeVoList.Add(bpmnNodeVo);
                }
            }

            nodeVoList.Reverse();

            List<string> approverList = new List<string>();
            foreach (var bpmnNode in nodeVoList)
            {
                if (bpmnNode.Params.ParamType == 1)
                {
                    SinglePlayerNodeDeduplication(bpmnNode, approverList);
                    continue;
                }

                if (bpmnNode.Params.ParamType == 2)
                {
                    bpmnNode.Params.AssigneeList.Reverse();
                    MultiPlayerNodeDeduplication(bpmnNode, approverList, true);
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

            while (!string.IsNullOrEmpty(bpmnNodeVo.Params.NodeTo))
            {
                bpmnNodeVo = mapNodes[bpmnNodeVo.Params.NodeTo];

                if (bpmnNodeVo.Params.ParamType == 1)
                {
                    SinglePlayerNodeDeduplication(bpmnNodeVo, approverList);
                    continue;
                }

                if (bpmnNodeVo.Params.ParamType == 2)
                {
                    MultiPlayerNodeDeduplication(bpmnNodeVo, approverList, true);
                }
            }

            return bpmnConfVo;
        }

        private void SinglePlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo, List<string> approverList)
        {
            if (bpmnNodeVo.Params.IsNodeDeduplication == 1)
            {
                return;
            }

            BpmnNodeParamsAssigneeVo assignee = bpmnNodeVo.Params.Assignee;
            if (approverList.Contains(assignee.Assignee))
            {
                assignee.IsDeduplication = 1;
                bpmnNodeVo.Params.IsNodeDeduplication = 1;
            }
            else
            {
                approverList.Add(assignee.Assignee);
            }
        }

        private void MultiPlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo, List<string> approverList, bool flag)
        {
            if (bpmnNodeVo.Params.IsNodeDeduplication == 1)
            {
                return;
            }

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
                }
                else
                {
                    if (flag)
                    {
                        approverList.Add(assignee.Assignee);
                    }
                    isNodeDeduplication = 0;
                }
            }
            bpmnNodeVo.Params.IsNodeDeduplication = isNodeDeduplication;
        }
    }