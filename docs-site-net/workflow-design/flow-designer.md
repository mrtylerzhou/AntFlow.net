# 流程设计器使用指南

## 概述

AntFlowCore 流程设计器是可视化流程配置的核心工具。管理员通过拖拽方式即可完成流程模板的设计，无需编写代码。流程设计器基于 BPMN 2.0 规范，结合 AntFlowCore 独创的虚拟节点（VNode）模式，使流程配置变得简单直观。

## 流程模板数据结构

流程模板由 `BpmnConf`（流程配置）和 `BpmnNode`（节点配置）两层结构组成。

### BpmnConf 流程配置

流程配置是流程模板的顶层容器，定义流程的基本属性。

```csharp
// 源码位置: AntFlowCore.Base/entity/BpmnConf.cs
public class BpmnConf
{
    public long Id { get; set; }              // 自增主键
    public string BpmnCode { get; set; }      // 流程编码（自动生成）
    public string BpmnName { get; set; }      // 流程名称
    public int? BpmnType { get; set; }        // 流程类型
    public string FormCode { get; set; }      // 表单编码
    public int? AppId { get; set; }           // 应用ID
    public int? DeduplicationType { get; set; } // 去重类型
    public int EffectiveStatus { get; set; }  // 生效状态（0=未生效，1=已生效）
    public int IsAll { get; set; }            // 是否全员可见
    public int? IsOutSideProcess { get; set; } // 是否第三方流程
    public int? IsLowCodeFlow { get; set; }   // 是否低代码流程
    public long? BusinessPartyId { get; set; } // 业务方ID
    public string ConfConfigJson { get; set; } // 流程级JSON配置
}
```

### BpmnNode 节点配置

节点定义流程中的每个步骤，包含节点类型、属性、审批标准等信息。

```csharp
// 源码位置: AntFlowCore.Base/entity/BpmnNode.cs
public class BpmnNode
{
    public long Id { get; set; }
    public long ConfId { get; set; }          // 所属流程配置ID
    public string NodeId { get; set; }        // 节点唯一标识
    public int NodeType { get; set; }         // 节点类型
    public int NodeProperty { get; set; }     // 节点属性（审批人规则）
    public string NodeFrom { get; set; }      // 来源节点
    public int BatchStatus { get; set; }      // 批量状态
    public int ApprovalStandard { get; set; } // 审批标准
    public string NodeName { get; set; }      // 节点名称
    public string NodeDisplayName { get; set; } // 节点显示名称
    public int IsDeduplication { get; set; }  // 是否去重
    public int IsSignUp { get; set; }         // 是否加签
    public string NodeFroms { get; set; }     // 多个来源节点
    public bool? IsDynamicCondition { get; set; } // 是否动态条件
    public bool? IsParallel { get; set; }     // 是否并行
    public string NodeConfigJson { get; set; } // 节点级JSON配置
}
```

## 流程设计器操作流程

### 1. 创建新流程

通过流程设计器创建新流程模板的流程如下：

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  新建流程    │ -> │  设计流程    │ -> │  保存草稿    │ -> │  发布生效    │
│  填写基本信息 │    │  拖拽节点    │    │  管理员核对  │    │  正式使用    │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
```

### 2. 流程编辑接口

流程模板的新增和编辑通过 `BpmnConfController.Edit` 接口完成：

```http
POST /BpmnConf/Edit
Content-Type: application/json

{
  "bpmnName": "请假审批流程",
  "formCode": "leave_form",
  "bpmnType": 1,
  "nodes": [...]
}
```

**源码实现**（`BpmnConfController.cs:47-52`）：

```csharp
[HttpPost("Edit")]
public Result<String> Edit([FromBody] BpmnConfVo bpmnConfVo)
{
    _freeSql.Ado.Transaction(() => _bpmnConfBizService.Edit(bpmnConfVo));
    return Result<string>.Succ("ok");
}
```

### 3. 流程生效

流程设计完成后默认处于**未生效**状态，需要管理员手动点击"生效"按钮才能正式使用。这种设计保证了流程配置的安全性——管理员有充分时间核对流程配置，避免错误配置直接上线。

```http
GET /BpmnConf/effectiveBpmn/{id}
```

**源码实现**（`BpmnConfController.cs:164-168`）：

```csharp
[HttpGet("effectiveBpmn/{id}")]
public Result<bool> EffectiveBpmn(int id)
{
    _bpmnConfService.EffectiveBpmnConf(id);
    return Result<bool>.Succ(true);
}
```

## 流程预览

流程设计器提供实时预览功能，管理员可以在设计阶段查看流程的完整审批路径。

![流程设计器界面](/images/form-management.png)

### 流程预览接口

```http
POST /BpmnConf/preview
Content-Type: application/json

{
  "bpmnConfVo": { ... },
  "formData": { ... }
}
```

**源码实现**（`BpmnConfController.cs:87-93`）：

```csharp
[HttpPost("preview")]
public Result<PreviewNode> Preview([FromServices] IHttpContextAccessor accessor)
{
    string values = accessor.HttpContext!.ReadRawBodyAsString();
    PreviewNode previewNode = _bpmnConfCommonService.PreviewNode(values);
    return Result<PreviewNode>.Succ(previewNode);
}
```

### 发起页预览

发起页预览用于模拟用户发起流程时的审批路径展示：

```http
POST /BpmnConf/startPagePreviewNode
Content-Type: application/json

{
  "isStartPreview": true,
  "formCode": "leave_form",
  "formData": { ... }
}
```

**源码实现**（`BpmnConfController.cs:100-113`）：

```csharp
[HttpPost("startPagePreviewNode")]
public Result<PreviewNode> StartPagePreviewNode([FromServices] IHttpContextAccessor accessor)
{
    string paramsJson = accessor.HttpContext!.ReadRawBodyAsString();
    JsonNode? jsonObject = JsonNodeHelper.SafeParse(paramsJson);
    bool isStartPreview = jsonObject?["isStartPreview"]?.GetValue<bool>() ?? false;

    if (isStartPreview)
    {
        return Result<PreviewNode>.Succ(_bpmnConfCommonService.StartPagePreviewNode(paramsJson));
    }
    return Result<PreviewNode>.Succ(_bpmnConfCommonService.TaskPagePreviewNode(paramsJson));
}
```

## 流程列表与查询

### 流程列表分页查询

```http
POST /BpmnConf/listPage
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "entity": { "bpmnName": "请假" }
}
```

**源码实现**（`BpmnConfController.cs:74-80`）：

```csharp
[HttpPost("listPage")]
public Result<ResultAndPage<BpmnConfVo>> ListPage([FromBody] PageRequestDto<BpmnConfVo> dto)
{
    PageDto page = dto.PageDto;
    BpmnConfVo vo = dto.Entity;
    return Result<ResultAndPage<BpmnConfVo>>.Succ(_bpmnConfBizService.SelectPage(page, vo));
}
```

### 流程详情查看

```http
GET /BpmnConf/detail/{id}
POST /BpmnConf/detail/{id}
```

## 流程配置验证规则

流程名称在保存时会进行严格验证（`BpmnConf.cs:109-145`）：

```csharp
public static void ValidateBpmnName(string bpmnName)
{
    if (string.IsNullOrEmpty(bpmnName))
        throw new AFBizException("审批流名称必须存在!");
    
    if (Regex.IsMatch(bpmnName, PATTERN))
        throw new AFBizException("审批流名称不合法");
    
    if (string.IsNullOrWhiteSpace(bpmnName))
        throw new AFBizException("审批流名称不得包含空格");
    
    if (Regex.IsMatch(bpmnName, StringConstants.SPECIAL_CHARACTERS))
        throw new AFBizException("审批流名称中不得包含特殊字符!");
    
    if (bpmnName.Length > NumberConstants.BPMN_NAME_MAX_LEN)
        throw new AFBizException("审批流名称过长");
}
```

## 流程去重配置

AntFlowCore 支持三种审批人去重策略，在流程级别配置：

| 去重类型 | 值 | 说明 |
|---------|---|------|
| 不去重 | 1 | 不进行任何去重处理 |
| 前去重 | 2 | 审批人重复出现时，只在最后一次审批 |
| 后去重 | 3 | 审批人重复出现时，只在第一次审批 |
| 跳过去重 | 4 | 审批人仅在相邻节点重复时，后续自动同意 |

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/DeduplicationTypeEnum.cs
public enum DeduplicationTypeEnum
{
    DEDUPLICATION_TYPE_NULL = 1,        // 不去重
    DEDUPLICATION_TYPE_FORWARD = 2,     // 前去重
    DEDUPLICATION_TYPE_BACKWARD = 3,    // 后去重
    DEDUPLICATION_TYPE_SKIP_NEXT = 4    // 跳过去重
}
```

## 流程模板 JSON 配置

AntFlowCore 使用 JSON 配置替代传统的多表关联设计，核心配置存储在 `ConfConfigJson` 和 `NodeConfigJson` 字段中：

- **ConfConfigJson**：流程级配置，包含按钮模板、审批提醒、视图页面按钮等
- **NodeConfigJson**：节点级配置，包含条件配置、审批人参数、加签配置等

这种设计使得流程模板可以灵活扩展，无需修改数据库表结构即可添加新配置项。

## 审批路径查看

管理员可以查看已发起流程的完整审批路径：

```http
GET /BpmnConf/getBpmVerifyInfoVos?processNumber={processNumber}
```

**源码实现**（`BpmnConfController.cs:120-123`）：

```csharp
[HttpGet("getBpmVerifyInfoVos")]
public Result<List<BpmVerifyInfoVo>> GetBpmVerifyInfoVos(String processNumber)
{
    return Result<List<BpmVerifyInfoVo>>.Succ(
        _bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false));
}
```

## 最佳实践

1. **流程命名规范**：使用有意义的中文名称，如"请假审批流程"、"采购报销流程"
2. **先设计后生效**：流程设计完成后先保存草稿，核对无误后再点击生效
3. **合理使用去重**：对于审批人可能重复出现的流程，建议配置去重策略
4. **充分测试**：正式生效前，使用预览功能模拟各种场景验证流程正确性
5. **版本管理**：重大流程变更时创建新版本，保留旧版本以支持在途流程继续执行
