# 我的任务管理

## 概述

AntFlowCore 提供完整的任务管理功能，用户可以查看自己发起的流程、待审批的任务、已审批的任务等。所有任务列表通过统一的 `listPage` 接口，根据 `type` 参数区分不同任务类型。

## 任务类型

```http
POST /BpmnConf/process/listPage/{type}
```

| Type | 路由 | 说明 |
|------|------|------|
| 1 | `/process/listPage/1` | 待办任务（待我审批） |
| 2 | `/process/listPage/2` | 已办任务（我已审批） |
| 3 | `/process/listPage/3` | 我发起的 |
| 4 | `/process/listPage/4` | 抄送给我的 |
| 5 | `/process/listPage/5` | 草稿箱 |

**源码实现**（`BpmnConfController.cs:136-143`）：

```csharp
/// <summary>
/// 流程我的待办/已办等列表页面出口方法,此方法根据type区分请求类型,
/// 但是没有使用策略模板,而是简单的switch case,这里面没有很复杂的逻辑,
/// 基本上都是稍复杂一些的查询
/// </summary>
[HttpPost("process/listPage/{type}")]
public ResultAndPage<TaskMgmtVO> ViewPcProcessList(
    [FromRoute] int type, 
    [FromBody] DetailRequestDto requestDto)
{
    PageDto pageDto = requestDto.PageDto;
    TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
    taskMgmtVO.Type = type;
    return _processApprovalService.FindPcProcessList(pageDto, taskMgmtVO);
}
```

## 任务查询接口

### 待办任务

查询当前用户待审批的任务：

```http
POST /BpmnConf/process/listPage/1
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {
    "search": "",
    "processState": 1
  }
}
```

### 已办任务

查询当前用户已审批的任务：

```http
POST /BpmnConf/process/listPage/2
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {
    "search": "请假"
  }
}
```

### 我发起的

查询当前用户发起的流程：

```http
POST /BpmnConf/process/listPage/3
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {
    "processState": 1
  }
}
```

### 抄送给我的

查询抄送给当前用户的流程：

```http
POST /BpmnConf/process/listPage/4
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {}
}
```

### 草稿箱

查询当前用户保存的草稿：

```http
POST /BpmnConf/process/listPage/5
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {}
}
```

## TaskMgmtVO 数据结构

```csharp
// 源码位置: AntFlowCore.Base/vo/TaskMgmtVO.cs
public class TaskMgmtVO
{
    [JsonPropertyName("taskId")]
    public string TaskId { get; set; }               // 任务ID
    
    [JsonPropertyName("taskName")]
    public string TaskName { get; set; }             // 任务名称
    
    [JsonPropertyName("processName")]
    public string ProcessName { get; set; }          // 流程名称
    
    [JsonPropertyName("processInstanceId")]
    public string ProcessInstanceId { get; set; }    // 流程实例ID
    
    [JsonPropertyName("confId")]
    public long ConfId { get; set; }                 // 流程配置ID
    
    [JsonPropertyName("processId")]
    public string ProcessId { get; set; }            // 流程ID
    
    [JsonPropertyName("processKey")]
    public string ProcessKey { get; set; }           // 流程Key
    
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }        // 创建时间
    
    [JsonPropertyName("applyUser")]
    public string ApplyUser { get; set; }            // 发起人
    
    [JsonPropertyName("applyUserName")]
    public string ApplyUserName { get; set; }        // 发起人姓名
    
    [JsonPropertyName("applyDate")]
    public string ApplyDate { get; set; }            // 发起日期
    
    [JsonPropertyName("applyDept")]
    public string ApplyDept { get; set; }            // 发起部门
    
    [JsonPropertyName("actualName")]
    public string ActualName { get; set; }           // 实际审批人
    
    [JsonPropertyName("deptName")]
    public string DeptName { get; set; }             // 部门名称
    
    [JsonPropertyName("originalName")]
    public string OriginalName { get; set; }         // 原审批人
    
    [JsonPropertyName("processNumber")]
    public string ProcessNumber { get; set; }        // 流程编号
    
    [JsonPropertyName("taskState")]
    public string TaskState { get; set; }            // 任务状态
    
    [JsonPropertyName("taskStype")]
    public int TaskStype { get; set; }               // 任务类型
    
    [JsonPropertyName("processState")]
    public int? ProcessState { get; set; }           // 流程状态
    
    [JsonPropertyName("businessId")]
    public string BusinessId { get; set; }           // 业务ID
    
    [JsonPropertyName("handleUserName")]
    public string HandleUserName { get; set; }       // 处理人姓名
    
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; }            // 开始时间
    
    [JsonPropertyName("runTime")]
    public DateTime? RunTime { get; set; }           // 处理时间
    
    [JsonPropertyName("endTime")]
    public string EndTime { get; set; }              // 结束时间
    
    [JsonPropertyName("type")]
    public int Type { get; set; }                    // 查询类型
    
    [JsonPropertyName("search")]
    public string Search { get; set; }               // 搜索关键词
    
    [JsonPropertyName("disagreeType")]
    public int DisagreeType { get; set; }            // 不同意类型
    
    [JsonPropertyName("operationType")]
    public int OperationType { get; set; }           // 操作类型
    
    [JsonPropertyName("approvalComment")]
    public string ApprovalComment { get; set; }      // 审批意见
    
    [JsonPropertyName("todoCount")]
    public int TodoCount { get; set; }               // 待办数量
    
    [JsonPropertyName("doneTodayCount")]
    public int DoneTodayCount { get; set; }          // 今日已办数量
    
    [JsonPropertyName("doneCreateCount")]
    public int DoneCreateCount { get; set; }         // 已发起数量
    
    [JsonPropertyName("draftCount")]
    public int DraftCount { get; set; }              // 草稿数量
    
    [JsonPropertyName("isRead")]
    public int IsRead { get; set; }                  // 是否已读
    
    [JsonPropertyName("headImg")]
    public string HeadImg { get; set; }              // 头像
    
    [JsonPropertyName("departmentPath")]
    public string DepartmentPath { get; set; }       // 部门路径
    
    [JsonPropertyName("applyUserId")]
    public int ApplyUserId { get; set; }             // 发起人ID
    
    [JsonPropertyName("concernState")]
    public int ConcernState { get; set; }            // 关注状态
    
    [JsonPropertyName("isBatchSubmit")]
    public bool IsBatchSubmit { get; set; }          // 是否批量提交
    
    [JsonPropertyName("isForward")]
    public bool IsForward { get; set; }              // 是否转发
    
    [JsonPropertyName("isOld")]
    public bool IsOld { get; set; }                  // 是否旧数据
    
    [JsonPropertyName("routeUrl")]
    public string RouteUrl { get; set; }             // 路由URL
    
    [JsonPropertyName("entryId")]
    public string EntryId { get; set; }              // 入口ID
    
    [JsonPropertyName("version")]
    public int Version { get; set; }                 // 版本
    
    [JsonPropertyName("appTime")]
    public string AppTime { get; set; }              // 应用时间
    
    [JsonPropertyName("overtimeUrl")]
    public string OvertimeUrl { get; set; }          // 超时URL
    
    [JsonPropertyName("isLeftStroke")]
    public bool IsLeftStroke { get; set; }           // 是否左划
    
    [JsonPropertyName("title")]
    public string Title { get; set; }                // 标题
    
    [JsonPropertyName("processCode")]
    public string ProcessCode { get; set; }          // 流程编码
    
    [JsonPropertyName("processNumbers")]
    public List<string> ProcessNumbers { get; set; } // 流程编号列表
    
    [JsonPropertyName("processDigest")]
    public string ProcessDigest { get; set; }        // 流程摘要
    
    [JsonPropertyName("processType")]
    public string ProcessType { get; set; }          // 流程类型
    
    [JsonPropertyName("processTypeName")]
    public string ProcessTypeName { get; set; }      // 流程类型名称
    
    [JsonPropertyName("code")]
    public string Code { get; set; }                 // 编码
    
    [JsonPropertyName("nodeType")]
    public int NodeType { get; set; }                // 节点类型
    
    [JsonPropertyName("id")]
    public string Id { get; set; }                   // ID
    
    [JsonPropertyName("name")]
    public string Name { get; set; }                 // 名称
    
    [JsonPropertyName("userId")]
    public string UserId { get; set; }               // 用户ID
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; }             // 用户姓名
    
    [JsonPropertyName("roleIds")]
    public List<int> RoleIds { get; set; }           // 角色ID列表
    
    [JsonPropertyName("userIds")]
    public List<int> UserIds { get; set; }           // 用户ID列表
    
    [JsonPropertyName("changeHandlers")]
    public List<ContansDataVo> ChangeHandlers { get; set; } // 变更处理人
    
    [JsonPropertyName("createUser")]
    public string CreateUser { get; set; }           // 创建人
    
    [JsonPropertyName("includeAllFlag")]
    public int IncludeAllFlag { get; set; }          // 包含全部标记
}
```

## 流程状态筛选

任务列表支持按流程状态筛选：

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

| 状态 | 值 | 说明 |
|------|---|------|
| 审批中 | 1 | 流程正在审批 |
| 审批通过 | 2 | 流程全部审批通过 |
| 作废 | 3 | 流程已作废 |
| 审批拒绝 | 6 | 流程被拒绝 |

![我的任务列表](/images/todo-tasks.png)

## 任务统计

### 获取待办统计

```http
GET /BpmnConf/todoList
```

**响应示例**：

```json
{
  "todoCount": 5,
  "doneTodayCount": 3,
  "doneCreateCount": 12,
  "draftCount": 2
}
```

**源码实现**（`BpmnConfController.cs:173-177`）：

```csharp
/// <summary>
/// 通用用于办公界面首页流程相关的统计信息,用户非必须使用,可以酌情考虑
/// </summary>
[HttpGet("todoList")]
public Result<TaskMgmtVO> TodoList()
{
    TaskMgmtVO taskMgmtVO = _processApprovalService.ProcessStatistics();
    return ResultHelper.Success(taskMgmtVO);
}
```

## 任务详情

### 查看流程详情

```http
POST /BpmnConf/process/viewBusinessProcess?formCode={formCode}
Content-Type: application/json

{
  "processNumber": "LEAVE20260726001",
  "formCode": "leave_form"
}
```

**源码实现**（`BpmnConfController.cs:124-128`）：

```csharp
[HttpPost("process/viewBusinessProcess")]
public Result<dynamic> ViewBusinessProcess(
    [FromServices] IHttpContextAccessor accessor, 
    String formCode)
{
    string values = accessor.HttpContext!.ReadRawBodyAsString();
    return Result<dynamic>.Succ(_processApprovalService.GetBusinessInfo(values, formCode));
}
```

### 查看审批路径

```http
GET /BpmnConf/getBpmVerifyInfoVos?processNumber={processNumber}
```

**响应示例**：

```json
[
  {
    "id": "verify_001",
    "verifyUserId": "emp_001",
    "verifyUserName": "张三",
    "verifyStatus": 2,
    "verifyStatusName": "已通过",
    "verifyDesc": "同意",
    "verifyDate": "2026-07-26 10:30:00",
    "taskName": "部门经理审批",
    "elementId": "approve_001",
    "nodeType": 4,
    "sort": 2
  },
  {
    "id": "verify_002",
    "verifyUserId": "emp_002",
    "verifyUserName": "李四",
    "verifyStatus": 1,
    "verifyStatusName": "审批中",
    "verifyDesc": "",
    "taskName": "总经理审批",
    "elementId": "approve_002",
    "nodeType": 4,
    "sort": 3
  }
]
```

## 批量操作

AntFlowCore 支持批量审批操作：

```json
{
  "operationType": 3,
  "taskIds": ["task_001", "task_002", "task_003"],
  "approvalComment": "批量同意",
  "isBatchSubmit": true
}
```

## 任务列表页面结构

```
┌─────────────────────────────────────────────────────────────────┐
│                         我的任务                                 │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐  │
│  │ 待办(5) │ │ 已办(20)│ │ 发起(12)│ │ 抄送(3) │ │ 草稿(2) │  │
│  └─────────┘ └─────────┘ └─────────┘ └─────────┘ └─────────┘  │
├─────────────────────────────────────────────────────────────────┤
│  搜索: [________________]  状态: [全部▼]                         │
├─────────────────────────────────────────────────────────────────┤
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ 🔴 请假审批 - LEAVE20260726001                            │  │
│  │    发起人: 张三 | 发起时间: 2026-07-26 10:00              │  │
│  │    当前节点: 部门经理审批 | 状态: 审批中                   │  │
│  │    [同意] [拒绝] [打回] [转发] [加批]                     │  │
│  └───────────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ 🟢 报销审批 - EXP20260725001                              │  │
│  │    发起人: 李四 | 发起时间: 2026-07-25 14:00              │  │
│  │    当前节点: 已完成 | 状态: 审批通过                       │  │
│  │    [查看详情]                                              │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## 任务处理流程

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  查看待办    │ -> │  打开任务    │ -> │  审批操作    │ -> │  提交结果    │
│  List       │    │  Detail     │    │  Approve    │    │  Submit     │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                                               │
                                                               ▼
                                                         ┌─────────────┐
                                                         │  流程流转    │
                                                         │  Next Node  │
                                                         └─────────────┘
```

## 任务超时提醒

待办任务支持超时提醒配置：

```json
{
  "overtimeUrl": "/api/task/overtime",
  "isLeftStroke": false
}
```

## 任务转发

用户可以将待办任务转发给其他人查看：

```json
{
  "operationType": 15,
  "taskId": "task_001",
  "userIds": ["emp_010"],
  "approvalComment": "请知悉"
}
```

## 任务关注

用户可以关注特定流程，接收流程进展通知：

```json
{
  "concernState": 1,
  "processNumber": "LEAVE20260726001"
}
```

## 任务列表查询参数

### DetailRequestDto

```json
{
  "pageDto": {
    "pageNo": 1,
    "pageSize": 10
  },
  "taskMgmtVO": {
    "type": 1,
    "search": "请假",
    "processState": 1,
    "processNumber": "LEAVE20260726001",
    "applyUser": "张三",
    "startTime": "2026-07-01",
    "endTime": "2026-07-31"
  }
}
```

## 移动端适配

AntFlowCore 任务管理支持移动端适配：

- 左划快速操作（同意/拒绝）
- 下拉刷新
- 触底加载更多
- 批量选择操作

## 最佳实践

1. **及时处理待办**：养成每日查看待办的习惯，避免流程积压
2. **使用搜索过滤**：任务较多时使用搜索和筛选快速定位
3. **批量操作**：相同类型的任务使用批量审批提高效率
4. **关注重要流程**：为重要流程设置关注，及时获取进展通知
5. **定期清理草稿**：定期清理过期草稿，保持列表整洁
