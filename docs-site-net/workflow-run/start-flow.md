# 发起流程详解

## 概述

发起流程是工作流引擎的入口。用户填写表单并提交后，引擎根据流程模板配置创建流程实例、生成审批任务，并按流程定义驱动流转。

## 发起流程架构

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  填写表单    │ -> │  提交流程    │ -> │  引擎处理    │ -> │  生成任务    │
│  FormCode   │    │  Submit     │    │  Runtime    │    │  Task       │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                                               │
                                                               ▼
                                                         ┌─────────────┐
                                                         │  通知审批人  │
                                                         │  Notify     │
                                                         └─────────────┘
```

## 核心数据结构

### BusinessDataVo

发起流程时，前端提交的数据封装为 `BusinessDataVo`：

```csharp
// 源码位置: AntFlowCore.Base/vo/BusinessDataVo.cs
public class BusinessDataVo
{
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }       // 流程编号
    
    [JsonPropertyName("processKey")]
    public string ProcessKey { get; set; }           // 流程Key
    
    [JsonPropertyName("businessId")]
    public string BusinessId { get; set; }           // 业务ID
    
    [JsonPropertyName("formCode")]
    public string FormCode { get; set; }             // 表单编码
    
    [JsonPropertyName("operationType")]
    public int? OperationType { get; set; }          // 操作类型
    
    [JsonPropertyName("approvalComment")]
    public string ApprovalComment { get; set; }      // 审批意见
    
    [JsonPropertyName("taskId")]
    public string TaskId { get; set; }               // 任务ID
    
    [JsonPropertyName("nodeId")]
    public string NodeId { get; set; }               // 节点ID
    
    [JsonPropertyName("params")]
    public string Params { get; set; }               // 业务参数
    
    [JsonPropertyName("userIds")]
    public List<string> UserIds { get; set; }        // 用户ID列表
    
    [JsonPropertyName("flag")]
    public bool? Flag { get; set; }                  // 标记
    
    [JsonPropertyName("startUserId")]
    public string StartUserId { get; set; }          // 发起人ID
    
    [JsonPropertyName("startUserName")]
    public string StartUserName { get; set; }        // 发起人姓名
    
    [JsonPropertyName("isMigration")]
    public bool? IsMigration { get; set; }           // 是否迁移
    
    [JsonPropertyName("backToModifyType")]
    public int? BackToModifyType { get; set; }       // 打回修改类型
    
    [JsonPropertyName("backToNodeId")]
    public string BackToNodeId { get; set; }         // 打回节点ID
}
```

### 操作类型

发起流程使用 `ProcessOperationEnum.BUTTON_TYPE_SUBMIT`：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ProcessOperationEnum.cs
public enum ProcessOperationEnum
{
    BUTTON_TYPE_SUBMIT = 1,            // 流程提交
    BUTTON_TYPE_RESUBMIT = 2,          // 重新提交
    // ...
}
```

## 发起流程接口

### 流程操作统一接口

```http
POST /BpmnConf/process/buttonsOperation?formCode={formCode}
Content-Type: application/json

{
  "operationType": 1,
  "formCode": "leave_form",
  "businessId": "biz_001",
  "params": "{\"amount\": 5000, \"days\": 3}",
  "approvalComment": "申请年假3天"
}
```

**源码实现**（`BpmnConfController.cs:60-67`）：

```csharp
[HttpPost("process/buttonsOperation")]
public Result<BusinessDataVo> ButtonsOperation(
    [FromServices] IHttpContextAccessor accessor, 
    [FromQuery] String formCode)
{
    string values = accessor.HttpContext!.ReadRawBodyAsString();
    BusinessDataVo dataVo = _processApprovalService.ButtonsOperation(values, formCode);
    return Result<BusinessDataVo>.Succ(dataVo);
}
```

## 发起流程处理流程

```
┌─────────────────────────────────────────────────────────────────┐
│                    发起流程处理流程                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌───────────────┐                                              │
│  │ 1. 接收请求    │  解析 BusinessDataVo                         │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 2. 表单转换    │  _formFactory.DataFormConversion()           │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 3. 确定操作    │  ProcessOperationEnum.BUTTON_TYPE_SUBMIT      │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 4. 设置发起人  │  SecurityUtils.GetLogInEmpId()               │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 5. 开启事务    │  _freeSql.Ado.Transaction()                  │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 6. 执行操作    │  _buttonOperationService                     │
│  │               │    .ButtonsOperationTransactional()           │
│  └───────┬───────┘                                              │
│          ▼                                                      │
│  ┌───────────────┐                                              │
│  │ 7. 返回结果    │  BusinessDataVo                              │
│  └───────────────┘                                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

**源码实现**（`ProcessApprovalService.cs:65-100`）：

```csharp
public BusinessDataVo ButtonsOperation(String parameters, String formCode)
{
    // 1. 反序列化参数
    BusinessDataVo vo = _formFactory.DataFormConversion(parameters, formCode);
    
    // 2. 确定操作类型
    ProcessOperationEnum? poEnum = ProcessOperationEnumExtensions.GetEnumByCode(vo.OperationType);
    if (poEnum == null)
    {
        throw new AFBizException("unknown operation type,please Contact the Administrator");
    }
    
    formCode = vo.FormCode;
    ThreadLocalContainer.Set(StringConstants.FORM_CODE, formCode);
    
    // 3. 设置标记
    if (poEnum == ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE || 
        poEnum == ProcessOperationEnum.BUTTON_TYPE_STOP)
    {
        vo.Flag = false;
    }
    else if (poEnum == ProcessOperationEnum.BUTTON_TYPE_ABANDON)
    {
        vo.Flag = true;
    }
    
    // 4. 设置发起人信息
    if (string.IsNullOrEmpty(vo.StartUserId))
    {
        vo.StartUserId = SecurityUtils.GetLogInEmpId();
        vo.StartUserName = SecurityUtils.GetLogInEmpName();
    }
    
    BusinessDataVo dataVo = null;
    
    // 5. 事务执行
    _freeSql.Ado.Transaction(() => { 
        dataVo = _buttonOperationService.ButtonsOperationTransactional(vo); 
    });
    
    return dataVo;
}
```

## 重新提交

当流程被打回修改后，发起人修改表单内容后重新提交：

```json
{
  "operationType": 2,
  "processNumber": "LEAVE20260726001",
  "formCode": "leave_form",
  "businessId": "biz_001",
  "params": "{\"amount\": 3000, \"days\": 2}",
  "approvalComment": "修改为申请年假2天"
}
```

## 保存草稿

发起人可以将未完成的流程保存为草稿，后续继续填写：

```json
{
  "operationType": 30,
  "formCode": "leave_form",
  "params": "{\"amount\": 5000}"
}
```

```csharp
// 源码位置: ProcessOperationEnum
BUTTON_TYPE_SAVE_DRAFT = 30  // 保存草稿
```

## 流程状态

流程发起后的状态通过 `ProcessStateEnum` 标识：

```csharp
// 源码位置: AntFlowCore.Base/constant/enums/ProcessStateEnum.cs
public enum ProcessStateEnum
{
    HANDLING_STATE = 1,  // 审批中
    END_STATE = 3,       // 作废
    HANDLE_STATE = 2,    // 审批通过
    REJECT_STATE = 6     // 审批拒绝
}
```

## 发起页预览

在实际发起前，用户可以预览流程的审批路径：

```http
POST /BpmnConf/startPagePreviewNode
Content-Type: application/json

{
  "isStartPreview": true,
  "formCode": "leave_form",
  "formData": {
    "amount": 5000,
    "days": 3
  }
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

## PreviewNode 结构

预览结果包含完整的流程路径信息：

```csharp
// 源码位置: AntFlowCore.Base/vo/PreviewNode.cs
public class PreviewNode
{
    public string BpmnName { get; set; }                    // 流程名称
    public string FormCode { get; set; }                    // 表单编码
    public List<BpmnNodeVo> BpmnNodeList { get; set; }      // 节点列表
    public PrevEmployeeInfo StartUserInfo { get; set; }     // 发起人信息
    public PrevEmployeeInfo EmployeeInfo { get; set; }      // 员工信息
    public int? DeduplicationType { get; set; }             // 去重类型
    public string DeduplicationTypeName { get; set; }       // 去重类型名称
    public String CurrentNodeId { get; set; }               // 当前节点ID
    public List<String> BeforeNodeIds { get; set; }         // 前置节点ID列表
    public List<String> AfterNodeIds { get; set; }          // 后置节点ID列表
}
```

## 流程统计

用户可以查看自己的流程统计信息：

```http
GET /BpmnConf/todoList
```

**源码实现**（`BpmnConfController.cs:173-177`）：

```csharp
[HttpGet("todoList")]
public Result<TaskMgmtVO> TodoList()
{
    TaskMgmtVO taskMgmtVO = _processApprovalService.ProcessStatistics();
    return ResultHelper.Success(taskMgmtVO);
}
```

### 统计数据结构

```csharp
// TaskMgmtVO 中的统计字段
public int TodoCount { get; set; }          // 待办数量
public int DoneTodayCount { get; set; }     // 今日已办数量
public int DoneCreateCount { get; set; }    // 已发起数量
public int DraftCount { get; set; }         // 草稿数量
```

## 发起流程时序图

```
发起人          前端           API           引擎          数据库
  │             │             │             │             │
  │ 填写表单     │             │             │             │
  │────────────▶│             │             │             │
  │             │             │             │             │
  │ 预览流程     │             │             │             │
  │────────────▶│ startPage   │             │             │
  │             │ PreviewNode │             │             │
  │             │────────────▶│ 解析条件     │             │
  │             │             │ 计算路径     │             │
  │             │◀────────────│ 返回节点列表 │             │
  │◀────────────│             │             │             │
  │             │             │             │             │
  │ 提交流程     │             │             │             │
  │────────────▶│ buttons     │             │             │
  │             │ Operation   │             │             │
  │             │────────────▶│ 创建流程实例 │             │
  │             │             │────────────▶│ 保存流程    │
  │             │             │             │────────────▶│
  │             │             │             │◀────────────│
  │             │             │◀────────────│ 返回任务    │
  │             │◀────────────│ 返回结果     │             │
  │◀────────────│             │             │             │
  │             │             │             │             │
```

## 第三方流程发起

AntFlowCore 支持外部系统通过 Open API 发起流程：

```json
{
  "operationType": 1,
  "formCode": "leave_form",
  "formData": "{...}",
  "bpmConfVo": { ... },
  "bpmnConfVo": { ... },
  "bpmFlowCallbackUrl": "https://api.example.com/callback",
  "viewUrl": "https://example.com/view",
  "submitUrl": "https://example.com/submit",
  "conditionsUrl": "https://example.com/conditions"
}
```

## 低代码流程发起

低代码流程使用 `lfFields` 传递表单字段值：

![发起流程页面](/images/my-initiate.png)

```json
{
  "operationType": 1,
  "formCode": "lowcode_form",
  "lfFields": {
    "amount": 5000,
    "days": 3,
    "type": "年假"
  },
  "lfConditions": {
    "amount": { "operator": 1, "value": 3000 }
  }
}
```

## 错误处理

发起流程时可能遇到的错误：

| 错误类型 | 说明 | 处理方式 |
|---------|------|---------|
| 流程模板未生效 | 管理员未发布流程 | 联系管理员发布流程 |
| 审批人为空 | 审批人规则无法解析出有效人员 | 检查审批人配置 |
| 条件不满足 | 所有条件分支都不满足 | 检查条件配置 |
| 表单数据缺失 | 必填字段未填写 | 补全表单数据 |
| 无权限 | 用户无权发起该流程 | 联系管理员授权 |

## 最佳实践

1. **预览确认**：发起前先预览审批路径，确保流程正确
2. **完整填写**：确保表单数据完整，避免审批人信息不足
3. **明确意见**：在审批意见中清晰说明申请事由
4. **关注通知**：发起后关注审批进度，及时补充材料
5. **草稿保存**：复杂表单分次填写时先保存草稿
