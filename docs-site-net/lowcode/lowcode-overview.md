# 低代码概述

## 什么是低代码流程

AntFlowCore 的低代码流程引擎允许非技术人员通过可视化拖拽方式设计和部署审批流程，无需编写代码即可实现企业级工作流管理。低代码流程（Low Code Flow）是 AntFlowCore 的核心设计理念之一，它将流程设计、表单搭建、审批规则配置全部抽象为可配置的 JSON 数据结构。

## 核心架构

低代码模块主要由以下组件构成：

```
┌─────────────────────────────────────────────────────┐
│                   前端设计器                          │
│  (Vue 3 + vform 表单设计器 + 流程节点拖拽画布)        │
└─────────────────┬───────────────────────────────────┘
                  │ JSON 配置数据
                  ▼
┌─────────────────────────────────────────────────────┐
│              AntFlowCore API 层                      │
│  ┌─────────────────┐  ┌─────────────────────────┐   │
│  │LowCodeFlowController│  │ProcessControlController│   │
│  │  表单代码管理      │  │  流程配置管理           │   │
│  └─────────────────┘  └─────────────────────────┘   │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              领域服务层                               │
│  ┌─────────────────┐  ┌─────────────────────────┐   │
│  │DictService       │  │BpmnConfLFFormDataBiz    │   │
│  │  字典/表单码服务   │  │  表单数据业务服务        │   │
│  └─────────────────┘  └─────────────────────────┘   │
└─────────────────┬───────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────┐
│              持久化层 (FreeSql ORM)                   │
│  ┌─────────────────┐  ┌─────────────────────────┐   │
│  │  BpmnConfLf     │  │  DictData               │   │
│  │  Formdata 表     │  │  字典数据表              │   │
│  └─────────────────┘  └─────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

## 关键实体

### BpmnConfLfFormdata（低代码表单数据表）

低代码流程的表单定义以 JSON 形式存储在 `BpmnConfLfFormdata` 实体中：

```csharp
public class BpmnConfLfFormdata
{
    public long Id { get; set; }              // 主键ID
    public long BpmnConfId { get; set; }      // 关联的流程配置ID
    public string Formdata { get; set; }      // 表单JSON数据
    public int IsDel { get; set; }            // 删除标记(0=正常,1=删除)
    public string TenantId { get; set; }      // 租户ID
    public string CreateUser { get; set; }    // 创建人
    public DateTime? CreateTime { get; set; } // 创建时间
    public string UpdateUser { get; set; }    // 更新人
    public DateTime? UpdateTime { get; set; } // 更新时间
}
```

### Formdata JSON 结构

`Formdata` 字段存储的是 vform 设计器生成的 JSON，描述了表单的布局、字段类型、校验规则等信息。每种字段类型由 `LFFieldTypeEnum` 枚举定义。

## 字段类型

AntFlowCore 低代码表单支持以下字段类型：

| 类型编码 | 枚举名称 | 说明 | 对应数据库类型 |
|---------|---------|------|--------------|
| 1 | STRING | 字符串 | VARCHAR |
| 2 | NUMBER | 数字 | DECIMAL/INT |
| 3 | DATE | 日期 | DATE |
| 4 | DATE_TIME | 日期时间 | DATETIME |
| 5 | TEXT | 长字符串 | TEXT/NVARCHAR(MAX) |
| 6 | BOOLEAN | 布尔 | BIT |
| 7 | BLOB | 二进制 | VARBINARY |

## 低代码流程生命周期

```
创建表单码 → 设计表单 → 配置流程节点 → 关联表单与流程 → 测试发布 → 正式生效
```

### 1. 创建表单码（FormCode）

每个低代码表单都需要一个唯一的表单码作为标识：

```http
POST /lowcode/createLowCodeFormCode
Content-Type: application/json

{
  "key": "leave_form",
  "value": "请假申请表"
}
```

### 2. 设计表单

使用 vform 设计器拖拽生成表单 JSON，然后通过 FormCode 关联存储：

![表单设计界面](/images/form-management.png)

```http
GET /lowcode/getformDataByFormCode?formCode=leave_form
```

### 3. 查看已有表单码

```http
GET /lowcode/getLowCodeFlowFormCodes
```

## 低代码 vs DIY 对比

| 特性 | 低代码流程 | DIY 自定义流程 |
|------|-----------|---------------|
| 目标用户 | 非技术人员/业务人员 | 开发人员 |
| 表单搭建 | 拖拽设计器生成 | 前端硬编码或模板生成 |
| 流程配置 | 可视化流程画布 | 代码定义或配置文件 |
| 扩展性 | 受限于设计器能力 | 完全自由定制 |
| 维护成本 | 低 | 中-高 |
| 上线速度 | 极快 | 需要开发周期 |

## 适用场景

### 适合低代码的场景
- 企业日常审批流程（请假、报销、出差等）
- 表单字段变化频繁的业务
- 需要快速上线的临时流程
- 无专职开发团队的业务部门

### 不适合低代码的场景
- 复杂业务逻辑判断（如动态金额计算规则）
- 需要与外部系统深度集成的场景
- 对 UI/UX 有极高定制要求的场景
- 需要复杂前端交互（如动态联动、图表）的流程

## 技术实现要点

1. **JSON 驱动**：整个表单和流程定义均为 JSON 数据，前端设计器生成 JSON，后端存储和解析 JSON
2. **字典驱动配置**：表单码通过字典表（DictData）统一管理，支持分页查询和模糊搜索
3. **字段类型映射**：`LFFieldTypeEnum` 负责前端控件类型到数据库类型的映射
4. **虚拟节点模式**：流程流转通过虚拟节点抽象，低代码流程同样遵循此设计模式

## 相关文档

- [表单设计详解](./lowcode-form.md) - 深入了解表单设计器和字段配置
- [低代码 vs 自定义表单对比](./lowcode-vs-diy.md) - 两种模式的详细对比
