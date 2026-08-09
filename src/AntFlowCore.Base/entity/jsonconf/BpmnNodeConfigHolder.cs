using System.Text.Json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

/// <summary>
/// Helper for building BpmnNodeConfigJson from BpmnNodeVo data during edit flow.
/// Each adaptor calls the appropriate static method to populate its section.
/// </summary>
public static class BpmnNodeConfigHolder
{
    /// <summary>
    /// Build personnel approver config from node VO
    /// </summary>
    public static void SetPersonnelConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        var employees = new List<ApproverEmployeeInfo>();

        if (prop.EmplIds != null && prop.EmplIds.Count > 0)
        {
            if (prop.EmplList != null && prop.EmplList.Count > 0)
            {
                foreach (var e in prop.EmplList)
                {
                    employees.Add(new ApproverEmployeeInfo
                    {
                        EmplId = e.Id,
                        EmplName = e.Name
                    });
                }
            }
            else
            {
                foreach (var id in prop.EmplIds)
                {
                    employees.Add(new ApproverEmployeeInfo
                    {
                        EmplId = id
                    });
                }
            }
        }

        approverConf.PersonnelConf = new ApproverPersonnelConf
        {
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio,
            Employees = employees
        };
    }

    /// <summary>
    /// Build role approver config from node VO
    /// </summary>
    public static void SetRoleConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null || prop.RoleList == null || prop.RoleList.Count == 0) return;

        var approverConf = GetOrCreateApproverConf(vo);
        var roleConfList = new List<ApproverRoleConf>();

        foreach (var role in prop.RoleList)
        {
            var rc = new ApproverRoleConf
            {
                RoleId = role.Id,
                RoleName = role.Name,
                SignType = prop.SignType,
                ArbitrationRatio = prop.ArbitrationRatio
            };

            if (vo.IsOutSideProcess != null && vo.IsOutSideProcess == 1
                && prop.EmplList != null && prop.EmplList.Count > 0)
            {
                rc.OutsideEmployees = prop.EmplList.Select(e => new ApproverEmployeeInfo
                {
                    EmplId = e.Id,
                    EmplName = e.Name
                }).ToList();
            }

            roleConfList.Add(rc);
        }

        approverConf.RoleConfList = roleConfList;
    }

    /// <summary>
    /// Build loop approver config from node VO
    /// </summary>
    public static void SetLoopConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.LoopConf = new ApproverLoopConf
        {
            LoopEndType = prop.LoopEndType,
            LoopNumberPlies = prop.LoopNumberPlies,
            LoopEndPerson = JoinList(prop.LoopEndPersonList),
            NoparticipatingStaffIds = JoinList(prop.NoparticipatingStaffIds),
            LoopEndGrade = prop.LoopEndGrade
        };
    }

    /// <summary>
    /// Build assign level approver config from node VO
    /// </summary>
    public static void SetAssignLevelConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.AssignLevelConf = new ApproverAssignLevelConf
        {
            AssignLevelType = prop.AssignLevelType,
            AssignLevelGrade = prop.AssignLevelGrade
        };
    }

    /// <summary>
    /// Build HRBP approver config from node VO
    /// </summary>
    public static void SetHrbpConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.HrbpConf = new ApproverHrbpConf
        {
            HrbpConfType = prop.HrbpConfType
        };
    }

    /// <summary>
    /// Build customize approver config from node VO
    /// </summary>
    public static void SetCustomizeConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.CustomizeConf = new ApproverCustomizeConf
        {
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio
        };
    }

    /// <summary>
    /// Build outside access approver config from node VO
    /// </summary>
    public static void SetOutSideAccessConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.OutSideAccessConf = new ApproverOutSideAccessConf
        {
            NodeMark = prop.NodeMark,
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio
        };
    }

    /// <summary>
    /// Build business table approver config from node VO
    /// </summary>
    public static void SetBusinessTableConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.BusinessTableConf = new ApproverBusinessTableConf
        {
            ConfigurationTableType = prop.ConfigurationTableType,
            TableFieldType = prop.TableFieldType,
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio
        };
    }

    /// <summary>
    /// Build conditions config from pre-built condition groups, matching Java
    /// </summary>
    public static void SetConditionsConf(BpmnNodeVo vo, List<BpmnNodeConditionsConfJson.ConditionGroup> groups, string? outSideId)
    {
        var config = vo.GetOrCreateNodeConfigJson();
        config.ConditionsConf = new BpmnNodeConditionsConfJson
        {
            ConditionGroups = groups,
            OutSideConditionId = outSideId
        };
    }

    /// <summary>
    /// Persists autoNodeConf (conditionList + groupRelation) from BpmnNodeVo.AutoNodeConf
    /// into node_config_json.autoNodeConf. Used by condition-approve (12) and
    /// condition-copy (13) nodes. Mirrors Java BpmnNodeConfigHolder.setAutoNodeConf.
    /// </summary>
    public static void SetAutoNodeConf(BpmnNodeVo vo)
    {
        if (vo.AutoNodeConf == null)
        {
            return;
        }
        BpmnNodeConfigJson config = vo.GetOrCreateNodeConfigJson();
        config.AutoNodeConf = vo.AutoNodeConf;
    }

    /// <summary>
    /// Persists forward config (forwardType + forwardNodeIds) from BpmnNodeVo
    /// into node_config_json.forwardType/forwardNodeIds.
    /// Used by auto-advance (18) and auto-complete (18 sub-type) nodes.
    /// Mirrors Java BpmnNodeConfigHolder.setForwardConf (equivalent).
    /// 补全了此前缺失的 forward 持久化链路:
    /// 运行时 ProcessAutoAdvanceNode 从 NodeConfigJson.ForwardNodeIds 读目标节点,
    /// 反显时 PopulateForwardConf 也从 NodeConfigJson 读回 forward 给前端.
    /// </summary>
    public static void SetForwardConf(BpmnNodeVo vo)
    {
        if (vo.ForwardType == null || vo.ForwardType == 0)
        {
            return;
        }
        BpmnNodeConfigJson config = vo.GetOrCreateNodeConfigJson();
        config.ForwardType = vo.ForwardType;
        config.ForwardNodeIds = vo.ForwardNodeIds ?? new List<string>();
    }

    /// <summary>
    /// Build button/sign config from node VO
    /// </summary>
    public static void SetButtonSignConf(BpmnNodeVo vo)
    {
        var config = vo.GetOrCreateNodeConfigJson();
        var bs = new BpmnNodeButtonSignConfJson();

        // Buttons - BpmnNodeButtonConfBaseVo has startPage, approvalPage, viewPage (List<BpmnConfCommonButtonPropertyVo>)
        var btns = vo.Buttons;
        if (btns != null)
        {
            var buttonList = new List<ButtonSignButtonConf>();
            AddButtonsFromList(buttonList, btns.StartPage, 1, 0);
            AddButtonsFromList(buttonList, btns.ApprovalPage, 2, 0);
            AddButtonsFromList(buttonList, btns.ViewPage, 3, 0);

            // START(发起人)节点的审批页必须要有"重新提交"按钮
            // 前端设计器(promoterDrawer)没有审批按钮配置UI，需要后端兜底
            bool isStartNode = vo.NodeType != null && vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START;
            if (isStartNode)
            {
                bool hasResubmit = btns.ApprovalPage != null
                    && btns.ApprovalPage.Any(b => (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT == b.ButtonType);
                if (!hasResubmit)
                {
                    buttonList.Add(new ButtonSignButtonConf
                    {
                        ButtonPageType = 2,
                        ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT,
                        ButtonName = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT),
                        StartPageOnly = 0
                    });
                }
            }

            bs.ButtonConfList = buttonList;
        }

        // Sign-up
        if (vo.IsSignUp == 1)
        {
            var prop = vo.Property;
            if (prop != null)
            {
                bs.SignUpConf = new ButtonSignSignUpConf
                {
                    AfterSignUpWay = prop.AfterSignUpWay,
                    SignUpType = prop.SignUpType
                };
            }
        }

        // Additional sign
        var prop2 = vo.Property;
        if (prop2 != null && prop2.AdditionalSignInfoList != null && prop2.AdditionalSignInfoList.Count > 0)
        {
            var addSignList = new List<ButtonSignAdditionalSignConf>();
            foreach (var info in prop2.AdditionalSignInfoList)
            {
                addSignList.Add(new ButtonSignAdditionalSignConf
                {
                    SignInfos = JsonConfUtil.ToJsonString(info.SignInfos),
                    SignProperty = info.NodeProperty,
                    SignPropertyType = info.PropertyType,
                    SignType = prop2.SignType
                });
            }
            bs.AdditionalSignConfList = addSignList;
        }

        // Operation types (migrated from bpm_process_operation)
        if (vo.OperationTypes != null && vo.OperationTypes.Count > 0)
        {
            bs.OperationTypes = vo.OperationTypes;
        }

        // Labels (node labels persisted into buttonSignConf.labels)
        if (vo.LabelList != null && vo.LabelList.Count > 0)
        {
            bs.Labels = vo.LabelList
                .Select(l => new ButtonSignNodeLabel
                {
                    LabelName = l.LabelName,
                    LabelValue = l.LabelValue
                })
                .ToList();
        }

        config.ButtonSignConf = bs;
    }

    /// <summary>
    /// Build template/reminder config from node VO
    /// </summary>
    public static void SetTemplateConf(BpmnNodeVo vo)
    {
        var config = vo.GetOrCreateNodeConfigJson();
        var tc = new BpmnNodeTemplateConfJson();

        // Templates — map to flat format; Java uses TemplateConf with messageSendType as string.
        // .NET BpmnTemplateVo serializes lists only when non-null, so keep lists null to match Java's string shape.
        if (vo.TemplateVos != null && vo.TemplateVos.Count > 0)
        {
            tc.Templates = vo.TemplateVos;
        }

        // Approve remind
        var remind = vo.ApproveRemindVo;
        if (remind != null) {
            tc.ApproveRemind = new ApproveRemindConf
            {
                TemplateId = remind.TemplateId,
                Days = remind.Days,
                StandardMinutes = remind.StandardMinutes,
                NoticeTypes = remind.NoticeTypes
            };
        }

        // Overtime conf (migrated from bpm_process_node_overtime)
        if (vo.OvertimeConf != null)
        {
            tc.OvertimeConf = vo.OvertimeConf;
        }

        config.TemplateConf = tc;
    }

    /// <summary>
    /// Build low-code field control config from node VO
    /// </summary>
    public static void SetLowCodeConf(BpmnNodeVo vo)
    {
        bool hasFieldControls = vo.LfFieldControlVOs != null && vo.LfFieldControlVOs.Count > 0;
        bool hasFormHidden = vo.FormHidden != null && vo.FormHidden.Count > 0;
        if (!hasFieldControls && !hasFormHidden) return;

        var config = vo.GetOrCreateNodeConfigJson();
        var lowCodeConf = new BpmnNodeLowCodeConfJson();
        if (hasFieldControls)
        {
            lowCodeConf.FieldControls = vo.LfFieldControlVOs;
        }
        if (hasFormHidden)
        {
            lowCodeConf.FormHidden = vo.FormHidden;
        }
        config.LowCodeConf = lowCodeConf;
    }

    /// <summary>
    /// Set back type for disagree action (migrated from bpm_process_node_back)
    /// </summary>
    public static void SetBackType(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null || prop.BackType == null) return;

        var config = vo.GetOrCreateNodeConfigJson();
        config.BackType = prop.BackType;
    }

    /// <summary>
    /// Build UDR (custom rule) approver config from node VO
    /// </summary>
    public static void SetUdrConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null || prop.UdrAssigneeProperty == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.UdrConfList ??= new List<ApproverUDRConf>();
        approverConf.UdrConfList.Add(new ApproverUDRConf
        {
            ValueJson = prop.UdrValueJson,
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio,
            UdrProperty = prop.UdrAssigneeProperty.Id,
            UdrPropertyName = prop.UdrAssigneeProperty.Name,
            Ext1 = prop.Ext1,
            Ext2 = prop.Ext2,
            Ext3 = prop.Ext3,
            Ext4 = prop.Ext4
        });
    }

    /// <summary>
    /// Build form-related user approver config from node VO
    /// </summary>
    public static void SetFormRelatedUserConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null || prop.FormInfos == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.FormRelatedUserConfList ??= new List<ApproverFormRelatedUserConf>();
        approverConf.FormRelatedUserConfList.Add(new ApproverFormRelatedUserConf
        {
            ValueJson = JsonConfUtil.ToJsonString(prop.FormInfos),
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio,
            ValueType = prop.FormAssigneeProperty,
            ValueTypeName = prop.FormAssigneeProperty != null
                ? NodeFormAssigneePropertyEnumExtensions.GetDescByCode(prop.FormAssigneeProperty.Value)
                : null
        });
    }

    /// <summary>
    /// Build previous-node-related user approver config from node VO.
    /// Unlike form-related config, this only needs node property (no form elements).
    /// </summary>
    public static void SetPrevNodeRelatedUserConf(BpmnNodeVo vo)
    {
        var prop = vo.Property;
        if (prop == null || prop.FormAssigneeProperty == null) return;

        var approverConf = GetOrCreateApproverConf(vo);
        approverConf.PrevNodeRelatedUserConfList ??= new List<ApproverPrevNodeRelatedUserConf>();
        approverConf.PrevNodeRelatedUserConfList.Add(new ApproverPrevNodeRelatedUserConf
        {
            SignType = prop.SignType,
            ArbitrationRatio = prop.ArbitrationRatio,
            ValueType = prop.FormAssigneeProperty,
            ValueTypeName = NodePrevNodeAssigneePropertyEnumExtensions.GetDescByCode(prop.FormAssigneeProperty.Value)
        });
    }

    // ============ Private helpers ============

    private static BpmnNodeApproverConfJson GetOrCreateApproverConf(BpmnNodeVo vo)
    {
        var config = vo.GetOrCreateNodeConfigJson();
        config.ApproverConf ??= new BpmnNodeApproverConfJson();
        return config.ApproverConf;
    }

    private static void AddButtonsFromList(List<ButtonSignButtonConf> list,
        List<BpmnConfCommonButtonPropertyVo> buttonItems, int pageType, int startPageOnly)
    {
        if (buttonItems == null || buttonItems.Count == 0) return;

        foreach (var item in buttonItems)
        {
            int btnType = item.ButtonType ?? 0;
            string customName = item.ButtonName;
            // 自定义名称非空时使用自定义值,否则回退到按钮类型对应的默认名称
            string resolvedName = !string.IsNullOrWhiteSpace(customName)
                ? customName
                : ButtonTypeEnumExtensions.GetDescByCode(btnType);
            list.Add(new ButtonSignButtonConf
            {
                ButtonPageType = pageType,
                ButtonType = btnType,
                ButtonName = resolvedName,
                StartPageOnly = startPageOnly
            });
        }
    }

    private static string? JoinList(List<string> list)
    {
        if (list == null || list.Count == 0) return null;
        return string.Join(",", list);
    }
}
