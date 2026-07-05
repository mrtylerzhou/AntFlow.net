/***********************************************
 * SQL Server Compatible Database Initialization Script
 * Converted from MySQL script: new_mysql.sql
 * Target DB: Microsoft SQL Server
 * Notes:
 *   1. AUTO_INCREMENT converted to IDENTITY(1,1)
 *   2. MySQL TIMESTAMP with ON UPDATE CURRENT_TIMESTAMP
 *      converted to DATETIME2 DEFAULT GETDATE().
 *      SQL Server does not support ON UPDATE CURRENT_TIMESTAMP natively;
 *      use triggers or application logic if auto-update is required.
 *   3. VARCHAR/TEXT/LONGTEXT with utf8mb4 converted to NVARCHAR/NVARCHAR(MAX)
 *   4. All table/column identifiers use square brackets []
 *   5. MySQL COMMENT preserved as inline SQL comments or removed
 *      (SQL Server uses sp_addextendedproperty for full metadata support)
 ***********************************************/

-- ============================================================
-- 1. t_bpmn_conf
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpmn_conf]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpmn_conf]
(
    [id]                  INT             NOT NULL IDENTITY(1,1), -- COMMENT: 'Auto Incr ID'
    [bpmn_code]           NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'Process code'
    [bpmn_name]           NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'Process Name'
    [bpmn_type]           INT             NULL     DEFAULT NULL,   -- COMMENT: 'Process Type'
    [form_code]           NVARCHAR(100)   NOT NULL DEFAULT N'',    -- COMMENT: 'Process Business Code'
    [app_id]              INT             NULL     DEFAULT NULL,   -- COMMENT: 'associated app id'
    [deduplication_type]  INT             NOT NULL DEFAULT 1,      -- COMMENT: 'deduplication way 1.no deduplication,2 forward deduplication,3.backward deduplication'
    [effective_status]    INT             NOT NULL DEFAULT 0,      -- COMMENT: 'is effect 0:no 1:yes'
    [is_all]              INT             NOT NULL DEFAULT 0,      -- COMMENT: 'is to all,0 no 1yes'
    [is_out_side_process] INT             NULL     DEFAULT 0,      -- COMMENT: 'is it a third party process'
    [is_lowcode_flow]     TINYINT         NULL     DEFAULT 0,      -- COMMENT: '是否是低代码审批流0,否,1是'
    [business_party_id]   INT             NULL     DEFAULT NULL,   -- COMMENT: 'its belong to business party'
    [extra_flags]         INT             NULL,
    [conf_config_json]    NVARCHAR(MAX)   NULL,                    -- COMMENT: 'process-level consolidated json config'
    [remark]              NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [tenant_id]           NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [is_del]              TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:in use,1:delete'
    [create_user]         NVARCHAR(32)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]         DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]         NVARCHAR(32)    NULL     DEFAULT N'',    -- COMMENT: '更新人'
    [update_time]         DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除，SQL Server需Trigger实现)
    CONSTRAINT [PK_t_bpmn_conf] PRIMARY KEY ([id]),
    CONSTRAINT [UQ_t_bpmn_conf_bpmn_code] UNIQUE ([bpmn_code])
);
CREATE INDEX [IX_t_bpmn_conf_business_party_id] ON [dbo].[t_bpmn_conf] ([business_party_id]);
CREATE INDEX [IX_t_bpmn_conf_form_code]         ON [dbo].[t_bpmn_conf] ([form_code]);
END;
-- COMMENT: 'process main configuration table'
GO

-- ============================================================
-- 2. t_bpmn_node
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpmn_node]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpmn_node]
(
    [id]                   BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [conf_id]              BIGINT          NOT NULL,               -- COMMENT: 'the main conf id'
    [node_id]              NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'node id'
    [node_type]            INT             NOT NULL,               -- COMMENT: 'its node type,see NodeTypeEnum for detail'
    [node_property]        INT             NOT NULL,               -- COMMENT: 'node property,rules for finding out approvers,see NodePropertyEnum for detail'
    [node_from]            NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'its prev node'
    [node_froms]           NVARCHAR(255)   NULL     DEFAULT NULL,   -- COMMENT: 'all its prev nodes'
    [batch_status]         INT             NOT NULL DEFAULT 0,      -- COMMENT: 'can the process approved in batch,0:no,1:Yes'
    [approval_standard]    INT             NOT NULL DEFAULT 2,      -- COMMENT: 'approve standard,current not used'
    [node_name]            NVARCHAR(255)   NULL     DEFAULT NULL,   -- COMMENT: 'node name'
    [node_display_name]    NVARCHAR(255)   NULL     DEFAULT N'',    -- COMMENT: 'node display name shown in web or app'
    [annotation]           NVARCHAR(255)   NULL     DEFAULT NULL,   -- COMMENT: 'annotation on this conf'
    [is_deduplication]     INT             NOT NULL DEFAULT 0,      -- COMMENT: 'whether this node should be deduplicated,0:No,1:Yes'
    [deduplicationExclude] TINYINT         NULL     DEFAULT 0,      -- COMMENT: '0 for no,default value,and 1 for yes'
    [is_dynamicCondition]  TINYINT         NULL     DEFAULT 0,      -- COMMENT: '是否是动态条件节点,0,否,1是'
    [is_parallel]          TINYINT         NULL     DEFAULT 0,
    [is_sign_up]           INT             NOT NULL DEFAULT 0,      -- COMMENT: 'whether this node can be sign up,0:No,1:Yes'
    [no_header_action]     TINYINT         NULL,
    [remark]               NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [tenant_id]            NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [extra_flags]          INT             NULL,
    [node_config_json]     NVARCHAR(MAX)   NULL,                    -- COMMENT: 'node-level consolidated json config'
    [is_del]               TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:No,1:yes'
    [create_user]          NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]          DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]          NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]          DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_bpmn_node] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_bpmn_node_conf_id] ON [dbo].[t_bpmn_node] ([conf_id]);
END;
-- COMMENT: 'the conf,s node table'
GO

-- ============================================================
-- 3. t_information_template
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_information_template]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_information_template]
(
    [id]             BIGINT        NOT NULL IDENTITY(1,1),
    [name]           NVARCHAR(30)  NOT NULL DEFAULT N'',    -- COMMENT: 'name'
    [num]            NVARCHAR(10)  NOT NULL DEFAULT N'',    -- COMMENT: 'num'
    [system_title]   NVARCHAR(100) NOT NULL DEFAULT N'',    -- COMMENT: 'title'
    [system_content] NVARCHAR(500) NOT NULL DEFAULT N'',    -- COMMENT: 'content'
    [mail_title]     NVARCHAR(100) NOT NULL DEFAULT N'',    -- COMMENT: 'mail title'
    [mail_content]   NVARCHAR(500) NOT NULL DEFAULT N'',    -- COMMENT: 'mail content'
    [note_content]   NVARCHAR(200) NOT NULL DEFAULT N'',    -- COMMENT: 'sms content'
    [jump_url]       INT           NULL     DEFAULT NULL,   -- COMMENT: 'url to jump to'
    [remark]         NVARCHAR(200) NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [status]         TINYINT       NOT NULL DEFAULT 0,      -- COMMENT: 'status 0:in use,1:disabled'
    [event]          INT           NULL,
    [event_name]     NVARCHAR(50)  NULL,
    [is_del]         TINYINT       NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]      NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_time]    DATETIME2     NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [create_user]    NVARCHAR(50)  NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]    DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [update_user]    NVARCHAR(50)  NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [is_default]          INT           NULL,
    CONSTRAINT [PK_t_information_template] PRIMARY KEY ([id])
);
END;
-- COMMENT: '消息模板'
GO

-- ============================================================
-- 4. bpm_flowrun_entrust
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_flowrun_entrust]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_flowrun_entrust]
(
    [id]            INT           NOT NULL IDENTITY(1,1),
    [runinfoid]     NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'process instance id'
    [runtaskid]     NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'task id'
    [original]      NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'original assignee'
    [original_name] NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'original assignee name'
    [actual]        NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'actual assignee'
    [actual_name]   NVARCHAR(100) NULL,                    -- COMMENT: 'actual assignee name'
    [type]          INT           NULL     DEFAULT NULL,   -- COMMENT: 'type 1: entrust 2:view'
    [is_read]       INT           NULL     DEFAULT 2,      -- COMMENT: 'is read 1:yes,2:no'
    [proc_def_id]   NVARCHAR(100) NULL     DEFAULT NULL,   -- COMMENT: 'proces deployment id'
    [is_view]       INT           NOT NULL DEFAULT 0,
    [tenant_id]     NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [is_del]        INT           NULL     DEFAULT 0,
    [node_id]       NVARCHAR(64)  NULL,
    [action_type]   INT           NULL     DEFAULT 0,      -- COMMENT: '0 global user configed entrust,1.change assignee entrust,2 add assignee 3 remove assignee'
    CONSTRAINT [PK_bpm_flowrun_entrust] PRIMARY KEY ([id])
);
CREATE INDEX [BPM_IDX_ID] ON [dbo].[bpm_flowrun_entrust] ([runinfoid], [original], [actual]);
END;
-- COMMENT: 'entrust and forward view conf table'
GO

-- ============================================================
-- 5. t_bpmn_node_to
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpmn_node_to]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpmn_node_to]
(
    [id]           BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [bpmn_node_id] BIGINT          NOT NULL,               -- COMMENT: 'node id'
    [node_to]      NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'node to'
    [remark]       NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [is_del]       TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]    NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]  NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]  DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]  NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]  DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_bpmn_node_to] PRIMARY KEY ([id])
);
END;
-- COMMENT: '审批流节点走向表'
GO

-- ============================================================
-- 6. bpm_process_forward
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_forward]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_forward]
(
    [id]                 INT           NOT NULL IDENTITY(1,1),
    [forward_user_id]    NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'forwarded user id'
    [Forward_user_name]  NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'forwarded user name'
    [processInstance_Id] NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'process instance id'
    [node_id]            NVARCHAR(64)  NULL,
    [create_time]        DATETIME2     NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [create_user_id]     NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'as its name says'
    [task_id]            NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'taskid'
    [is_read]            INT           NULL     DEFAULT 0,      -- COMMENT: 'is read'
    [is_del]             INT           NULL     DEFAULT 0,
    [tenant_id]          NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [update_time]        DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    [process_number]     NVARCHAR(50)  NOT NULL DEFAULT N'',    -- COMMENT: 'process number'
    CONSTRAINT [PK_bpm_process_forward] PRIMARY KEY ([id])
);
CREATE INDEX [IX_bpm_process_forward_forward_user_id]         ON [dbo].[bpm_process_forward] ([forward_user_id]);
CREATE INDEX [IX_bpm_process_forward_forward_user_id_is_read] ON [dbo].[bpm_process_forward] ([forward_user_id], [is_read]);
END;
-- COMMENT: 'process forward table'
GO

-- ============================================================
-- 7. bpm_process_node_submit
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_node_submit]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_node_submit]
(
    [id]                 BIGINT       NOT NULL IDENTITY(1,1),
    [processInstance_Id] NVARCHAR(64) NULL     DEFAULT NULL,   -- COMMENT: 'process instance id'
    [back_type]          TINYINT      NULL     DEFAULT NULL,   -- COMMENT: 'back type'
    [node_key]           NVARCHAR(50) NULL     DEFAULT NULL,   -- COMMENT: 'node key'
    [create_time]        DATETIME2    NOT NULL DEFAULT GETDATE(),
    [create_user]        NVARCHAR(50) NULL     DEFAULT NULL,   -- COMMENT: 'creator'
    [state]              TINYINT      NULL     DEFAULT NULL,   -- COMMENT: 'state'
    [is_del]             INT          NULL     DEFAULT 0,
    [tenant_id]          NVARCHAR(64) NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_bpm_process_node_submit] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'process node submit'
GO

-- ============================================================
-- 8. bpm_taskconfig
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_taskconfig]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_taskconfig]
(
    [id]            BIGINT        NOT NULL IDENTITY(1,1),
    [proc_def_id_]  NVARCHAR(100) NULL     DEFAULT NULL,   -- COMMENT: 'process def id'
    [task_def_key_] NVARCHAR(100) NULL     DEFAULT NULL,   -- COMMENT: 'task def key'
    [user_id]       BIGINT        NULL     DEFAULT NULL,   -- COMMENT: 'user id'
    [number]        INT           NULL     DEFAULT NULL,   -- COMMENT: 'number'
    [status]        TINYINT       NULL     DEFAULT NULL,   -- COMMENT: 'status'
    [original_type] TINYINT       NULL     DEFAULT NULL,   -- COMMENT: 'orginal type'
    [is_del]        INT           NULL     DEFAULT 0,
    [tenant_id]     NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_bpm_taskconfig] PRIMARY KEY ([id])
);
CREATE INDEX [BPM_IDX__TASK_CONFIG] ON [dbo].[bpm_taskconfig] ([proc_def_id_], [task_def_key_]);
END;
-- COMMENT: 'task config'
GO

-- ============================================================
-- 9. t_bpm_variable
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpm_variable]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpm_variable]
(
    [id]                       BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [process_num]              NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'process number'
    [process_name]             NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'process name'
    [process_desc]             NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'process desc'
    [process_start_conditions] NVARCHAR(MAX)   NOT NULL,               -- COMMENT: 'process start conditions'
    [bpmn_code]                NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'bpmn code'
    [is_new_data]              INT             NULL     DEFAULT 0,      -- COMMENT: 'is new data 0:no 1:yes'
    [variable_config_json]     NVARCHAR(MAX)   NULL,                    -- COMMENT: 'runtime variable consolidated json config'
    [remark]                   NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [is_del]                   TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]                NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]              NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]              DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]              NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]              DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_bpm_variable] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_bpm_variable_process_num] ON [dbo].[t_bpm_variable] ([process_num]);
END;
-- COMMENT: 'process variable table'
GO

-- ============================================================
-- 10. t_bpm_variable_multiplayer
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpm_variable_multiplayer]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpm_variable_multiplayer]
(
    [id]              BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [variable_id]     BIGINT          NOT NULL,               -- COMMENT: 'variable id'
    [element_id]      NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'element id'
    [element_name]    NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'element name'
    [node_id]         NVARCHAR(60)    NULL,
    [collection_name] NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'collection name'
    [sign_type]       INT             NOT NULL,               -- COMMENT: 'sign type 1: all sign 2:or sign'
    [remark]          NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [is_del]          TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]       NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]     NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]     DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]     NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]     DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_bpm_variable_multiplayer] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_bpm_variable_multiplayer_variable_id]            ON [dbo].[t_bpm_variable_multiplayer] ([variable_id]);
CREATE INDEX [IX_t_bpm_variable_multiplayer_variable_id_element_id] ON [dbo].[t_bpm_variable_multiplayer] ([variable_id], [element_id]);
END;
-- COMMENT: 'process multiplayer variable table'
GO

-- ============================================================
-- 11. t_bpm_variable_multiplayer_personnel
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpm_variable_multiplayer_personnel]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpm_variable_multiplayer_personnel]
(
    [id]                      BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [variable_multiplayer_id] BIGINT          NOT NULL,               -- COMMENT: 'variable id'
    [assignee]                NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'assignee,that is the approver'
    [assignee_name]           NVARCHAR(60)    NOT NULL DEFAULT N'',    -- COMMENT: 'assignee name'
    [undertake_status]        INT             NOT NULL,               -- COMMENT: 'is undertaked(0:no,1:yes)'
    [remark]                  NVARCHAR(255)   NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [is_del]                  TINYINT         NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]               NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]             NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]             DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]             NVARCHAR(50)    NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [update_time]             DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_bpm_variable_multiplayer_personnel] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_bpm_variable_multiplayer_personnel_vm_id] ON [dbo].[t_bpm_variable_multiplayer_personnel] ([variable_multiplayer_id]);
END;
-- COMMENT: 'multiplayer assignees variable table'
GO

-- ============================================================
-- 12. bpm_verify_info
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_verify_info]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_verify_info]
(
    [id]               BIGINT        NOT NULL IDENTITY(1,1),
    [run_info_id]      NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'process instance id'
    [verify_user_id]   NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'approver'
    [verify_user_name] NVARCHAR(100) NULL     DEFAULT NULL,   -- COMMENT: 'approver name'
    [verify_status]    INT           NULL     DEFAULT NULL,   -- COMMENT: 'verify status'
    [verify_desc]      NVARCHAR(500) NULL     DEFAULT NULL,   -- COMMENT: 'verify desc'
    [verify_date]      DATETIME2     NOT NULL DEFAULT GETDATE(),
    [task_name]        NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'tsk name'
    [task_id]          NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'task id'
    [task_def_key]     NVARCHAR(255) NULL,
    [business_type]    INT           NULL     DEFAULT NULL,   -- COMMENT: 'business type'
    [business_id]      NVARCHAR(128) NULL     DEFAULT NULL,   -- COMMENT: 'business id'
    [original_id]      NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'orig approver name'
    [process_code]     NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'process number'
    [is_del]           TINYINT       NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]        NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [attachments_json]  NVARCHAR(500) NULL     DEFAULT NULL,   -- COMMENT: 'attachments json'
    CONSTRAINT [PK_bpm_verify_info] PRIMARY KEY ([id])
);
CREATE INDEX [BPM_IDX__INFOR]     ON [dbo].[bpm_verify_info] ([business_type], [business_id]);
CREATE INDEX [process_code_index] ON [dbo].[bpm_verify_info] ([process_code]);
END;
-- COMMENT: 'verify info'
GO

-- ============================================================
-- 13. t_method_replay
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_method_replay]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_method_replay]
(
    [id]                   INT           NOT NULL IDENTITY(1,1),
    [PROJECT_NAME]         NVARCHAR(100) NULL,                    -- COMMENT: 'project name'
    [CLASS_NAME]           NVARCHAR(255) NULL,
    [METHOD_NAME]          NVARCHAR(255) NULL,
    [PARAM_TYPE]           NVARCHAR(255) NULL,
    [ARGS]                 NVARCHAR(MAX) NULL,
    [NOW_TIME]             DATETIME2     NULL,
    [ERROR_MSG]            NVARCHAR(MAX) NULL,
    [ALREADY_REPLAY_TIMES] INT           NULL,
    [MAX_REPLAY_TIMES]     INT           NULL,
    CONSTRAINT [PK_t_method_replay] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_method_replay_NOW_TIME] ON [dbo].[t_method_replay] ([NOW_TIME]);
END;
-- COMMENT: 'method replay records'
GO

-- ============================================================
-- 14. t_user_entrust
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_user_entrust]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_user_entrust]
(
    [id]            INT          NOT NULL IDENTITY(1,1),
    [sender]        NVARCHAR(64) NOT NULL,                  -- COMMENT: 'sender id'
    [receiver_id]   NVARCHAR(64) NOT NULL,
    [receiver_name] NVARCHAR(255)NULL     DEFAULT NULL,
    [power_id]      NVARCHAR(100)NOT NULL,
    [begin_time]    DATETIME2    NULL     DEFAULT NULL,
    [end_time]      DATETIME2    NULL     DEFAULT NULL,
    [create_time]   DATETIME2    NOT NULL DEFAULT GETDATE(), -- COMMENT: '创建时间' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [update_time]   DATETIME2    NULL     DEFAULT GETDATE(), -- COMMENT: '更新时间' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [create_user]   NVARCHAR(50) NOT NULL,
    [update_user]   NVARCHAR(50) NOT NULL,
    [is_del]        TINYINT      NOT NULL DEFAULT 0,         -- COMMENT: '（0:no 1:yes'
    [tenant_id]     NVARCHAR(64) NULL     DEFAULT N'',       -- COMMENT: 'tenantId'
    CONSTRAINT [PK_t_user_entrust] PRIMARY KEY ([id]),
    CONSTRAINT [UQ_t_user_entrust_s_r_id] UNIQUE ([sender], [receiver_id], [power_id])
);
CREATE INDEX [IX_t_user_entrust_sender_power_id] ON [dbo].[t_user_entrust] ([sender], [power_id]);
END;
-- COMMENT: 'user entrust info'
GO

-- ============================================================
-- 15. t_user_message_status
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_user_message_status]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_user_message_status]
(
    [id]                     INT          NOT NULL IDENTITY(1,1),
    [user_id]                NVARCHAR(64) NOT NULL,
    [message_status]         TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: 'sms status'
    [mail_status]            TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: 'email status'
    [not_trouble_time_end]   TIME         NULL     DEFAULT NULL,   -- COMMENT: 'do not disturb end time'
    [not_trouble_time_begin] DATETIME2    NULL     DEFAULT NULL,   -- COMMENT: 'do not disturb begin time'
    [not_trouble]            TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: 'is do not disturb enabled'
    [shock]                  TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: 'should shock'
    [sound]                  TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: 'is in silent mode'
    [open_phone]             TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: ''
    [create_time]            DATETIME2    NOT NULL DEFAULT GETDATE(),
    [update_time]            DATETIME2    NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    [create_user]            NVARCHAR(50) NOT NULL,
    [update_user]            NVARCHAR(50) NOT NULL,
    [is_del]                 TINYINT      NOT NULL DEFAULT 0,      -- COMMENT: '（0:no 1:yes'
    [tenant_id]              NVARCHAR(64) NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_t_user_message_status] PRIMARY KEY ([id])
);
CREATE INDEX [IX_t_user_message_status_user_id] ON [dbo].[t_user_message_status] ([user_id]);
END;
-- COMMENT: 'user receive message table'
GO

-- ============================================================
-- 16. bpm_business_process
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_business_process]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_business_process]
(
    [id]                BIGINT          NOT NULL IDENTITY(1,1), -- COMMENT: 'id'
    [PROCESSINESS_KEY]  NVARCHAR(64)    NULL     DEFAULT NULL,
    [BUSINESS_ID]       NVARCHAR(64)    NOT NULL,               -- COMMENT: 'business id'
    [BUSINESS_NUMBER]   NVARCHAR(64)    NULL     DEFAULT NULL,   -- COMMENT: 'process number'
    [ENTRY_ID]          NVARCHAR(64)    NULL     DEFAULT NULL,
    [VERSION]           NVARCHAR(30)    NULL     DEFAULT NULL,   -- COMMENT: 'version'
    [CREATE_TIME]       DATETIME2       NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [UPDATE_TIME]       DATETIME2       NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [description]       NVARCHAR(100)   NULL     DEFAULT NULL,   -- COMMENT: 'title'
    [process_state]     INT             NULL     DEFAULT NULL,   -- COMMENT: 'process state 1:approving 2:approved 3:invalid 6:rejected'
    [create_user]       NVARCHAR(64)    NULL     DEFAULT NULL,
    [process_digest]    NVARCHAR(MAX)   NULL,                    -- COMMENT: 'process digest'
    [is_del]            TINYINT         NULL     DEFAULT 0,      -- COMMENT: '0: no 1: yes）'
    [tenant_id]         NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [data_source_id]    BIGINT          NULL     DEFAULT NULL,   -- COMMENT: 'data source id'
    [PROC_INST_ID_]     NVARCHAR(64)    NULL     DEFAULT N'',    -- COMMENT: 'process instance id'
    [back_user_id]      NVARCHAR(64)    NULL     DEFAULT NULL,   -- COMMENT: 'back to user id'
    [user_name]         NVARCHAR(255)   NULL,
    [is_out_side_process] TINYINT       NULL     DEFAULT 0,      -- COMMENT: 'is it an outside process,0 no,1 yes'
    [is_lowcode_flow]   TINYINT         NULL     DEFAULT 0,      -- COMMENT: '是否是低代码工作流0,否,1是'
    CONSTRAINT [PK_bpm_business_process] PRIMARY KEY ([id])
);
CREATE INDEX [IX_bpm_business_process_PROC_INST_ID] ON [dbo].[bpm_business_process] ([PROC_INST_ID_]);
CREATE INDEX [IX_bpm_business_process_ENTRY_ID]     ON [dbo].[bpm_business_process] ([ENTRY_ID]);
CREATE INDEX [IX_bpm_business_process_PROCESSINESS_KEY] ON [dbo].[bpm_business_process] ([PROCESSINESS_KEY]);
CREATE INDEX [IX_bpm_business_process_BUSINESS_NUMBER]  ON [dbo].[bpm_business_process] ([BUSINESS_NUMBER]);
CREATE INDEX [IX_bpm_business_process_process_state]    ON [dbo].[bpm_business_process] ([process_state]);
END;
-- COMMENT: 'process and business association table'
GO

-- ============================================================
-- 17. t_user_message
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_user_message]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_user_message]
(
    [id]          BIGINT       NOT NULL IDENTITY(1,1),
    [user_id]     NVARCHAR(64) NULL,                    -- COMMENT: '用户id'
    [title]       NVARCHAR(50) NULL,                    -- COMMENT: '标题'
    [content]     NVARCHAR(255)NULL,                    -- COMMENT: '消息内容'
    [url]         NVARCHAR(255)NULL,                    -- COMMENT: '发送url'
    [node]        NVARCHAR(50) NULL,                    -- COMMENT: '发送节点id'
    [params]      NVARCHAR(255)NULL,                    -- COMMENT: '发送类型'
    [is_read]     TINYINT      NULL,                    -- COMMENT: '0为未读 1为已读'
    [is_del]      TINYINT      NULL,                    -- COMMENT: '0为未删除 1为已删除'
    [tenant_id]   NVARCHAR(64) NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_time] DATETIME2    NULL,
    [update_time] DATETIME2    NULL,
    [create_user] NVARCHAR(50) NULL,
    [update_user] NVARCHAR(50) NULL,
    [app_url]     NVARCHAR(255)NULL,                    -- COMMENT: 'appurl'
    [source]      INT          NULL,
    CONSTRAINT [PK_t_user_message] PRIMARY KEY ([id])
);
END;
GO

-- ============================================================
-- 18. t_op_log
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_op_log]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_op_log]
(
    [id]             BIGINT        NOT NULL IDENTITY(1,1),
    [msg_id]         NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: ' msg id'
    [op_flag]        TINYINT       NULL     DEFAULT NULL,   -- COMMENT: '0=success, 1=fail, 2=business exception'
    [op_user_no]     NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'user no'
    [op_user_name]   NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'user name'
    [op_method]      NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'op method'
    [op_time]        DATETIME2     NOT NULL DEFAULT GETDATE(), -- COMMENT: 'op time'
    [op_use_time]    BIGINT        NULL     DEFAULT NULL,   -- COMMENT: 'time cost'
    [op_param]       NVARCHAR(MAX) NULL,                    -- COMMENT: 'op params'
    [op_result]      NVARCHAR(MAX) NULL,                    -- COMMENT: 'op result'
    [system_type]    TINYINT       NULL     DEFAULT NULL,   -- COMMENT: 'operation system type，iOS，Android，1=PC'
    [app_version]    NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'app version'
    [hardware]       NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'hardware info'
    [system_version] NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'app version'
    [remark]         NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'remark'
    [is_del]         INT           NULL     DEFAULT 0,
    [tenant_id]      NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_t_op_log] PRIMARY KEY ([id])
);
END;
GO

-- ============================================================
-- 19. t_biz_account_apply
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_biz_account_apply]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_biz_account_apply]
(
    [id]                 INT           NOT NULL IDENTITY(1,1),
    [account_type]       TINYINT       NULL     DEFAULT NULL,   -- COMMENT: 'account type'
    [account_owner_name] NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'account owner'
    [remark]             NVARCHAR(200) NULL     DEFAULT NULL,   -- COMMENT: 'remark'
    [is_del]             INT           NULL     DEFAULT 0,
    [tenant_id]          NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_t_biz_account_apply] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'a third party account apply demo'
GO

-- ============================================================
-- 20. bpm_process_app_application
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_app_application]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_app_application]
(
    [id]               INT           NOT NULL IDENTITY(1,1), -- COMMENT: 'Primary key'
    [business_code]    NVARCHAR(50)  NULL,                    -- COMMENT: 'Business code...'
    [process_name]     NVARCHAR(50)  NULL,                    -- COMMENT: 'Application name'
    [apply_type]       INT           NULL,                    -- COMMENT: 'Application type...'
    [permissions_code] NVARCHAR(50)  NULL,
    [pc_icon]          NVARCHAR(500) NULL,                    -- COMMENT: 'PC icon URL or path'
    [effective_source] NVARCHAR(500) NULL,                    -- COMMENT: 'Mobile platform icon...'
    [is_son]           INT           NULL,                    -- COMMENT: 'Whether it is a child application...'
    [look_url]         NVARCHAR(500) NULL,                    -- COMMENT: 'URL for viewing...'
    [submit_url]       NVARCHAR(500) NULL,                    -- COMMENT: 'URL for submitting...'
    [condition_url]    NVARCHAR(500) NULL,                    -- COMMENT: 'URL for accessing conditions...'
    [parent_id]        INT           NULL,                    -- COMMENT: 'Parent application ID...'
    [application_url]  NVARCHAR(500) NULL,                    -- COMMENT: 'Main URL of the application'
    [user_request_uri] NVARCHAR(500) NULL,                    -- COMMENT: 'get user info'
    [role_request_uri] NVARCHAR(500) NULL,                    -- COMMENT: 'get Role info'
    [route]            NVARCHAR(500) NULL,                    -- COMMENT: 'Application route or path'
    [process_key]      NVARCHAR(50)  NULL,                    -- COMMENT: 'Process key or identifier'
    [create_time]      DATETIME2     NOT NULL DEFAULT GETDATE(), -- COMMENT: 'Creation timestamp'
    [update_time]      DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'Last update timestamp' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [is_del]           TINYINT       NOT NULL DEFAULT 0,
    [create_user_id]   NVARCHAR(64)  NULL,
    [update_user]      NVARCHAR(255) NULL,
    [is_all]           TINYINT       NULL     DEFAULT 0,
    [state]            TINYINT       NULL     DEFAULT 1,
    [sort]             INT           NULL,
    [source]           NVARCHAR(255) NULL,
    CONSTRAINT [PK_bpm_process_app_application] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'BPM Process Application Table'
GO

-- ============================================================
-- 21. bpm_process_app_data
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_app_data]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_app_data]
(
    [id]             BIGINT       NOT NULL IDENTITY(1,1), -- COMMENT: 'Primary key'
    [process_key]    NVARCHAR(50) NULL,                    -- COMMENT: 'Process key'
    [process_name]   NVARCHAR(50) NULL,                    -- COMMENT: 'Process name'
    [state]          INT          NULL,                    -- COMMENT: 'Is online (0 for no, 1 for yes)'
    [route]          NVARCHAR(500)NULL,                    -- COMMENT: 'APP route'
    [sort]           INT          NULL,                    -- COMMENT: 'Sort order'
    [source]         NVARCHAR(500)NULL,                    -- COMMENT: 'Pic source route'
    [is_all]         TINYINT      NULL,                    -- COMMENT: 'Is for all (0 or 1)'
    [version_id]     BIGINT       NULL,                    -- COMMENT: 'Version ID'
    [application_id] BIGINT       NULL,                    -- COMMENT: 'Application ID'
    [type]           INT          NULL,                    -- COMMENT: 'Type (1 for version app, 2 for app data)'
    CONSTRAINT [PK_bpm_process_app_data] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'App Online Process Data Table'
GO

-- ============================================================
-- 22. bpm_process_category
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_category]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_category]
(
    [id]                BIGINT       NOT NULL IDENTITY(1,1), -- COMMENT: 'Primary key'
    [process_type_name] NVARCHAR(255)NULL,                    -- COMMENT: 'Process type name'
    [is_del]            TINYINT      NULL,                    -- COMMENT: 'Deletion flag...'
    [tenant_id]         NVARCHAR(64) NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [state]             INT          NULL,                    -- COMMENT: 'State of the category...'
    [sort]              INT          NULL,                    -- COMMENT: 'Sort order'
    [is_app]            TINYINT      NULL,                    -- COMMENT: 'Is for app...'
    [entrance]          NVARCHAR(255)NULL,                    -- COMMENT: 'Entrance...'
    CONSTRAINT [PK_bpm_process_category] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'BPM Process Category Table'
GO

-- ============================================================
-- 23. bpm_process_permissions
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_process_permissions]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_process_permissions]
(
    [id]               BIGINT       NOT NULL IDENTITY(1,1), -- COMMENT: 'Primary key'
    [user_id]          NVARCHAR(64) NULL,                    -- COMMENT: 'User ID'
    [dep_id]           BIGINT       NULL,                    -- COMMENT: 'Department ID'
    [permissions_type] INT          NULL,                    -- COMMENT: 'Permission type...'
    [create_user]      NVARCHAR(64) NULL,                    -- COMMENT: 'Create user ID'
    [create_time]      DATETIME2    NOT NULL DEFAULT GETDATE(), -- COMMENT: 'Create time'
    [process_key]      NVARCHAR(50) NULL,                    -- COMMENT: 'Process key'
    [office_id]        BIGINT       NULL,                    -- COMMENT: 'Office ID'
    [is_del]           INT          NOT NULL DEFAULT 0,      -- COMMENT: '0 for normal 1 for delete'
    [tenant_id]        NVARCHAR(64) NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_bpm_process_permissions] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'process permission'
GO

-- ============================================================
-- 24. t_out_side_bpm_access_business
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_access_business]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_out_side_bpm_access_business]
(
    [id]            BIGINT        NOT NULL IDENTITY(1,1),
    [business_party_id] BIGINT    NOT NULL,
    [bpmn_conf_id]  BIGINT        NOT NULL,
    [form_code]     NVARCHAR(50)  NULL     DEFAULT NULL,
    [process_number]NVARCHAR(50)  NULL     DEFAULT NULL,
    [form_data_pc]  NVARCHAR(MAX) NULL,
    [form_data_app] NVARCHAR(MAX) NULL,
    [template_mark] NVARCHAR(50)  NULL     DEFAULT NULL,
    [start_username]NVARCHAR(50)  NULL     DEFAULT NULL,
    [remark]        NVARCHAR(MAX) NULL,
    [is_del]        TINYINT       NULL     DEFAULT 0,
    [create_user]   NVARCHAR(50)  NULL     DEFAULT NULL,
    [create_time]   DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]   NVARCHAR(50)  NULL     DEFAULT NULL,
    [update_time]   DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_out_side_bpm_access_business] PRIMARY KEY ([id])
);
CREATE INDEX [idx_bpm_conf_id]        ON [dbo].[t_out_side_bpm_access_business] ([bpmn_conf_id]);
CREATE INDEX [idx_business_party_id]  ON [dbo].[t_out_side_bpm_access_business] ([business_party_id]);
END;
GO

-- ============================================================
-- 25. t_out_side_bpm_admin_personnel
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_admin_personnel]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_out_side_bpm_admin_personnel]
(
    [id]                BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: 'Auto increment ID'
    [business_party_id] BIGINT        NULL     DEFAULT NULL,   -- COMMENT: 'Business party main table ID'
    [type]              INT           NULL     DEFAULT NULL,   -- COMMENT: 'Administrator type...'
    [employee_id]       NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'Administrator ID...'
    [employee_name]     NVARCHAR(64)  NULL     DEFAULT NULL,   -- COMMENT: 'Administrator name...'
    [remark]            NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'Remark'
    [is_del]            INT           NULL     DEFAULT NULL,   -- COMMENT: 'Deletion flag...'
    [create_user]       NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'Creator user'
    [create_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'Creation time'
    [update_user]       NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'Updater user'
    [update_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'Update time' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_out_side_bpm_admin_personnel] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'Workflow External Service - Business Party Administrator Table'
GO

-- ============================================================
-- 26. t_out_side_bpm_business_party
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_business_party]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_out_side_bpm_business_party]
(
    [id]                  BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: 'Auto incr id'
    [business_party_mark] NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'Business party mark'
    [name]                NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'Business party name'
    [type]                TINYINT       NULL     DEFAULT NULL,   -- COMMENT: 'Business type...'
    [remark]              NVARCHAR(255) NULL     DEFAULT NULL,   -- COMMENT: 'Remark'
    [is_del]              TINYINT       NULL     DEFAULT 0,      -- COMMENT: 'Deletion flag...'
    [create_user]         NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'Creator user'
    [create_time]         DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'Creation time'
    [update_user]         NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: 'Updater user'
    [update_time]         DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'Update time' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_out_side_bpm_business_party] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'Table for storing business party information in the external BPM system'
GO

-- ============================================================
-- 27. t_out_side_bpm_callback_url_conf
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_callback_url_conf]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_out_side_bpm_callback_url_conf]
(
    [id]                    BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: 'auto increment id'
    [business_party_id]     BIGINT        NULL,                    -- COMMENT: 'business party id'
    [application_id]        BIGINT        NULL,                    -- COMMENT: 'business partys application id'
    [bpmn_conf_id]          BIGINT        NULL,                    -- COMMENT: 'bpmn confi id'
    [form_code]             NVARCHAR(64)  NULL,                    -- COMMENT: 'formcode'
    [bpm_conf_callback_url] NVARCHAR(500) NULL,                    -- COMMENT: 'conf callback url'
    [bpm_flow_callback_url] NVARCHAR(500) NULL,                    -- COMMENT: 'process flow call back url'
    [api_client_id]         NVARCHAR(100) NULL,                    -- COMMENT: 'appId'
    [api_client_secret]     NVARCHAR(100) NULL,                    -- COMMENT: 'appSecret'
    [status]                TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for enable,1 for disable'
    [create_user]           NVARCHAR(50)  NULL,                    -- COMMENT: 'as its name says'
    [update_user]           NVARCHAR(50)  NULL,
    [remark]                NVARCHAR(50)  NULL,                    -- COMMENT: 'remark'
    [is_del]                TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for normal,1 for delete'
    [create_time]           DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_time]           DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_out_side_bpm_callback_url_conf] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'business party callback url conf'
GO

-- ============================================================
-- 28. t_out_side_bpm_approve_template
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_approve_template]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_out_side_bpm_approve_template];
CREATE TABLE [dbo].[t_out_side_bpm_approve_template]
(
    [id]                BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: 'auto increment id'
    [business_party_id] BIGINT        NULL,                    -- COMMENT: '业务方项目 Id'
    [application_id]    INT           NULL,                    -- COMMENT: '项目下业务表单 id'
    [approve_type_id]   INT           NULL,                    -- COMMENT: '审批人类型 id'
    [approve_type_name] NVARCHAR(50)  NULL,                    -- COMMENT: '审批人类型名称'
    [api_client_id]     NVARCHAR(50)  NULL,                    -- COMMENT: 'api_client_id'
    [api_client_secret] NVARCHAR(50)  NULL,                    -- COMMENT: 'api_client_secret'
    [api_token]         NVARCHAR(50)  NULL,                    -- COMMENT: 'api_token'
    [api_url]           NVARCHAR(50)  NULL,                    -- COMMENT: 'api_url'
    [remark]            NVARCHAR(255) NULL,                    -- COMMENT: 'remark'
    [is_del]            TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for normal, 1 for delete'
    [create_user]       NVARCHAR(50)  NULL,                    -- COMMENT: 'as its name says'
    [create_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]       NVARCHAR(50)  NULL,                    -- COMMENT: 'as its name says'
    [update_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [create_user_id]    NVARCHAR(64)  NULL,                    -- COMMENT: 'as its name says'
    CONSTRAINT [PK_t_out_side_bpm_approve_template] PRIMARY KEY ([id])
);
-- COMMENT: 'outside access process,approve template config'
GO

-- ============================================================
-- 29. t_out_side_bpm_conditions_template
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_conditions_template]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_out_side_bpm_conditions_template];
CREATE TABLE [dbo].[t_out_side_bpm_conditions_template]
(
    [id]                BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: 'auto increment id'
    [business_party_id] BIGINT        NULL,                    -- COMMENT: 'business party Id'
    [template_mark]     NVARCHAR(50)  NULL,                    -- COMMENT: 'template mark'
    [template_name]     NVARCHAR(50)  NULL,                    -- COMMENT: 'template name'
    [application_id]    INT           NULL,                    -- COMMENT: 'application id'
    [remark]            NVARCHAR(255) NULL,                    -- COMMENT: 'remark'
    [is_del]            TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for normal, 1 for delete'
    [create_user]       NVARCHAR(50)  NULL,                    -- COMMENT: 'as its name says'
    [create_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]       NVARCHAR(50)  NULL,                    -- COMMENT: 'as its name says'
    [update_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    [create_user_id]    NVARCHAR(64)  NULL,                    -- COMMENT: 'as its name says'
    CONSTRAINT [PK_t_out_side_bpm_conditions_template] PRIMARY KEY ([id])
);
-- COMMENT: 'outside access process,condition template config'
GO

-- ============================================================
-- 30. t_out_side_bpm_call_back_record
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_out_side_bpm_call_back_record]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_out_side_bpm_call_back_record]
(
    [id]                  INT           NOT NULL IDENTITY(1,1), -- COMMENT: 'auto increment id'
    [process_number]      NVARCHAR(50)  NULL,                    -- COMMENT: 'process number'
    [status]              TINYINT       NULL,                    -- COMMENT: 'push status...'
    [retry_times]         TINYINT       NULL,                    -- COMMENT: 'retry times'
    [button_operation_type] TINYINT     NULL,                    -- COMMENT: 'operation type...'
    [call_back_type_name] NVARCHAR(255) NULL,                    -- COMMENT: 'call back type name...'
    [business_id]         BIGINT        NULL,                    -- COMMENT: 'business id'
    [form_code]           NVARCHAR(50)  NULL,                    -- COMMENT: 'form code'
    [is_del]              TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for normal,1 for delete'
    [create_user]         NVARCHAR(50)  NULL,                    -- COMMENT: 'create user'
    [create_time]         DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'create time'
    [update_user]         NVARCHAR(50)  NULL,                    -- COMMENT: 'update user'
    [update_time]         DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'update time' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_t_out_side_bpm_call_back_record] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'Table for storing callback records'
GO

-- ============================================================
-- 31. t_quick_entry
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_quick_entry]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_quick_entry]
(
    [id]                INT           NOT NULL IDENTITY(1,1),
    [title]             NVARCHAR(100) NOT NULL,
    [effective_source]  NVARCHAR(255) NULL,
    [is_del]            TINYINT       NULL     DEFAULT 0,
    [tenant_id]         NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [route]             NVARCHAR(500) NOT NULL,
    [sort]              TINYINT       NULL     DEFAULT 0,
    [create_time]       DATETIME2     NULL     DEFAULT GETDATE(),
    [status]            TINYINT       NOT NULL DEFAULT 0,
    [variable_url_flag] TINYINT       NOT NULL DEFAULT 0,
    [type_config_json]  NVARCHAR(MAX) NULL,                    -- COMMENT: 'type config json...'
    CONSTRAINT [PK_t_quick_entry] PRIMARY KEY ([id])
);
CREATE INDEX [idx_route] ON [dbo].[t_quick_entry] ([route]);
END;
GO

-- ============================================================
-- 32. t_sys_version
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_sys_version]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_sys_version]
(
    [id]          BIGINT        NOT NULL IDENTITY(1,1),
    [create_time] DATETIME2     NULL     DEFAULT GETDATE(),
    [update_time] DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    [is_del]      TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0 for normal, 1 for deleted'
    [tenant_id]   NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [version]     NVARCHAR(100) NOT NULL,               -- COMMENT: 'Version'
    [description] NVARCHAR(255) NULL,                    -- COMMENT: 'Version description'
    [index]       INT           NULL,                    -- COMMENT: 'Index'
    [is_force]    TINYINT       NULL,                    -- COMMENT: 'Force update...'
    [android_url] NVARCHAR(500) NULL,                    -- COMMENT: 'Android download URL'
    [ios_url]     NVARCHAR(500) NULL,                    -- COMMENT: 'iOS download URL'
    [create_user] NVARCHAR(50)  NULL,                    -- COMMENT: 'Create user'
    [update_user] NVARCHAR(50)  NULL,                    -- COMMENT: 'Update user'
    [is_hide]     TINYINT       NULL,                    -- COMMENT: '0 for not hide and 1 for hide'
    [download_code] NVARCHAR(255)NULL,                    -- COMMENT: 'Download code'
    [effective_time] DATETIME2  NULL     DEFAULT GETDATE(), -- COMMENT: 'Effective time'
    CONSTRAINT [PK_t_sys_version] PRIMARY KEY ([id])
);
CREATE INDEX [idx_version] ON [dbo].[t_sys_version] ([version]);
END;
-- COMMENT: 'sys version control'
GO

-- ============================================================
-- 33. t_department
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_department]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_department]
(
    [id]          INT           NOT NULL IDENTITY(1,1), -- COMMENT: 'Primary key'
    [name]        NVARCHAR(255) NULL,                    -- COMMENT: 'Name'
    [short_name]  NVARCHAR(255) NULL,                    -- COMMENT: 'Short name'
    [parent_id]   INT           NULL,                    -- COMMENT: 'Parent ID'
    [path]        NVARCHAR(255) NULL,                    -- COMMENT: 'Path'
    [level]       INT           NULL,                    -- COMMENT: 'Department level'
    [leader_id]   BIGINT        NULL,
    [sort]        INT           NULL,                    -- COMMENT: 'Sort order'
    [is_del]      TINYINT       NULL,                    -- COMMENT: 'Is deleted (0 for no, 1 for yes)'
    [is_hide]     TINYINT       NULL,                    -- COMMENT: 'Is hidden (0 for show, 1 for hide)'
    [create_user] NVARCHAR(255) NULL,                    -- COMMENT: 'Create user'
    [update_user] NVARCHAR(255) NULL,                    -- COMMENT: 'Update user'
    [create_time] DATETIME2     NULL,                    -- COMMENT: 'Creation time'
    [update_time] DATETIME2     NULL,                    -- COMMENT: 'Update time'
    CONSTRAINT [PK_t_department] PRIMARY KEY ([id])
);
END;
-- COMMENT: 'department info'
GO

-- ============================================================
-- 34. t_user
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_user]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_user];
IF OBJECT_ID(N'[dbo].[t_user]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_user]
(
    [id]             INT           NOT NULL IDENTITY(1,1),
    [user_name]      NVARCHAR(255) NULL,
    [mobile]         NVARCHAR(50)  NULL,                    -- COMMENT: 'user''s mobile phone number'
    [email]          NVARCHAR(50)  NULL,                    -- COMMENT: 'user''s email address'
    [leader_id]      BIGINT        NULL,                    -- COMMENT: 'emp direct leader id'
    [hrbp_id]        BIGINT        NULL,                    -- COMMENT: '用户的hrb的id...'
    [mobile_is_show] TINYINT       NULL     DEFAULT 0,      -- COMMENT: '是否展示用户手机号...'
    [department_id]  BIGINT        NULL,                    -- COMMENT: '部门id'
    [path]           NVARCHAR(1000)NULL,                    -- COMMENT: '员工组织线path...'
    [is_del]         TINYINT       NULL     DEFAULT 0,      -- COMMENT: '0,正常1,删除'
    [head_img]       NVARCHAR(3000)NULL,
    CONSTRAINT [PK_t_user] PRIMARY KEY ([id])
);
END;
GO

-- ============================================================
-- 35. t_role
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_role]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_role];
CREATE TABLE [dbo].[t_role]
(
    [id]        INT           NOT NULL IDENTITY(1,1),
    [role_name] NVARCHAR(255) NULL DEFAULT NULL,
    CONSTRAINT [PK_t_role] PRIMARY KEY ([id])
);
GO

-- ============================================================
-- 36. t_biz_leavetime
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_biz_leavetime]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_biz_leavetime];
CREATE TABLE [dbo].[t_biz_leavetime]
(
    [id]              INT           NOT NULL IDENTITY(1,1),
    [leave_user_id]   INT           NOT NULL,
    [leave_user_name] NVARCHAR(255) NOT NULL,
    [leave_type]      INT           NOT NULL,
    [begin_time]      DATETIME2     NULL,
    [end_time]        DATETIME2     NULL,
    [leavehour]       FLOAT         NOT NULL,
    [remark]          NVARCHAR(255) NULL     DEFAULT NULL,
    [create_user]     NVARCHAR(255) NOT NULL,
    [create_time]     DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]     NVARCHAR(255) NULL     DEFAULT NULL,
    [update_time]     DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_biz_leavetime] PRIMARY KEY ([id])
);
GO

-- ============================================================
-- 37. t_biz_purchase
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_biz_purchase]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_biz_purchase];
CREATE TABLE [dbo].[t_biz_purchase]
(
    [id]                           INT           NOT NULL IDENTITY(1,1),
    [purchase_user_id]             INT           NOT NULL,
    [purchase_user_name]           NVARCHAR(255) NOT NULL,
    [purchase_type]                INT           NOT NULL DEFAULT 1,
    [purchase_time]                DATETIME2     NULL     DEFAULT GETDATE(),
    [plan_procurement_total_money] FLOAT         NOT NULL DEFAULT 0,
    [remark]                       NVARCHAR(255) NULL     DEFAULT NULL,
    [create_user]                  NVARCHAR(255) NOT NULL,
    [create_time]                  DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]                  NVARCHAR(255) NULL     DEFAULT NULL,
    [update_time]                  DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_biz_purchase] PRIMARY KEY ([id])
);
GO

-- ============================================================
-- 38. t_biz_ucar_refuel
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_biz_ucar_refuel]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_biz_ucar_refuel];
CREATE TABLE [dbo].[t_biz_ucar_refuel]
(
    [id]                 INT           NOT NULL IDENTITY(1,1),
    [license_plate_number] NVARCHAR(32)NULL     DEFAULT NULL,   -- COMMENT: '车牌号'
    [refuel_time]        DATETIME2     NULL     DEFAULT NULL,   -- COMMENT: '加油日期'
    [remark]             NVARCHAR(255) NULL     DEFAULT NULL,
    [create_user]        NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: '创建人'
    [create_time]        DATETIME2     NULL     DEFAULT NULL,   -- COMMENT: '创建日期'
    [update_user]        NVARCHAR(50)  NULL     DEFAULT NULL,   -- COMMENT: '更新人'
    [update_time]        DATETIME2     NULL     DEFAULT NULL,   -- COMMENT: '更新日期'
    CONSTRAINT [PK_t_biz_ucar_refuel] PRIMARY KEY ([id])
);
-- COMMENT: '加油表'
GO

-- ============================================================
-- 39. t_biz_refund
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_biz_refund]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_biz_refund];
CREATE TABLE [dbo].[t_biz_refund]
(
    [id]              INT           NOT NULL IDENTITY(1,1),
    [refund_user_id]  INT           NOT NULL,
    [refund_user_name]NVARCHAR(255) NOT NULL,
    [refund_type]     INT           NOT NULL DEFAULT 1,
    [refund_date]     DATETIME2     NOT NULL,
    [refund_money]    FLOAT         NOT NULL DEFAULT 0,
    [remark]          NVARCHAR(255) NULL     DEFAULT NULL,
    [create_user]     NVARCHAR(255) NOT NULL,
    [create_time]     DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]     NVARCHAR(255) NULL     DEFAULT NULL,
    [update_time]     DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_biz_refund] PRIMARY KEY ([id])
);
GO

-- ============================================================
-- 40. t_lf_main
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_lf_main]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_lf_main]
(
    [id]          BIGINT        NOT NULL,
    [conf_id]     BIGINT        NULL,
    [form_code]   NVARCHAR(255) NULL,
    [is_del]      TINYINT       NOT NULL DEFAULT 0,
    [tenant_id]   NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user] NVARCHAR(255) NULL,
    [create_time] DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user] NVARCHAR(255) NULL,
    [update_time] DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [t_lf_main_pk] PRIMARY KEY ([id])
);
END;
-- COMMENT: '低代码表单主表'
GO

-- ============================================================
-- 41. t_lf_main_field
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_lf_main_field]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_lf_main_field]
(
    [id]                BIGINT        NOT NULL,
    [main_id]           BIGINT        NOT NULL,
    [form_code]         NVARCHAR(255) NULL,
    [field_id]          NVARCHAR(255) NULL,
    [field_name]        NVARCHAR(255) NULL,
    [parent_field_id]   NVARCHAR(255) NULL,
    [parent_field_name] NVARCHAR(255) NULL,
    [field_value]       NVARCHAR(2000)NULL,
    [field_value_number]DECIMAL(14,2) NULL,
    [field_value_dt]    DATETIME2     NULL,
    [field_value_text]  NVARCHAR(MAX) NULL,
    [sort]              INT           NOT NULL DEFAULT 0,
    [is_del]            TINYINT       NOT NULL DEFAULT 0,
    [tenant_id]         NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]       NVARCHAR(255) NULL,
    [create_time]       DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]       NVARCHAR(255) NULL,
    [update_time]       DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [t_lf_main_field_pk] PRIMARY KEY ([id])
);
END;
-- COMMENT: '低代码表单字段值表'
GO

-- ============================================================
-- 42. t_dict_data
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_dict_data]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_dict_data]
(
    [id]          BIGINT        NOT NULL IDENTITY(1,1), -- COMMENT: '字典编码'
    [dict_sort]   INT           NULL     DEFAULT 0,      -- COMMENT: '字典排序'
    [dict_label]  NVARCHAR(100) NULL     DEFAULT N'',    -- COMMENT: '字典标签'
    [dict_value]  NVARCHAR(100) NULL     DEFAULT N'',    -- COMMENT: '字典键值'
    [dict_type]   NVARCHAR(100) NULL     DEFAULT N'',    -- COMMENT: '字典类型'
    [css_class]   NVARCHAR(100) NULL,                    -- COMMENT: '样式属性...'
    [list_class]  NVARCHAR(100) NULL,                    -- COMMENT: '表格回显样式'
    [is_default]  CHAR(1)       NULL     DEFAULT N'N',   -- COMMENT: '是否默认（Y是 N否）'
    [is_del]      TINYINT       NOT NULL DEFAULT 0,
    [tenant_id]   NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user] NVARCHAR(255) NULL,
    [create_time] DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user] NVARCHAR(255) NULL,
    [update_time] DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    [remark]      NVARCHAR(500) NULL,                    -- COMMENT: '备注'
    CONSTRAINT [PK_t_dict_data] PRIMARY KEY ([id])
);
END;
-- COMMENT: '字典表子表...'
GO

-- ============================================================
-- 43. Additional Indexes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'idx_processInstance_Id' AND object_id = OBJECT_ID(N'[dbo].[bpm_process_node_submit]', N'U'))
    CREATE INDEX [idx_processInstance_Id] ON [dbo].[bpm_process_node_submit] ([processInstance_Id]);
GO

-- SQL Server默认启用外键检查，无需显式设置 FOREIGN_KEY_CHECKS
-- SET FOREIGN_KEY_CHECKS = 1;  -- MySQL专用，已移除
GO

-- ============================================================
-- 44. t_bpmn_conf_lf_formdata
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_bpmn_conf_lf_formdata];
CREATE TABLE [dbo].[t_bpmn_conf_lf_formdata]
(
    [id]           BIGINT        NOT NULL IDENTITY(1,1),
    [bpmn_conf_id] BIGINT        NOT NULL,
    [formdata]     NVARCHAR(MAX) NULL,
    [is_del]       TINYINT       NOT NULL DEFAULT 0,
    [tenant_id]    NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]  NVARCHAR(255) NULL     DEFAULT NULL,
    [create_time]  DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]  NVARCHAR(255) NULL     DEFAULT NULL,
    [update_time]  DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_bpmn_conf_lf_formdata] PRIMARY KEY ([id])
);
GO

-- ============================================================
-- 45. t_bpmn_conf_lf_formdata_field
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata_field]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_bpmn_conf_lf_formdata_field];
CREATE TABLE [dbo].[t_bpmn_conf_lf_formdata_field]
(
    [id]            BIGINT        NOT NULL IDENTITY(1,1),
    [bpmn_conf_id]  BIGINT        NULL     DEFAULT NULL,
    [formdata_id]   BIGINT        NULL     DEFAULT NULL,
    [field_id]      NVARCHAR(255) NULL     DEFAULT NULL,
    [field_name]    NVARCHAR(255) NULL     DEFAULT NULL,
    [field_type]    TINYINT       NULL     DEFAULT NULL,
    [is_condition]  TINYINT       NULL     DEFAULT 0,      -- COMMENT: '是否是流程条件,0否,1是'
    [is_del]        TINYINT       NOT NULL DEFAULT 0,
    [tenant_id]     NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]   NVARCHAR(255) NULL     DEFAULT NULL,
    [create_time]   DATETIME2     NULL     DEFAULT GETDATE(),
    [update_user]   NVARCHAR(255) NULL     DEFAULT NULL,
    [update_time]   DATETIME2     NULL     DEFAULT GETDATE(), -- 原ON UPDATE CURRENT_TIMESTAMP已移除
    CONSTRAINT [PK_t_bpmn_conf_lf_formdata_field] PRIMARY KEY ([id])
);
-- COMMENT: '低代码配置字段明细表'
GO

-- ============================================================
-- 46. t_user_role
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_user_role]', N'U') IS NOT NULL
    DROP TABLE [dbo].[t_user_role];
CREATE TABLE [dbo].[t_user_role]
(
    [id]      INT NOT NULL IDENTITY(1,1),
    [user_id] INT NULL DEFAULT NULL, -- COMMENT: ' user id '
    [role_id] INT NULL DEFAULT NULL, -- COMMENT: 'role id'
    CONSTRAINT [PK_t_user_role] PRIMARY KEY ([id])
);
-- COMMENT: '用户角色关联表'
GO

-- ============================================================
-- 47. t_bpm_dynamic_condition_choosen
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpm_dynamic_condition_choosen]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpm_dynamic_condition_choosen]
(
    [id]             BIGINT        NOT NULL IDENTITY(1,1),
    [process_number] NVARCHAR(255) NULL,                    -- COMMENT: '流程编号'
    [node_id]        NVARCHAR(100) NULL,                    -- COMMENT: '被选中条件节点的id'
    [node_from]      NVARCHAR(100) NULL,
    [is_del]         TINYINT       NOT NULL DEFAULT 0,      -- COMMENT: '0:正常,1:删除'
    [tenant_id]      NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [t_bpm_dynamic_condition_choosen_pk] PRIMARY KEY ([id])
);
CREATE INDEX [indx_process_number] ON [dbo].[t_bpm_dynamic_condition_choosen] ([process_number]);
END;
-- COMMENT: '流程动态条件选择条件记录表'
GO

-- ============================================================
-- 48. bpm_af_deployment
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_af_deployment]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_af_deployment]
(
    [id]            NVARCHAR(64)  NOT NULL,
    [rev]           INT           NULL,
    [name]          NVARCHAR(255) NULL,
    [content]       NVARCHAR(MAX) NULL,
    [remark]        NVARCHAR(255) NOT NULL DEFAULT N'',    -- COMMENT: 'remark'
    [is_del]        TINYINT       NOT NULL DEFAULT 0,      -- COMMENT: '0:in use,1:delete'
    [tenant_id]     NVARCHAR(64)  NULL     DEFAULT N'',    -- COMMENT: 'tenantId'
    [create_user]   NVARCHAR(32)  NULL     DEFAULT N'',    -- COMMENT: 'as its name says'
    [create_time]   DATETIME2     NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [update_user]   NVARCHAR(32)  NULL     DEFAULT N'',    -- COMMENT: '更新人'
    [update_time]   DATETIME2     NULL     DEFAULT GETDATE(), -- COMMENT: 'as its name says' (原ON UPDATE CURRENT_TIMESTAMP已移除)
    CONSTRAINT [PK_bpm_af_deployment] PRIMARY KEY ([id])
);
END;
GO

-- ============================================================
-- 49. bpm_af_taskinst
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_af_taskinst]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_af_taskinst]
(
    [id]                     NVARCHAR(64)  NOT NULL,
    [proc_def_id]            NVARCHAR(64)  NULL,
    [task_def_key]           NVARCHAR(255) NULL,
    [proc_inst_id]           NVARCHAR(64)  NULL,
    [execution_id]           NVARCHAR(64)  NULL,
    [name]                   NVARCHAR(255) NULL,
    [parent_task_id]         NVARCHAR(64)  NULL,
    [owner]                  NVARCHAR(255) NULL,
    [assignee]               NVARCHAR(255) NULL,
    [assignee_name]          NVARCHAR(255) NULL,
    [original_assignee]      NVARCHAR(255) NULL,
    [original_assignee_name] NVARCHAR(255) NULL,
    [transfer_reason]        NVARCHAR(1000)NULL,
    [verify_status]          INT           NULL,
    [verify_desc]            NVARCHAR(2000)NULL,
    [start_time]             DATETIME2(3)  NOT NULL,
    [claim_time]             DATETIME2(3)  NULL,
    [end_time]               DATETIME2(3)  NULL,
    [duration]               BIGINT        NULL,
    [delete_reason]          NVARCHAR(4000)NULL,
    [priority]               INT           NULL,
    [due_date]               DATETIME2(3)  NULL,
    [form_key]               NVARCHAR(255) NULL,
    [category]               NVARCHAR(255) NULL,
    [tenant_id]              NVARCHAR(255) NULL     DEFAULT N'',
    [description]            NVARCHAR(4000)NULL,
    [update_user]            NVARCHAR(64)  NULL,
    CONSTRAINT [PK_bpm_af_taskinst] PRIMARY KEY ([id])
);
CREATE INDEX [AF_HI_TASK_INST_PROCINST] ON [dbo].[bpm_af_taskinst] ([proc_inst_id]);
CREATE INDEX [idx_assignee_name]        ON [dbo].[bpm_af_taskinst] ([assignee_name]);
CREATE INDEX [idx_task_def_key]         ON [dbo].[bpm_af_taskinst] ([task_def_key]);
END;
GO

-- ============================================================
-- 50. bpm_af_task
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_af_task]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_af_task]
(
    [id]               NVARCHAR(64)  NOT NULL,
    [rev]              INT           NULL,
    [execution_id]     NVARCHAR(64)  NULL,
    [proc_inst_id]     NVARCHAR(64)  NULL,
    [proc_def_id]      NVARCHAR(64)  NULL,
    [name]             NVARCHAR(255) NULL,
    [parent_task_id]   NVARCHAR(64)  NULL,
    [task_def_key]     NVARCHAR(255) NULL,
    [owner]            NVARCHAR(255) NULL,
    [assignee]         NVARCHAR(255) NULL,
    [assignee_name]    NVARCHAR(255) NULL,
    [node_id]          NVARCHAR(64)  NULL,                    -- COMMENT: 'current node''s virtual node id'
    [node_type]        INT           NULL,                    -- COMMENT: 'current element''s virual node nodetype'
    [delegation]       NVARCHAR(64)  NULL,
    [priority]         INT           NULL,
    [create_time]      DATETIME2(3)  NULL,
    [due_date]         DATETIME2(3)  NULL,
    [category]         NVARCHAR(255) NULL,
    [suspension_state] INT           NULL,
    [tenant_id]        NVARCHAR(255) NULL     DEFAULT N'',
    [form_key]         NVARCHAR(255) NULL,
    [description]      NVARCHAR(4000)NULL,
    CONSTRAINT [PK_bpm_af_task] PRIMARY KEY ([id])
);
CREATE INDEX [AF_IDX_TASK_CREATE]   ON [dbo].[bpm_af_task] ([create_time]);
CREATE INDEX [AF_IDX_PROCINSTID]    ON [dbo].[bpm_af_task] ([proc_inst_id]);
CREATE INDEX [AF_IDX_TASK_DEF_KEY]  ON [dbo].[bpm_af_task] ([task_def_key]);
CREATE INDEX [AF_IDX_TASK_ASSIGNEE] ON [dbo].[bpm_af_task] ([assignee]);
END;
GO

-- ============================================================
-- 51. bpm_af_execution
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_af_execution]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_af_execution]
(
    [id]                NVARCHAR(64)  NOT NULL,
    [rev_]              INT           NULL,
    [proc_inst_id]      NVARCHAR(64)  NULL,
    [business_key]      NVARCHAR(255) NULL,
    [parent_id]         NVARCHAR(64)  NULL,
    [proc_def_id]       NVARCHAR(64)  NULL,
    [super_exec]        NVARCHAR(64)  NULL,
    [root_proc_inst_id] NVARCHAR(64)  NULL,
    [act_id]            NVARCHAR(255) NULL,
    [is_active]         TINYINT       NULL,
    [is_concurrent]     TINYINT       NULL,
    [tenant_id]         NVARCHAR(255) NULL     DEFAULT N'',
    [name]              NVARCHAR(255) NULL,
    [start_time]        DATETIME2     NULL,
    [start_user_id]     NVARCHAR(255) NULL,
    [is_count_enabled]  TINYINT       NULL,
    [evt_subscr_count]  INT           NULL,
    [task_count]        INT           NULL,
    [var_count]         INT           NULL,
    [sign_type]         INT           NULL,
    CONSTRAINT [PK_bpm_af_execution] PRIMARY KEY ([id])
);
CREATE INDEX [AF_IDX_EXEC_PROCINSTID] ON [dbo].[bpm_af_execution] ([proc_inst_id]);
CREATE INDEX [AF_IDX_EXEC_BUSKEY]     ON [dbo].[bpm_af_execution] ([business_key]);
END;
GO

-- ============================================================
-- 52. Seed Data: t_user
-- ============================================================
SET IDENTITY_INSERT [dbo].[t_user] ON;
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (1, N'张三', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (2, N'李四', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (3, N'王五', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (4, N'菜六', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (5, N'牛七', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (6, N'马八', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (7, N'李九', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (8, N'周十', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (9, N'肖十一', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (10, N'令狐冲', NULL, N'zypqqgc@qq.com', 13, 17, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (11, N'风清扬', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (12, N'刘正风', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (13, N'岳不群', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (14, N'宁中则', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (15, N'桃谷六仙', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (16, N'不介和尚', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (17, N'丁一师太', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (18, N'依林师妹', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (19, N'邱灵珊', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (20, N'任盈盈', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (235, N'斯克', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (237, N'川普', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
INSERT INTO [dbo].[t_user] ([id], [user_name], [mobile], [email], [leader_id], [hrbp_id], [mobile_is_show], [path], [is_del], [head_img], [department_id]) VALUES (1001, N'小马', NULL, N'zypqqgc@qq.com', 18, 19, 0, NULL, 0, NULL, 9);
SET IDENTITY_INSERT [dbo].[t_user] OFF;
GO

-- ============================================================
-- 53. Seed Data: t_role
-- ============================================================
SET IDENTITY_INSERT [dbo].[t_role] ON;
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (1, N'审核管理员');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (2, N'招商事业部');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (3, N'互联网部门');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (4, N'销售部');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (5, N'战区一');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (6, N'战区二');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (7, N'JAVA开发');
INSERT INTO [dbo].[t_role] ([id], [role_name]) VALUES (8, N'测试审批角色');
SET IDENTITY_INSERT [dbo].[t_role] OFF;
GO

-- ============================================================
-- 54. Seed Data: t_user_role
-- ============================================================
SET IDENTITY_INSERT [dbo].[t_user_role] ON;
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (1, 1, 1);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (2, 1, 1);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (3, 1, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (4, 2, 2);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (5, 2, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (6, 2, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (7, 3, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (8, 4, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (9, 5, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (10, 6, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (11, 7, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (12, 11, 3);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (13, 10, 6);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (14, 8, 7);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (15, 19, 8);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (16, 12, 4);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (17, 13, 5);
INSERT INTO [dbo].[t_user_role] ([id], [user_id], [role_id]) VALUES (18, 16, 4);
SET IDENTITY_INSERT [dbo].[t_user_role] OFF;
GO

-- ============================================================
-- 55. Seed Data: t_department
-- ============================================================
SET IDENTITY_INSERT [dbo].[t_department] ON;
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (1, N'一级部门', NULL, NULL, N'/1', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (2, N'二级部门', NULL, 3, N'/1/2', 2, 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (3, N'三级部门', NULL, 4, N'/1/2/3', 3, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (4, N'四级部门', NULL, 5, N'/1/2/3/4', 4, 4, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (5, N'五级部门', NULL, 6, N'/1/2/3/4/5', 5, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (6, N'六级部门', NULL, 7, N'/1/2/3/4/5/6', 6, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (7, N'七级部门', NULL, 8, N'/1/2/3/4/5/6/7', 7, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (8, N'市场部', NULL, 9, N'/1/2/3/4/5/6/7/8', 8, 8, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
INSERT INTO [dbo].[t_department] ([id], [name], [short_name], [parent_id], [path], [level], [leader_id], [sort], [is_del], [is_hide], [create_user], [update_user], [create_time], [update_time]) VALUES (9, N'销售部', NULL, 9, N'/1/2/3/4/5/6/7/8/9', 9, 9, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
SET IDENTITY_INSERT [dbo].[t_department] OFF;
GO

-- ============================================================
-- End of Script
-- ============================================================
-- ============================================================
-- bpm_business_draft (process draft)
-- ============================================================
IF OBJECT_ID(N'[dbo].[bpm_business_draft]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[bpm_business_draft]
(
    [id]               BIGINT         NOT NULL IDENTITY(1,1),
    [bpmn_code]        NVARCHAR(64)   NULL     DEFAULT NULL,   -- COMMENT: 'business id'
    [create_time]      DATETIME2      NOT NULL DEFAULT GETDATE(), -- COMMENT: 'as its name says'
    [process_code]     NVARCHAR(50)   NULL     DEFAULT NULL,   -- COMMENT: 'process Number'
    [create_user_name] NVARCHAR(50)   NULL     DEFAULT NULL,   -- COMMENT: 'as its name says'
    [create_user]      NVARCHAR(50)   NULL     DEFAULT NULL,   -- COMMENT: 'as its name says'
    [process_key]      NVARCHAR(50)   NULL     DEFAULT NULL,   -- COMMENT: 'as its name says'
    [draft_json]       NVARCHAR(MAX)  NULL,                    -- COMMENT: 'serialized form data json'
    [is_del]           TINYINT        NOT NULL DEFAULT 0,      -- COMMENT: '0:no,1:yes'
    [tenant_id]        NVARCHAR(255)  NOT NULL DEFAULT N'',    -- COMMENT: 'tenantId'
    CONSTRAINT [PK_bpm_business_draft] PRIMARY KEY ([id])
);
CREATE UNIQUE INDEX [UQ_bpm_business_draft_bpmn_code_create_user] ON [dbo].[bpm_business_draft] ([bpmn_code], [create_user]);
CREATE INDEX        [IX_bpm_business_draft_process_key]          ON [dbo].[bpm_business_draft] ([process_key]);
END;
-- COMMENT: 'process draft'
GO

-- ============================================================
-- t_bpm_dynamic_condition_choosen (dynamic condition chosen)
-- ============================================================
IF OBJECT_ID(N'[dbo].[t_bpm_dynamic_condition_choosen]', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[t_bpm_dynamic_condition_choosen]
(
    [id]              BIGINT         NOT NULL IDENTITY(1,1),
    [process_number]  NVARCHAR(64)   NULL     DEFAULT NULL,   -- COMMENT: 'process number'
    [node_id]         NVARCHAR(64)   NULL     DEFAULT NULL,   -- COMMENT: 'chosen condition node id'
    [node_from]       NVARCHAR(64)   NULL     DEFAULT NULL,   -- COMMENT: 'gateway node id'
    CONSTRAINT [PK_t_bpm_dynamic_condition_choosen] PRIMARY KEY ([id])
);
CREATE INDEX [IX_bpm_dyn_cond_process_number] ON [dbo].[t_bpm_dynamic_condition_choosen] ([process_number]);
END;
GO
