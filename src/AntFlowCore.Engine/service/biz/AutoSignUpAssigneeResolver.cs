using System.Text.Json;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 条件自动加批节点加批规则解析器: 把前端保存的 autoSignUpConf({nodeProperty, resolvedProperty})
/// 按审批人规则类型复用现有 personnel provider 运行期解析为具体用户列表.
/// 支持: 5指定人员 / 4指定角色 / 13直属领导 / 3指定层级 / 6HRBP / 12发起人自己 / 17自定义.
/// 直属领导/层级/HRBP 的基准人为流程发起人(与审批人节点语义一致).
/// 对应 Java AutoSignUpAssigneeResolver.
/// </summary>
public class AutoSignUpAssigneeResolver
{
    private readonly UserPointedPersonnelProvider _userPointed;
    private readonly RolePersonnelProvider _role;
    private readonly DirectLeaderPersonnelProvider _directLeader;
    private readonly LevelPersonnelProvider _level;
    private readonly HrbpPersonnelProvider _hrbp;
    private readonly StartUserPersonnelProvider _startUser;
    private readonly UDRPersonnelProvider _udr;
    private readonly ILogger<AutoSignUpAssigneeResolver> _logger;

    public AutoSignUpAssigneeResolver(
        UserPointedPersonnelProvider userPointed,
        RolePersonnelProvider role,
        DirectLeaderPersonnelProvider directLeader,
        LevelPersonnelProvider level,
        HrbpPersonnelProvider hrbp,
        StartUserPersonnelProvider startUser,
        UDRPersonnelProvider udr,
        ILogger<AutoSignUpAssigneeResolver> logger)
    {
        _userPointed = userPointed;
        _role = role;
        _directLeader = directLeader;
        _level = level;
        _hrbp = hrbp;
        _startUser = startUser;
        _udr = udr;
        _logger = logger;
    }

    /// <summary>
    /// 解析加批规则为具体用户列表.
    /// </summary>
    /// <returns>null=配置缺失或类型不支持; 空列表=解析结果为空(调用方视为条件不满足)</returns>
    public List<BaseIdTranStruVo>? Resolve(JsonElement? autoSignUpConf, string? startUserId, BusinessDataVo? businessDataVo)
    {
        if (autoSignUpConf == null || autoSignUpConf.Value.ValueKind == JsonValueKind.Null || autoSignUpConf.Value.ValueKind == JsonValueKind.Undefined)
        {
            return null;
        }
        JsonElement confJson = autoSignUpConf.Value;
        int? nodeProperty = null;
        if (confJson.TryGetProperty("nodeProperty", out var np) && np.ValueKind == JsonValueKind.Number)
        {
            nodeProperty = np.GetInt32();
        }
        if (nodeProperty == null && confJson.TryGetProperty("setType", out var st) && st.ValueKind == JsonValueKind.Number)
        {
            nodeProperty = st.GetInt32();
        }
        if (nodeProperty == null)
        {
            return null;
        }

        // 合成虚拟节点 VO: nodeProperty + resolvedProperty→property
        BpmnNodeVo conf = new BpmnNodeVo { NodeProperty = nodeProperty, NodeName = "条件自动加批" };
        if (confJson.TryGetProperty("resolvedProperty", out var rp) && rp.ValueKind == JsonValueKind.Object)
        {
            conf.Property = JsonSerializer.Deserialize<BpmnNodePropertysVo>(rp.GetRawText(), JsonConfUtil.Options);
        }

        BpmnStartConditionsVo sc = new BpmnStartConditionsVo { StartUserId = startUserId, BusinessDataVo = businessDataVo };

        List<BpmnNodeParamsAssigneeVo>? assignees;
        try
        {
            assignees = nodeProperty switch
            {
                5 => _userPointed.GetAssigneeList(conf, sc),
                4 => _role.GetAssigneeList(conf, sc),
                13 => _directLeader.GetAssigneeList(conf, sc),
                3 => _level.GetAssigneeList(conf, sc),
                6 => _hrbp.GetAssigneeList(conf, sc),
                12 => _startUser.GetAssigneeList(conf, sc),
                17 => _udr.GetAssigneeList(conf, sc),
                _ => null
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "条件自动加批: 规则解析异常, setType={T}, startUserId={S}", nodeProperty, startUserId);
            return null;
        }
        if (assignees == null)
        {
            return null;
        }
        return assignees
            .Where(a => a.Assignee != null)
            .Select(a => new BaseIdTranStruVo { Id = a.Assignee, Name = a.AssigneeName })
            .ToList();
    }
}