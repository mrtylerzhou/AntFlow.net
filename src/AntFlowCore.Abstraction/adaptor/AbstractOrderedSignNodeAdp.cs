using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.adaptor;

public abstract class AbstractOrderedSignNodeAdp : IAdaptorService
    {
        private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

        protected AbstractOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils)
        {
            _assigneeVoBuildUtils = assigneeVoBuildUtils;
        }

        /// <summary>
        /// 返回层层审批的审批人,外层=层,内层=层内多人。
        /// 每层至少 1 人;空层用 [zeroVo] 表示,会被框架 filter 掉。
        /// 层内多人时走会签/或签(由节点 SignType 决定)。
        /// </summary>
        public abstract List<List<string>> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions);

        public void FormatNodes(
            BpmnNodeVo nodeVo,
            BpmnStartConditionsVo bpmnStartConditions,
            string nextNodeId,
            Dictionary<string, BpmnNodeVo> mapPreNodes,
            HashSet<BpmnNodeVo> setAddNodes)
        {
            var orderedSignNodes = GetOrderedSignNodes(nodeVo, bpmnStartConditions, nextNodeId);
            // b=true 表示 nodeVo 自己身上至少有一个真人审批(原 Count==1 判定在多人层下会误判,
            // 改成 Any 非占位符即可。"0" 对应 Java 版的 TO_BE_REMOVED)
            var hasRealAssignee = nodeVo.Params.AssigneeList != null
                                   && nodeVo.Params.AssigneeList.Any(a => a.Assignee != "0");

            if (orderedSignNodes.Any() || hasRealAssignee)
            {
                if (orderedSignNodes.Any())
                {
                    mapPreNodes[nextNodeId].Params.NodeTo = orderedSignNodes.Last().NodeId;
                    setAddNodes.UnionWith(orderedSignNodes);
                }
                else
                {
                    mapPreNodes[nextNodeId].Params.NodeTo = nodeVo.NodeId;
                }
            }
            else
            {
                nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { _assigneeVoBuildUtils.BuildZeroVo() };
            }
        }

        private List<BpmnNodeVo> GetOrderedSignNodes(
            BpmnNodeVo nodeVo,
            BpmnStartConditionsVo bpmnStartConditions,
            string nextNodeId)
        {
            if (nodeVo.Params.AssigneeList == null)
            {
                nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo>
                {
                    new BpmnNodeParamsAssigneeVo { Assignee = "0", IsDeduplication = 0 }
                };
            }

            var result = new List<BpmnNodeVo>();

            //first of all find all assignee layers (List<List<>>)
            var listAssignLayers = GetAssignees(nodeVo, bpmnStartConditions);
            if (listAssignLayers == null || !listAssignLayers.Any())
            {
                return result;
            }

            //filter 掉"整层全 0"的层(包括末尾的 [[zeroVo]] 占位层)
            listAssignLayers = listAssignLayers
                .Where(layer => layer != null && layer.Any()
                                && layer.Any(a => a.Assignee != "0"))
                .ToList();

            //if no real layer left,then return
            if (!listAssignLayers.Any())
            {
                return result;
            }

            //第 0 层挂到 nodeVo 自己身上
            var firstLayer = listAssignLayers[0];
            if (nodeVo.Params.AssigneeList.Count == 1 && nodeVo.Params.AssigneeList[0].Assignee == "0")
            {
                nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo>(firstLayer);
                //even through it only has one assignee,also treat it as multiplayer node
                nodeVo.Params.ParamType = (int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER;
                //层内多人时才标 IsMultiPeople=1;单人不标(与原行为对齐)
                if (firstLayer.Count > 1)
                {
                    nodeVo.Property.IsMultiPeople = 1;
                }
            }

            int nameLast = 1;

            //第 1..N 层各 clone 一个节点,每层 AssigneeList = 整层
            for (int i = 1; i < listAssignLayers.Count; i++)
            {
                var layer = listAssignLayers[i];

                //each layer generate a ordered sign node
                var copyNode = SerializationUtils.Clone(nodeVo);
                copyNode.NodeId = $"{nodeVo.NodeId}copy{nameLast}";
                nameLast++;

                // indicate multi people
                copyNode.Property.IsMultiPeople = 1;
                copyNode.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo>(layer);
                copyNode.Params.ParamType = (int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER;
                // SignType 继承自 nodeVo(clone 自带),不动
                result.Add(copyNode);
            }

            //set next node id
            var forNext = nextNodeId;
            foreach (var vo in result)
            {
                vo.Params.NodeTo = forNext;
                forNext = vo.NodeId;
            }

            return result;
        }

        private List<List<BpmnNodeParamsAssigneeVo>> GetAssignees(
            BpmnNodeVo nodeVo,
            BpmnStartConditionsVo bpmnStartConditions)
        {
            var assigneeLayers = GetAssigneeIds(nodeVo, bpmnStartConditions);
            if (assigneeLayers == null || !assigneeLayers.Any())
            {
                return new List<List<BpmnNodeParamsAssigneeVo>>();
            }

            //不去重(跨层):跨层/层内去重交给全局机制
            //(由流程级 DeduplicationType 配置控制,BpmnElementLoopAdaptor 按 DuplicationProcessStrategy 过滤)
            //
            //但【层内必须去重】:BpmnElementLoopAdaptor.DoFormatNodesToElements 多人分支用 ToDictionary,
            //重复 key 会抛 ArgumentException(这点与 Java 版的 toMap((k1,k2)->k1) 自动合并不同)。
            //所以层内同 id 只保留首个,跨层允许重复。
            var validLayers = new List<List<string>>();
            foreach (var layer in assigneeLayers)
            {
                if (layer == null || !layer.Any()) continue;

                //层内去重:同 id 同层只保留首个
                var seenIds = new HashSet<string>();
                var dedupedLayer = new List<string>();
                foreach (var id in layer)
                {
                    if (seenIds.Add(id))
                    {
                        dedupedLayer.Add(id);
                    }
                }
                if (dedupedLayer.Any())
                {
                    validLayers.Add(dedupedLayer);
                }
            }

            if (!validLayers.Any())
            {
                return new List<List<BpmnNodeParamsAssigneeVo>>();
            }

            //每层独立调 BuildVos(无状态,安全),elementName 后缀每层从 _1 重置
            var result = new List<List<BpmnNodeParamsAssigneeVo>>();
            foreach (var layer in validLayers)
            {
                var builtVos = _assigneeVoBuildUtils.BuildVos(layer, nodeVo.NodeName, true);
                //filter 掉 BuildVos 内部可能产生的 "0"
                builtVos = builtVos.Where(a => a.Assignee != "0").ToList();
                if (builtVos.Any())
                {
                    result.Add(builtVos);
                }
            }

            //末尾追加 [[zeroVo]] 占位层,保证至少有一层 + 配合 b 兜底
            result.Add(new List<BpmnNodeParamsAssigneeVo> { _assigneeVoBuildUtils.BuildZeroVo() });
            return result;
        }

        public abstract void SetSupportBusinessObjects();

    }
