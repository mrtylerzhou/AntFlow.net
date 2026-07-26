# API 接口参考

## 概述

AntFlowCore 提供 RESTful API 接口，支持流程管理、任务审批、表单配置等所有核心功能。所有接口遵循统一的响应格式。

## Swagger API 文档

AntFlowCore 集成了 Swagger/OpenAPI 文档，可以通过浏览器直接查看和测试所有 API 接口。

![Swagger API文档](/images/swagger-api.png)

## 响应格式

### 标准响应

```json
{
  "code": 0,
  "msg": "success",
  "data": { ... }
}
```

| 字段 | 类型 | 说明 |
|------|------|------|
| code | int | 状态码，0 表示成功 |
| msg | string | 响应消息 |
| data | object | 响应数据 |

### 分页响应

```json
{
  "code": 0,
  "msg": "success",
  "data": {
    "total": 100,
    "list": [ ... ]
  }
}
```

## 低代码流程接口

### 基础路径：`/lowcode`

#### 创建表单码

```http
POST /lowcode/createLowCodeFormCode
Content-Type: application/json

{
  "key": "leave_form",
  "value": "请假申请表"
}
```

**响应**：
```json
{
  "code": 0,
  "msg": "success",
  "data": 1001
}
```

---

#### 获取所有表单码

```http
GET /lowcode/getLowCodeFlowFormCodes
```

**响应**：
```json
{
  "code": 0,
  "msg": "success",
  "data": [
    { "key": "leave_form", "value": "请假申请表" },
    { "key": "expense_form", "value": "费用报销单" }
  ]
}
```

---

#### 分页获取表单码列表

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

---

#### 获取已激活表单码

```http
POST /lowcode/getLFActiveFormCodePageList
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "taskMgmtVO": {}
}
```

---

#### 获取表单数据

```http
GET /lowcode/getformDataByFormCode?formCode=leave_form
```

**响应**：
```json
{
  "code": 0,
  "msg": "success",
  "data": "{\"widgetList\":[...],\"formConfig\":{...}}"
}
```

## 流程配置接口

### 基础路径：`/BpmnConf`

#### 新增/编辑流程模板

```http
POST /BpmnConf/Edit
Content-Type: application/json

{
  "id": 0,
  "processName": "请假流程",
  "formCode": "leave_form",
  "nodes": [ ... ],
  "sequenceFlows": [ ... ]
}
```

---

#### 流程模板列表

```http
POST /BpmnConf/listPage
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 20 },
  "entity": {
    "processName": "请假"
  }
}
```

---

#### 流程模板详情

```http
GET /BpmnConf/detail/123
```

---

#### 生效流程模板

```http
GET /BpmnConf/effectiveBpmn/123
```

---

#### 流程预览

```http
POST /BpmnConf/preview
Content-Type: application/json

{
  "formCode": "leave_form",
  "nodes": [ ... ]
}
```

---

#### 发起页预览

```http
POST /BpmnConf/startPagePreviewNode
Content-Type: application/json

{
  "isStartPreview": true,
  "formCode": "leave_form"
}
```

---

#### 查看审批路径

```http
GET /BpmnConf/getBpmVerifyInfoVos?processNumber=PROC20240101001
```

---

#### 待办统计

```http
GET /BpmnConf/todoList
```

## 流程操作接口

### 基础路径：`/BpmnConf/process`

#### 审批操作（同意/拒绝/驳回等）

```http
POST /BpmnConf/process/buttonsOperation?formCode=leave_form
Content-Type: application/json

{
  "processNumber": "PROC20240101001",
  "taskId": 1001,
  "action": "agree",
  "opinion": "同意",
  "variables": {}
}
```

**参数说明**：

| 参数 | 类型 | 说明 |
|------|------|------|
| processNumber | string | 流程编号 |
| taskId | long | 任务ID |
| action | string | 操作类型：agree/disagree/reject/addSign/transfer |
| opinion | string | 审批意见 |
| variables | object | 流程变量 |

---

#### 查看业务流程

```http
POST /BpmnConf/process/viewBusinessProcess?formCode=leave_form
Content-Type: application/json

{
  "processNumber": "PROC20240101001"
}
```

---

#### 流程列表（待办/已办/我发起的）

```http
POST /BpmnConf/process/listPage/{type}
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 20 },
  "taskMgmtVO": {}
}
```

**type 参数**：

| 值 | 说明 |
|----|------|
| 1 | 待办 |
| 2 | 已办 |
| 3 | 我发起的 |
| 4 | 抄送我的 |

## 流程控制接口

### 基础路径：`/taskMgmt`

#### 保存流程通知配置

```http
POST /taskMgmt/taskMgmt
Content-Type: application/json

{
  "processNumber": "PROC20240101001",
  "noticeType": "email",
  "template": "您有新的待办任务"
}
```

---

#### 获取表单关联选项

```http
GET /taskMgmt/getFormRelatedOptions
```

**响应**：
```json
{
  "code": 0,
  "data": [
    { "id": 1, "name": "表单中的人员" },
    { "id": 2, "name": "表单中的角色" },
    { "id": 3, "name": "表单中人员的直属领导" }
  ]
}
```

---

#### 获取自定义规则选项

```http
GET /taskMgmt/getUDROptions
```

**响应**：
```json
{
  "code": 0,
  "data": [
    { "id": "rule1", "name": "自定义审批人1" },
    { "id": "rule2", "name": "自定义审批人2" }
  ]
}
```

## 业务管理接口

### 基础路径：`/bpmnBusiness`

#### 获取DIY表单列表

```http
GET /bpmnBusiness/getDIYFormCodeList?desc=请假
```

---

#### 委托列表

```http
POST /bpmnBusiness/entrustlist/1
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 }
}
```

---

#### 委托详情

```http
GET /bpmnBusiness/entrustDetail/123
```

---

#### 编辑委托

```http
POST /bpmnBusiness/editEntrust
Content-Type: application/json

{
  "id": 123,
  "entrustUserId": "user2",
  "startTime": "2024-01-01",
  "endTime": "2024-01-31"
}
```

---

#### 获取自选审批人节点

```http
GET /bpmnBusiness/getStartUserChooseModules?formCode=leave_form
```

## 外部接入接口

### 基础路径：`/outSide`

#### 外部流程提交

```http
POST /outSide/processSubmit
Content-Type: application/json

{
  "formCode": "external_form",
  "businessId": "BIZ001",
  "userId": "user1",
  "data": { ... }
}
```

**响应**：
```json
{
  "code": 0,
  "data": {
    "processNumber": "PROC20240101999",
    "status": "PROCESSING"
  }
}
```

---

#### 获取外部表单码

```http
POST /outSide/getOutSideFormCodePageList
Content-Type: application/json

{
  "pageDto": { "pageNo": 1, "pageSize": 10 },
  "entity": {}
}
```

---

#### 流程终止

```http
POST /outSide/processBreak
Content-Type: application/json

{
  "processNumber": "PROC20240101999",
  "userId": "user1"
}
```

---

#### 外部流程记录

```http
GET /outSide/outSideProcessRecord?processNumber=PROC20240101999
```

## 错误码参考

| 错误码 | 说明 | 处理建议 |
|--------|------|---------|
| 0 | 成功 | - |
| 10001 | 流程配置不存在 | 检查 formCode |
| 10002 | 流程未生效 | 先调用生效接口 |
| 10003 | 任务不存在 | 检查任务状态 |
| 10004 | 无操作权限 | 确认当前用户身份 |
| 10005 | 审批人未配置 | 检查节点配置 |
| 10006 | 条件表达式错误 | 检查语法 |
| 20001 | 数据库错误 | 查看数据库日志 |
| 20002 | 数据库超时 | 检查网络和SQL |
| 401 | 未认证 | 检查 Token |
| 403 | 无权限 | 检查用户角色 |
| 404 | 接口不存在 | 检查路由 |
| 500 | 服务器内部错误 | 查看详细日志 |

## 认证方式

AntFlowCore 使用 JWT Bearer Token 认证：

```http
GET /BpmnConf/todoList
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## 限流策略

| 接口类别 | 限流阈值 | 说明 |
|---------|---------|------|
| 查询接口 | 100次/分钟 | 普通查询 |
| 写操作 | 30次/分钟 | 审批、创建等 |
| 文件上传 | 10次/分钟 | 大文件限制 |
| 外部接入 | 60次/分钟 | API调用 |

## 接口调用示例

### cURL

```bash
# 获取待办列表
curl -X GET "http://localhost:8080/BpmnConf/todoList" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 审批操作
curl -X POST "http://localhost:8080/BpmnConf/process/buttonsOperation?formCode=leave_form" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "processNumber": "PROC20240101001",
    "taskId": 1001,
    "action": "agree",
    "opinion": "同意"
  }'
```

### C# HttpClient

```csharp
var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:8080/");
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

// 获取表单数据
var response = await client.GetAsync("lowcode/getformDataByFormCode?formCode=leave_form");
var result = await response.Content.ReadFromJsonAsync<Result<string>>();

// 审批操作
var approval = new
{
    processNumber = "PROC20240101001",
    taskId = 1001L,
    action = "agree",
    opinion = "同意"
};
response = await client.PostAsJsonAsync(
    "BpmnConf/process/buttonsOperation?formCode=leave_form", approval);
```

### JavaScript Fetch

```javascript
// 获取表单码列表
const response = await fetch('http://localhost:8080/lowcode/getLowCodeFlowFormCodes', {
    headers: {
        'Authorization': `Bearer ${token}`
    }
});
const data = await response.json();

// 审批操作
const result = await fetch('http://localhost:8080/BpmnConf/process/buttonsOperation?formCode=leave_form', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
        processNumber: 'PROC20240101001',
        taskId: 1001,
        action: 'agree',
        opinion: '同意'
    })
});
```
