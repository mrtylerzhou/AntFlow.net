# AntFlowCore.NET 已删除/可删除表清单

> 对齐 Java 版：`D:\projects\jimuoffice\doc\tables_to_drop.md`。
> .NET 版已完成全部 JSON-first 迁移，旧表对应的 Entity/Repository/Service 文件已全部删除，SQL 初始化脚本中 DDL 已替换为 REMOVED 注释。

---

## 当前 JSON 承载字段

| 主表 | 字段 | 用途 | 状态 |
|---|---|---|---|
| `t_bpmn_conf` | `conf_config_json` | 流程级配置：查看页按钮、通知模板、通知渠道、低代码表单 | ✅ 已实现 |
| `t_bpmn_node` | `node_config_json` | 节点级配置：审批人属性、条件、按钮、签收、模板、催办、低代码字段权限、回退类型、操作类型 | ✅ 已实现 |
| `t_bpm_variable` | `variable_config_json` | 运行时变量配置：按钮、消息、报名节点、催办 | ✅ 已实现 |
| `bpm_verify_info` | `attachments_json` | 审批记录附件 | ✅ 已实现 |
| `t_quick_entry` | `type_config_json` | 快捷入口类型配置 | ✅ 已实现 |
| `t_information_template` | `is_default` | 默认模板标记 | ✅ 已实现 |
| `bpm_process_app_application` | `category_config_json` | 应用分类关联配置 | ✅ 已实现 |

---

## 一、BPMN 配置子表（26 张 → JSON 合并）

原 26 张 `t_bpmn_*` 配置子表已合并为 JSON 字段存储在两张主表上：
- `t_bpmn_conf.conf_config_json` — 流程级配置
- `t_bpmn_node.node_config_json` — 节点级配置

| # | 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|---|------|------|---------|------------------------|
| 1 | `t_bpmn_view_page_button` | 流程级视图页按钮 | `conf_config_json` → `viewPageButtons[]` | ✅ 已删除 |
| 2 | `t_bpmn_conf_notice_template` | 通知模板头 | `conf_config_json` → `noticeTemplateConfig` | ✅ 已删除 |
| 3 | `t_bpmn_conf_notice_template_detail` | 通知模板明细 | `conf_config_json` → `noticeTemplateConfig.details[]` | ✅ 已删除 |
| 4 | `t_bpmn_node_button_conf` | 节点按钮配置 | `node_config_json` → `buttonSignConf.buttonConfList[]` | ✅ 已删除 |
| 5 | `t_bpmn_node_sign_up_conf` | 节点签收配置 | `node_config_json` → `buttonSignConf.signUpConf` | ✅ 已删除 |
| 6 | `t_bpmn_node_labels` | 节点标签 | `node_config_json` → `buttonSignConf.labels[]` | ✅ .NET 中不存在此实体 |
| 7 | `t_bpmn_node_additional_sign_conf` | 额外加签审批人 | `node_config_json` → `buttonSignConf.additionalSignConfList[]` | ✅ 已删除 |
| 8 | `t_bpmn_node_personnel_conf` | 指定人员审批（主表） | `node_config_json` → `approverConf.personnelConf` | ✅ 已删除 |
| 9 | `t_bpmn_node_personnel_empl_conf` | 指定人员审批（人员明细） | `node_config_json` → `approverConf.personnelConf.employees[]` | ✅ 已删除 |
| 10 | `t_bpmn_node_role_conf` | 指定角色审批 | `node_config_json` → `approverConf.roleConfList[]` | ✅ 已删除 |
| 11 | `t_bpmn_node_role_outside_emp_conf` | 角色外部人员 | `node_config_json` → `approverConf.roleConfList[].outsideEmployees[]` | ✅ 已删除 |
| 12 | `t_bpmn_node_loop_conf` | 层层审批配置 | `node_config_json` → `approverConf.loopConf` | ✅ 已删除 |
| 13 | `t_bpmn_node_assign_level_conf` | 指定层级审批 | `node_config_json` → `approverConf.assignLevelConf` | ✅ 已删除 |
| 14 | `t_bpmn_node_hrbp_conf` | HRBP 审批配置 | `node_config_json` → `approverConf.hrbpConf` | ✅ 已删除 |
| 15 | `t_bpmn_node_customize_conf` | 自选审批人配置 | `node_config_json` → `approverConf.customizeConf` | ✅ 已删除 |
| 16 | `t_bpmn_node_udr_conf` | 自定义规则审批 | `node_config_json` → `approverConf.udrConfList[]` | ✅ .NET 中不存在此实体 |
| 17 | `t_bpmn_node_form_related_user_conf` | 表单关联用户审批 | `node_config_json` → `approverConf.formRelatedUserConfList[]` | ✅ .NET 中不存在此实体 |
| 18 | `t_bpmn_node_out_side_access_conf` | 外部接入审批 | `node_config_json` → `approverConf.outSideAccessConf` | ✅ 已删除 |
| 19 | `t_bpmn_node_business_table_conf` | 关联业务表审批 | `node_config_json` → `approverConf.businessTableConf` | ✅ 已删除 |
| 20 | `t_bpmn_node_conditions_conf` | 条件节点配置 | `node_config_json` → `conditionsConf.conditionGroups[].extJson` | ✅ 已删除 |
| 21 | `t_bpmn_node_conditions_param_conf` | 条件参数配置 | `node_config_json` → `conditionsConf.conditionGroups[].extJson` | ✅ 已删除 |
| 22 | `t_out_side_bpmn_node_conditions_conf` | 外部条件配置 | `node_config_json` → `conditionsConf.outSideConditionId` | ✅ 已删除 |
| 23 | `t_bpmn_template`（node_id 非空） | 节点级通知模板 | `node_config_json` → `templateConf.templates[]` | ✅ 已删除（保留 BpmnTemplateVo） |
| 24 | `t_bpmn_approve_remind` | 审批催办配置 | `node_config_json` → `templateConf.approveRemind` | ✅ 已删除（保留 BpmnApproveRemindVo） |
| 25 | `t_bpmn_node_lf_formdata_field_control` | 低代码字段权限 | `node_config_json` → `lowCodeConf.fieldControls[]` | ✅ 已删除 |
| 26 | `t_bpmn_template`（node_id 为空） | 流程级通知模板 | `conf_config_json` → `confTemplates[]` | ✅ 已删除 |

**代码变更：**
- 9 个 Adaptor（Personnel/Role/Loop/Level/Hrbp/Customize/BusinessTable/OutSideAccess/AdditionalSign）改为从 JSON 读取，移除旧表依赖
- `BpmnConfBizService` edit 流程改为调用 `BpmnNodeConfigHolder.SetXxxConf()` 序列化到 JSON
- `BpmnConfBizService` detail 流程移除 DB 回退路径，无 JSON 时抛异常

---

## 二、流程变量子表（6 张 → variable_config_json）

| # | 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|---|------|------|---------|------------------------|
| 1 | `t_bpm_variable_button` | 流程变量按钮配置 | `variable_config_json` → `buttons[]` | ✅ 已删除 |
| 2 | `t_bpm_variable_view_page_button` | 流程变量查看页按钮 | `variable_config_json` → `buttons[].viewType` | ✅ 已删除 |
| 3 | `t_bpm_variable_message` | 流程变量消息配置 | `variable_config_json` → `messages[]` | ✅ 已删除 |
| 4 | `t_bpm_variable_approve_remind` | 流程变量催办配置 | `variable_config_json` → `approveReminds[]` | ✅ 已删除 |
| 5 | `t_bpm_variable_sign_up` | 流程变量报名节点 | `variable_config_json` → `signUps[]` | ✅ 已删除 |
| 6 | `t_bpm_variable_sign_up_personnel` | 报名人员明细 | `variable_config_json` → `signUps[].personnelByElement` | ✅ 已删除 |

**代码变更：**
- `BpmnInsertVariablesService` 移除所有旧表写路径，改为只写 `variable_config_json`
- `ConfigFlowButtonContantService` 改为从 JSON 解析按钮
- `BpmVerifyInfoBizService` 改为从 JSON 解析报名信息

---

## 三、通知配置合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_process_notice` | 流程通知渠道配置 | `t_bpmn_conf.conf_config_json` → `noticeChannelTypes: List<Integer>` | ✅ 已删除 |

**代码变更：**
- `ActivitiBpmMsgTemplateService` 从 `conf_config_json` 解析 `noticeChannelTypes`
- `BpmProcessControlController.saveProcessNotices` 端点标记 DEPRECATED
- 移除 `BaseKeyValueStruVo.ProcessNotices`、`DIYProcessInfoDTO.ProcessNotices` 死代码

---

## 四、催办配置合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_process_node_overtime` | 节点超时催办配置 | `t_bpmn_node.node_config_json` → `templateConf.overtimeConf.{noticeTime, noticeTypes}` | ✅ 已删除 |

---

## 五、操作配置合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_process_operation` | 节点操作类型配置 | `t_bpmn_node.node_config_json` → `buttonSignConf.operationTypes: List<Integer>` | ✅ 已删除 |

---

## 六、回退配置合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_process_node_back` | 节点回退类型配置 | `t_bpmn_node.node_config_json` → `backType: Integer` | ✅ 已删除 |

---

## 七、审批附件合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_verify_attachment` | 审批记录附件 | `bpm_verify_info` 新增 `attachments_json TEXT` 列 | ✅ 已删除 |

**Schema 变更：**
- `bpm_verify_info` 新增 `attachments_json TEXT` 列，存储 `List<BpmVerifyAttachmentVo>` 的 JSON

**新增文件：**
- `BpmVerifyAttachmentVo.cs` — JSON 序列化 VO

---

## 八、快捷入口类型合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `t_quick_entry_type` | 快捷入口类型（PC/APP） | `t_quick_entry` 新增 `type_config_json VARCHAR(500)` 列 | ✅ 已删除 |

**Schema 变更：**
- `t_quick_entry` 新增 `type_config_json` 列

---

## 九、流程名称表合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `bpm_process_name` | 流程名称/搜索表（与 `t_bpmn_conf` 冗余） | `t_bpmn_conf.form_code` / `bpmn_name` 直接查询 | ✅ 已删除 |
| `bpm_process_name_relevancy` | 流程名称关联表（已废弃） | 无 | ✅ 已删除 |

---

## 十、默认模板表合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `t_default_template` | 事件→默认模板映射表 | `t_information_template` 新增 `is_default` 列 | ✅ 已删除 |

**Schema 变更：**
- `t_information_template` 新增 `is_default TINYINT DEFAULT 0` 列

**删除的文件：**
- `DefaultTemplateVo.cs`

---

## 十一、邮件发送表合并

| 表名 | 说明 | 替代方案 | Entity/Repo/Service 删除 |
|------|------|---------|------------------------|
| `t_user_email_send` | 邮件发送记录（写表，仅做计数） | `t_op_log` 新增 `log_type=1` + `receiver` 列 | ✅ 已删除 |

---

## 十二、死表 / 废弃表删除

| 表名 | 说明 | Entity/Repo/Service 删除 |
|------|------|------------------------|
| `t_bpm_business` | 业务流程表（死表，无读写） | ✅ 已删除 |
| `t_bpm_variable_single` | 单人审批变量表（已废弃） | ✅ 已删除 |
| `t_bpm_variable_sequence_flow` | 流程变量顺序流（仅写入从未读取） | ✅ 已删除 |
| `t_dict_main` | 字典主表（死表，无读写） | ✅ 已删除 |
| `t_bpm_process_audit` | 流程表单变更审计（写表，零读取者） | ✅ .NET 中不存在此实体 |
| `bpm_flowruninfo` | 流程运行时信息（死表，零调用者） | ✅ 已删除 |
| `bpm_manual_notify` | 手动催办记录（死表，零调用者） | ✅ 已删除 |
| `bpm_process_node_record` | 节点超时记录（死表，零调用者） | ✅ 已删除 |
| `bpm_process_dept` | 流程部门配置（已废弃） | ✅ 已删除 |

---

## 十三、应用分类关联表 JSON 化

| 表名 | 说明 | 替代方案 | 状态 |
|------|------|---------|------|
| `bpm_process_application_type` | 应用与分类的多对多关联表 | `bpm_process_app_application.category_config_json` | ✅ JSON-first 已实现，Entity/Vo 已删除 |

**Schema 变更：**
- `bpm_process_app_application` 新增 `category_config_json TEXT` 列

**新增文件：**
- `AppCategoryConfigJson.cs` — JSON 结构定义类

---

## 保留的表

| 表名 | 说明 | 新增字段 |
|------|------|---------|
| `t_bpmn_conf` | 流程配置主表 | `conf_config_json` TEXT |
| `t_bpmn_node` | 节点主表 | `node_config_json` TEXT |
| `t_bpmn_node_to` | 节点流转关系 | — |
| `t_bpm_variable` | 流程变量主表 | `variable_config_json` TEXT |
| `t_bpm_variable_multiplayer` | 多人审批变量 | — |
| `t_bpm_variable_multiplayer_personnel` | 多人审批人员 | — |
| `bpm_verify_info` | 审批记录 | `attachments_json` TEXT |
| `t_quick_entry` | 快捷入口 | `type_config_json` VARCHAR(500) |
| `t_information_template` | 通知模板 | `is_default` TINYINT |
| `t_op_log` | 操作/邮件日志 | `log_type` TINYINT, `receiver` VARCHAR(255) |
| `bpm_process_app_application` | 应用主表 | `category_config_json` TEXT |

---

## 迁移路线总结

```
第一批（t_bpmn_* 配置子表 → JSON）: 26 张表
  └── Entity/Repository/Service 全部删除，Adaptor 改为 JSON 读取

第二批（流程变量子表 → JSON）: 6 张表
  └── BpmnInsertVariablesService 改为只写 variable_config_json

第三批（通知/催办/操作/回退配置合并）: 4 张表
  ├── bpm_process_notice → conf_config_json.noticeChannelTypes
  ├── bpm_process_node_overtime → node_config_json.templateConf.overtimeConf
  ├── bpm_process_operation → node_config_json.buttonSignConf.operationTypes
  └── bpm_process_node_back → node_config_json.backType

第四批（子表合并/JSON 化）: 5 张表
  ├── bpm_verify_attachment → bpm_verify_info.attachments_json
  ├── t_quick_entry_type → t_quick_entry.type_config_json
  ├── t_default_template → t_information_template.is_default
  ├── t_user_email_send → t_op_log
  └── bpm_process_application_type → category_config_json

第五批（死表/废弃表）: 9 张表
  ├── t_bpm_business, t_bpm_variable_single, t_bpm_variable_sequence_flow
  ├── t_dict_main, t_bpm_process_audit, bpm_flowruninfo
  ├── bpm_manual_notify, bpm_process_node_record, bpm_process_dept
  └── bpm_process_name, bpm_process_name_relevancy

合计: 50 张表已删除/合并
```

---

## 注意事项

1. **数据迁移**：合并表需要执行数据迁移脚本，将子表数据回填到父表的 JSON 字段
2. **SQL 脚本**：已删除表的 DDL 在 `bpm_init_db_mysql.sql` 和 `bpm_init_db_sqlserver.sql` 中已替换为 `-- REMOVED` 注释
3. **验证步骤**：
   - `dotnet build` 编译通过
   - 确认无残留引用
4. **生产环境建议**：先在测试环境验证，确认无误后再在生产环境执行
