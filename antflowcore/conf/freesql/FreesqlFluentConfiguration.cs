using antflowcore.entity;
using AntFlowCore.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace antflowcore.conf.serviceregistration;

public static class FreesqlFluentConfiguration
{
    public static void AddFreeSqlFluentConfig(this IServiceProvider service)
    {
        IFreeSql freeSql = service.GetRequiredService<IFreeSql>();
        freeSql.CodeFirst
            .ConfigEntity<BpmnConf>(a =>
            {
                a.Name("t_bpmn_conf");

                a.Property(b => b.Id).Name("id").IsIdentity(true).IsPrimary(true);
                a.Property(b => b.BpmnCode).Name("bpmn_code");
                a.Property(b => b.BpmnName).Name("bpmn_name");
                a.Property(b => b.BpmnType).Name("bpmn_type");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.AppId).Name("app_id");
                a.Property(b => b.DeduplicationType).Name("deduplication_type");
                a.Property(b => b.EffectiveStatus).Name("effective_status");
                a.Property(b => b.IsAll).Name("is_all");
                a.Property(b => b.IsOutSideProcess).Name("is_out_side_process");
                a.Property(b => b.IsLowCodeFlow).Name("is_lowcode_flow");
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.ExtraFlags).Name("extra_flags");
            }).ConfigEntity<BpmAfDeployment>(a =>
            {
                a.Name("bpm_af_deployment");
                a.Property(b => b.Id).Name("id").IsPrimary(true);
                a.Property(b => b.Rev).Name("rev");
                a.Property(b => b.Name).Name("name");
                a.Property(b => b.Content).Name("content");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmAfExecution>(a =>
            {
                a.Name("bpm_af_execution");
                a.Property(b => b.Id).IsPrimary(true).Name("id");
                a.Property(b => b.Rev).Name("rev_");
                a.Property(b => b.ProcInstId).Name("proc_inst_id");
                a.Property(b => b.BusinessKey).Name("business_key");
                a.Property(b => b.ParentId).Name("parent_id");
                a.Property(b => b.ProcDefId).Name("proc_def_id");
                a.Property(b => b.SuperExec).Name("super_exec");
                a.Property(b => b.RootProcInstId).Name("root_proc_inst_id");
                a.Property(b => b.ActId).Name("act_id");
                a.Property(b => b.IsActive).Name("is_active");
                a.Property(b => b.IsConcurrent).Name("is_concurrent");
                a.Property(b => b.TenantId).Name("tenant_id");
                a.Property(b => b.Name).Name("name");
                a.Property(b => b.StartTime).Name("start_time");
                a.Property(b => b.StartUserId).Name("start_user_id");
                a.Property(b => b.IsCountEnabled).Name("is_count_enabled");
                a.Property(b => b.EvtSubscrCount).Name("evt_subscr_count");
                a.Property(b => b.TaskCount).Name("task_count");
                a.Property(b => b.VarCount).Name("var_count");
                a.Property(b => b.SignType).Name("sign_type");
            }).ConfigEntity<BpmAfTask>(a =>
            {
                a.Name("bpm_af_task");
                a.Property(b => b.Id).IsPrimary(true).Name("id");
                a.Property(b => b.Rev).Name("rev");
                a.Property(b => b.ExecutionId).Name("execution_id");
                a.Property(b => b.ProcInstId).Name("proc_inst_id");
                a.Property(b => b.ProcDefId).Name("proc_def_id");
                a.Property(b => b.Name).Name("name");
                a.Property(b => b.ParentTaskId).Name("parent_task_id");
                a.Property(b => b.TaskDefKey).Name("task_def_key");
                a.Property(b => b.Owner).Name("owner");
                a.Property(b => b.Assignee).Name("assignee");
                a.Property(b => b.AssigneeName).Name("assignee_name");
                a.Property(b => b.Delegation).Name("delegation");
                a.Property(b => b.Priority).Name("priority");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.DueDate).Name("due_date");
                a.Property(b => b.Category).Name("category");
                a.Property(b => b.SuspensionState).Name("suspension_state");
                a.Property(b => b.TenantId).Name("tenant_id");
                a.Property(b => b.FormKey).Name("form_key");
                a.Property(b => b.Description).Name("description");

                // 忽略映射字段
                a.Property(b => b.ProcessNumber).IsIgnore(true);
                a.Property(b => b.IsNextNodeSignUp).IsIgnore(true);
            }).ConfigEntity<BpmAfTaskInst>(a =>
            {
                a.Name("bpm_af_taskinst");
                a.Property(b => b.Id).IsPrimary(true).Name("id");
                a.Property(b => b.ProcDefId).Name("proc_def_id");
                a.Property(b => b.TaskDefKey).Name("task_def_key");
                a.Property(b => b.ProcInstId).Name("proc_inst_id");
                a.Property(b => b.ExecutionId).Name("execution_id");
                a.Property(b => b.Name).Name("name");
                a.Property(b => b.ParentTaskId).Name("parent_task_id");
                a.Property(b => b.Owner).Name("owner");
                a.Property(b => b.Assignee).Name("assignee");
                a.Property(b => b.AssigneeName).Name("assignee_name");
                a.Property(b => b.OriginalAssignee).Name("original_assignee");
                a.Property(b => b.OriginalAssigneeName).Name("original_assignee_name");
                a.Property(b => b.TransferReason).Name("transfer_reason");
                a.Property(b => b.VerifyStatus).Name("verify_status");
                a.Property(b => b.VerifyDesc).Name("verify_desc");
                a.Property(b => b.StartTime).Name("start_time");
                a.Property(b => b.ClaimTime).Name("claim_time");
                a.Property(b => b.EndTime).Name("end_time");
                a.Property(b => b.Duration).Name("duration");
                a.Property(b => b.DeleteReason).Name("delete_reason");
                a.Property(b => b.Priority).Name("priority");
                a.Property(b => b.DueDate).Name("due_date");
                a.Property(b => b.FormKey).Name("form_key");
                a.Property(b => b.Category).Name("category");
                a.Property(b => b.TenantId).Name("tenant_id");
                a.Property(b => b.Description).Name("description");
                a.Property(b => b.UpdateUser).Name("update_user");
            })
            .ConfigEntity<BpmBusiness>(a =>
            {
                a.Name("bpm_business");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BusinessId).Name("business_id");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.ProcessCode).Name("process_code");
                a.Property(b => b.CreateUserName).Name("create_user_name");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.IsDel).Name("is_del");
            })
            .ConfigEntity<BpmBusinessProcess>(a =>
            {
                a.Name("bpm_business_process");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessinessKey).Name("PROCESSINESS_KEY");
                a.Property(b => b.BusinessId).Name("BUSINESS_ID");
                a.Property(b => b.BusinessNumber).Name("BUSINESS_NUMBER");
                a.Property(b => b.EntryId).Name("ENTRY_ID");
                a.Property(b => b.Version).Name("VERSION");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.Description).Name("description");
                a.Property(b => b.ProcessState).Name("process_state");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UserName).Name("user_name");
                a.Property(b => b.ProcessDigest).Name("process_digest");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.DataSourceId).Name("data_source_id");
                a.Property(b => b.ProcInstId).Name("PROC_INST_ID_");
                a.Property(b => b.BackUserId).Name("back_user_id");
                a.Property(b => b.IsOutSideProcess).Name("is_out_side_process");
                a.Property(b => b.IsLowCodeFlow).Name("is_lowcode_flow");
            })
            .ConfigEntity<BpmFlowrunEntrust>(a =>
            {
                a.Name("bpm_flowrun_entrust");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.RunInfoId).Name("runinfoid");
                a.Property(b => b.RunTaskId).Name("runtaskid");
                a.Property(b => b.Original).Name("original");
                a.Property(b => b.OriginalName).Name("original_name");
                a.Property(b => b.Actual).Name("actual");
                a.Property(b => b.ActualName).Name("actual_name");
                a.Property(b => b.Type).Name("type");
                a.Property(b => b.IsRead).Name("is_read");
                a.Property(b => b.ProcDefId).Name("proc_def_id");
                a.Property(b => b.IsView).Name("is_view");
            }).ConfigEntity<BpmFlowruninfo>(a =>
            {
                a.Name("bpm_flowruninfo");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.RunInfoId).Name("runinfoid");
                a.Property(b => b.CreateUserId).Name("create_UserId");
                a.Property(b => b.EntityKey).Name("entitykey");
                a.Property(b => b.EntityClass).Name("entityclass");
                a.Property(b => b.EntityKeyType).Name("entitykeytype");
                a.Property(b => b.CreateActor).Name("createactor");
                a.Property(b => b.CreateDepart).Name("createdepart");
                a.Property(b => b.CreateDate).Name("createdate");
            }).ConfigEntity<BpmManualNotify>(a =>
            {
                a.Name("bpm_manual_notify");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BusinessId).Name("business_id");
                a.Property(b => b.Code).Name("code");
                a.Property(b => b.LastTime).Name("last_time");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnApproveRemind>(a =>
            {
                a.Name("t_bpmn_approve_remind");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ConfId).Name("conf_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.TemplateId).Name("template_id");
                a.Property(b => b.Days).Name("days");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<BpmnConfLfFormdata>(a =>
            {
                a.Name("t_bpmn_conf_lf_formdata");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnConfId).Name("bpmn_conf_id");
                a.Property(b => b.Formdata).Name("formdata");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnConfLfFormdataField>(a =>
            {
                a.Name("t_bpmn_conf_lf_formdata_field");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnConfId).Name("bpmn_conf_id");
                a.Property(b => b.FormDataId).Name("formdata_id");
                a.Property(b => b.FieldId).Name("field_id");
                a.Property(b => b.FieldName).Name("field_name");
                a.Property(b => b.FieldType).Name("field_type");
                a.Property(b => b.IsConditionField).Name("is_condition");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnConfNoticeTemplate>(a =>
            {
                a.Name("t_bpmn_conf_notice_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnCode).Name("bpmn_code");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnConfNoticeTemplateDetail>(a =>
            {
                a.Name("t_bpmn_conf_notice_template_detail");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnCode).Name("bpmn_code");
                a.Property(b => b.NoticeTemplateType).Name("notice_template_type");
                a.Property(b => b.NoticeTemplateDetail).Name("notice_template_detail");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNode>(a =>
            {
                a.Name("t_bpmn_node");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ConfId).Name("conf_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.NodeType).Name("node_type");
                a.Property(b => b.NodeProperty).Name("node_property");
                a.Property(b => b.NodeFrom).Name("node_from");
                a.Property(b => b.BatchStatus).Name("batch_status");
                a.Property(b => b.ApprovalStandard).Name("approval_standard");
                a.Property(b => b.NodeName).Name("node_name");
                a.Property(b => b.NodeDisplayName).Name("node_display_name");
                a.Property(b => b.Annotation).Name("annotation");
                a.Property(b => b.IsDeduplication).Name("is_deduplication");
                a.Property(b => b.IsSignUp).Name("is_sign_up");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.NodeFroms).Name("node_froms");
                a.Property(b => b.IsDynamicCondition).Name("is_dynamicCondition");
                a.Property(b => b.IsParallel).Name("is_parallel");
                a.Property(b => b.IsOutSideProcess).IsIgnore(true);
                a.Property(b => b.IsLowCodeFlow).IsIgnore(true);
                a.Property(b => b.ExtraFlags).IsIgnore(true);
            }).ConfigEntity<BpmnNodeAssignLevelConf>(a =>
            {
                a.Name("t_bpmn_node_assign_level_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.AssignLevelType).Name("assign_level_type");
                a.Property(b => b.AssignLevelGrade).Name("assign_level_grade");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            })
            .ConfigEntity<BpmnNodeBusinessTableConf>(a =>
            {
                a.Name("t_bpmn_node_business_table_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.ConfigurationTableType).Name("configuration_table_type");
                a.Property(b => b.TableFieldType).Name("table_field_type");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeButtonConf>(a =>
            {
                a.Name("t_bpmn_node_button_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.ButtonPageType).Name("button_page_type");
                a.Property(b => b.ButtonType).Name("button_type");
                a.Property(b => b.ButtonName).Name("button_name");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeConditionsConf>(a =>
            {
                a.Name("t_bpmn_node_conditions_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.IsDefault).Name("is_default");
                a.Property(b => b.Sort).Name("sort");
                a.Property(b => b.ExtJson).Name("ext_json");
                a.Property(b => b.GroupRelation).Name("group_relation");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeConditionsParamConf>(a =>
            {
                a.Name("t_bpmn_node_conditions_param_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeConditionsId).Name("bpmn_node_conditions_id");
                a.Property(b => b.ConditionParamType).Name("condition_param_type");
                a.Property(b => b.ConditionParamName).Name("condition_param_name");
                a.Property(b => b.ConditionParamJsom).Name("condition_param_jsom");
                a.Property(b => b.TheOperator).Name("operator");
                a.Property(b => b.CondRelation).Name("cond_relation");
                a.Property(b => b.CondGroup).Name("cond_group");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<antflowcore.entity.BpmnNodeCustomizeConf>(a =>
            {
                a.Name("t_bpmn_node_customize_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeHrbpConf>(a =>
            {
                a.Name("t_bpmn_node_hrbp_conf"); // 表名
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.HrbpConfType).Name("hrbp_conf_type");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeLfFormdataFieldControl>(a =>
            {
                a.Name("t_bpmn_node_lf_formdata_field_control");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.FormdataId).Name("formdata_id");
                a.Property(b => b.FieldId).Name("field_id");
                a.Property(b => b.FieldName).Name("field_name");
                a.Property(b => b.Perm).Name("field_perm");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeLoopConf>(a =>
            {
                a.Name("t_bpmn_node_loop_conf"); // 表名配置

                // 主键自增
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);

                // 字段名映射配置
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.LoopEndType).Name("loop_end_type");
                a.Property(b => b.LoopNumberPlies).Name("loop_number_plies");
                a.Property(b => b.LoopEndPerson).Name("loop_end_person");
                a.Property(b => b.NoparticipatingStaffIds).Name("noparticipating_staff_ids");
                a.Property(b => b.LoopEndGrade).Name("loop_end_grade");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeOutSideAccessConf>(a =>
            {
                a.Name("t_bpmn_node_out_side_access_conf"); // 表名

                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);

                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.NodeMark).Name("node_mark");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodePersonnelConf>(a =>
            {
                a.Name("t_bpmn_node_personnel_conf");

                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodePersonnelEmplConf>(a =>
            {
                a.Name("t_bpmn_node_personnel_empl_conf");

                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BpmnNodePersonneId).Name("bpmn_node_personne_id");
                a.Property(b => b.EmplId).Name("empl_id");
                a.Property(b => b.EmplName).Name("empl_name");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeRoleConf>(a =>
            {
                a.Name("t_bpmn_node_role_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.RoleId).Name("role_id");
                a.Property(b => b.RoleName).Name("role_name");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeRoleOutsideEmpConf>(a =>
            {
                a.Name("t_bpmn_node_role_outside_emp_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.EmplId).Name("empl_id");
                a.Property(b => b.EmplName).Name("empl_name");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeSignUpConf>(a =>
            {
                a.Name("t_bpmn_node_sign_up_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.AfterSignUpWay).Name("after_sign_up_way");
                a.Property(b => b.SignUpType).Name("sign_up_type");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnNodeTo>(a =>
            {
                a.Name("t_bpmn_node_to");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.NodeTo).Name("node_to");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmnOutsideConf>(a =>
            {
                a.Name("t_bpmn_outside_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.CallBackUrl).Name("call_back_url");
                a.Property(b => b.DetailUrl).Name("detail_url");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.BusinessName).Name("business_name");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.CreateUserId).Name("create_user_id");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.ModifiedUserId).Name("modified_user_id");
                a.Property(b => b.ModifiedTime).Name("modified_time");
            }).ConfigEntity<BpmnTemplate>(a =>
            {
                a.Name("t_bpmn_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ConfId).Name("conf_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.TemplateId).Name("template_id");
                a.Property(b => b.MessageSendType).Name("message_send_type");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.Event).Name("event");
                a.Property(b => b.Informs).Name(("informs"));
                a.Property(b => b.Roles).Name(("roles"));
                a.Property(b => b.Funcs).Name(("funcs"));
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<BpmnViewPageButton>(a =>
            {
                a.Name("t_bpmn_view_page_button");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ConfId).Name("conf_id");
                a.Property(b => b.ViewType).Name("view_type");
                a.Property(b => b.ButtonType).Name("button_type");
                a.Property(b => b.ButtonName).Name("button_name");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmProcessAppApplication>(a =>
            {
                a.Name("bpm_process_app_application");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessCode).Name("business_code");
                a.Property(b => b.Title).Name("process_name");
                a.Property(b => b.ApplyType).Name("apply_type");
                a.Property(b => b.PcIcon).Name("pc_icon");
                a.Property(b => b.EffectiveSource).Name("effective_source");
                a.Property(b => b.IsSon).Name("is_son");
                a.Property(b => b.LookUrl).Name("look_url");
                a.Property(b => b.SubmitUrl).Name("submit_url");
                a.Property(b => b.ConditionUrl).Name("condition_url");
                a.Property(b => b.ParentId).Name("parent_id");
                a.Property(b => b.ApplicationUrl).Name("application_url");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.PermissionsCode).Name("permissions_code");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUserId).Name("create_user_id");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.IsAll).Name("is_all");
                a.Property(b => b.UserRequestUri).Name("user_request_uri");
                a.Property(b => b.RoleRequestUri).Name("role_request_uri");
            }).ConfigEntity<BpmProcessAppData>(a =>
            {
                a.Name("bpm_process_app_data");
                a.Property(b => b.Id).Name("id").IsPrimary(true).IsIdentity(true);
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.ProcessName).Name("process_name");
                a.Property(b => b.State).Name("state");
                a.Property(b => b.Route).Name("route");
                a.Property(b => b.Sort).Name("sort");
                a.Property(b => b.Source).Name("source");
                a.Property(b => b.IsAll).Name("is_all");
                a.Property(b => b.VersionId).Name("version_id");
                a.Property(b => b.ApplicationId).Name("application_id");
                a.Property(b => b.Type).Name("type");
            }).ConfigEntity<BpmProcessCategory>(a =>
            {
                a.Name("bpm_process_category");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessTypeName).Name("process_type_name");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.State).Name("state");
                a.Property(b => b.Sort).Name("sort");
                a.Property(b => b.IsApp).Name("is_app");
                a.Property(b => b.Entrance).Name("entrance");
            }).ConfigEntity<BpmProcessDept>(a =>
            {
                a.Name("bpm_process_dept");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessCode).Name("process_code");
                a.Property(b => b.ProcessType).Name("process_type");
                a.Property(b => b.ProcessName).Name("process_name");
                a.Property(b => b.DeptId).Name("dep_id");
                a.Property(b => b.Remarks).Name("remarks");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.IsAll).Name("is_all");
            }).ConfigEntity<BpmProcessForward>(a =>
            {
                a.Name("bpm_process_forward");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ForwardUserId).Name("forward_user_id");
                a.Property(b => b.ForwardUserName).Name("forward_user_name");
                a.Property(b => b.ProcessInstanceId).Name("processInstance_Id");
                a.Property(b => b.CreateTime).Name("create_time");    
                a.Property(b => b.CreateUserId).Name("create_user_id");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.IsRead).Name("is_read");
                a.Property(b => b.TaskId).Name("task_id");
                a.Property(b => b.ProcessNumber).Name("process_number");
            }).ConfigEntity<BpmProcessName>(a =>
            {
                a.Name("bpm_process_name");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessName).Name("process_name").IsNullable(true);
                a.Property(b => b.IsDel).Name("is_del").IsNullable(false);
                a.Property(b => b.CreateTime).Name("create_time").IsNullable(true);
            }).ConfigEntity<BpmProcessNameRelevancy>(a =>
            {
                a.Name("bpm_process_name_relevancy");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessNameId).Name("process_name_id");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateTime).Name("create_time");
            }).ConfigEntity<BpmProcessNodeOvertime>(a =>
            {
                a.Name("bpm_process_node_overtime");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.NoticeType).Name("notice_type");
                a.Property(b => b.NodeName).Name("node_name");
                a.Property(b => b.NodeKey).Name("node_key");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.NoticeTime).Name("notice_time");
            }).ConfigEntity<BpmProcessNodeRecord>(a =>
            {
                a.Name("bpm_process_node_record");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessInstanceId).Name("processInstance_id");
                a.Property(b => b.TaskId).Name("task_id");
                a.Property(b => b.CreateTime).Name("create_time");
            }).ConfigEntity<BpmProcessNodeSubmit>(a =>
            {
                a.Name("bpm_process_node_submit");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessInstanceId).Name("processInstance_Id");
                a.Property(b => b.BackType).Name("back_type");
                a.Property(b => b.NodeKey).Name("node_key");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.State).Name("state");
            }).ConfigEntity<BpmProcessNotice>(a =>
            {
                a.Name("bpm_process_notice");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.Type).Name("type");
                a.Property(b => b.ProcessKey).Name("process_key");
            }).ConfigEntity<BpmProcessOperation>(a =>
            {
                a.Name("bpm_process_operation");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.ProcessNode).Name("process_node");
                a.Property(b => b.Type).Name("type");
            }).ConfigEntity<BpmProcessPermissions>(a =>
            {
                a.Name("bpm_process_permissions");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.UserId).Name("user_id");
                a.Property(b => b.DepId).Name("dep_id");
                a.Property(b => b.PermissionsType).Name("permissions_type");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.ProcessKey).Name("process_key");
                a.Property(b => b.OfficeId).Name("office_id");
            }).ConfigEntity<BpmTaskconfig>(a =>
            {
                a.Name("bpm_taskconfig");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcDefId).Name("proc_def_id_");
                a.Property(b => b.TaskDefKey).Name("task_def_key_");
                a.Property(b => b.UserId).Name("user_id");
                a.Property(b => b.Number).Name("number");
            }).ConfigEntity<BpmVariable>(a =>
            {
                a.Name("t_bpm_variable");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.ProcessNum).Name("process_num");
                a.Property(b => b.ProcessName).Name("process_name");
                a.Property(b => b.ProcessDesc).Name("process_desc");
                a.Property(b => b.ProcessStartConditions).Name("process_start_conditions");
                a.Property(b => b.BpmnCode).Name("bpmn_code");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableApproveRemind>(a =>
            {
                a.Name("t_bpm_variable_approve_remind");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.Content).Name("content");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableButton>(a =>
            {
                a.Name("t_bpm_variable_button");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.ButtonPageType).Name("button_page_type");
                a.Property(b => b.ButtonType).Name("button_type");
                a.Property(b => b.ButtonName).Name("button_name");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableMessage>(a =>
            {
                a.Name("t_bpm_variable_message");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.MessageType).Name("message_type");
                a.Property(b => b.EventType).Name("event_type");
                a.Property(b => b.Content).Name("content");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableMultiplayer>(a =>
            {
                a.Name("t_bpm_variable_multiplayer");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.ElementName).Name("element_name");
                a.Property(b => b.CollectionName).Name("collection_name");
                a.Property(b => b.SignType).Name("sign_type");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.UnderTakeStatus).IsIgnore(true);
            }).ConfigEntity<BpmVariableMultiplayerPersonnel>(a =>
            {
                a.Name("t_bpm_variable_multiplayer_personnel");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableMultiplayerId).Name("variable_multiplayer_id");
                a.Property(b => b.Assignee).Name("assignee");
                a.Property(b => b.AssigneeName).Name("assignee_name");
                a.Property(b => b.UndertakeStatus).Name("undertake_status");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableSequenceFlow>(a =>
            {
                a.Name("t_bpm_variable_sequence_flow");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.ElementName).Name("element_name");
                a.Property(b => b.ElementFromId).Name("element_from_id");
                a.Property(b => b.ElementToId).Name("element_to_id");
                a.Property(b => b.SequenceFlowType).Name("sequence_flow_type");
                a.Property(b => b.SequenceFlowConditions).Name("sequence_flow_conditions");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableSignUp>(a =>
            {
                a.Name("t_bpm_variable_sign_up");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.AfterSignUpWay).Name("after_sign_up_way");
                a.Property(b => b.SubElements).Name("sub_elements");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableSignUpPersonnel>(a =>
            {
                a.Name("t_bpm_variable_sign_up_personnel");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.Assignee).Name("assignee");
                a.Property(b => b.AssigneeName).Name("assignee_name");
                a.Property(b => b.Remark).Name("remark");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableSingle>(a =>
            {
                a.Name("t_bpm_variable_single"); // 表名

                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ElementId).Name("element_id");
                a.Property(b => b.NodeId).Name("node_id");
                a.Property(b => b.ElementName).Name("element_name");
                a.Property(b => b.AssigneeParamName).Name("assignee_param_name");
                a.Property(b => b.Assignee); // 默认映射属性名，无需Name
                a.Property(b => b.AssigneeName).Name("assignee_name");
                a.Property(b => b.Remark); // 默认映射
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVariableViewPageButton>(a =>
            {
                a.Name("t_bpm_variable_view_page_button"); // 表名
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.VariableId).Name("variable_id");
                a.Property(b => b.ViewType).Name("view_type");
                a.Property(b => b.ButtonType).Name("button_type");
                a.Property(b => b.ButtonName).Name("button_name");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<BpmVerifyInfo>(a =>
            {
                a.Name("bpm_verify_info");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.RunInfoId).Name("run_info_id");
                a.Property(b => b.VerifyUserId).Name("verify_user_id");
                a.Property(b => b.VerifyUserName).Name("verify_user_name");
                a.Property(b => b.VerifyStatus).Name("verify_status");
                a.Property(b => b.VerifyDesc).Name("verify_desc");
                a.Property(b => b.VerifyDate).Name("verify_date");
                a.Property(b => b.TaskName).Name("task_name");
                a.Property(b => b.TaskId).Name("task_id");
                a.Property(b => b.TaskDefKey).Name("task_def_key");
                a.Property(b => b.BusinessType).Name("business_type");
                a.Property(b => b.BusinessId).Name("business_id");
                a.Property(b => b.OriginalId).Name("original_id");
                a.Property(b => b.ProcessCode).Name("process_code");
            }).ConfigEntity<DefaultTemplate>(a =>
            {
                a.Name("t_default_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.Event).Name("event");
                a.Property(b => b.TemplateId).Name("template_id");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<Department>(entity =>
            {
                entity.Name("t_department");
                entity.Property(a => a.Id).Name("id").IsPrimary(true).IsIdentity(true);
                entity.Property(a => a.Name).Name("name");
                entity.Property(a => a.ShortName).Name("short_name");
                entity.Property(a => a.ParentId).Name("parent_id");
                entity.Property(a => a.Path).Name("path");
                entity.Property(a => a.Level).Name("level");
                entity.Property(a => a.LeaderId).Name("leader_id");
                entity.Property(a => a.Sort).Name("sort");
                entity.Property(a => a.IsDel).Name("is_del");
                entity.Property(a => a.IsHide).Name("is_hide");
                entity.Property(a => a.CreateUser).Name("create_user");
                entity.Property(a => a.UpdateUser).Name("update_user");
                entity.Property(a => a.CreateTime).Name("create_time");
                entity.Property(a => a.UpdateTime).Name("update_time");
            }).ConfigEntity<DictData>(entity =>
            {
                entity.Name("t_dict_data");
                entity.Property(a => a.Id).Name("id").IsPrimary(true).IsIdentity(true);
                entity.Property(a => a.Sort).Name("dict_sort");
                entity.Property(a => a.Label).Name("dict_label");
                entity.Property(a => a.Value).Name("dict_value");
                entity.Property(a => a.DictType).Name("dict_type");
                entity.Property(a => a.CssClass).Name("css_class");
                entity.Property(a => a.ListClass).Name("list_class");
                entity.Property(a => a.IsDefault).Name("is_default");
                entity.Property(a => a.IsDel).Name("is_del");
                entity.Property(a => a.CreateTime).Name("create_time");
                entity.Property(a => a.CreateUser).Name("create_user");
                entity.Property(a => a.UpdateTime).Name("update_time");
                entity.Property(a => a.UpdateUser).Name("update_user");
                entity.Property(a => a.Remark).Name("remark");
            }).ConfigEntity<DictMain>(entity =>
            {
                entity.Name("t_dict_main");
                entity.Property(a => a.Id).Name("id").IsPrimary(true).IsIdentity(true);
                entity.Property(a => a.DictName).Name("dict_name");
                entity.Property(a => a.DictType).Name("dict_type");
                entity.Property(a => a.IsDel).Name("is_del");
                entity.Property(a => a.CreateTime).Name("create_time");
                entity.Property(a => a.CreateUser).Name("create_user");
                entity.Property(a => a.UpdateTime).Name("update_time");
                entity.Property(a => a.UpdateUser).Name("update_user");
                entity.Property(a => a.Remark).Name("remark");
            }).ConfigEntity<InformationTemplate>(a =>
            {
                a.Name("t_information_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.Name).Name("name").IsNullable(false);
                a.Property(b => b.Num).Name("num").IsNullable(false);
                a.Property(b => b.SystemTitle).Name("system_title").IsNullable(false);
                a.Property(b => b.SystemContent).Name("system_content").IsNullable(false);
                a.Property(b => b.MailTitle).Name("mail_title").IsNullable(false);
                a.Property(b => b.MailContent).Name("mail_content").IsNullable(false);
                a.Property(b => b.NoteContent).Name("note_content").IsNullable(false);
                a.Property(b => b.JumpUrl).Name("jump_url").IsNullable(false);
                a.Property(b => b.Remark).Name("remark").IsNullable(false);
                a.Property(b => b.Status).Name("status").IsNullable(false);
                a.Property(b => b.Evt).Name("event").IsNullable(true);
                a.Property(b => b.EventName).Name("event_name").IsNullable(false);
                a.Property(b => b.IsDel).Name("is_del").IsNullable(false);
                a.Property(b => b.CreateTime).Name("create_time").IsNullable(true);
                a.Property(b => b.CreateUser).Name("create_user").IsNullable(false);
                a.Property(b => b.UpdateTime).Name("update_time").IsNullable(true);
                a.Property(b => b.UpdateUser).Name("update_user").IsNullable(false);
            }).ConfigEntity<LFMain>(a =>
            {
                a.Name("t_lf_main");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(false);
                a.Property(b => b.ConfId).Name("conf_id").IsNullable(true);
                a.Property(b => b.FormCode).Name("form_code").IsNullable(true);
                a.Property(b => b.IsDel).Name("is_del").IsNullable(false);
                a.Property(b => b.CreateUser).Name("create_user").IsNullable(true);
                a.Property(b => b.CreateTime).Name("create_time").CanUpdate(false).IsNullable(true);
                a.Property(b => b.UpdateUser).Name("update_user").IsNullable(true);
                a.Property(b => b.UpdateTime).Name("update_time").IsNullable(true);
            }).ConfigEntity<LFMain>(a =>
            {
                a.Name("t_lf_main");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(false);
                a.Property(b => b.ConfId).Name("conf_id").IsNullable(true);
                a.Property(b => b.FormCode).Name("form_code").IsNullable(true);
                a.Property(b => b.IsDel).Name("is_del").IsNullable(false);
                a.Property(b => b.CreateUser).Name("create_user").IsNullable(true);
                a.Property(b => b.CreateTime).Name("create_time").CanUpdate(false).IsNullable(true);
                a.Property(b => b.UpdateUser).Name("update_user").IsNullable(true);
                a.Property(b => b.UpdateTime).Name("update_time").IsNullable(true);
            }).ConfigEntity<OutSideBpmAccessBusiness>(a =>
            {
                a.Name("t_out_side_bpm_access_business");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.BpmnConfId).Name("bpmn_conf_id");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.ProcessNumber).Name("process_number");
                a.Property(b => b.FormDataPc).Name("form_data_pc");
                a.Property(b => b.FormDataApp).Name("form_data_app");
                a.Property(b => b.TemplateMark).Name("template_mark");
                a.Property(b => b.StartUsername).Name("start_username");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<OutSideBpmAdminPersonnel>(a =>
            {
                a.Name("t_out_side_bpm_admin_personnel");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.Type);
                a.Property(b => b.EmployeeId).Name("employee_id");
                a.Property(b => b.EmployeeName).Name("employee_name");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<OutSideBpmApproveTemplate>(a =>
            {
                a.Name("t_out_side_bpm_approve_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.ApplicationId).Name("application_id");
                a.Property(b => b.ApproveTypeId).Name("approve_type_id");
                a.Property(b => b.ApproveTypeName).Name("approve_type_name");
                a.Property(b => b.ApiClientId).Name("api_client_id");
                a.Property(b => b.ApiClientSecret).Name("api_client_secret");
                a.Property(b => b.ApiToken).Name("api_token");
                a.Property(b => b.ApiUrl).Name("api_url");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUserId).Name("create_user_id");
            }).ConfigEntity<OutSideBpmBusinessParty>(a =>
            {
                a.Name("t_out_side_bpm_business_party");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyMark).Name("business_party_mark");
                a.Property(b => b.Name);
                a.Property(b => b.Type).Name("type");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<OutSideBpmCallbackUrlConf>(a =>
            {
                a.Name("t_out_side_bpm_callback_url_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.ApplicationId).Name("application_id");
                a.Property(b => b.BpmnConfId).Name("bpmn_conf_id");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.BpmConfCallbackUrl).Name("bpm_conf_callback_url");
                a.Property(b => b.BpmFlowCallbackUrl).Name("bpm_flow_callback_url");
                a.Property(b => b.ApiClientId).Name("api_client_id");
                a.Property(b => b.ApiClientSecret).Name("api_client_secret");
                a.Property(b => b.Status).Name("status");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<OutSideBpmConditionsTemplate>(a =>
            {
                a.Name("t_out_side_bpm_conditions_template");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BusinessPartyId).Name("business_party_id");
                a.Property(b => b.TemplateMark).Name("template_mark");
                a.Property(b => b.TemplateName).Name("template_name");
                a.Property(b => b.ApplicationId).Name("application_id");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUserId).Name("create_user_id");
            }).ConfigEntity<OutSideBpmnNodeConditionsConf>(a =>
            {
                a.Name("t_out_side_bpmn_node_conditions_conf");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.BpmnNodeId).Name("bpmn_node_id");
                a.Property(b => b.OutSideId).Name("out_side_id");
                a.Property(b => b.Remark);
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<OutSideCallBackRecord>(a =>
            {
                a.Name("t_out_side_bpm_call_back_record");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.ProcessNumber).Name("process_number");
                a.Property(b => b.Status).Name("status");
                a.Property(b => b.RetryTimes).Name("retry_times");
                a.Property(b => b.ButtonOperationType).Name("button_operation_type");
                a.Property(b => b.CallBackTypeName).Name("call_back_type_name");
                a.Property(b => b.BusinessId).Name("business_id");
                a.Property(b => b.FormCode).Name("form_code");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.UpdateTime).Name("update_time");
            }).ConfigEntity<QuickEntry>(a =>
            {
                a.Name("t_quick_entry");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.Title).Name("title");
                a.Property(b => b.EffectiveSource).Name("effective_source");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.Route).Name("route");
                a.Property(b => b.Sort).Name("sort");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.Status).Name("status");
                a.Property(b => b.VariableUrlFlag).Name("variable_url_flag");
            }).ConfigEntity<QuickEntryType>(a =>
            {
                a.Name("t_quick_entry_type");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.QuickEntryId).Name("quick_entry_id");
                a.Property(b => b.Type).Name("type");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.TypeName).Name("type_name");
            }).ConfigEntity<SysVersion>(a =>
            {
                a.Name("t_sys_version");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.Version).Name("version");
                a.Property(b => b.Description).Name("description");
                a.Property(b => b.Index).Name("index");
                a.Property(b => b.IsForce).Name("is_force");
                a.Property(b => b.AndroidUrl).Name("android_url");
                a.Property(b => b.IosUrl).Name("ios_url");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.IsHide).Name("is_hide");
                a.Property(b => b.DownloadCode).Name("download_code");
                a.Property(b => b.EffectiveTime).Name("effective_time");
            }).ConfigEntity<ThirdPartyAccountApply>(a =>
            {
                a.Name("t_biz_account_apply");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.AccountType).Name("account_type");
                a.Property(b => b.AccountOwnerName).Name("account_owner_name");
                a.Property(b => b.Remark).Name("remark");
            }).ConfigEntity<User>(a =>
            {
                a.Name("t_user");
                a.Property(b => b.Id).IsPrimary(true); // 主键，默认非自增，如果是自增加 IsIdentity(true)
                a.Property(b => b.Name).Name("user_name");
                a.Property(b => b.Mobile).Name("mobile");
                a.Property(b => b.Email).Name("email");
                a.Property(b => b.LeaderId).Name("leader_id");
                a.Property(b => b.HrbpId).Name("hrbp_id");
                a.Property(b => b.MobileIsShow).Name("mobile_is_show");
                a.Property(b => b.DepartmentId).Name("department_id");
                a.Property(b => b.Path).Name("path");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.HeadImg).Name("head_img");
            }).ConfigEntity<UserEmailSend>(a =>
            {
                a.Name("t_user_email_send");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.Sender).Name("sender");
                a.Property(b => b.Receiver).Name("receiver");
                a.Property(b => b.Title).Name("title");
                a.Property(b => b.Content).Name("content");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<UserEntrust>(a =>
            {
                a.Name("t_user_entrust");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.Sender).Name("sender");
                a.Property(b => b.ReceiverId).Name("receiver_id");
                a.Property(b => b.ReceiverName).Name("receiver_name");
                a.Property(b => b.PowerId).Name("power_id");
                a.Property(b => b.BeginTime).Name("begin_time");
                a.Property(b => b.EndTime).Name("end_time");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<UserMessage>(a =>
            {
                a.Name("t_user_message");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true).Name("id");
                a.Property(b => b.UserId).Name("user_id");
                a.Property(b => b.Title).Name("title");
                a.Property(b => b.Content).Name("content");
                a.Property(b => b.Url).Name("url");
                a.Property(b => b.Node).Name("node");
                a.Property(b => b.Params).Name("params");
                a.Property(b => b.UrlParams).IsIgnore(true);
                a.Property(b => b.IsRead).Name("is_read");
                a.Property(b => b.IsDel).Name("is_del");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
                a.Property(b => b.AppUrl).Name("app_url");
                a.Property(b => b.Source).Name("source");
            }).ConfigEntity<UserMessageStatus>(a =>
            {
                a.Name("t_user_message_status");
                a.Property(b => b.Id).IsPrimary(true).IsIdentity(true);
                a.Property(b => b.UserId).Name("user_id");
                a.Property(b => b.MessageStatus).Name("message_status");
                a.Property(b => b.MailStatus).Name("mail_status");
                a.Property(b => b.OpenPhone).Name("open_phone");
                a.Property(b => b.NotTrouble).Name("not_trouble");
                a.Property(b => b.NotTroubleTimeBegin).Name("not_trouble_time_begin");
                a.Property(b => b.NotTroubleTimeEnd).Name("not_trouble_time_end");
                a.Property(b => b.CreateTime).Name("create_time");
                a.Property(b => b.UpdateTime).Name("update_time");
                a.Property(b => b.CreateUser).Name("create_user");
                a.Property(b => b.UpdateUser).Name("update_user");
            }).ConfigEntity<UserRole>(entity =>
            {
                entity.Name("t_user_role");
                entity.Property(e => e.Id).IsPrimary(true).IsIdentity(true);
                entity.Property(e => e.UserId).Name("user_id");
                entity.Property(e => e.RoleId).Name("role_id");
            }).ConfigEntity<LFMainField>(entity =>
            {
                entity.Name("t_lf_main_field"); // 表名

                entity.Property(e => e.Id).IsPrimary(true).IsIdentity(false);
                entity.Property(e => e.MainId).Name("main_id");
                entity.Property(e => e.FormCode).Name("form_code");
                entity.Property(e => e.FieldId).Name("field_id");
                entity.Property(e => e.FieldName).Name("field_name");
                entity.Property(e => e.ParentFieldId).Name("parent_field_id");
                entity.Property(e => e.ParentFieldName).Name("parent_field_name");
                entity.Property(e => e.FieldValue).Name("field_value");
                entity.Property(e => e.FieldValueNumber).Name("field_value_number");
                entity.Property(e => e.FieldValueDt).Name("field_value_dt");
                entity.Property(e => e.FieldValueText).Name("field_value_text");
                entity.Property(e => e.Sort).Name("sort");
                entity.Property(e => e.IsDel).Name("is_del");
                entity.Property(e => e.CreateUser).Name("create_user");
                entity.Property(e => e.CreateTime).Name("create_time").CanInsert(true);
                entity.Property(e => e.UpdateUser).Name("update_user");
                entity.Property(e => e.UpdateTime).Name("update_time").CanInsert(true).CanUpdate(true);
            });
    }
}