# AntFlowCore .NET 数据库设计

## 1. 概述

AntFlowCore 使用 FreeSQL 作为 ORM 框架，支持多种关系型数据库（MySQL、PostgreSQL、SQL Server、Oracle 等）。数据库设计遵循工作流引擎的标准模式，同时扩展了低代码、动态条件、加签会签等高级特性。

## 2. 数据库表分类

| 分类 | 表名 | 说明 |
|------|------|------|
| **流程定义** | `bpmn_conf` | 流程模板主表 |
| | `bpmn_node` | 流程节点表 |
| | `bpmn_node_to` | 节点连线关系表 |
| | `bpmn_conf_lf_formdata` | 低代码表单数据表 |
| | `bpmn_conf_lf_formdata_field` | 低代码表单字段表 |
| **流程运行** | `bpm_business_process` | 业务流程实例表 |
| | `bpm_af_deployment` | Activiti 部署表 |
| | `bpm_af_task` | Activiti 任务表 |
| | `bpm_af_task_inst` | 任务实例表 |
| | `bpm_af_execution` | Activiti 执行实例表 |
| **变量配置** | `bpm_variable` | 流程变量表 |
| | `bpm_variable_multiplayer` | 多人会签变量表 |
| | `bpm_variable_multiplayer_personnel` | 多人会签人员表 |
| **审批记录** | `bpm_verify_info` | 审批记录表 |
| | `bpm_process_forward` | 转发记录表 |
| | `bpm_flowrun_entrust` | 委托/转交记录表 |
| | `bpm_process_node_submit` | 节点提交记录表 |
| **条件分支** | `bpm_dynamic_condition_choosen` | 动态条件选择记录表 |
| | `out_side_bpm_conditions_template` | 外部系统条件模板表 |
| **通知消息** | `user_message` | 用户消息表 |
| | `user_message_status` | 消息状态表 |
| | `information_template` | 通知模板表 |
| **应用配置** | `bpm_process_app_application` | 应用URL配置表 |
| **组织架构** | `user` | 用户表 |
| | `role` | 角色表 |
| | `department` | 部门表 |
| | `user_role` | 用户角色关联表 |
| **第三方集成** | `out_side_bpm_access_business` | 外部流程接入表 |
| | `out_side_bpm_business_party` | 业务方配置表 |
| | `out_side_bpm_callback_url_conf` | 回调URL配置表 |
| | `out_side_bpm_admin_personnel` | 外部管理员表 |
| | `third_party_account_apply` | 第三方账户申请表 |

## 3. 核心表详解

### 3.1 bpmn_conf（流程模板主表）

流程模板的核心表，每个流程模板对应一条记录。

```sql
CREATE TABLE `bpmn_conf` (
    `id` BIGINT NOT NULL AUTO_INCREMENT COMMENT '自增ID',
    `bpmn_code` VARCHAR(50) NOT NULL COMMENT '流程编码（唯一）',
    `bpmn_name` VARCHAR(200) NOT NULL COMMENT '流程名称',
    `bpmn_type` INT DEFAULT NULL COMMENT '流程类型',
    `form_code` VARCHAR(100) DEFAULT NULL COMMENT '关联表单编码',
    `app_id` INT DEFAULT NULL COMMENT '应用ID',
    `deduplication_type` INT DEFAULT NULL COMMENT '去重类型(1-不去重 2-前向去重 3-后向去重)',
    `effective_status` INT NOT NULL DEFAULT 0 COMMENT '生效状态(0-未生效 1-已生效)',
    `is_all` INT NOT NULL DEFAULT 0 COMMENT '是否全员适用(0-否 1-是)',
    `is_out_side_process` INT DEFAULT NULL COMMENT '是否第三方流程(0-否 1-是)',
    `is_low_code_flow` INT DEFAULT NULL COMMENT '是否低代码流程',
    `business_party_id` BIGINT DEFAULT NULL COMMENT '业务方ID',
    `remark` TEXT DEFAULT NULL COMMENT '备注',
    `is_del` INT NOT NULL DEFAULT 0 COMMENT '删除标记',
    `tenant_id` VARCHAR(50) DEFAULT NULL COMMENT '租户ID',
    `create_user` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
    `create_time` DATETIME DEFAULT NULL COMMENT '创建时间',
    `update_user` VARCHAR(50) DEFAULT NULL COMMENT '更新人',
    `update_time` DATETIME DEFAULT NULL COMMENT '更新时间',
    `extra_flags` INT DEFAULT NULL COMMENT '扩展标记',
    `conf_config_json` TEXT DEFAULT NULL COMMENT '流程级JSON配置',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_bpmn_code` (`bpmn_code`)
) COMMENT '流程模板主表';
```

**重要字段说明**：

| 字段 | 说明 |
|------|------|
| `effective_status` | 流程模板设计完成后默认不生效，管理员手动激活 |
| `deduplication_type` | 控制审批人重复时的处理策略 |
| `conf_configJson` | JSON格式存储流程级配置（如通知渠道、去重策略等） |
| `is_out_side_process` | 标记是否为第三方系统接入的流程 |

### 3.2 bpmn_node（流程节点表）

存储流程模板中每个节点的配置信息。

```sql
CREATE TABLE `bpmn_node` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `conf_id` BIGINT NOT NULL COMMENT '所属流程配置ID',
    `node_id` VARCHAR(100) NOT NULL COMMENT '节点ID（BPMN元素ID）',
    `node_type` INT NOT NULL COMMENT '节点类型',
    `node_property` INT NOT NULL COMMENT '节点属性（审批人类型）',
    `node_from` VARCHAR(100) DEFAULT NULL COMMENT '来源节点ID',
    `batch_status` INT DEFAULT 0 COMMENT '批次状态',
    `approval_standard` INT DEFAULT NULL COMMENT '审批标准',
    `node_name` VARCHAR(200) DEFAULT NULL COMMENT '节点名称',
    `node_display_name` VARCHAR(200) DEFAULT NULL COMMENT '节点显示名称',
    `annotation` TEXT DEFAULT NULL COMMENT '节点注释',
    `is_deduplication` INT DEFAULT 0 COMMENT '是否去重',
    `is_sign_up` INT DEFAULT 0 COMMENT '是否加签',
    `remark` TEXT DEFAULT NULL COMMENT '备注',
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `create_time` DATETIME DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `node_froms` TEXT DEFAULT NULL COMMENT '来源节点ID列表（逗号分隔）',
    `is_dynamic_condition` TINYINT DEFAULT NULL COMMENT '是否动态条件',
    `is_parallel` TINYINT DEFAULT NULL COMMENT '是否并行',
    `is_out_side_process` INT DEFAULT NULL,
    `is_low_code_flow` INT DEFAULT NULL,
    `node_config_json` TEXT DEFAULT NULL COMMENT '节点级JSON配置',
    PRIMARY KEY (`id`),
    KEY `idx_conf_id` (`conf_id`)
) COMMENT '流程节点表';
```

**节点类型枚举（node_type）**：

| 值 | 类型 | 说明 |
|----|------|------|
| 1 | 审批节点 | 单人审批 |
| 2 | 会签节点 | 多人同时审批 |
| 3 | 条件节点 | 条件分支判断 |
| 4 | 普通节点 | 一般审批节点 |
| 5 | 开始节点 | 流程开始 |
| 6 | 结束节点 | 流程结束 |
| 7 | 抄送节点 | 抄送通知 |
| 8 | 并行分支 | 并行网关 |
| 9 | 自动节点 | 自动执行（设计时） |
| 12 | 条件审批节点 | 满足条件自动通过（设计时） |
| 13 | 条件抄送节点 | 满足条件才抄送（设计时） |

### 3.3 bpmn_node_to（节点连线关系表）

记录节点之间的连线关系，决定流程的流转方向。

```sql
CREATE TABLE `bpmn_node_to` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `bpmn_node_id` BIGINT NOT NULL COMMENT '节点ID',
    `node_to` VARCHAR(100) NOT NULL COMMENT '目标节点ID',
    `remark` TEXT DEFAULT NULL,
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `create_time` DATETIME DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    PRIMARY KEY (`id`),
    KEY `idx_bpmn_node_id` (`bpmn_node_id`)
) COMMENT '节点连线关系表';
```

### 3.4 bpm_business_process（业务流程实例表）

运行时流程实例的核心关联表，连接 Activiti 引擎和业务数据。

```sql
CREATE TABLE `bpm_business_process` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `processiness_key` VARCHAR(100) DEFAULT NULL COMMENT '流程Key',
    `business_id` VARCHAR(100) DEFAULT NULL COMMENT '业务ID',
    `business_number` VARCHAR(100) NOT NULL COMMENT '业务编号（流程号）',
    `entry_id` VARCHAR(100) DEFAULT NULL COMMENT '入口ID',
    `version` VARCHAR(20) DEFAULT NULL COMMENT '版本',
    `create_time` DATETIME DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `description` TEXT DEFAULT NULL,
    `process_state` INT DEFAULT NULL COMMENT '状态(1-已通过 2-审批中 3-已撤销)',
    `create_user` VARCHAR(50) DEFAULT NULL,
    `user_name` VARCHAR(100) DEFAULT NULL,
    `process_digest` VARCHAR(500) DEFAULT NULL COMMENT '流程摘要',
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `data_source_id` BIGINT DEFAULT NULL COMMENT '数据源ID',
    `proc_inst_id` VARCHAR(100) DEFAULT NULL COMMENT 'Activiti流程实例ID（关键关联字段）',
    `back_user_id` VARCHAR(50) DEFAULT NULL COMMENT '退回人ID',
    `is_out_side_process` INT DEFAULT 0,
    `is_low_code_flow` INT DEFAULT 0,
    PRIMARY KEY (`id`),
    KEY `idx_proc_inst_id` (`proc_inst_id`),
    KEY `idx_business_number` (`business_number`)
) COMMENT '业务流程实例表';
```

### 3.5 bpm_variable（流程变量表）

存储流程运行时的变量配置，包括元素映射、加签人员、会签配置等。

```sql
CREATE TABLE `bpm_variable` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `process_num` VARCHAR(100) NOT NULL COMMENT '流程编号',
    `process_name` VARCHAR(200) DEFAULT NULL COMMENT '流程名称',
    `process_desc` TEXT DEFAULT NULL COMMENT '流程描述',
    `process_start_conditions` TEXT DEFAULT NULL COMMENT '流程启动条件',
    `bpmn_code` VARCHAR(50) DEFAULT NULL COMMENT '流程编码',
    `remark` TEXT DEFAULT NULL,
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `create_time` DATETIME DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `variable_config_json` TEXT DEFAULT NULL COMMENT '运行时变量JSON配置',
    PRIMARY KEY (`id`),
    KEY `idx_process_num` (`process_num`)
) COMMENT '流程变量表';
```

`variable_config_json` 存储结构示例：

```json
{
  "signUps": [
    {
      "elementId": "task_001",
      "subElements": "[{\"elementId\":\"task_001_1\",\"isBackSignUp\":0}]",
      "afterSignUpWay": 1,
      "personnelByElement": {
        "task_001_1": [{"assignee": "user123", "assigneeName": "张三"}]
      }
    }
  ],
  "multiplayerNodes": [
    {
      "elementId": "task_002",
      "signType": 1
    }
  ]
}
```

### 3.6 bpm_verify_info（审批记录表）

记录每个节点的审批操作历史，是审批路径查询的数据来源。

```sql
CREATE TABLE `bpm_verify_info` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `run_info_id` VARCHAR(100) DEFAULT NULL COMMENT '流程实例ID',
    `verify_user_id` VARCHAR(50) DEFAULT NULL COMMENT '审批人ID',
    `verify_user_name` VARCHAR(100) DEFAULT NULL COMMENT '审批人姓名',
    `verify_status` INT DEFAULT NULL COMMENT '审批状态(1-提交 2-同意 3-不同意)',
    `verify_desc` TEXT DEFAULT NULL COMMENT '审批意见',
    `verify_date` DATETIME DEFAULT NULL COMMENT '审批日期',
    `task_name` VARCHAR(200) DEFAULT NULL COMMENT '任务名称',
    `task_id` VARCHAR(100) DEFAULT NULL COMMENT '任务ID',
    `task_def_key` VARCHAR(100) DEFAULT NULL COMMENT '任务定义Key',
    `business_type` INT DEFAULT NULL COMMENT '业务类型',
    `business_id` VARCHAR(100) DEFAULT NULL COMMENT '业务ID',
    `original_id` VARCHAR(50) DEFAULT NULL COMMENT '原始审批人ID',
    `process_code` VARCHAR(50) DEFAULT NULL COMMENT '流程编码',
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `attachments_json` TEXT DEFAULT NULL COMMENT '附件JSON',
    PRIMARY KEY (`id`),
    KEY `idx_run_info_id` (`run_info_id`),
    KEY `idx_process_code` (`process_code`)
) COMMENT '审批记录表';
```

### 3.7 bpm_process_forward（转发记录表）

记录流程转发（抄送）操作。

```sql
CREATE TABLE `bpm_process_forward` (
    `id` INT NOT NULL AUTO_INCREMENT,
    `forward_user_id` VARCHAR(50) DEFAULT NULL COMMENT '转发人ID',
    `forward_user_name` VARCHAR(100) DEFAULT NULL COMMENT '转发人姓名',
    `process_instance_id` VARCHAR(100) DEFAULT NULL COMMENT '流程实例ID',
    `create_time` DATETIME DEFAULT NULL,
    `create_user_id` VARCHAR(50) DEFAULT NULL,
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `is_read` INT DEFAULT 0 COMMENT '是否已读',
    `task_id` VARCHAR(100) DEFAULT NULL,
    `process_number` VARCHAR(100) DEFAULT NULL COMMENT '流程编号',
    `node_id` VARCHAR(100) DEFAULT NULL COMMENT '节点ID',
    PRIMARY KEY (`id`)
) COMMENT '转发记录表';
```

### 3.8 bpm_flowrun_entrust（委托/转交记录表）

记录流程的委托和转交操作。

```sql
CREATE TABLE `bpm_flowrun_entrust` (
    `id` INT NOT NULL AUTO_INCREMENT,
    `run_info_id` VARCHAR(100) DEFAULT NULL COMMENT '流程实例ID',
    `run_task_id` VARCHAR(100) DEFAULT NULL COMMENT '任务ID',
    `original` VARCHAR(50) DEFAULT NULL COMMENT '原始审批人',
    `original_name` VARCHAR(100) DEFAULT NULL,
    `actual` VARCHAR(50) DEFAULT NULL COMMENT '实际审批人',
    `actual_name` VARCHAR(100) DEFAULT NULL,
    `type` INT DEFAULT NULL COMMENT '类型(1-委托 2-转交)',
    `is_read` INT DEFAULT 0,
    `proc_def_id` VARCHAR(100) DEFAULT NULL,
    `is_view` INT DEFAULT 0,
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `node_id` VARCHAR(100) DEFAULT NULL,
    `action_type` INT DEFAULT NULL COMMENT '操作类型(0-全局委托 1-变更处理人 2-添加处理人 3-移除处理人)',
    PRIMARY KEY (`id`)
) COMMENT '委托/转交记录表';
```

### 3.9 bpm_dynamic_condition_choosen（动态条件选择记录表）

记录流程实例在条件网关中选择的分支，用于重新提交时检测条件变化。

```sql
CREATE TABLE `bpm_dynamic_condition_choosen` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `process_number` VARCHAR(100) NOT NULL COMMENT '流程编号',
    `node_id` VARCHAR(100) NOT NULL COMMENT '选择的条件节点ID',
    `node_from` VARCHAR(100) DEFAULT NULL COMMENT '网关节点ID',
    PRIMARY KEY (`id`),
    KEY `idx_process_number` (`process_number`)
) COMMENT '动态条件选择记录表';
```

### 3.10 user_message（用户消息表）

存储流程通知消息。

```sql
CREATE TABLE `user_message` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `user_id` VARCHAR(50) DEFAULT NULL COMMENT '用户ID',
    `title` VARCHAR(200) DEFAULT NULL COMMENT '标题',
    `content` TEXT DEFAULT NULL COMMENT '内容',
    `url` VARCHAR(500) DEFAULT NULL COMMENT '跳转URL',
    `node` VARCHAR(100) DEFAULT NULL COMMENT '节点ID',
    `params` TEXT DEFAULT NULL COMMENT '参数',
    `url_params` TEXT DEFAULT NULL COMMENT 'URL参数JSON',
    `is_read` TINYINT DEFAULT 0 COMMENT '是否已读',
    `is_del` TINYINT DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `create_time` DATETIME DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `app_url` VARCHAR(500) DEFAULT NULL COMMENT 'App端URL',
    `source` INT DEFAULT NULL COMMENT '消息来源',
    PRIMARY KEY (`id`),
    KEY `idx_user_id` (`user_id`)
) COMMENT '用户消息表';
```

### 3.11 information_template（通知模板表）

定义不同事件类型的通知模板，支持系统消息、邮件、短信三种渠道。

```sql
CREATE TABLE `information_template` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `name` VARCHAR(200) DEFAULT NULL COMMENT '模板名称',
    `num` VARCHAR(100) DEFAULT NULL COMMENT '模板编号',
    `system_title` VARCHAR(200) DEFAULT NULL COMMENT '系统消息标题',
    `system_content` TEXT DEFAULT NULL COMMENT '系统消息内容',
    `mail_title` VARCHAR(200) DEFAULT NULL COMMENT '邮件标题',
    `mail_content` TEXT DEFAULT NULL COMMENT '邮件内容',
    `note_content` TEXT DEFAULT NULL COMMENT '短信内容',
    `jump_url` INT DEFAULT NULL COMMENT '跳转URL类型(1-审批页 2-详情页 3-待办列表)',
    `remark` TEXT DEFAULT NULL,
    `status` INT DEFAULT 0 COMMENT '状态(0-启用 1-禁用)',
    `evt` INT DEFAULT NULL COMMENT '事件类型',
    `event_name` VARCHAR(100) DEFAULT NULL,
    `is_del` INT NOT NULL DEFAULT 0,
    `tenant_id` VARCHAR(50) DEFAULT NULL,
    `is_default` INT DEFAULT NULL COMMENT '是否默认模板',
    `create_time` DATETIME DEFAULT NULL,
    `create_user` VARCHAR(50) DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    PRIMARY KEY (`id`)
) COMMENT '通知模板表';
```

### 3.12 bpm_process_app_application（应用URL配置表）

管理流程关联的应用URL配置（查看URL、提交URL、条件URL）。

```sql
CREATE TABLE `bpm_process_app_application` (
    `id` INT DEFAULT NULL,
    `business_code` VARCHAR(100) DEFAULT NULL COMMENT '业务编码',
    `title` VARCHAR(200) DEFAULT NULL COMMENT '应用名称',
    `apply_type` INT DEFAULT NULL COMMENT '应用类型(1-流程 2-App 3-父应用)',
    `pc_icon` VARCHAR(500) DEFAULT NULL COMMENT 'PC端图标',
    `effective_source` VARCHAR(500) DEFAULT NULL COMMENT '生效来源',
    `is_son` INT DEFAULT 0 COMMENT '是否子应用',
    `look_url` TEXT DEFAULT NULL COMMENT '查看URL',
    `submit_url` TEXT DEFAULT NULL COMMENT '提交URL',
    `condition_url` TEXT DEFAULT NULL COMMENT '条件URL',
    `parent_id` INT DEFAULT 0 COMMENT '父应用ID',
    `application_url` VARCHAR(500) DEFAULT NULL,
    `route` VARCHAR(500) DEFAULT NULL COMMENT '路由',
    `process_key` VARCHAR(100) DEFAULT NULL COMMENT '流程Key',
    `permissions_code` VARCHAR(100) DEFAULT NULL COMMENT '权限码',
    `is_del` INT NOT NULL DEFAULT 0,
    `create_user_id` VARCHAR(50) DEFAULT NULL,
    `create_time` DATETIME DEFAULT NULL,
    `update_user` VARCHAR(50) DEFAULT NULL,
    `update_time` DATETIME DEFAULT NULL,
    `is_all` INT DEFAULT 0,
    `state` INT DEFAULT 0,
    `sort` INT DEFAULT 0,
    `source` VARCHAR(100) DEFAULT NULL,
    `user_request_uri` VARCHAR(500) DEFAULT NULL,
    `role_request_uri` VARCHAR(500) DEFAULT NULL,
    `category_config_json` TEXT DEFAULT NULL,
    PRIMARY KEY (`id`)
) COMMENT '应用URL配置表';
```

### 3.13 Activiti 引擎表

| 表名 | 说明 |
|------|------|
| `bpm_af_deployment` | 流程部署信息 |
| `bpm_af_task` | Activiti 任务记录 |
| `bpm_af_task_inst` | 任务实例（扩展字段） |
| `bpm_af_execution` | 执行实例（流程运行栈） |

## 4. ER 关系图

```
                    ┌──────────────────┐
                    │   bpmn_conf      │
                    │   (流程模板)      │
                    └────────┬─────────┘
                             │ 1:N
                    ┌────────┴─────────┐
                    │   bpmn_node      │
                    │   (流程节点)      │
                    └───┬─────────┬────┘
                        │         │
                   1:N  │         │  1:N
              ┌─────────┴──┐  ┌──┴───────────┐
              │bpmn_node_to│  │bpmn_conf_lf_*│
              │(连线关系)   │  │(低代码表单)   │
              └────────────┘  └──────────────┘

┌────────────────────┐        ┌──────────────────────────┐
│  bpm_business_     │ 1:1    │  bpm_variable            │
│  process           │───────▶│  (流程变量/加签/会签)     │
│  (业务实例)        │        └──────────────────────────┘
└────────┬───────────┘
         │ 1:N
┌────────┴───────────┐        ┌──────────────────────────┐
│  bpm_verify_info   │        │  bpm_process_forward     │
│  (审批记录)        │        │  (转发记录)              │
└────────────────────┘        └──────────────────────────┘

┌────────────────────┐        ┌──────────────────────────┐
│  bpm_flowrun_      │        │  user_message            │
│  entrust           │        │  (通知消息)              │
│  (委托/转交)       │        └──────────────────────────┘
└────────────────────┘
```

## 5. 索引设计建议

| 表名 | 索引 | 用途 |
|------|------|------|
| `bpmn_conf` | `bpmn_code` (唯一) | 流程编码精确查找 |
| `bpmn_node` | `conf_id` | 按流程查询节点 |
| `bpm_business_process` | `proc_inst_id` | Activiti 关联查询 |
| `bpm_business_process` | `business_number` | 业务编号查询 |
| `bpm_variable` | `process_num` | 流程变量查询 |
| `bpm_verify_info` | `run_info_id` | 审批路径查询 |
| `user_message` | `user_id` | 用户消息查询 |

## 6. JSON 配置字段说明

AntFlowCore 大量使用 JSON 字段存储结构化配置，避免频繁的表结构变更：

| 表名 | JSON 字段 | 内容 |
|------|-----------|------|
| `bpmn_conf` | `conf_config_json` | 流程级配置（通知渠道、去重策略等） |
| `bpmn_node` | `node_config_json` | 节点级配置（条件配置、自动节点配置等） |
| `bpm_variable` | `variable_config_json` | 运行时变量（加签人员、会签配置、元素映射） |
| `bpm_verify_info` | `attachments_json` | 审批附件列表 |
| `bpm_process_app_application` | `category_config_json` | 应用分类配置 |

## 7. 多租户支持

所有核心表均包含 `tenant_id` 字段，支持多租户数据隔离。FreeSQL 的全局过滤器可自动在查询中附加租户条件：

```csharp
// FreeSQL 多租户过滤器配置
fsql.SetDbContextModelFilter<BpmnConf>(x => x.TenantId == currentTenantId);
```
