using AntFlowCore.Base.entity;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.sqlsugar;

public static class SqlSugarFluentConfiguration
{
    public static void AddSqlSugarFluentConfig(this IServiceProvider service)
    {
        ISqlSugarClient db = service.GetRequiredService<ISqlSugarClient>();
        ConfigureEntities(db);
    }

    public static void ConfigureEntities(ISqlSugarClient db)
    {
        // BpmnConf
        db.MappingTables.Add(nameof(BpmnConf), "t_bpmn_conf");
        db.MappingColumns.Add(nameof(BpmnConf.Id), "id", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.BpmnCode), "bpmn_code", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.BpmnName), "bpmn_name", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.BpmnType), "bpmn_type", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.FormCode), "form_code", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.AppId), "app_id", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.DeduplicationType), "deduplication_type", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.EffectiveStatus), "effective_status", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.IsAll), "is_all", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.IsOutSideProcess), "is_out_side_process", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.IsLowCodeFlow), "is_lowcode_flow", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.BusinessPartyId), "business_party_id", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.Remark), "remark", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.IsDel), "is_del", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.TenantId), "tenant_id", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.CreateUser), "create_user", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.CreateTime), "create_time", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.UpdateUser), "update_user", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.UpdateTime), "update_time", nameof(BpmnConf));
        db.MappingColumns.Add(nameof(BpmnConf.ExtraFlags), "extra_flags", nameof(BpmnConf));

        // BpmAfDeployment
        db.MappingTables.Add(nameof(BpmAfDeployment), "bpm_af_deployment");
        db.MappingColumns.Add(nameof(BpmAfDeployment.Id), "id", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.Rev), "rev", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.Name), "name", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.Content), "content", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.Remark), "remark", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.IsDel), "is_del", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.TenantId), "tenant_id", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.CreateUser), "create_user", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.CreateTime), "create_time", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.UpdateUser), "update_user", nameof(BpmAfDeployment));
        db.MappingColumns.Add(nameof(BpmAfDeployment.UpdateTime), "update_time", nameof(BpmAfDeployment));

        // BpmAfExecution
        db.MappingTables.Add(nameof(BpmAfExecution), "bpm_af_execution");
        db.MappingColumns.Add(nameof(BpmAfExecution.Id), "id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.Rev), "rev_", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.ProcInstId), "proc_inst_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.BusinessKey), "business_key", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.ParentId), "parent_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.ProcDefId), "proc_def_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.SuperExec), "super_exec", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.RootProcInstId), "root_proc_inst_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.ActId), "act_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.IsActive), "is_active", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.IsConcurrent), "is_concurrent", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.TenantId), "tenant_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.Name), "name", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.StartTime), "start_time", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.StartUserId), "start_user_id", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.IsCountEnabled), "is_count_enabled", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.EvtSubscrCount), "evt_subscr_count", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.TaskCount), "task_count", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.VarCount), "var_count", nameof(BpmAfExecution));
        db.MappingColumns.Add(nameof(BpmAfExecution.SignType), "sign_type", nameof(BpmAfExecution));

        // BpmAfTask
        db.MappingTables.Add(nameof(BpmAfTask), "bpm_af_task");
        db.MappingColumns.Add(nameof(BpmAfTask.Id), "id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Rev), "rev", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.ExecutionId), "execution_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.ProcInstId), "proc_inst_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.ProcDefId), "proc_def_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Name), "name", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.ParentTaskId), "parent_task_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.TaskDefKey), "task_def_key", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.NodeId), "node_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.NodeType), "node_type", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Owner), "owner", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Assignee), "assignee", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.AssigneeName), "assignee_name", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Delegation), "delegation", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Priority), "priority", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.CreateTime), "create_time", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.DueDate), "due_date", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Category), "category", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.SuspensionState), "suspension_state", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.TenantId), "tenant_id", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.FormKey), "form_key", nameof(BpmAfTask));
        db.MappingColumns.Add(nameof(BpmAfTask.Description), "description", nameof(BpmAfTask));
        db.IgnoreColumns.Add(nameof(BpmAfTask.IsNextNodeSignUp), nameof(BpmAfTask));
        db.IgnoreColumns.Add(nameof(BpmAfTask.ProcessNumber),nameof(BpmAfTask));

        // BpmAfTaskInst
        db.MappingTables.Add(nameof(BpmAfTaskInst), "bpm_af_taskinst");
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Id), "id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.ProcDefId), "proc_def_id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.TaskDefKey), "task_def_key", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.ProcInstId), "proc_inst_id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.ExecutionId), "execution_id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Name), "name", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.ParentTaskId), "parent_task_id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Owner), "owner", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Assignee), "assignee", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.AssigneeName), "assignee_name", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.OriginalAssignee), "original_assignee", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.OriginalAssigneeName), "original_assignee_name", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.TransferReason), "transfer_reason", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.VerifyStatus), "verify_status", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.VerifyDesc), "verify_desc", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.StartTime), "start_time", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.ClaimTime), "claim_time", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.EndTime), "end_time", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Duration), "duration", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.DeleteReason), "delete_reason", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Priority), "priority", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.DueDate), "due_date", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.FormKey), "form_key", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Category), "category", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.TenantId), "tenant_id", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.Description), "description", nameof(BpmAfTaskInst));
        db.MappingColumns.Add(nameof(BpmAfTaskInst.UpdateUser), "update_user", nameof(BpmAfTaskInst));

        // BpmBusiness
        db.MappingTables.Add(nameof(BpmBusiness), "bpm_business");
        db.MappingColumns.Add(nameof(BpmBusiness.Id), "id", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.BusinessId), "business_id", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.CreateTime), "create_time", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.ProcessCode), "process_code", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.CreateUserName), "create_user_name", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.CreateUser), "create_user", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.ProcessKey), "process_key", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.IsDel), "is_del", nameof(BpmBusiness));
        db.MappingColumns.Add(nameof(BpmBusiness.TenantId), "tenant_id", nameof(BpmBusiness));

        // BpmBusinessProcess
        db.MappingTables.Add(nameof(BpmBusinessProcess), "bpm_business_process");
        db.MappingColumns.Add(nameof(BpmBusinessProcess.Id), "id", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.ProcessinessKey), "PROCESSINESS_KEY", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.BusinessId), "BUSINESS_ID", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.BusinessNumber), "BUSINESS_NUMBER", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.EntryId), "ENTRY_ID", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.Version), "VERSION", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.CreateTime), "create_time", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.UpdateTime), "update_time", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.Description), "description", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.ProcessState), "process_state", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.CreateUser), "create_user", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.UserName), "user_name", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.ProcessDigest), "process_digest", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.IsDel), "is_del", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.TenantId), "tenant_id", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.DataSourceId), "data_source_id", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.ProcInstId), "PROC_INST_ID_", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.BackUserId), "back_user_id", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.IsOutSideProcess), "is_out_side_process", nameof(BpmBusinessProcess));
        db.MappingColumns.Add(nameof(BpmBusinessProcess.IsLowCodeFlow), "is_lowcode_flow", nameof(BpmBusinessProcess));

        // BpmFlowrunEntrust
        db.MappingTables.Add(nameof(BpmFlowrunEntrust), "bpm_flowrun_entrust");
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.Id), "id", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.RunInfoId), "runinfoid", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.RunTaskId), "runtaskid", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.Original), "original", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.OriginalName), "original_name", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.Actual), "actual", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.ActualName), "actual_name", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.Type), "type", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.IsRead), "is_read", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.ProcDefId), "proc_def_id", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.IsView), "is_view", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.IsDel), "is_del", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.TenantId), "tenant_id", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.NodeId), "node_id", nameof(BpmFlowrunEntrust));
        db.MappingColumns.Add(nameof(BpmFlowrunEntrust.ActionType), "action_type", nameof(BpmFlowrunEntrust));

        // BpmFlowruninfo
        db.MappingTables.Add(nameof(BpmFlowruninfo), "bpm_flowruninfo");
        db.MappingColumns.Add(nameof(BpmFlowruninfo.Id), "id", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.RunInfoId), "runinfoid", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.CreateUserId), "create_UserId", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.EntityKey), "entitykey", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.EntityClass), "entityclass", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.EntityKeyType), "entitykeytype", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.CreateActor), "createactor", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.CreateDepart), "createdepart", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.CreateDate), "createdate", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.IsDel), "is_del", nameof(BpmFlowruninfo));
        db.MappingColumns.Add(nameof(BpmFlowruninfo.TenantId), "tenant_id", nameof(BpmFlowruninfo));

        // BpmManualNotify
        db.MappingTables.Add(nameof(BpmManualNotify), "bpm_manual_notify");
        db.MappingColumns.Add(nameof(BpmManualNotify.Id), "id", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.BusinessId), "business_id", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.Code), "code", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.LastTime), "last_time", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.CreateTime), "create_time", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.UpdateTime), "update_time", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.IsDel), "is_del", nameof(BpmManualNotify));
        db.MappingColumns.Add(nameof(BpmManualNotify.TenantId), "tenant_id", nameof(BpmManualNotify));

        // BpmnApproveRemind
        db.MappingTables.Add(nameof(BpmnApproveRemind), "t_bpmn_approve_remind");
        db.MappingColumns.Add(nameof(BpmnApproveRemind.Id), "id", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.ConfId), "conf_id", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.NodeId), "node_id", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.TemplateId), "template_id", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.Days), "days", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.IsDel), "is_del", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.TenantId), "tenant_id", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.CreateTime), "create_time", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.CreateUser), "create_user", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.UpdateTime), "update_time", nameof(BpmnApproveRemind));
        db.MappingColumns.Add(nameof(BpmnApproveRemind.UpdateUser), "update_user", nameof(BpmnApproveRemind));

        // BpmnConfLfFormdata
        db.MappingTables.Add(nameof(BpmnConfLfFormdata), "t_bpmn_conf_lf_formdata");
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.Id), "id", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.BpmnConfId), "bpmn_conf_id", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.Formdata), "formdata", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.IsDel), "is_del", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.TenantId), "tenant_id", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.CreateUser), "create_user", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.CreateTime), "create_time", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.UpdateUser), "update_user", nameof(BpmnConfLfFormdata));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdata.UpdateTime), "update_time", nameof(BpmnConfLfFormdata));

        // BpmnConfLfFormdataField
        db.MappingTables.Add(nameof(BpmnConfLfFormdataField), "t_bpmn_conf_lf_formdata_field");
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.Id), "id", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.BpmnConfId), "bpmn_conf_id", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.FormDataId), "formdata_id", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.FieldId), "field_id", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.FieldName), "field_name", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.FieldType), "field_type", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.IsConditionField), "is_condition", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.IsDel), "is_del", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.TenantId), "tenant_id", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.CreateUser), "create_user", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.CreateTime), "create_time", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.UpdateUser), "update_user", nameof(BpmnConfLfFormdataField));
        db.MappingColumns.Add(nameof(BpmnConfLfFormdataField.UpdateTime), "update_time", nameof(BpmnConfLfFormdataField));

        // BpmnConfNoticeTemplate
        db.MappingTables.Add(nameof(BpmnConfNoticeTemplate), "t_bpmn_conf_notice_template");
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.Id), "id", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.BpmnCode), "bpmn_code", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.IsDel), "is_del", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.TenantId), "tenant_id", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.CreateUser), "create_user", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.CreateTime), "create_time", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.UpdateUser), "update_user", nameof(BpmnConfNoticeTemplate));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplate.UpdateTime), "update_time", nameof(BpmnConfNoticeTemplate));

        // BpmnConfNoticeTemplateDetail
        db.MappingTables.Add(nameof(BpmnConfNoticeTemplateDetail), "t_bpmn_conf_notice_template_detail");
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.Id), "id", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.BpmnCode), "bpmn_code", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.NoticeTemplateType), "notice_template_type", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.NoticeTemplateDetail), "notice_template_detail", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.IsDel), "is_del", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.TenantId), "tenant_id", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.CreateUser), "create_user", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.CreateTime), "create_time", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.UpdateUser), "update_user", nameof(BpmnConfNoticeTemplateDetail));
        db.MappingColumns.Add(nameof(BpmnConfNoticeTemplateDetail.UpdateTime), "update_time", nameof(BpmnConfNoticeTemplateDetail));

        // BpmnNode
        db.MappingTables.Add(nameof(BpmnNode), "t_bpmn_node");
        db.MappingColumns.Add(nameof(BpmnNode.Id), "id", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.ConfId), "conf_id", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeId), "node_id", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeType), "node_type", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeProperty), "node_property", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeFrom), "node_from", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.BatchStatus), "batch_status", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.ApprovalStandard), "approval_standard", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeName), "node_name", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeDisplayName), "node_display_name", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.Annotation), "annotation", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.IsDeduplication), "is_deduplication", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.IsSignUp), "is_sign_up", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.Remark), "remark", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.IsDel), "is_del", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.TenantId), "tenant_id", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.CreateUser), "create_user", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.CreateTime), "create_time", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.UpdateUser), "update_user", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.UpdateTime), "update_time", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NodeFroms), "node_froms", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.IsDynamicCondition), "is_dynamicCondition", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.IsParallel), "is_parallel", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.NoHeaderAction), "no_header_action", nameof(BpmnNode));
        db.MappingColumns.Add(nameof(BpmnNode.ExtraFlags), "extra_flags", nameof(BpmnNode));
        db.IgnoreColumns.Add(nameof(BpmnNode.IsOutSideProcess), nameof(BpmnNode));
        db.IgnoreColumns.Add(nameof(BpmnNode.IsLowCodeFlow), nameof(BpmnNode));
        db.IgnoreColumns.Add(nameof(BpmnNode.ConfExtraFlags), nameof(BpmnNode));

        // BpmnNodeAssignLevelConf
        db.MappingTables.Add(nameof(BpmnNodeAssignLevelConf), "t_bpmn_node_assign_level_conf");
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.Id), "id", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.AssignLevelType), "assign_level_type", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.AssignLevelGrade), "assign_level_grade", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.Remark), "remark", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.IsDel), "is_del", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.TenantId), "tenant_id", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.CreateUser), "create_user", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.CreateTime), "create_time", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.UpdateUser), "update_user", nameof(BpmnNodeAssignLevelConf));
        db.MappingColumns.Add(nameof(BpmnNodeAssignLevelConf.UpdateTime), "update_time", nameof(BpmnNodeAssignLevelConf));

        // BpmnNodeBusinessTableConf
        db.MappingTables.Add(nameof(BpmnNodeBusinessTableConf), "t_bpmn_node_business_table_conf");
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.Id), "id", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.ConfigurationTableType), "configuration_table_type", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.TableFieldType), "table_field_type", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.SignType), "sign_type", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.Remark), "remark", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.IsDel), "is_del", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.TenantId), "tenant_id", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.CreateUser), "create_user", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.CreateTime), "create_time", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.UpdateUser), "update_user", nameof(BpmnNodeBusinessTableConf));
        db.MappingColumns.Add(nameof(BpmnNodeBusinessTableConf.UpdateTime), "update_time", nameof(BpmnNodeBusinessTableConf));

        // BpmnNodeButtonConf
        db.MappingTables.Add(nameof(BpmnNodeButtonConf), "t_bpmn_node_button_conf");
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.Id), "id", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.ButtonPageType), "button_page_type", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.ButtonType), "button_type", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.ButtonName), "button_name", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.Remark), "remark", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.IsDel), "is_del", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.TenantId), "tenant_id", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.CreateUser), "create_user", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.CreateTime), "create_time", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.UpdateUser), "update_user", nameof(BpmnNodeButtonConf));
        db.MappingColumns.Add(nameof(BpmnNodeButtonConf.UpdateTime), "update_time", nameof(BpmnNodeButtonConf));

        // BpmnNodeConditionsConf
        db.MappingTables.Add(nameof(BpmnNodeConditionsConf), "t_bpmn_node_conditions_conf");
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.Id), "id", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.IsDefault), "is_default", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.Sort), "sort", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.ExtJson), "ext_json", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.GroupRelation), "group_relation", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.Remark), "remark", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.IsDel), "is_del", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.TenantId), "tenant_id", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.CreateUser), "create_user", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.CreateTime), "create_time", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.UpdateUser), "update_user", nameof(BpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsConf.UpdateTime), "update_time", nameof(BpmnNodeConditionsConf));

        // BpmnNodeConditionsParamConf
        db.MappingTables.Add(nameof(BpmnNodeConditionsParamConf), "t_bpmn_node_conditions_param_conf");
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.Id), "id", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.BpmnNodeConditionsId), "bpmn_node_conditions_id", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.ConditionParamType), "condition_param_type", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.ConditionParamName), "condition_param_name", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.ConditionParamJsom), "condition_param_jsom", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.TheOperator), "operator", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.CondRelation), "cond_relation", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.CondGroup), "cond_group", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.Remark), "remark", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.IsDel), "is_del", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.TenantId), "tenant_id", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.CreateUser), "create_user", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.CreateTime), "create_time", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.UpdateUser), "update_user", nameof(BpmnNodeConditionsParamConf));
        db.MappingColumns.Add(nameof(BpmnNodeConditionsParamConf.UpdateTime), "update_time", nameof(BpmnNodeConditionsParamConf));

        // BpmnNodeCustomizeConf
        db.MappingTables.Add(nameof(BpmnNodeCustomizeConf), "t_bpmn_node_customize_conf");
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.Id), "id", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.SignType), "sign_type", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.Remark), "remark", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.IsDel), "is_del", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.TenantId), "tenant_id", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.CreateUser), "create_user", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.CreateTime), "create_time", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.UpdateUser), "update_user", nameof(BpmnNodeCustomizeConf));
        db.MappingColumns.Add(nameof(BpmnNodeCustomizeConf.UpdateTime), "update_time", nameof(BpmnNodeCustomizeConf));

        // BpmnNodeHrbpConf
        db.MappingTables.Add(nameof(BpmnNodeHrbpConf), "t_bpmn_node_hrbp_conf");
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.Id), "id", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.HrbpConfType), "hrbp_conf_type", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.Remark), "remark", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.IsDel), "is_del", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.TenantId), "tenant_id", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.CreateUser), "create_user", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.CreateTime), "create_time", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.UpdateUser), "update_user", nameof(BpmnNodeHrbpConf));
        db.MappingColumns.Add(nameof(BpmnNodeHrbpConf.UpdateTime), "update_time", nameof(BpmnNodeHrbpConf));

        // BpmnNodeLfFormdataFieldControl
        db.MappingTables.Add(nameof(BpmnNodeLfFormdataFieldControl), "t_bpmn_node_lf_formdata_field_control");
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.Id), "id", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.NodeId), "node_id", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.FormdataId), "formdata_id", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.FieldId), "field_id", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.FieldName), "field_name", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.Perm), "field_perm", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.IsDel), "is_del", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.TenantId), "tenant_id", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.CreateUser), "create_user", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.CreateTime), "create_time", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.UpdateUser), "update_user", nameof(BpmnNodeLfFormdataFieldControl));
        db.MappingColumns.Add(nameof(BpmnNodeLfFormdataFieldControl.UpdateTime), "update_time", nameof(BpmnNodeLfFormdataFieldControl));

        // BpmnNodeLoopConf
        db.MappingTables.Add(nameof(BpmnNodeLoopConf), "t_bpmn_node_loop_conf");
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.Id), "id", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.LoopEndType), "loop_end_type", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.LoopNumberPlies), "loop_number_plies", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.LoopEndPerson), "loop_end_person", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.NoparticipatingStaffIds), "noparticipating_staff_ids", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.LoopEndGrade), "loop_end_grade", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.Remark), "remark", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.IsDel), "is_del", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.TenantId), "tenant_id", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.CreateUser), "create_user", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.CreateTime), "create_time", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.UpdateUser), "update_user", nameof(BpmnNodeLoopConf));
        db.MappingColumns.Add(nameof(BpmnNodeLoopConf.UpdateTime), "update_time", nameof(BpmnNodeLoopConf));

        // BpmnNodeOutSideAccessConf
        db.MappingTables.Add(nameof(BpmnNodeOutSideAccessConf), "t_bpmn_node_out_side_access_conf");
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.Id), "id", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.NodeMark), "node_mark", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.SignType), "sign_type", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.Remark), "remark", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.IsDel), "is_del", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.CreateUser), "create_user", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.CreateTime), "create_time", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.UpdateUser), "update_user", nameof(BpmnNodeOutSideAccessConf));
        db.MappingColumns.Add(nameof(BpmnNodeOutSideAccessConf.UpdateTime), "update_time", nameof(BpmnNodeOutSideAccessConf));

        // BpmnNodePersonnelConf
        db.MappingTables.Add(nameof(BpmnNodePersonnelConf), "t_bpmn_node_personnel_conf");
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.Id), "id", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.SignType), "sign_type", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.Remark), "remark", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.IsDel), "is_del", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.TenantId), "tenant_id", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.CreateUser), "create_user", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.CreateTime), "create_time", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.UpdateUser), "update_user", nameof(BpmnNodePersonnelConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelConf.UpdateTime), "update_time", nameof(BpmnNodePersonnelConf));

        // BpmnNodePersonnelEmplConf
        db.MappingTables.Add(nameof(BpmnNodePersonnelEmplConf), "t_bpmn_node_personnel_empl_conf");
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.Id), "id", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.BpmnNodePersonneId), "bpmn_node_personne_id", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.EmplId), "empl_id", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.EmplName), "empl_name", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.Remark), "remark", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.IsDel), "is_del", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.TenantId), "tenant_id", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.CreateUser), "create_user", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.CreateTime), "create_time", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.UpdateUser), "update_user", nameof(BpmnNodePersonnelEmplConf));
        db.MappingColumns.Add(nameof(BpmnNodePersonnelEmplConf.UpdateTime), "update_time", nameof(BpmnNodePersonnelEmplConf));

        // BpmnNodeRoleConf
        db.MappingTables.Add(nameof(BpmnNodeRoleConf), "t_bpmn_node_role_conf");
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.Id), "id", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.RoleId), "role_id", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.RoleName), "role_name", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.SignType), "sign_type", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.Remark), "remark", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.IsDel), "is_del", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.TenantId), "tenant_id", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.CreateUser), "create_user", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.CreateTime), "create_time", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.UpdateUser), "update_user", nameof(BpmnNodeRoleConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleConf.UpdateTime), "update_time", nameof(BpmnNodeRoleConf));

        // BpmnNodeRoleOutsideEmpConf
        db.MappingTables.Add(nameof(BpmnNodeRoleOutsideEmpConf), "t_bpmn_node_role_outside_emp_conf");
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.Id), "id", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.NodeId), "node_id", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.EmplId), "empl_id", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.EmplName), "empl_name", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.IsDel), "is_del", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.TenantId), "tenant_id", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.CreateUser), "create_user", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.CreateTime), "create_time", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.UpdateUser), "update_user", nameof(BpmnNodeRoleOutsideEmpConf));
        db.MappingColumns.Add(nameof(BpmnNodeRoleOutsideEmpConf.UpdateTime), "update_time", nameof(BpmnNodeRoleOutsideEmpConf));

        // BpmnNodeSignUpConf
        db.MappingTables.Add(nameof(BpmnNodeSignUpConf), "t_bpmn_node_sign_up_conf");
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.Id), "id", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.AfterSignUpWay), "after_sign_up_way", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.SignUpType), "sign_up_type", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.IsDel), "is_del", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.TenantId), "tenant_id", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.CreateUser), "create_user", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.CreateTime), "create_time", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.UpdateUser), "update_user", nameof(BpmnNodeSignUpConf));
        db.MappingColumns.Add(nameof(BpmnNodeSignUpConf.UpdateTime), "update_time", nameof(BpmnNodeSignUpConf));

        // BpmnNodeTo
        db.MappingTables.Add(nameof(BpmnNodeTo), "t_bpmn_node_to");
        db.MappingColumns.Add(nameof(BpmnNodeTo.Id), "id", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.NodeTo), "node_to", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.IsDel), "is_del", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.TenantId), "tenant_id", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.CreateUser), "create_user", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.CreateTime), "create_time", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.UpdateUser), "update_user", nameof(BpmnNodeTo));
        db.MappingColumns.Add(nameof(BpmnNodeTo.UpdateTime), "update_time", nameof(BpmnNodeTo));

        // BpmnOutsideConf
        db.MappingTables.Add(nameof(BpmnOutsideConf), "t_bpmn_outside_conf");
        db.MappingColumns.Add(nameof(BpmnOutsideConf.Id), "id", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.FormCode), "form_code", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.CallBackUrl), "call_back_url", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.DetailUrl), "detail_url", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.IsDel), "is_del", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.BusinessName), "business_name", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.Remark), "remark", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.CreateUserId), "create_user_id", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.CreateTime), "create_time", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.ModifiedUserId), "modified_user_id", nameof(BpmnOutsideConf));
        db.MappingColumns.Add(nameof(BpmnOutsideConf.ModifiedTime), "modified_time", nameof(BpmnOutsideConf));

        // BpmnTemplate
        db.MappingTables.Add(nameof(BpmnTemplate), "t_bpmn_template");
        db.MappingColumns.Add(nameof(BpmnTemplate.Id), "id", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.ConfId), "conf_id", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.NodeId), "node_id", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.TemplateId), "template_id", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.MessageSendType), "message_send_type", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.FormCode), "form_code", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.IsDel), "is_del", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.TenantId), "tenant_id", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.Event), "event", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.Informs), "informs", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.Roles), "roles", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.Funcs), "funcs", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.CreateTime), "create_time", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.CreateUser), "create_user", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.UpdateTime), "update_time", nameof(BpmnTemplate));
        db.MappingColumns.Add(nameof(BpmnTemplate.UpdateUser), "update_user", nameof(BpmnTemplate));

        // BpmnViewPageButton
        db.MappingTables.Add(nameof(BpmnViewPageButton), "t_bpmn_view_page_button");
        db.MappingColumns.Add(nameof(BpmnViewPageButton.Id), "id", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.ConfId), "conf_id", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.ViewType), "view_type", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.ButtonType), "button_type", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.ButtonName), "button_name", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.IsDel), "is_del", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.TenantId), "tenant_id", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.Remark), "remark", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.CreateUser), "create_user", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.CreateTime), "create_time", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.UpdateUser), "update_user", nameof(BpmnViewPageButton));
        db.MappingColumns.Add(nameof(BpmnViewPageButton.UpdateTime), "update_time", nameof(BpmnViewPageButton));

        // BpmProcessAppApplication
        db.MappingTables.Add(nameof(BpmProcessAppApplication), "bpm_process_app_application");
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.Id), "id", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.BusinessCode), "business_code", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.Title), "process_name", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.ApplyType), "apply_type", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.PcIcon), "pc_icon", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.EffectiveSource), "effective_source", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.IsSon), "is_son", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.LookUrl), "look_url", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.SubmitUrl), "submit_url", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.ConditionUrl), "condition_url", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.ParentId), "parent_id", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.ApplicationUrl), "application_url", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.ProcessKey), "process_key", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.PermissionsCode), "permissions_code", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.IsDel), "is_del", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.CreateUserId), "create_user_id", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.CreateTime), "create_time", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.UpdateUser), "update_user", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.UpdateTime), "update_time", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.IsAll), "is_all", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.UserRequestUri), "user_request_uri", nameof(BpmProcessAppApplication));
        db.MappingColumns.Add(nameof(BpmProcessAppApplication.RoleRequestUri), "role_request_uri", nameof(BpmProcessAppApplication));

        // BpmProcessAppData
        db.MappingTables.Add(nameof(BpmProcessAppData), "bpm_process_app_data");
        db.MappingColumns.Add(nameof(BpmProcessAppData.Id), "id", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.ProcessKey), "process_key", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.ProcessName), "process_name", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.State), "state", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.Route), "route", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.Sort), "sort", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.Source), "source", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.IsAll), "is_all", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.VersionId), "version_id", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.ApplicationId), "application_id", nameof(BpmProcessAppData));
        db.MappingColumns.Add(nameof(BpmProcessAppData.Type), "type", nameof(BpmProcessAppData));

        // BpmProcessCategory
        db.MappingTables.Add(nameof(BpmProcessCategory), "bpm_process_category");
        db.MappingColumns.Add(nameof(BpmProcessCategory.Id), "id", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.ProcessTypeName), "process_type_name", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.IsDel), "is_del", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.TenantId), "tenant_id", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.State), "state", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.Sort), "sort", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.IsApp), "is_app", nameof(BpmProcessCategory));
        db.MappingColumns.Add(nameof(BpmProcessCategory.Entrance), "entrance", nameof(BpmProcessCategory));

        // BpmProcessDept
        db.MappingTables.Add(nameof(BpmProcessDept), "bpm_process_dept");
        db.MappingColumns.Add(nameof(BpmProcessDept.Id), "id", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.ProcessCode), "process_code", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.ProcessType), "process_type", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.ProcessName), "process_name", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.DeptId), "dep_id", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.Remarks), "remarks", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.CreateTime), "create_time", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.CreateUser), "create_user", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.UpdateUser), "update_user", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.UpdateTime), "update_time", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.ProcessKey), "process_key", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.IsDel), "is_del", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.TenantId), "tenant_id", nameof(BpmProcessDept));
        db.MappingColumns.Add(nameof(BpmProcessDept.IsAll), "is_all", nameof(BpmProcessDept));

        // BpmProcessForward
        db.MappingTables.Add(nameof(BpmProcessForward), "bpm_process_forward");
        db.MappingColumns.Add(nameof(BpmProcessForward.Id), "id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.ForwardUserId), "forward_user_id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.ForwardUserName), "forward_user_name", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.ProcessInstanceId), "processInstance_Id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.CreateTime), "create_time", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.CreateUserId), "create_user_id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.IsDel), "is_del", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.TenantId), "tenant_id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.IsRead), "is_read", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.TaskId), "task_id", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.ProcessNumber), "process_number", nameof(BpmProcessForward));
        db.MappingColumns.Add(nameof(BpmProcessForward.NodeId), "node_id", nameof(BpmProcessForward));

        // BpmProcessName
        db.MappingTables.Add(nameof(BpmProcessName), "bpm_process_name");
        db.MappingColumns.Add(nameof(BpmProcessName.Id), "id", nameof(BpmProcessName));
        db.MappingColumns.Add(nameof(BpmProcessName.ProcessName), "process_name", nameof(BpmProcessName));
        db.MappingColumns.Add(nameof(BpmProcessName.IsDel), "is_del", nameof(BpmProcessName));
        db.MappingColumns.Add(nameof(BpmProcessName.TenantId), "tenant_id", nameof(BpmProcessName));
        db.MappingColumns.Add(nameof(BpmProcessName.CreateTime), "create_time", nameof(BpmProcessName));

        // BpmProcessNameRelevancy
        db.MappingTables.Add(nameof(BpmProcessNameRelevancy), "bpm_process_name_relevancy");
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.Id), "id", nameof(BpmProcessNameRelevancy));
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.ProcessNameId), "process_name_id", nameof(BpmProcessNameRelevancy));
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.ProcessKey), "process_key", nameof(BpmProcessNameRelevancy));
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.IsDel), "is_del", nameof(BpmProcessNameRelevancy));
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.TenantId), "tenant_id", nameof(BpmProcessNameRelevancy));
        db.MappingColumns.Add(nameof(BpmProcessNameRelevancy.CreateTime), "create_time", nameof(BpmProcessNameRelevancy));

        // BpmProcessNodeOvertime
        db.MappingTables.Add(nameof(BpmProcessNodeOvertime), "bpm_process_node_overtime");
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.Id), "id", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.NoticeType), "notice_type", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.NodeName), "node_name", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.NodeKey), "node_key", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.ProcessKey), "process_key", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.IsDel), "is_del", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.NoticeTime), "notice_time", nameof(BpmProcessNodeOvertime));
        db.MappingColumns.Add(nameof(BpmProcessNodeOvertime.TenantId), "tenant_id", nameof(BpmProcessNodeOvertime));

        // BpmProcessNodeRecord
        db.MappingTables.Add(nameof(BpmProcessNodeRecord), "bpm_process_node_record");
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.Id), "id", nameof(BpmProcessNodeRecord));
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.ProcessInstanceId), "processInstance_id", nameof(BpmProcessNodeRecord));
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.TaskId), "task_id", nameof(BpmProcessNodeRecord));
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.CreateTime), "create_time", nameof(BpmProcessNodeRecord));
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.IsDel), "is_del", nameof(BpmProcessNodeRecord));
        db.MappingColumns.Add(nameof(BpmProcessNodeRecord.TenantId), "tenant_id", nameof(BpmProcessNodeRecord));

        // BpmProcessNodeSubmit
        db.MappingTables.Add(nameof(BpmProcessNodeSubmit), "bpm_process_node_submit");
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.Id), "id", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.ProcessInstanceId), "processInstance_Id", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.BackType), "back_type", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.NodeKey), "node_key", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.CreateTime), "create_time", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.CreateUser), "create_user", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.State), "state", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.IsDel), "is_del", nameof(BpmProcessNodeSubmit));
        db.MappingColumns.Add(nameof(BpmProcessNodeSubmit.TenantId), "tenant_id", nameof(BpmProcessNodeSubmit));

        // BpmProcessNotice
        db.MappingTables.Add(nameof(BpmProcessNotice), "bpm_process_notice");
        db.MappingColumns.Add(nameof(BpmProcessNotice.Id), "id", nameof(BpmProcessNotice));
        db.MappingColumns.Add(nameof(BpmProcessNotice.Type), "type", nameof(BpmProcessNotice));
        db.MappingColumns.Add(nameof(BpmProcessNotice.ProcessKey), "process_key", nameof(BpmProcessNotice));
        db.MappingColumns.Add(nameof(BpmProcessNotice.IsDel), "is_del", nameof(BpmProcessNotice));
        db.MappingColumns.Add(nameof(BpmProcessNotice.TenantId), "tenant_id", nameof(BpmProcessNotice));

        // BpmProcessOperation
        db.MappingTables.Add(nameof(BpmProcessOperation), "bpm_process_operation");
        db.MappingColumns.Add(nameof(BpmProcessOperation.Id), "id", nameof(BpmProcessOperation));
        db.MappingColumns.Add(nameof(BpmProcessOperation.ProcessKey), "process_key", nameof(BpmProcessOperation));
        db.MappingColumns.Add(nameof(BpmProcessOperation.ProcessNode), "process_node", nameof(BpmProcessOperation));
        db.MappingColumns.Add(nameof(BpmProcessOperation.Type), "type", nameof(BpmProcessOperation));
        db.MappingColumns.Add(nameof(BpmProcessOperation.IsDel), "is_del", nameof(BpmProcessOperation));
        db.MappingColumns.Add(nameof(BpmProcessOperation.TenantId), "tenant_id", nameof(BpmProcessOperation));

        // BpmProcessPermissions
        db.MappingTables.Add(nameof(BpmProcessPermissions), "bpm_process_permissions");
        db.MappingColumns.Add(nameof(BpmProcessPermissions.Id), "id", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.UserId), "user_id", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.DepId), "dep_id", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.PermissionsType), "permissions_type", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.CreateUser), "create_user", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.CreateTime), "create_time", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.ProcessKey), "process_key", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.OfficeId), "office_id", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.IsDel), "is_del", nameof(BpmProcessPermissions));
        db.MappingColumns.Add(nameof(BpmProcessPermissions.TenantId), "tenant_id", nameof(BpmProcessPermissions));

        // BpmTaskconfig
        db.MappingTables.Add(nameof(BpmTaskconfig), "bpm_taskconfig");
        db.MappingColumns.Add(nameof(BpmTaskconfig.Id), "id", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.ProcDefId), "proc_def_id_", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.TaskDefKey), "task_def_key_", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.UserId), "user_id", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.Number), "number", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.IsDel), "is_del", nameof(BpmTaskconfig));
        db.MappingColumns.Add(nameof(BpmTaskconfig.TenantId), "tenant_id", nameof(BpmTaskconfig));

        // BpmVariable
        db.MappingTables.Add(nameof(BpmVariable), "t_bpm_variable");
        db.MappingColumns.Add(nameof(BpmVariable.Id), "id", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.ProcessNum), "process_num", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.ProcessName), "process_name", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.ProcessDesc), "process_desc", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.ProcessStartConditions), "process_start_conditions", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.BpmnCode), "bpmn_code", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.Remark), "remark", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.IsDel), "is_del", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.TenantId), "tenant_id", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.CreateUser), "create_user", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.CreateTime), "create_time", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.UpdateUser), "update_user", nameof(BpmVariable));
        db.MappingColumns.Add(nameof(BpmVariable.UpdateTime), "update_time", nameof(BpmVariable));

        // BpmVariableApproveRemind
        db.MappingTables.Add(nameof(BpmVariableApproveRemind), "t_bpm_variable_approve_remind");
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.Id), "id", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.VariableId), "variable_id", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.ElementId), "element_id", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.Content), "content", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.Remark), "remark", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.IsDel), "is_del", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.TenantId), "tenant_id", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.CreateUser), "create_user", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.CreateTime), "create_time", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.UpdateUser), "update_user", nameof(BpmVariableApproveRemind));
        db.MappingColumns.Add(nameof(BpmVariableApproveRemind.UpdateTime), "update_time", nameof(BpmVariableApproveRemind));

        // BpmVariableButton
        db.MappingTables.Add(nameof(BpmVariableButton), "t_bpm_variable_button");
        db.MappingColumns.Add(nameof(BpmVariableButton.Id), "id", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.VariableId), "variable_id", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.ElementId), "element_id", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.ButtonPageType), "button_page_type", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.ButtonType), "button_type", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.ButtonName), "button_name", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.Remark), "remark", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.IsDel), "is_del", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.TenantId), "tenant_id", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.CreateUser), "create_user", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.CreateTime), "create_time", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.UpdateUser), "update_user", nameof(BpmVariableButton));
        db.MappingColumns.Add(nameof(BpmVariableButton.UpdateTime), "update_time", nameof(BpmVariableButton));

        // BpmVariableMessage
        db.MappingTables.Add(nameof(BpmVariableMessage), "t_bpm_variable_message");
        db.MappingColumns.Add(nameof(BpmVariableMessage.Id), "id", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.VariableId), "variable_id", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.ElementId), "element_id", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.MessageType), "message_type", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.EventType), "event_type", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.Content), "content", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.Remark), "remark", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.IsDel), "is_del", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.TenantId), "tenant_id", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.CreateUser), "create_user", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.CreateTime), "create_time", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.UpdateUser), "update_user", nameof(BpmVariableMessage));
        db.MappingColumns.Add(nameof(BpmVariableMessage.UpdateTime), "update_time", nameof(BpmVariableMessage));

        // BpmVariableMultiplayer
        db.MappingTables.Add(nameof(BpmVariableMultiplayer), "t_bpm_variable_multiplayer");
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.Id), "id", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.VariableId), "variable_id", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.ElementId), "element_id", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.NodeId), "node_id", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.ElementName), "element_name", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.CollectionName), "collection_name", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.SignType), "sign_type", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.Remark), "remark", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.IsDel), "is_del", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.TenantId), "tenant_id", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.CreateUser), "create_user", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.CreateTime), "create_time", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.UpdateUser), "update_user", nameof(BpmVariableMultiplayer));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayer.UpdateTime), "update_time", nameof(BpmVariableMultiplayer));
        db.IgnoreColumns.Add(nameof(BpmVariableMultiplayer.UnderTakeStatus), nameof(BpmVariableMultiplayer));

        // BpmVariableMultiplayerPersonnel
        db.MappingTables.Add(nameof(BpmVariableMultiplayerPersonnel), "t_bpm_variable_multiplayer_personnel");
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.Id), "id", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.VariableMultiplayerId), "variable_multiplayer_id", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.Assignee), "assignee", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.AssigneeName), "assignee_name", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.UndertakeStatus), "undertake_status", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.Remark), "remark", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.IsDel), "is_del", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.TenantId), "tenant_id", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.CreateUser), "create_user", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.CreateTime), "create_time", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.UpdateUser), "update_user", nameof(BpmVariableMultiplayerPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableMultiplayerPersonnel.UpdateTime), "update_time", nameof(BpmVariableMultiplayerPersonnel));

        // BpmVariableSequenceFlow
        db.MappingTables.Add(nameof(BpmVariableSequenceFlow), "t_bpm_variable_sequence_flow");
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.Id), "id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.VariableId), "variable_id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.ElementId), "element_id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.ElementName), "element_name", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.ElementFromId), "element_from_id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.ElementToId), "element_to_id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.SequenceFlowType), "sequence_flow_type", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.SequenceFlowConditions), "sequence_flow_conditions", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.Remark), "remark", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.IsDel), "is_del", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.TenantId), "tenant_id", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.CreateUser), "create_user", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.CreateTime), "create_time", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.UpdateUser), "update_user", nameof(BpmVariableSequenceFlow));
        db.MappingColumns.Add(nameof(BpmVariableSequenceFlow.UpdateTime), "update_time", nameof(BpmVariableSequenceFlow));

        // BpmVariableSignUp
        db.MappingTables.Add(nameof(BpmVariableSignUp), "t_bpm_variable_sign_up");
        db.MappingColumns.Add(nameof(BpmVariableSignUp.Id), "id", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.VariableId), "variable_id", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.ElementId), "element_id", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.NodeId), "node_id", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.AfterSignUpWay), "after_sign_up_way", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.SubElements), "sub_elements", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.Remark), "remark", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.IsDel), "is_del", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.TenantId), "tenant_id", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.CreateUser), "create_user", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.CreateTime), "create_time", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.UpdateUser), "update_user", nameof(BpmVariableSignUp));
        db.MappingColumns.Add(nameof(BpmVariableSignUp.UpdateTime), "update_time", nameof(BpmVariableSignUp));

        // BpmVariableSignUpPersonnel
        db.MappingTables.Add(nameof(BpmVariableSignUpPersonnel), "t_bpm_variable_sign_up_personnel");
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.Id), "id", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.VariableId), "variable_id", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.ElementId), "element_id", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.Assignee), "assignee", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.AssigneeName), "assignee_name", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.Remark), "remark", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.IsDel), "is_del", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.TenantId), "tenant_id", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.CreateUser), "create_user", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.CreateTime), "create_time", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.UpdateUser), "update_user", nameof(BpmVariableSignUpPersonnel));
        db.MappingColumns.Add(nameof(BpmVariableSignUpPersonnel.UpdateTime), "update_time", nameof(BpmVariableSignUpPersonnel));

        // BpmVariableSingle
        db.MappingTables.Add(nameof(BpmVariableSingle), "t_bpm_variable_single");
        db.MappingColumns.Add(nameof(BpmVariableSingle.Id), "id", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.VariableId), "variable_id", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.ElementId), "element_id", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.NodeId), "node_id", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.ElementName), "element_name", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.AssigneeParamName), "assignee_param_name", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.Assignee), "assignee", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.AssigneeName), "assignee_name", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.Remark), "remark", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.IsDel), "is_del", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.TenantId), "tenant_id", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.CreateUser), "create_user", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.CreateTime), "create_time", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.UpdateUser), "update_user", nameof(BpmVariableSingle));
        db.MappingColumns.Add(nameof(BpmVariableSingle.UpdateTime), "update_time", nameof(BpmVariableSingle));

        // BpmVariableViewPageButton
        db.MappingTables.Add(nameof(BpmVariableViewPageButton), "t_bpm_variable_view_page_button");
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.Id), "id", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.VariableId), "variable_id", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.ViewType), "view_type", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.ButtonType), "button_type", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.ButtonName), "button_name", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.Remark), "remark", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.IsDel), "is_del", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.TenantId), "tenant_id", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.CreateUser), "create_user", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.CreateTime), "create_time", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.UpdateUser), "update_user", nameof(BpmVariableViewPageButton));
        db.MappingColumns.Add(nameof(BpmVariableViewPageButton.UpdateTime), "update_time", nameof(BpmVariableViewPageButton));

        // BpmVerifyInfo
        db.MappingTables.Add(nameof(BpmVerifyInfo), "bpm_verify_info");
        db.MappingColumns.Add(nameof(BpmVerifyInfo.Id), "id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.RunInfoId), "run_info_id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.VerifyUserId), "verify_user_id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.VerifyUserName), "verify_user_name", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.VerifyStatus), "verify_status", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.VerifyDesc), "verify_desc", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.VerifyDate), "verify_date", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.TaskName), "task_name", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.TaskId), "task_id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.TaskDefKey), "task_def_key", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.BusinessType), "business_type", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.BusinessId), "business_id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.OriginalId), "original_id", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.ProcessCode), "process_code", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.IsDel), "is_del", nameof(BpmVerifyInfo));
        db.MappingColumns.Add(nameof(BpmVerifyInfo.TenantId), "tenant_id", nameof(BpmVerifyInfo));

        // DefaultTemplate
        db.MappingTables.Add(nameof(DefaultTemplate), "t_default_template");
        db.MappingColumns.Add(nameof(DefaultTemplate.Id), "id", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.Event), "event", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.TemplateId), "template_id", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.IsDel), "is_del", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.TenantId), "tenant_id", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.CreateTime), "create_time", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.CreateUser), "create_user", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.UpdateTime), "update_time", nameof(DefaultTemplate));
        db.MappingColumns.Add(nameof(DefaultTemplate.UpdateUser), "update_user", nameof(DefaultTemplate));

        // Department
        db.MappingTables.Add(nameof(Department), "t_department");
        db.MappingColumns.Add(nameof(Department.Id), "id", nameof(Department));
        db.MappingColumns.Add(nameof(Department.Name), "name", nameof(Department));
        db.MappingColumns.Add(nameof(Department.ShortName), "short_name", nameof(Department));
        db.MappingColumns.Add(nameof(Department.ParentId), "parent_id", nameof(Department));
        db.MappingColumns.Add(nameof(Department.Path), "path", nameof(Department));
        db.MappingColumns.Add(nameof(Department.Level), "level", nameof(Department));
        db.MappingColumns.Add(nameof(Department.LeaderId), "leader_id", nameof(Department));
        db.MappingColumns.Add(nameof(Department.Sort), "sort", nameof(Department));
        db.MappingColumns.Add(nameof(Department.IsDel), "is_del", nameof(Department));
        db.MappingColumns.Add(nameof(Department.IsHide), "is_hide", nameof(Department));
        db.MappingColumns.Add(nameof(Department.CreateUser), "create_user", nameof(Department));
        db.MappingColumns.Add(nameof(Department.UpdateUser), "update_user", nameof(Department));
        db.MappingColumns.Add(nameof(Department.CreateTime), "create_time", nameof(Department));
        db.MappingColumns.Add(nameof(Department.UpdateTime), "update_time", nameof(Department));

        // DictData
        db.MappingTables.Add(nameof(DictData), "t_dict_data");
        db.MappingColumns.Add(nameof(DictData.Id), "id", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.Sort), "dict_sort", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.Label), "dict_label", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.Value), "dict_value", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.DictType), "dict_type", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.CssClass), "css_class", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.ListClass), "list_class", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.IsDefault), "is_default", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.IsDel), "is_del", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.TenantId), "tenant_id", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.CreateTime), "create_time", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.CreateUser), "create_user", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.UpdateTime), "update_time", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.UpdateUser), "update_user", nameof(DictData));
        db.MappingColumns.Add(nameof(DictData.Remark), "remark", nameof(DictData));

        // DictMain
        db.MappingTables.Add(nameof(DictMain), "t_dict_main");
        db.MappingColumns.Add(nameof(DictMain.Id), "id", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.DictName), "dict_name", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.DictType), "dict_type", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.IsDel), "is_del", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.TenantId), "tenant_id", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.CreateTime), "create_time", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.CreateUser), "create_user", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.UpdateTime), "update_time", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.UpdateUser), "update_user", nameof(DictMain));
        db.MappingColumns.Add(nameof(DictMain.Remark), "remark", nameof(DictMain));

        // InformationTemplate
        db.MappingTables.Add(nameof(InformationTemplate), "t_information_template");
        db.MappingColumns.Add(nameof(InformationTemplate.Id), "id", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.Name), "name", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.Num), "num", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.SystemTitle), "system_title", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.SystemContent), "system_content", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.MailTitle), "mail_title", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.MailContent), "mail_content", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.NoteContent), "note_content", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.JumpUrl), "jump_url", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.Remark), "remark", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.Status), "status", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.Evt), "event", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.EventName), "event_name", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.IsDel), "is_del", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.TenantId), "tenant_id", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.CreateTime), "create_time", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.CreateUser), "create_user", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.UpdateTime), "update_time", nameof(InformationTemplate));
        db.MappingColumns.Add(nameof(InformationTemplate.UpdateUser), "update_user", nameof(InformationTemplate));

        // LFMain
        db.MappingTables.Add(nameof(LFMain), "t_lf_main");
        db.MappingColumns.Add(nameof(LFMain.Id), "id", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.ConfId), "conf_id", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.FormCode), "form_code", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.IsDel), "is_del", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.TenantId), "tenant_id", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.CreateUser), "create_user", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.CreateTime), "create_time", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.UpdateUser), "update_user", nameof(LFMain));
        db.MappingColumns.Add(nameof(LFMain.UpdateTime), "update_time", nameof(LFMain));

        // OutSideBpmAccessBusiness
        db.MappingTables.Add(nameof(OutSideBpmAccessBusiness), "t_out_side_bpm_access_business");
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.Id), "id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.BusinessPartyId), "business_party_id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.BpmnConfId), "bpmn_conf_id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormCode), "form_code", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.ProcessNumber), "process_number", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormDataPc), "form_data_pc", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormDataApp), "form_data_app", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.TemplateMark), "template_mark", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.StartUsername), "start_username", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.Remark), "remark", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.IsDel), "is_del", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.CreateUser), "create_user", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.CreateTime), "create_time", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.UpdateUser), "update_user", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.UpdateTime), "update_time", nameof(OutSideBpmAccessBusiness));

        // OutSideBpmAdminPersonnel
        db.MappingTables.Add(nameof(OutSideBpmAdminPersonnel), "t_out_side_bpm_admin_personnel");
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Id), "id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.BusinessPartyId), "business_party_id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Type), "type", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.EmployeeId), "employee_id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.EmployeeName), "employee_name", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Remark), "remark", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.IsDel), "is_del", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.CreateUser), "create_user", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.CreateTime), "create_time", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.UpdateUser), "update_user", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.UpdateTime), "update_time", nameof(OutSideBpmAdminPersonnel));

        // OutSideBpmApproveTemplate
        db.MappingTables.Add(nameof(OutSideBpmApproveTemplate), "t_out_side_bpm_approve_template");
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.Id), "id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.BusinessPartyId), "business_party_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApplicationId), "application_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApproveTypeId), "approve_type_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApproveTypeName), "approve_type_name", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiClientId), "api_client_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiClientSecret), "api_client_secret", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiToken), "api_token", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiUrl), "api_url", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.Remark), "remark", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.IsDel), "is_del", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateUser), "create_user", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateTime), "create_time", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.UpdateUser), "update_user", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.UpdateTime), "update_time", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateUserId), "create_user_id", nameof(OutSideBpmApproveTemplate));

        // OutSideBpmBusinessParty
        db.MappingTables.Add(nameof(OutSideBpmBusinessParty), "t_out_side_bpm_business_party");
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Id), "id", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.BusinessPartyMark), "business_party_mark", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Name), "name", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Type), "type", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Remark), "remark", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.IsDel), "is_del", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.CreateUser), "create_user", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.CreateTime), "create_time", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.UpdateUser), "update_user", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.UpdateTime), "update_time", nameof(OutSideBpmBusinessParty));

        // OutSideBpmCallbackUrlConf
        db.MappingTables.Add(nameof(OutSideBpmCallbackUrlConf), "t_out_side_bpm_callback_url_conf");
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Id), "id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BusinessPartyId), "business_party_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApplicationId), "application_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmnConfId), "bpmn_conf_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.FormCode), "form_code", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmConfCallbackUrl), "bpm_conf_callback_url", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmFlowCallbackUrl), "bpm_flow_callback_url", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApiClientId), "api_client_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApiClientSecret), "api_client_secret", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Status), "status", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Remark), "remark", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.IsDel), "is_del", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.CreateUser), "create_user", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.CreateTime), "create_time", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.UpdateUser), "update_user", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.UpdateTime), "update_time", nameof(OutSideBpmCallbackUrlConf));

        // OutSideBpmConditionsTemplate
        db.MappingTables.Add(nameof(OutSideBpmConditionsTemplate), "t_out_side_bpm_conditions_template");
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.Id), "id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.BusinessPartyId), "business_party_id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.TemplateMark), "template_mark", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.TemplateName), "template_name", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.ApplicationId), "application_id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.Remark), "remark", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.IsDel), "is_del", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateUser), "create_user", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateTime), "create_time", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.UpdateUser), "update_user", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.UpdateTime), "update_time", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateUserId), "create_user_id", nameof(OutSideBpmConditionsTemplate));

        // OutSideBpmnNodeConditionsConf
        db.MappingTables.Add(nameof(OutSideBpmnNodeConditionsConf), "t_out_side_bpmn_node_conditions_conf");
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.Id), "id", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.BpmnNodeId), "bpmn_node_id", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.OutSideId), "out_side_id", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.Remark), "remark", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.IsDel), "is_del", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.CreateUser), "create_user", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.CreateTime), "create_time", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.UpdateUser), "update_user", nameof(OutSideBpmnNodeConditionsConf));
        db.MappingColumns.Add(nameof(OutSideBpmnNodeConditionsConf.UpdateTime), "update_time", nameof(OutSideBpmnNodeConditionsConf));

        // OutSideCallBackRecord
        db.MappingTables.Add(nameof(OutSideCallBackRecord), "t_out_side_bpm_call_back_record");
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.Id), "id", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.ProcessNumber), "process_number", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.Status), "status", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.RetryTimes), "retry_times", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.ButtonOperationType), "button_operation_type", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CallBackTypeName), "call_back_type_name", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.BusinessId), "business_id", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.FormCode), "form_code", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.IsDel), "is_del", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CreateUser), "create_user", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CreateTime), "create_time", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.UpdateUser), "update_user", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.UpdateTime), "update_time", nameof(OutSideCallBackRecord));

        // QuickEntry
        db.MappingTables.Add(nameof(QuickEntry), "t_quick_entry");
        db.MappingColumns.Add(nameof(QuickEntry.Id), "id", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Title), "title", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.EffectiveSource), "effective_source", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.IsDel), "is_del", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.TenantId), "tenant_id", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Route), "route", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Sort), "sort", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.CreateTime), "create_time", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Status), "status", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.VariableUrlFlag), "variable_url_flag", nameof(QuickEntry));

        // QuickEntryType
        db.MappingTables.Add(nameof(QuickEntryType), "t_quick_entry_type");
        db.MappingColumns.Add(nameof(QuickEntryType.Id), "id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.QuickEntryId), "quick_entry_id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.Type), "type", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.IsDel), "is_del", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.TenantId), "tenant_id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.CreateTime), "create_time", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.TypeName), "type_name", nameof(QuickEntryType));

        // SysVersion
        db.MappingTables.Add(nameof(SysVersion), "t_sys_version");
        db.MappingColumns.Add(nameof(SysVersion.Id), "id", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.CreateTime), "create_time", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.UpdateTime), "update_time", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsDel), "is_del", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.TenantId), "tenant_id", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Version), "version", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Description), "description", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Index), "index", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsForce), "is_force", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.AndroidUrl), "android_url", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IosUrl), "ios_url", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.CreateUser), "create_user", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.UpdateUser), "update_user", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsHide), "is_hide", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.DownloadCode), "download_code", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.EffectiveTime), "effective_time", nameof(SysVersion));

        // ThirdPartyAccountApply
        db.MappingTables.Add(nameof(ThirdPartyAccountApply), "t_biz_account_apply");
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.Id), "id", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.AccountType), "account_type", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.AccountOwnerName), "account_owner_name", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.Remark), "remark", nameof(ThirdPartyAccountApply));

        // User
        db.MappingTables.Add(nameof(User), "t_user");
        db.MappingColumns.Add(nameof(User.Id), "id", nameof(User));
        db.MappingColumns.Add(nameof(User.Name), "user_name", nameof(User));
        db.MappingColumns.Add(nameof(User.Mobile), "mobile", nameof(User));
        db.MappingColumns.Add(nameof(User.Email), "email", nameof(User));
        db.MappingColumns.Add(nameof(User.LeaderId), "leader_id", nameof(User));
        db.MappingColumns.Add(nameof(User.HrbpId), "hrbp_id", nameof(User));
        db.MappingColumns.Add(nameof(User.MobileIsShow), "mobile_is_show", nameof(User));
        db.MappingColumns.Add(nameof(User.DepartmentId), "department_id", nameof(User));
        db.MappingColumns.Add(nameof(User.Path), "path", nameof(User));
        db.MappingColumns.Add(nameof(User.IsDel), "is_del", nameof(User));
        db.MappingColumns.Add(nameof(User.HeadImg), "head_img", nameof(User));

        // Role
        db.MappingTables.Add(nameof(Role), "t_role");
        db.MappingColumns.Add(nameof(Role.Id), "id", nameof(Role));
        db.MappingColumns.Add(nameof(Role.RoleName), "role_name", nameof(Role));

        // UserEmailSend
        db.MappingTables.Add(nameof(UserEmailSend), "t_user_email_send");
        db.MappingColumns.Add(nameof(UserEmailSend.Id), "id", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Sender), "sender", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Receiver), "receiver", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Title), "title", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Content), "content", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.CreateTime), "create_time", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.UpdateTime), "update_time", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.CreateUser), "create_user", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.UpdateUser), "update_user", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.IsDel), "is_del", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.TenantId), "tenant_id", nameof(UserEmailSend));

        // UserEntrust
        db.MappingTables.Add(nameof(UserEntrust), "t_user_entrust");
        db.MappingColumns.Add(nameof(UserEntrust.Id), "id", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.Sender), "sender", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.ReceiverId), "receiver_id", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.ReceiverName), "receiver_name", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.PowerId), "power_id", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.BeginTime), "begin_time", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.EndTime), "end_time", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.CreateTime), "create_time", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.UpdateTime), "update_time", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.CreateUser), "create_user", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.UpdateUser), "update_user", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.IsDel), "is_del", nameof(UserEntrust));
        db.MappingColumns.Add(nameof(UserEntrust.TenantId), "tenant_id", nameof(UserEntrust));

        // UserMessage
        db.MappingTables.Add(nameof(UserMessage), "t_user_message");
        db.MappingColumns.Add(nameof(UserMessage.Id), "id", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.UserId), "user_id", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Title), "title", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Content), "content", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Url), "url", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Node), "node", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Params), "params", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.IsRead), "is_read", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.IsDel), "is_del", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.TenantId), "tenant_id", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.CreateTime), "create_time", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.UpdateTime), "update_time", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.CreateUser), "create_user", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.UpdateUser), "update_user", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.AppUrl), "app_url", nameof(UserMessage));
        db.MappingColumns.Add(nameof(UserMessage.Source), "source", nameof(UserMessage));
        db.IgnoreColumns.Add(nameof(UserMessage.UrlParams), nameof(UserMessage));

        // UserMessageStatus
        db.MappingTables.Add(nameof(UserMessageStatus), "t_user_message_status");
        db.MappingColumns.Add(nameof(UserMessageStatus.Id), "id", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.UserId), "user_id", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.MessageStatus), "message_status", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.MailStatus), "mail_status", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.OpenPhone), "open_phone", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.NotTrouble), "not_trouble", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.NotTroubleTimeBegin), "not_trouble_time_begin", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.NotTroubleTimeEnd), "not_trouble_time_end", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.CreateTime), "create_time", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.UpdateTime), "update_time", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.CreateUser), "create_user", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.UpdateUser), "update_user", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.IsDel), "is_del", nameof(UserMessageStatus));
        db.MappingColumns.Add(nameof(UserMessageStatus.TenantId), "tenant_id", nameof(UserMessageStatus));

        // UserRole
        db.MappingTables.Add(nameof(UserRole), "t_user_role");
        db.MappingColumns.Add(nameof(UserRole.Id), "id", nameof(UserRole));
        db.MappingColumns.Add(nameof(UserRole.UserId), "user_id", nameof(UserRole));
        db.MappingColumns.Add(nameof(UserRole.RoleId), "role_id", nameof(UserRole));

        // LFMainField
        db.MappingTables.Add(nameof(LFMainField), "t_lf_main_field");
        db.MappingColumns.Add(nameof(LFMainField.Id), "id", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.MainId), "main_id", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FormCode), "form_code", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldId), "field_id", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldName), "field_name", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.ParentFieldId), "parent_field_id", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.ParentFieldName), "parent_field_name", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldValue), "field_value", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldValueNumber), "field_value_number", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldValueDt), "field_value_dt", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.FieldValueText), "field_value_text", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.Sort), "sort", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.IsDel), "is_del", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.TenantId), "tenant_id", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.CreateUser), "create_user", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.CreateTime), "create_time", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.UpdateUser), "update_user", nameof(LFMainField));
        db.MappingColumns.Add(nameof(LFMainField.UpdateTime), "update_time", nameof(LFMainField));

        // BpmnNodeAdditionalSignConf
        db.MappingTables.Add(nameof(BpmnNodeAdditionalSignConf), "t_bpmn_node_additional_sign_conf");
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.Id), "id", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.BpmnNodeId), "bpmn_node_id", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.SignInfos), "sign_infos", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.SignProperty), "sign_property", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.SignPropertyType), "sign_property_type", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.SignType), "sign_type", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.Remark), "remark", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.IsDel), "is_del", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.TenantId), "tenant_id", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.CreateUser), "create_user", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.CreateTime), "create_time", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.UpdateUser), "update_user", nameof(BpmnNodeAdditionalSignConf));
        db.MappingColumns.Add(nameof(BpmnNodeAdditionalSignConf.UpdateTime), "update_time", nameof(BpmnNodeAdditionalSignConf));


        // OutSideBpmAccessBusiness
        db.MappingTables.Add(nameof(OutSideBpmAccessBusiness), "t_out_side_bpm_access_business");
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.Id), "id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.BusinessPartyId), "business_party_id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.BpmnConfId), "bpmn_conf_id", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormCode), "form_code", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.ProcessNumber), "process_number", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormDataPc), "form_data_pc", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.FormDataApp), "form_data_app", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.TemplateMark), "template_mark", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.StartUsername), "start_username", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.Remark), "remark", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.IsDel), "is_del", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.CreateUser), "create_user", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.CreateTime), "create_time", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.UpdateUser), "update_user", nameof(OutSideBpmAccessBusiness));
        db.MappingColumns.Add(nameof(OutSideBpmAccessBusiness.UpdateTime), "update_time", nameof(OutSideBpmAccessBusiness));

        // OutSideBpmAdminPersonnel
        db.MappingTables.Add(nameof(OutSideBpmAdminPersonnel), "t_out_side_bpm_admin_personnel");
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Id), "id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.BusinessPartyId), "business_party_id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Type), "type", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.EmployeeId), "employee_id", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.EmployeeName), "employee_name", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.Remark), "remark", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.IsDel), "is_del", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.CreateUser), "create_user", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.CreateTime), "create_time", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.UpdateUser), "update_user", nameof(OutSideBpmAdminPersonnel));
        db.MappingColumns.Add(nameof(OutSideBpmAdminPersonnel.UpdateTime), "update_time", nameof(OutSideBpmAdminPersonnel));

        // OutSideBpmApproveTemplate
        db.MappingTables.Add(nameof(OutSideBpmApproveTemplate), "t_out_side_bpm_approve_template");
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.Id), "id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.BusinessPartyId), "business_party_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApplicationId), "application_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApproveTypeId), "approve_type_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApproveTypeName), "approve_type_name", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiClientId), "api_client_id", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiClientSecret), "api_client_secret", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiToken), "api_token", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.ApiUrl), "api_url", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.Remark), "remark", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.IsDel), "is_del", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateUser), "create_user", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateTime), "create_time", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.UpdateUser), "update_user", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.UpdateTime), "update_time", nameof(OutSideBpmApproveTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmApproveTemplate.CreateUserId), "create_user_id", nameof(OutSideBpmApproveTemplate));

        // OutSideBpmBusinessParty
        db.MappingTables.Add(nameof(OutSideBpmBusinessParty), "t_out_side_bpm_business_party");
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Id), "id", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.BusinessPartyMark), "business_party_mark", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Name), "name", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Type), "type", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.Remark), "remark", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.IsDel), "is_del", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.CreateUser), "create_user", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.CreateTime), "create_time", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.UpdateUser), "update_user", nameof(OutSideBpmBusinessParty));
        db.MappingColumns.Add(nameof(OutSideBpmBusinessParty.UpdateTime), "update_time", nameof(OutSideBpmBusinessParty));

        // OutSideBpmCallbackUrlConf
        db.MappingTables.Add(nameof(OutSideBpmCallbackUrlConf), "t_out_side_bpm_callback_url_conf");
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Id), "id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BusinessPartyId), "business_party_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApplicationId), "application_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmnConfId), "bpmn_conf_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.FormCode), "form_code", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmConfCallbackUrl), "bpm_conf_callback_url", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.BpmFlowCallbackUrl), "bpm_flow_callback_url", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApiClientId), "api_client_id", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.ApiClientSecret), "api_client_secret", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Status), "status", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.Remark), "remark", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.IsDel), "is_del", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.CreateUser), "create_user", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.CreateTime), "create_time", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.UpdateUser), "update_user", nameof(OutSideBpmCallbackUrlConf));
        db.MappingColumns.Add(nameof(OutSideBpmCallbackUrlConf.UpdateTime), "update_time", nameof(OutSideBpmCallbackUrlConf));

        // OutSideBpmConditionsTemplate
        db.MappingTables.Add(nameof(OutSideBpmConditionsTemplate), "t_out_side_bpm_conditions_template");
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.Id), "id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.BusinessPartyId), "business_party_id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.TemplateMark), "template_mark", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.TemplateName), "template_name", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.ApplicationId), "application_id", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.Remark), "remark", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.IsDel), "is_del", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateUser), "create_user", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateTime), "create_time", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.UpdateUser), "update_user", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.UpdateTime), "update_time", nameof(OutSideBpmConditionsTemplate));
        db.MappingColumns.Add(nameof(OutSideBpmConditionsTemplate.CreateUserId), "create_user_id", nameof(OutSideBpmConditionsTemplate));

        // OutSideCallBackRecord
        db.MappingTables.Add(nameof(OutSideCallBackRecord), "t_out_side_call_back_record");
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.Id), "id", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.ProcessNumber), "process_number", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.Status), "status", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.RetryTimes), "retry_times", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.ButtonOperationType), "button_operation_type", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CallBackTypeName), "call_back_type_name", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.BusinessId), "business_id", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.FormCode), "form_code", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.IsDel), "is_del", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CreateUser), "create_user", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.CreateTime), "create_time", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.UpdateUser), "update_user", nameof(OutSideCallBackRecord));
        db.MappingColumns.Add(nameof(OutSideCallBackRecord.UpdateTime), "update_time", nameof(OutSideCallBackRecord));

        // SysVersion
        db.MappingTables.Add(nameof(SysVersion), "t_sys_version");
        db.MappingColumns.Add(nameof(SysVersion.Id), "id", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.CreateTime), "create_time", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.UpdateTime), "update_time", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsDel), "is_del", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.TenantId), "tenant_id", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Version), "version", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Description), "description", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.Index), "index", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsForce), "is_force", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.AndroidUrl), "android_url", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IosUrl), "ios_url", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.CreateUser), "create_user", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.UpdateUser), "update_user", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.IsHide), "is_hide", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.DownloadCode), "download_code", nameof(SysVersion));
        db.MappingColumns.Add(nameof(SysVersion.EffectiveTime), "effective_time", nameof(SysVersion));

        // QuickEntry
        db.MappingTables.Add(nameof(QuickEntry), "t_quick_entry");
        db.MappingColumns.Add(nameof(QuickEntry.Id), "id", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Title), "title", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.EffectiveSource), "effective_source", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.IsDel), "is_del", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.TenantId), "tenant_id", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Route), "route", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Sort), "sort", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.CreateTime), "create_time", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.Status), "status", nameof(QuickEntry));
        db.MappingColumns.Add(nameof(QuickEntry.VariableUrlFlag), "variable_url_flag", nameof(QuickEntry));

        // QuickEntryType
        db.MappingTables.Add(nameof(QuickEntryType), "t_quick_entry_type");
        db.MappingColumns.Add(nameof(QuickEntryType.Id), "id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.QuickEntryId), "quick_entry_id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.Type), "type", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.IsDel), "is_del", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.TenantId), "tenant_id", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.CreateTime), "create_time", nameof(QuickEntryType));
        db.MappingColumns.Add(nameof(QuickEntryType.TypeName), "type_name", nameof(QuickEntryType));

        // UserEmailSend
        db.MappingTables.Add(nameof(UserEmailSend), "t_user_email_send");
        db.MappingColumns.Add(nameof(UserEmailSend.Id), "id", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Sender), "sender", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Receiver), "receiver", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Title), "title", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.Content), "content", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.CreateTime), "create_time", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.UpdateTime), "update_time", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.CreateUser), "create_user", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.UpdateUser), "update_user", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.IsDel), "is_del", nameof(UserEmailSend));
        db.MappingColumns.Add(nameof(UserEmailSend.TenantId), "tenant_id", nameof(UserEmailSend));

        // ThirdPartyAccountApply
        db.MappingTables.Add(nameof(ThirdPartyAccountApply), "t_biz_account_apply");
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.Id), "id", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.AccountType), "account_type", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.AccountOwnerName), "account_owner_name", nameof(ThirdPartyAccountApply));
        db.MappingColumns.Add(nameof(ThirdPartyAccountApply.Remark), "remark", nameof(ThirdPartyAccountApply));

        // Employee
        db.MappingTables.Add(nameof(Employee), "t_employee");
        db.MappingColumns.Add(nameof(Employee.Id), "id", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.Username), "username", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.LeaderId), "leader_id", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.Email), "email", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.Mobile), "mobile", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.IsDel), "is_del", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.HrbpId), "hrbp_id", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.HeadImg), "head_img", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.MobileIsShow), "mobile_is_show", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.Path), "path", nameof(Employee));
        db.MappingColumns.Add(nameof(Employee.DepartmentId), "department_id", nameof(Employee));
    }
}
