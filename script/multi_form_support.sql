-- =====================================================================
-- Multi-form support for low-code flow (.NET version)
-- 支持低代码流程绑定多个外部表单
--
-- 本脚本包含 MySQL 和 SQL Server 两个方言;请按目标数据库选择执行对应小节
-- =====================================================================


-- #####################################################################
-- #                          MySQL 方言                                #
-- #####################################################################

-- 1. t_bpmn_conf_lf_formdata: 扩展为同时承载"独立表单模板"和"内联设计表单"
--    bpmn_conf_id 为 NULL => 独立表单(由表单管理模块管理); 非 NULL => 内联表单(向后兼容)
ALTER TABLE `t_bpmn_conf_lf_formdata`
    ADD COLUMN `form_code` varchar(100) NULL DEFAULT NULL COMMENT '独立表单家族标识(同族各版本共享;内联表单为NULL)' AFTER `bpmn_conf_id`,
    ADD COLUMN `form_name` varchar(255) NULL DEFAULT NULL COMMENT '独立表单显示名(内联表单为NULL)' AFTER `form_code`,
    ADD COLUMN `effective_status` tinyint NOT NULL DEFAULT 0 COMMENT '是否当前生效版本 0否 1是(仅独立表单使用;内联表单恒为0)' AFTER `form_name`,
    MODIFY COLUMN `bpmn_conf_id` bigint NULL DEFAULT NULL COMMENT '流程配置ID(独立表单为NULL)';
-- 独立表单查询索引(按家族列出生效版本)
ALTER TABLE `t_bpmn_conf_lf_formdata`
    ADD KEY `idx_lf_formdata_form_code_eff` (`form_code`, `effective_status`),
    ADD KEY `idx_lf_formdata_bpmn_conf_id` (`bpmn_conf_id`);

-- 2. t_bpmn_conf: 外部表单模式所需的引用列表
--    lf_formdata_ids: CSV of t_bpmn_conf_lf_formdata.id (版本id), 顺序即 tab 顺序
--    模式标记复用 extra_flags 位掩码(BpmnConfFlagsEnum.USE_EXTERNAL_FORM=0b1000000), 无需新增列
ALTER TABLE `t_bpmn_conf`
    ADD COLUMN `lf_formdata_ids` varchar(500) NULL DEFAULT NULL COMMENT '外部表单引用的表单版本id列表(CSV),仅外部表单模式使用' AFTER `is_lowcode_flow`;

-- 3. t_lf_main_field: 多表单已填数据按表单版本区分
--    formdata_id 指向 t_bpmn_conf_lf_formdata.id; 旧数据为NULL => 内联模式回退
ALTER TABLE `t_lf_main_field`
    ADD COLUMN `formdata_id` bigint NULL DEFAULT NULL COMMENT '表单版本ID(t_bpmn_conf_lf_formdata.id);内联模式旧数据为NULL' AFTER `form_code`;
ALTER TABLE `t_lf_main_field`
    ADD KEY `idx_lf_main_field_formdata_id` (`formdata_id`);


-- #####################################################################
-- #                       SQL Server 方言                              #
-- #  注意: SQL Server ALTER TABLE 一次只能添加一列,且不支持 AFTER 子句   #
-- #####################################################################

-- 1. t_bpmn_conf_lf_formdata
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]') AND name = N'form_code')
    ALTER TABLE [dbo].[t_bpmn_conf_lf_formdata] ADD [form_code] NVARCHAR(100) NULL DEFAULT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]') AND name = N'form_name')
    ALTER TABLE [dbo].[t_bpmn_conf_lf_formdata] ADD [form_name] NVARCHAR(255) NULL DEFAULT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]') AND name = N'effective_status')
    ALTER TABLE [dbo].[t_bpmn_conf_lf_formdata] ADD [effective_status] TINYINT NOT NULL DEFAULT 0;
-- bpmn_conf_id 允许为 NULL(独立表单模式)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]') AND name = N'bpmn_conf_id')
    ALTER TABLE [dbo].[t_bpmn_conf_lf_formdata] ALTER COLUMN [bpmn_conf_id] BIGINT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'idx_lf_formdata_form_code_eff' AND object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]'))
    CREATE INDEX [idx_lf_formdata_form_code_eff] ON [dbo].[t_bpmn_conf_lf_formdata] ([form_code], [effective_status]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'idx_lf_formdata_bpmn_conf_id' AND object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf_lf_formdata]'))
    CREATE INDEX [idx_lf_formdata_bpmn_conf_id] ON [dbo].[t_bpmn_conf_lf_formdata] ([bpmn_conf_id]);

-- 2. t_bpmn_conf
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_bpmn_conf]') AND name = N'lf_formdata_ids')
    ALTER TABLE [dbo].[t_bpmn_conf] ADD [lf_formdata_ids] NVARCHAR(500) NULL DEFAULT NULL;

-- 3. t_lf_main_field
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[t_lf_main_field]') AND name = N'formdata_id')
    ALTER TABLE [dbo].[t_lf_main_field] ADD [formdata_id] BIGINT NULL DEFAULT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'idx_lf_main_field_formdata_id' AND object_id = OBJECT_ID(N'[dbo].[t_lf_main_field]'))
    CREATE INDEX [idx_lf_main_field_formdata_id] ON [dbo].[t_lf_main_field] ([formdata_id]);
