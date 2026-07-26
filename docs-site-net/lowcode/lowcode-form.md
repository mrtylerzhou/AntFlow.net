# 表单设计详解

## 概述

AntFlowCore 低代码表单引擎基于 JSON Schema 设计，支持通过拖拽设计器（vform）或手动编辑 JSON 来创建表单。表单数据存储在 `BpmnConfLfFormdata` 实体中，以 `formCode` 作为唯一标识进行关联。

## 表单数据结构

### 核心存储模型

```csharp
// AntFlowCore.Base/entity/BpmnConfLfFormdata.cs
public class BpmnConfLfFormdata
{
    public long Id { get; set; }              // 主键ID
    public long BpmnConfId { get; set; }      // 关联的流程配置ID
    public string Formdata { get; set; }      // 表单JSON数据
    public int IsDel { get; set; }            // 删除标记
    public string TenantId { get; set; }      // 租户ID（多租户支持）
    public string CreateUser { get; set; }    // 创建人
    public DateTime? CreateTime { get; set; } // 创建时间
    public string UpdateUser { get; set; }    // 更新人
    public DateTime? UpdateTime { get; set; } // 更新时间
}
```

### Formdata JSON 格式示例

```json
{
  "widgetList": [
    {
      "key": "leave_type",
      "type": "select",
      "icon": "select-field",
      "formItem": {
        "label": "请假类型",
        "required": true
      },
      "options": {
        "optionItems": [
          { "label": "年假", "value": "annual" },
          { "label": "事假", "value": "personal" },
          { "label": "病假", "value": "sick" }
        ]
      }
    },
    {
      "key": "start_date",
      "type": "date",
      "icon": "date-field",
      formItem: {
        "label": "开始日期",
        "required": true
      },
      "options": {
        "format": "yyyy-MM-dd"
      }
    },
    {
      "key": "days",
      "type": "number",
      "icon": "number-field",
      "formItem": {
        "label": "请假天数",
        "required": true
      },
      "options": {
        "min": 0.5,
        "max": 30,
        "step": 0.5
      }
    },
    {
      "key": "reason",
      "type": "textarea",
      "icon": "textarea-field",
      "formItem": {
        "label": "请假事由",
        "required": true
      },
      "options": {
        "rows": 4,
        "maxlength": 500
      }
    }
  ],
  "formConfig": {
    "labelWidth": 120,
    "labelPosition": "right",
    "size": "default"
  }
}
```

## 字段类型详解

字段类型由 `LFFieldTypeEnum` 枚举定义：

```csharp
// AntFlowCore.Base/constant/enums/LFFieldTypeEnum.cs
public class LFFieldTypeEnum : EnumBase<LFFieldTypeEnum>
{
    public static readonly LFFieldTypeEnum STRING    = new(1, "字符串");
    public static readonly LFFieldTypeEnum NUMBER    = new(2, "数字");
    public static readonly LFFieldTypeEnum DATE      = new(3, "日期");
    public static readonly LFFieldTypeEnum DATE_TIME = new(4, "日期时间");
    public static readonly LFFieldTypeEnum TEXT      = new(5, "长字符串");
    public static readonly LFFieldTypeEnum BOOLEAN   = new(6, "布尔");
    public static readonly LFFieldTypeEnum BLOB      = new(7, "二进制");
}
```

### 字段类型使用指南

| 类型 | 编码 | 前端控件 | 典型用途 | 校验规则示例 |
|------|------|---------|---------|-------------|
| STRING | 1 | 单行文本框 | 姓名、标题、工号 | `maxlength: 50`, `pattern: "^[\\u4e00-\\u9fa5]+$"` |
| NUMBER | 2 | 数字输入框 | 金额、数量、天数 | `min: 0`, `max: 999999`, `step: 0.01` |
| DATE | 3 | 日期选择器 | 开始日期、结束日期 | `format: "yyyy-MM-dd"`, 范围限制 |
| DATE_TIME | 4 | 日期时间选择器 | 预约时间、截止时间 | `format: "yyyy-MM-dd HH:mm:ss"` |
| TEXT | 5 | 多行文本框 | 备注、说明、意见 | `maxlength: 2000`, `rows: 6` |
| BOOLEAN | 6 | 开关/复选框 | 是否同意、是否加急 | `trueValue: true`, `falseValue: false` |
| BLOB | 7 | 文件上传 | 附件、图片、文档 | `accept: ".pdf,.doc,.docx"`, `maxSize: 10MB` |

## 表单控件类型

除了字段类型，表单还支持以下控件类型（`LFControlTypeEnum`）：

```csharp
public class LFControlTypeEnum
{
    public static readonly LFControlTypeEnum SELECT = new(1, "select", "下拉框");
}
```

> **注意**：当前版本仅内置了下拉框控件，更多控件类型通过 vform 设计器的扩展机制实现。

## 表单管理页面

通过表单管理界面可以创建、编辑和预览低代码表单：

![表单管理](/images/form-management.png)

## API 接口

### 创建表单码

```http
POST /lowcode/createLowCodeFormCode
Content-Type: application/json

{
  "key": "expense_form",
  "value": "费用报销单"
}
```

**响应示例**：
```json
{
  "code": 0,
  "msg": "success",
  "data": 1001
}
```

### 获取所有表单码

```http
GET /lowcode/getLowCodeFlowFormCodes
```

**响应示例**：
```json
{
  "code": 0,
  "msg": "success",
  "data": [
    { "key": "leave_form", "value": "请假申请表" },
    { "key": "expense_form", "value": "费用报销单" },
    { "key": "travel_form", "value": "出差申请表" }
  ]
}
```

### 分页获取表单码列表

```http
POST /lowcode/getLFFormCodePageList
Content-Type: application/json

{
  "pageDto": {
    "pageNo": 1,
    "pageSize": 10
  },
  "taskMgmtVO": {
    "name": "请假"
  }
}
```

### 获取表单数据

```http
GET /lowcode/getformDataByFormCode?formCode=leave_form
```

**响应示例**：
```json
{
  "code": 0,
  "msg": "success",
  "data": "{\"widgetList\":[...],\"formConfig\":{...}}"
}
```

> **注意**：返回的 `data` 字段是 JSON 字符串，前端需要自行解析。

## 表单权限控制

低代码表单支持字段级权限控制，每个字段可设置三种状态：

| 标记 | 含义 | 说明 |
|------|------|------|
| R | ReadOnly | 只读，不可编辑 |
| E | Editable | 可编辑，正常状态 |
| H | Hidden | 隐藏，不可见 |

权限配置在流程节点的表单属性中设置，不同审批节点可以有不同的字段权限组合。

## 表单与流程的关联

低代码表单通过 `formCode` 与流程模板（BpmnConf）关联：

```
┌──────────────┐     formCode      ┌──────────────────┐
│  DictData    │ ─────────────────▶│  BpmnConf        │
│  (字典表)     │                   │  (流程配置表)     │
│  key=formCode│                   │  formCode字段关联  │
└──────────────┘                   └────────┬─────────┘
                                            │ 1:N
                                            ▼
                                   ┌──────────────────┐
                                   │ BpmnConfLf       │
                                   │ Formdata         │
                                   │ (表单JSON数据)    │
                                   └──────────────────┘
```

## 最佳实践

### 1. 表单码命名规范

```
{业务域}_{功能}_form

示例：
- hr_leave_form     (人事-请假表单)
- finance_expense_form (财务-报销表单)
- it_device_form    (IT-设备申请)
```

### 2. 字段设计原则

- **精简原则**：只保留审批必要的字段，避免冗余
- **命名一致**：相同含义的字段在不同表单中使用相同的 key
- **类型匹配**：数字类型用于可参与条件判断的字段（如金额、天数）
- **默认值**：为常用字段设置合理的默认值

### 3. 表单版本管理

当表单结构需要变更时：
1. 不要直接修改已有表单的 JSON
2. 创建新版本的表单数据
3. 通过流程版本迁移机制切换

### 4. 性能考虑

- 单个表单字段数量建议不超过 50 个
- 避免嵌套层级超过 3 层的复杂布局
- 大文本字段（TEXT/BLOB）单独存储，避免影响列表查询性能

## 常见问题

**Q: 表单数据如何参与条件判断？**

A: 表单中的 NUMBER 和 STRING 类型字段可以直接作为条件变量。在流程条件规则中引用 `formCode.fieldKey` 即可获取字段值进行判断。

**Q: 低代码表单支持子表单/表格吗？**

A: 通过 vform 设计器的表格组件可以实现子表单效果，数据以 JSON 数组形式存储在同一个 Formdata 中。

**Q: 如何获取表单中的指定字段值？**

A: 通过 `getformDataByFormCode` 接口获取完整 JSON 后，前端解析并提取对应字段。后端条件引擎会自动解析表单 JSON 中的字段值。
