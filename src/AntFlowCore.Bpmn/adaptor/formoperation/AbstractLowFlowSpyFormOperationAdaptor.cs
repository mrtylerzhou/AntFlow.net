using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.formoperation;

public abstract class AbstractLowFlowSpyFormOperationAdaptor<T> : IFormOperationAdaptor<T> where T : BusinessDataVo
{
    /// <summary>
    /// 处理表单关联人员配置（DIY 表单版本）
    /// 直接从数据库中读取节点配置的 diyFormFieldName，无需硬编码
    /// </summary>
    protected void ProcessDIYFormRelatedAssignees(T businessDataVo, BpmnConfVo bpmnConfVo)
    {
        if (bpmnConfVo == null)
        {
            return;
        }

        int? extraFlags = bpmnConfVo.ExtraFlags;

        // 检查是否启用了表单关联审批人功能
        bool hasFlag = extraFlags != null && BpmnConfFlagsEnum.HasFlag(extraFlags, BpmnConfFlagsEnum.HAS_FORM_RELATED_ASSIGNEES);

        if (hasFlag)
        {
            // 从数据库读取该流程的所有 DIY 表单字段节点配置
            var mapping = LoadDIYFormFieldMappingFromDatabase(bpmnConfVo.Id);



            if (mapping == null || mapping.Count == 0)
            {
                return;
            }

            var node2formRelatedAssignees = new Dictionary<string, List<string>>();

            // 使用反射从业务数据对象中获取字段值
            var businessDataType = businessDataVo.GetType();

            foreach (var entry in mapping)
            {
                long nodeId = entry.Key;
                string fieldName = entry.Value;

                if (string.IsNullOrEmpty(fieldName))
                {
                    continue;
                }

                var property = businessDataType.GetProperty(fieldName);
                if (property == null)
                {
                    continue;
                }

                var fieldValue = property.GetValue(businessDataVo);

                if (fieldValue == null)
                {
                    continue;
                }

                List<string> assigneeIds = new List<string>();

                // 处理单个值（字符串）
                if (fieldValue is string strValue && !string.IsNullOrEmpty(strValue))
                {
                    assigneeIds.Add(strValue);
                }
                // 处理列表值
                else if (fieldValue is System.Collections.IEnumerable enumerable && !(fieldValue is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                        {
                            assigneeIds.Add(item.ToString());
                        }
                    }
                }
                // 处理数字类型（long, int等）
                else
                {
                    assigneeIds.Add(fieldValue.ToString());
                }

                if (assigneeIds.Count > 0)
                {
                    node2formRelatedAssignees[nodeId.ToString()] = assigneeIds;
                }
            }
            businessDataVo.Node2formRelatedAssignees = node2formRelatedAssignees;
        }
    }

    /// <summary>
    /// 从数据库加载 DIY 表单字段映射配置
    /// 读取所有 nodeProperty=20 的节点，从 node_config_json 中提取 diyFormFieldName
    /// </summary>
    private Dictionary<long, string> LoadDIYFormFieldMappingFromDatabase(long confId)
    {
        var mapping = new Dictionary<long, string>();

        try
        {
            // 获取 BpmnNodeService
            var bpmnNodeService = ServiceProviderUtils.GetService<IBpmnNodeService>();
            if (bpmnNodeService == null)
            {
                return mapping;
            }

            // 查询该流程配置下所有 nodeProperty=20 的节点
            var diyFormNodes = bpmnNodeService._repository
                .Find(n => n.ConfId == confId && n.NodeProperty == 20 && n.IsDel == 0)
                .ToList();

            foreach (var node in diyFormNodes)
            {
                if (string.IsNullOrEmpty(node.NodeConfigJson))
                {
                    continue;
                }

                // 解析 node_config_json
                var nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
                if (nodeConfig?.ApproverConf?.FormRelatedUserConfList != null
                    && nodeConfig.ApproverConf.FormRelatedUserConfList.Count > 0)
                {
                    var formRelatedConf = nodeConfig.ApproverConf.FormRelatedUserConfList[0];
                    if (!string.IsNullOrEmpty(formRelatedConf.DiyFormFieldName))
                    {
                        mapping[node.Id] = formRelatedConf.DiyFormFieldName;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 记录日志但不抛出异常，避免影响主流程
            // 可以添加日志记录
        }

        return mapping;
    }


    public abstract void PreviewSetCondition(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo PreviewSetCondition(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        conditionsVo.BusinessDataVo = vo; // 传递完整的业务数据，供审批人提供者使用
        PreviewSetCondition(conditionsVo, vo);

        // 处理 DIY 表单关联审批人
        if (vo.BpmnConfVo != null)
        {
            ProcessDIYFormRelatedAssignees(vo, vo.BpmnConfVo);
        }

        return conditionsVo;
    }

    public abstract void LaunchParameters(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public BpmnStartConditionsVo LaunchParameters(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        conditionsVo.BusinessDataVo = vo; // 传递完整的业务数据，供审批人提供者使用
        LaunchParameters(conditionsVo, vo);

        // 处理 DIY 表单关联审批人（仅在启动时需要）
        if (vo.BpmnConfVo != null)
        {
            ProcessDIYFormRelatedAssignees(vo, vo.BpmnConfVo);
        }
        return conditionsVo;
    }

    public abstract void OnInitData(T vo);

    public abstract void OnQueryData(T vo);

    public abstract void OnSubmitData(T vo);

    public abstract void OnConsentData(T vo);

    public abstract void OnDisagreeData(T vo);

    public abstract void OnBackToModifyData(T vo);

    public abstract void OnCancellationData(T vo);
    public abstract void OnFinishData(BusinessDataVo vo);
}
