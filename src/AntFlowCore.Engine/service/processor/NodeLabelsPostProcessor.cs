using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.processor;

/// <summary>
/// Post-processor that persists node labels after a BPMN conf edit.
/// <para>
/// In the .NET version, labels are consolidated into node_config_json
/// (buttonSignConf.labels). The design-time persistence is already handled by
/// <see cref="BpmnNodeConfigHolder.SetButtonSignConf"/>. This post-processor exists as
/// an extension point mirroring the Java NodeLabelsPostProcessor, so that
/// dynamically-added labels (e.g. runtime-injected labels) can be persisted here
/// if a dedicated labels table is introduced later.
/// </para>
/// </summary>
public class NodeLabelsPostProcessor : IAntFlowOrderPostProcessor<BpmnConfVo>
{
    private readonly IBpmnNodeService _bpmnNodeService;

    public NodeLabelsPostProcessor(IBpmnNodeService bpmnNodeService)
    {
        _bpmnNodeService = bpmnNodeService;
    }

    public int Order() => 1;

    public void PostProcess(BpmnConfVo confVo)
    {
        if (confVo?.Nodes == null || confVo.Nodes.Count == 0)
        {
            return;
        }

        foreach (var nodeVo in confVo.Nodes)
        {
            var labelList = nodeVo.LabelList;
            if (labelList == null || labelList.Count == 0)
            {
                continue;
            }

            // Labels are already written into node_config_json via SetButtonSignConf
            // during the edit loop. Here we only ensure dynamically-added labels
            // (those appended after SetButtonSignConf ran) are also reflected in the
            // persisted node config JSON, matching the Java behaviour.
            UpdateLabelsToNodeJson(nodeVo.Id, labelList);
        }
    }

    private void UpdateLabelsToNodeJson(long nodeId, List<BpmnNodeLabelVO> labelList)
    {
        var node = _bpmnNodeService._repository.FirstOrDefault(a => a.Id == nodeId);
        if (node == null)
        {
            return;
        }

        BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson) ?? new BpmnNodeConfigJson();
        nodeConfig.ButtonSignConf ??= new BpmnNodeButtonSignConfJson();

        nodeConfig.ButtonSignConf.Labels = labelList
            .Select(l => new ButtonSignNodeLabel
            {
                LabelName = l.LabelName,
                LabelValue = l.LabelValue
            })
            .ToList();

        node.NodeConfigJson = JsonConfUtil.ToNodeConfigJson(nodeConfig);
        _bpmnNodeService._repository.Update(node);
    }
}
