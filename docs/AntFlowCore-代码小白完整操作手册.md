# AntFlowCore 代码小白完整操作手册

这份手册按“能跑起来、能看懂、能扩展、能接入、知道风险”的顺序写。你可以先照着做一个最小流程，再回头看源码原理。

## 1. 这个项目到底是什么

AntFlowCore 是一个 .NET 版工作流后端。它负责：

- 保存流程模板：谁审批、什么条件走哪条分支、按钮和通知怎么配。
- 发起流程实例：把一张业务单据变成一个正在流转的审批流程。
- 生成待办任务：根据流程模板找到审批人。
- 处理审批动作：同意、不同意、打回、转办、加签、撤回等。
- 回调业务系统：流程完成或节点变化后通知外部系统。

这个仓库本身主要是后端，不包含完整前端。README 说明前端来自 Java 版仓库里的 `antflow-vue`，或者使用独立设计器 `AntFlow-Designer`。

## 2. 项目结构怎么读

先记住这几个目录就够了：

| 目录 | 作用 |
| --- | --- |
| `src/AntFlowCore.Web` | Web 启动项目，`Program.cs` 配置 Swagger、CORS、FreeSql、AntFlow 服务 |
| `src/AntFlowCore.Api/controller` | 对外 API，流程保存、发起、审批、外部接入都在这里 |
| `src/AntFlowCore.Engine` | 业务编排层，DIY 表单、低代码表单、流程操作分发 |
| `src/AntFlowCore.Bpmn` | 工作流核心：节点转换、条件判断、任务流转、审批人查找 |
| `src/AntFlowCore.Persist` | 数据访问层，FreeSql 仓储 |
| `src/AntFlowCore.Base` | VO、实体、枚举、工具类、特性 |
| `script` | 数据库初始化脚本，目前 MySQL 和 SQL Server 脚本有内容 |
| `docs` | 现有说明文档 |

最常看的代码入口：

- 后端启动：`src/AntFlowCore.Web/Program.cs`
- 流程模板接口：`src/AntFlowCore.Api/controller/BpmnConfController.cs`
- 低代码接口：`src/AntFlowCore.Api/controller/LowCodeFlowController.cs`
- 外部系统接入：`src/AntFlowCore.Api/controller/OutSideBpmAccessController.cs`
- DIY 示例：`src/AntFlowCore.Engine/service/formprocess/ThirdPartyAccountApplyFlowService.cs`
- DIY 表单基类：`src/AntFlowCore.Bpmn/adaptor/formoperation/AbstractLowFlowSpyFormOperationAdaptor.cs`
- 条件类型：`src/AntFlowCore.Bpmn/constants/ConditionTypeEnum.cs`
- 审批人规则：`src/AntFlowCore.Base/constant/enums/NodePropertyEnum.cs`
- 服务注册：`src/AntFlowCore.AspNetCore/conf/di/serviceregistration/ServiceRegistration.cs`

## 3. 先把后端跑起来

### 3.1 安装环境

当前项目目标框架定义在 `version.props`：

```xml
<MicrosoftTargetFramework>net10.0</MicrosoftTargetFramework>
```

所以本地需要 .NET 10 SDK。当前仓库能用 .NET 10 preview 编译通过。生产环境建议使用正式版 SDK 后再上线。

还需要：

- MySQL，或 SQL Server。
- 如果要跑前端，需要 Node.js。

### 3.2 初始化数据库

MySQL：

```sql
CREATE DATABASE antflow DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

然后导入：

```powershell
script\bpm_init_db_mysql.sql
```

SQL Server 可导入：

```powershell
script\bpm_init_db_sqlserver.sql
```

注意：Oracle、PostgreSQL 脚本在当前仓库是空文件。

### 3.3 修改数据库配置

配置文件在：

```text
src/AntFlowCore.Web/appsettings.json
```

当前默认连接是：

```json
"MySqlConnection": "server=antflow.top;userid=root;pwd=123456;port=3306;database=antflow.net;sslmode=none;Charset=utf8"
```

必须改成你自己的数据库，不能直接用默认值。

示例：

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "server=127.0.0.1;userid=root;pwd=你的密码;port=3306;database=antflow;sslmode=none;Charset=utf8mb4"
  }
}
```

### 3.4 编译和运行

在仓库根目录执行：

```powershell
dotnet restore
dotnet build AntFlowCore.sln
dotnet run --project src\AntFlowCore.Web\AntFlowCore.Web.csproj
```

默认启动地址来自：

```text
src/AntFlowCore.Web/Properties/launchSettings.json
```

后端地址：

```text
http://localhost:8001
```

Swagger：

```text
http://localhost:8001/swagger
```

### 3.5 用户身份怎么传

当前项目没有内置 JWT 鉴权。它通过请求头读取用户：

```http
userId: 1
userName: 张三
```

`HeaderMiddleware` 会把它放入线程上下文，后续 `SecurityUtils.GetLogInEmpId()` 等方法会读取这个用户。

这对 Demo 很方便，但生产必须接入你自己的登录鉴权，风险见最后一章。

## 4. 核心运行流程

你可以把一次审批理解成 6 步：

1. 管理员在前端设计流程模板。
2. 前端调用 `POST /BpmnConf/Edit` 保存模板。
3. 管理员调用 `GET /BpmnConf/effectiveBpmn/{id}` 或在页面点击启用，让模板生效。
4. 用户发起业务表单，调用 `POST /BpmnConf/process/buttonsOperation?formCode=xxx`。
5. 引擎根据模板和条件，筛出本次实际流转节点，生成任务。
6. 审批人继续调用 `POST /BpmnConf/process/buttonsOperation?formCode=xxx` 进行同意、驳回、转办等。

核心接口：

| 场景 | 接口 |
| --- | --- |
| 新增/编辑流程模板 | `POST /BpmnConf/Edit` |
| 流程模板列表 | `POST /BpmnConf/listPage` |
| 查看模板详情 | `GET or POST /BpmnConf/detail/{id}` |
| 启用模板 | `GET /BpmnConf/effectiveBpmn/{id}` |
| 发起、同意、驳回等统一操作 | `POST /BpmnConf/process/buttonsOperation?formCode=xxx` |
| 流程预览 | `POST /BpmnConf/preview` |
| 我的待办/已办列表 | `POST /BpmnConf/process/listPage/{type}` |
| 查看审批记录 | `GET /BpmnConf/getBpmVerifyInfoVos?processNumber=xxx` |
| 外部系统发起 | `POST /outSide/processSubmit` |

## 5. 低代码流程怎么建

低代码流程适合简单表单，不想写业务代码时使用。

### 5.1 页面操作

1. 登录前端。
2. 进入流程模板设计。
3. 新增流程模板，填写名称和唯一 `formCode`。
4. 进入流程设计 LF。
5. 先设计表单字段。
6. 再设计流程节点。
7. 如果有条件分支，条件字段来自刚才设计的表单字段。
8. 保存模板。
9. 在流程设计列表里启用模板。
10. 到发起请求里选择该低代码流程发起。

### 5.2 低代码条件编码

条件类型来自 `ConditionTypeEnum`：

| 编码 | 名称 | 用法 |
| --- | --- | --- |
| `10000` | 字符串条件 | 字符串相等判断 |
| `10001` | 数字条件 | 大于、小于、等于、区间 |
| `10002` | 日期条件 | 日期判断 |
| `10003` | 日期时间条件 | 当前代码存在风险，见风险清单 |
| `10004` | 集合条件 | 判断配置集合是否包含用户输入 |

设计器提交的条件核心字段：

```json
{
  "columnId": "10001",
  "columnDbname": "amount",
  "showName": "报销金额",
  "optType": 2,
  "zdy1": "1000"
}
```

解释：

- `columnId`：告诉后端使用哪个条件判断器。
- `columnDbname`：业务字段名，发起流程时必须带这个字段。
- `optType`：比较符，见下一节。
- `zdy1`、`zdy2`：条件值，区间判断时会用两个值。

### 5.3 数字比较符

来自 `JudgeOperatorEnum`：

| optType | 含义 |
| --- | --- |
| `1` | `>=` |
| `2` | `>` |
| `3` | `<=` |
| `4` | `<` |
| `5` | `=` |
| `6` | `first < a < second` |
| `7` | `first <= a < second` |
| `8` | `first < a <= second` |
| `9` | `first <= a <= second` |

## 6. DIY 流程怎么新建

DIY 流程适合复杂业务：比如要写自己的业务表、提交时保存主表和明细表、审批通过后更新状态、完成后回调其它系统。

项目里已有一个示例：

```text
src/AntFlowCore.Engine/service/formprocess/ThirdPartyAccountApplyFlowService.cs
```

它的 `formCode` 是：

```csharp
[DIYFormServiceAnno(SvcName = "DSFZH_WMA", Desc = "三方账号申请")]
```

### 6.1 DIY 流程必须包含什么

一个 DIY 流程至少需要：

1. 一个唯一 `formCode`。
2. 一个表单 VO，继承 `BusinessDataVo`。
3. 一个业务表和对应仓储，或你自己的业务保存方式。
4. 一个表单适配器，继承 `AbstractLowFlowSpyFormOperationAdaptor<T>`。
5. 在 `ServiceRegistration.cs` 注册 `IFormOperationAdaptor<T>`。
6. 前端设计流程模板，模板 `formCode` 要和注解 `SvcName` 一致。

### 6.2 第一步：定义 formCode

建议用大写英文、下划线，保持全局唯一：

```text
EXPENSE_APPLY
```

这个值会贯穿前端流程模板、后端适配器、接口调用。

### 6.3 第二步：新建表单 VO

示例：

```csharp
using System.Text.Json.Serialization;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.vo;

public class ExpenseApplyVo : BusinessDataVo
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [JsonPropertyName("departmentId")]
    public string DepartmentId { get; set; }
}
```

注意：

- 必须继承 `BusinessDataVo`。
- 条件字段建议使用简单属性，比如 `Amount`。
- 前端条件里的 `columnDbname` 要能通过反射匹配到属性名。比如 `amount` 可以匹配 `Amount`，因为代码用了忽略大小写查找；但 `department_id` 不会自动匹配 `DepartmentId`。

### 6.4 第三步：新建业务表

你可以用自己的已有业务表，也可以在 AntFlowCore 里新增实体和仓储。

最低要求：

- 发起时能保存业务数据。
- 保存成功后能得到业务主键。
- 把主键写回 `vo.BusinessId`。

引擎会把 `BusinessId` 保存到流程业务关联表里，审批页面查询详情时再带回来。

### 6.5 第四步：新建表单适配器

示例：

```csharp
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.adaptor.formoperation;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.formprocess;

[DIYFormServiceAnno(SvcName = "EXPENSE_APPLY", Desc = "报销申请")]
public class ExpenseApplyFlowService : AbstractLowFlowSpyFormOperationAdaptor<ExpenseApplyVo>
{
    private readonly IExpenseApplyService _expenseApplyService;

    public ExpenseApplyFlowService(
        IExpenseApplyService expenseApplyService,
        IBpmnNodeConditionsConfService bpmnNodeConditionsConfService)
        : base(bpmnNodeConditionsConfService)
    {
        _expenseApplyService = expenseApplyService;
    }

    public override void PreviewSetCondition(BpmnStartConditionsVo conditionsVo, ExpenseApplyVo vo)
    {
        // 通常不用写。基类会自动从流程条件配置里读取字段名，再从 vo 里取值放入 LfConditions。
    }

    public override void LaunchParameters(BpmnStartConditionsVo conditionsVo, ExpenseApplyVo vo)
    {
        // 如果条件字段不在 vo 里，比如发起人部门、岗位、公司，可在这里查库后补充。
        // conditionsVo.StartUserDeptId = ...
    }

    public override void OnInitData(ExpenseApplyVo vo)
    {
    }

    public override void OnQueryData(ExpenseApplyVo vo)
    {
        // 审批人打开审批页时，引擎会带 businessId 进来。
        // 这里根据 vo.BusinessId 查业务表，再把字段填回 vo。
    }

    public override void OnSubmitData(ExpenseApplyVo vo)
    {
        // 发起流程时保存业务数据。
        // 保存后必须设置 BusinessId、ProcessTitle、ProcessDigest。
        var entity = new ExpenseApply
        {
            Amount = vo.Amount,
            Reason = vo.Reason,
            CreateUser = vo.StartUserId
        };

        _expenseApplyService.baseRepo.Insert(entity);

        vo.BusinessId = entity.Id.ToString();
        vo.ProcessTitle = "报销申请";
        vo.ProcessDigest = $"金额：{vo.Amount}";
        vo.EntityName = nameof(ExpenseApply);
    }

    public override void OnConsentData(ExpenseApplyVo vo)
    {
        // 审批同意或重新提交时触发。
        if (vo.OperationType == (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT)
        {
            // 如果打回后重新提交，通常在这里更新业务表。
        }
    }

    public override void OnBackToModifyData(ExpenseApplyVo vo)
    {
        // 被打回修改时触发，可更新业务状态。
    }

    public override void OnCancellationData(ExpenseApplyVo vo)
    {
        // 作废、取消时触发。
    }

    public override void OnFinishData(BusinessDataVo vo)
    {
        // 流程结束时触发，可更新业务状态为“审批通过”。
    }
}
```

### 6.6 第五步：注册 DIY 服务

在：

```text
src/AntFlowCore.AspNetCore/conf/di/serviceregistration/ServiceRegistration.cs
```

增加：

```csharp
services.AddSingleton<IFormOperationAdaptor<ExpenseApplyVo>, ExpenseApplyFlowService>();
services.AddSingleton<ExpenseApplyFlowService>();
services.AddSingleton<IExpenseApplyService, ExpenseApplyService>();
services.AddSingleton<ExpenseApplyService>();
```

当前项目大量服务使用 `AddSingleton`，为了保持一致可以先照这个模式写。如果你的业务服务里持有非线程安全状态，改成 `Scoped` 前要一起检查依赖链。

### 6.7 第六步：前端设计流程模板

在流程设计器里：

1. 新增模板，`formCode` 填 `EXPENSE_APPLY`。
2. 设计节点。
3. 如果要条件审批，比如金额大于 1000 走经理，金额大于 10000 走总监，就添加条件分支。
4. 条件字段 `columnDbname` 填 `amount` 或 `Amount`。
5. 保存模板。
6. 启用模板。

### 6.8 第七步：发起 DIY 流程

请求：

```http
POST /BpmnConf/process/buttonsOperation?formCode=EXPENSE_APPLY
userId: 1
userName: 张三
Content-Type: application/json
```

请求体示例：

```json
{
  "formCode": "EXPENSE_APPLY",
  "operationType": 1,
  "startUserId": "1",
  "startUserName": "张三",
  "amount": 1200,
  "reason": "客户拜访交通费",
  "departmentId": "D001"
}
```

`operationType` 常用值：

| 值 | 含义 |
| --- | --- |
| `0` | 预览 |
| `1` | 提交/发起 |
| `2` | 重新提交 |
| `3` | 同意 |
| `4` | 不同意 |
| `18` | 打回修改 |
| `21` | 转办 |
| `24` | 减签 |
| `25` | 加签 |
| `29` | 撤回 |

### 6.9 第八步：审批 DIY 流程

审批人先查待办：

```http
POST /BpmnConf/process/listPage/1
userId: 审批人ID
userName: 审批人姓名
```

然后审批：

```http
POST /BpmnConf/process/buttonsOperation?formCode=EXPENSE_APPLY
userId: 审批人ID
userName: 审批人姓名
Content-Type: application/json
```

请求体示例：

```json
{
  "formCode": "EXPENSE_APPLY",
  "operationType": 3,
  "processNumber": "流程编号",
  "taskId": "待办任务ID",
  "taskDefKey": "当前节点Key",
  "approvalComment": "同意"
}
```

实际字段以待办列表返回为准。前端通常会把待办里的 `processNumber`、`taskId`、`taskDefKey` 带到审批请求。

## 7. 条件审批怎么设置

条件审批的核心是：流程设计时配置条件，流程发起时提供同名字段。

### 7.1 一个最常见的金额条件

目标：

- 金额 `< 1000`：部门主管审批。
- 金额 `>= 1000` 且 `< 10000`：经理审批。
- 金额 `>= 10000`：总监审批。

流程设计：

1. 发起人节点后添加条件分支。
2. 分支 1：`columnId = 10001`，`columnDbname = amount`，`optType = 4`，`zdy1 = 1000`。
3. 分支 2：`columnId = 10001`，`columnDbname = amount`，`optType = 7`，`zdy1 = 1000`，`zdy2 = 10000`。
4. 分支 3：`columnId = 10001`，`columnDbname = amount`，`optType = 1`，`zdy1 = 10000`。
5. 每个条件分支后接对应审批人节点。
6. 最好设置一个默认分支，避免所有条件都不匹配时报错。

发起流程时必须传：

```json
{
  "amount": 1200
}
```

### 7.2 条件字段从哪里来

DIY 流程里，基类会做这件事：

1. 根据 `formCode` 找到已生效流程模板。
2. 查询模板中的条件字段名，也就是 `columnDbname`。
3. 用反射从你的 VO 上取同名属性。
4. 放入 `BpmnStartConditionsVo.LfConditions`。
5. 条件判断器根据 `LfConditions` 判断分支。

所以要注意：

- 条件字段名必须稳定。
- 条件字段必须存在于请求体和 VO。
- 字段值不能为 `null`，否则条件判断会抛业务异常。
- 多分支条件最好配置默认分支。

### 7.3 多条件关系

条件支持组内关系和组间关系：

- 组内 AND：同一组里所有条件都满足。
- 组内 OR：同一组里任一条件满足。
- 组间 AND：所有条件组满足。
- 组间 OR：任一条件组满足。

当前后端字段是：

- `condGroup`：条件组。
- `condRelation`：组内关系。
- `groupRelation`：组间关系。

### 7.4 自定义一种新条件

不建议一开始就新增条件类型。优先用 `10000` 到 `10004` 的通用条件。

如果必须新增，比如“城市等级条件”：

1. 在 `ConditionTypeEnum` 增加一个枚举值。
2. 写一个实现 `IConditionJudge` 的判断器。
3. 在 `ServiceRegistration.cs` 注册 `IConditionJudge`。
4. 前端设计器增加该条件类型，提交对应的 `columnId`。

后端新增条件判断器的核心接口：

```csharp
public interface IConditionJudge
{
    bool Judge(
        string nodeId,
        BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo,
        int group);
}
```

## 8. 审批人规则怎么扩展

内置审批人规则来自 `NodePropertyEnum`：

| 编码 | 规则 |
| --- | --- |
| `2` | 层层审批 |
| `3` | 指定层级审批 |
| `4` | 指定角色 |
| `5` | 指定人员 |
| `6` | HRBP |
| `7` | 自选模块 |
| `8` | 关联业务表 |
| `11` | 外部传入人员 |
| `12` | 发起人 |
| `13` | 直属领导 |

### 8.1 新手推荐做法

不要一开始完整新增审批人规则。推荐“改造一个你不用的规则”。

例如你公司没有 HRBP：

1. 前端把 HRBP 显示名称改成“项目负责人审批”。
2. 后端找到 `HrbpPersonnelProvider`。
3. 把里面查询 HRBP 的逻辑改成查询项目负责人。
4. 确保返回 `BaseIdTranStruVo` 或用户 ID 列表。

### 8.2 必须对接自己的用户系统

最少要改这些：

- `IUserService.QueryUserById`
- `IUserService.QueryUserByIds`
- `IUserService.GetById`
- `IUserService.SelectUserPageList`
- `IRoleService` 里按角色查人的方法

返回统一结构：

```csharp
public class BaseIdTranStruVo
{
    public string Id { get; set; }
    public string Name { get; set; }
}
```

工作流不关心你的用户表有多复杂，只需要知道用户 ID 和姓名。

## 9. 集成到自己的项目

有三种模式。

### 9.1 模式一：AntFlowCore 独立部署，业务系统调用 API

适合你想先快速接入，不想动现有项目结构。

做法：

1. 独立部署 AntFlowCore 后端。
2. 独立部署流程设计器前端。
3. 在 AntFlowCore 中维护流程模板。
4. 你的业务系统保存业务单据后，调用 AntFlowCore 发起流程。
5. 审批结果通过回调或查询接口同步回业务系统。

优点：改造最小。

缺点：用户、权限、菜单、审批状态同步都要通过 API 打通。

### 9.2 模式二：把工作流页面嵌入你的前端，后端仍独立

适合你希望用户在自己的系统里看到流程管理、待办、已办。

做法：

1. 将前端流程页面或独立设计器集成到你的前端。
2. API 地址指向 AntFlowCore 后端。
3. 所有请求带上你的登录用户信息。
4. 后端通过 `IUserService` 对接你的用户系统。

优点：用户体验更完整。

缺点：前端集成工作量更大。

### 9.3 模式三：把 AntFlowCore 当模块嵌入你的 .NET 后端

适合你希望工作流和业务系统部署在同一个后端。

做法：

1. 把这些项目引用到你的解决方案：
   - `AntFlowCore.Api`
   - `AntFlowCore.AspNetCore`
   - `AntFlowCore.Base`
   - `AntFlowCore.Bpmn`
   - `AntFlowCore.Engine`
   - `AntFlowCore.Persist`
   - `AntFlowCore.Persist.api`
   - `AntFlowCore.Abstraction`
2. 在你的 `Program.cs` 里复制必要配置：
   - `AddControllers().AddAFApplicationComponents()`
   - `FreeSqlSet(configuration)`
   - `AddFreeRepository()`
   - `AddScoped<UnitOfWorkManager>()`
   - `AntFlowServiceSetUp(configuration)`
   - `UseMiddleware<TransactionalMiddleware>()`
   - `UseMiddleware<HeaderMiddleware>()`
   - `UseMiddleware<GlobalExceptionMiddleware>()`
   - `MapControllers()`
3. 注册你自己的 `IUserService`、`IRoleService`。
4. 配置数据库连接和迁移脚本。
5. 处理鉴权和 CORS。

示例：

```csharp
builder.Services.AddControllers().AddAFApplicationComponents();
builder.Services.FreeSqlSet(builder.Configuration);
builder.Services.AddFreeRepository();
builder.Services.AddScoped<UnitOfWorkManager>();
builder.Services.AntFlowServiceSetUp(builder.Configuration);

// 如果使用自己的用户系统，需要覆盖默认实现。
builder.Services.AddSingleton<IUserService, YourUserService>();
builder.Services.AddSingleton<IRoleService, YourRoleService>();

app.UseMiddleware<TransactionalMiddleware>();
app.UseMiddleware<HeaderMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();
```

注意：当前 `AntFlowServiceSetUp` 内部已经注册了默认 `IUserService` 和 `IRoleService`。你要确认最终 DI 解析的是自己的实现。必要时调整注册顺序或改造 `ServiceRegistration.cs`。

## 10. 外部系统 API 接入

外部接入入口：

```http
POST /outSide/processSubmit
```

控制器：

```text
src/AntFlowCore.Api/controller/OutSideBpmAccessController.cs
```

适合 AntFlowCore 作为流程中台，业务系统只提交表单数据和人员信息。

常见字段来自 `OutSideBpmAccessBusinessVo`，你需要重点关注：

- `formCode`：流程编码。
- `formDataPc` / `formData`：业务表单 JSON。
- `startUserId`、`startUserName`：发起人。
- `embedNodes`：外部传入的节点或人员。
- `outSideLevelNodes`：外部层级节点。
- `callbackUrl` 或回调配置：流程变化后通知业务系统。

外部模式要特别注意：

- 外部回调密钥不要明文暴露。
- 回调必须做验签。
- 业务系统要保证发起请求幂等，避免重复提交生成多个流程。

## 11. 上线前必须检查的风险点

以下是我根据当前仓库代码看到的重点问题。

### 11.1 严重：没有真正的接口鉴权

代码位置：

- `src/AntFlowCore.Web/Program.cs`
- `src/AntFlowCore.AspNetCore/conf/middleware/HeaderMiddleware.cs`
- `src/AntFlowCore.Base/util/SecurityUtils.cs`

当前没有看到 `AddAuthentication`、`UseAuthentication`、`UseAuthorization`、`[Authorize]`。用户身份来自请求头 `userId` 和 `userName`。

风险：任何人只要能访问接口，就可能伪造用户发起、审批、撤回流程。

建议：

- 接入你自己的 JWT、Cookie、网关鉴权。
- 后端从可信 token 解析用户，不要信任前端传来的 `userId`。
- 所有流程操作接口加权限校验。

### 11.2 严重：默认数据库连接包含公网地址和弱密码

代码位置：

- `src/AntFlowCore.Web/appsettings.json`

当前默认值包含：

```text
server=antflow.top; userid=root; pwd=123456
```

风险：误用默认配置会连接到非预期数据库，也会暴露弱密码习惯。

建议：

- 删除默认真实地址。
- 使用环境变量或密钥管理。
- 生产数据库账号最小权限，不用 root。

### 11.3 严重：CORS 允许任意来源并允许凭证

代码位置：

- `src/AntFlowCore.Web/Program.cs`

当前配置：

```csharp
.SetIsOriginAllowed((host) => true)
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials()
```

风险：浏览器跨域限制基本被放开。如果再叠加 Cookie 或弱鉴权，会扩大攻击面。

建议：

- 生产环境只允许你的前端域名。
- 不需要 Cookie 时去掉 `AllowCredentials()`。

### 11.4 严重：低代码日期时间条件 `10003` 可能不可用

代码位置：

- `src/AntFlowCore.Bpmn/constants/ConditionTypeEnum.cs`
- `src/AntFlowCore.AspNetCore/conf/di/serviceregistration/ServiceRegistration.cs`
- `src/AntFlowCore.Bpmn/adaptor/nodetypecondition/judge/AbstractLFDateTimeConditionJudge.cs`

问题：

- `10003` 的 `ConditionJudgeClass` 指向抽象类 `AbstractLFDateTimeConditionJudge`。
- DI 注册的是 `LFDateTimeConditionJudge`。
- 条件服务按类型精确匹配，可能找不到服务。
- 区间日期里 `dateFromDb2` 解析后没有赋值，区间判断也可能不正确。

建议：

- 上线前修复后再使用 `10003`。
- 暂时使用 `10002` 日期条件或 `10001` 数字条件替代。

### 11.5 高：异常响应 HTTP 状态码总是 200

代码位置：

- `src/AntFlowCore.AspNetCore/conf/middleware/GlobalExceptionMiddleware.cs`

当前异常会返回业务失败结构，但 HTTP 状态码仍是 `200 OK`。

风险：网关、监控、前端拦截器、调用方可能误判为成功。

建议：

- 业务异常可用 400。
- 未授权用 401/403。
- 系统异常用 500。
- 至少在统一响应里让调用方稳定判断 `success` 或 `code`。

### 11.6 高：SQL 会直接打印到控制台

代码位置：

- `src/AntFlowCore.AspNetCore/conf/freesql/FreeSqlSetUp.cs`

当前启用了：

```csharp
UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}"))
```

风险：生产日志可能泄露业务数据、用户数据、审批内容。

建议：

- 生产关闭 SQL 明文输出。
- 如需审计，使用脱敏日志。

### 11.7 高：外部回调密钥和 token 明文存储

代码位置：

- `src/AntFlowCore.Base/entity/OutSideBpmApproveTemplate.cs`
- `src/AntFlowCore.Base/entity/OutSideBpmCallbackUrlConf.cs`
- `src/AntFlowCore.Engine/factory/ThirdPartyCallbackFactory.cs`

风险：数据库泄露后回调密钥直接泄露。当前回调签名使用 MD5，也偏弱。

建议：

- 密钥加密存储。
- 回调签名改为 HMAC-SHA256。
- 增加时间戳、nonce、防重放。

### 11.8 中：DIY 适配器查找有早退风险

代码位置：

- `src/AntFlowCore.Base/factory/tagparser/ActivitiTagParser.cs`

当前遍历所有 `IFormOperationAdaptor<>` 服务时，如果遇到一个没有 `[DIYFormServiceAnno]` 的服务，会直接 `return null`。

风险：后续新增某个未标注的适配器，可能导致其它正常 DIY 流程也找不到适配器。

建议：

- 把 `return null` 改成 `continue`。
- 新增 DIY 服务时必须加 `[DIYFormServiceAnno]`。

### 11.9 中：多条件字段可能共用第一个操作符

代码位置：

- `src/AntFlowCore.Bpmn/adaptor/nodetypecondition/judge/AbstractLFConditionJudge.cs`

当前 `iterIndex` 初始化为 0，但循环内没有递增。

风险：同一组里多个条件字段时，可能都使用第一个条件的 `optType`。

建议：

- 循环末尾增加 `iterIndex++`。
- 增加多字段条件测试。

### 11.10 中：项目缺少测试工程

当前解决方案里没有看到独立测试项目。`dotnet build AntFlowCore.sln` 能编译成功，但警告非常多。

建议至少补这些测试：

- DIY 流程发起成功。
- 条件分支命中。
- 默认分支命中。
- 审批同意流转。
- 打回后重新提交。
- 外部接入回调验签。

### 11.11 中：.NET 版本说明不一致

README 徽章写 `.NET 9.0`，环境文档同时提到 `.NET 10` 和 `ASP.NET Core .NET 9`，实际项目目标框架是 `net10.0`。

建议：

- 统一文档。
- 生产使用正式版 SDK。
- 添加 `global.json` 固定 SDK 版本。

### 11.12 法务风险：许可证描述不完全一致

README 有 Apache 2.0 徽章，但正文又写了“禁止将源码二次开源”等限制性说明。

建议：

- 商用、二次分发、二次开源前先确认作者授权和 LICENSE 条款。
- 公司项目上线前让法务确认。

## 12. 推荐学习顺序

1. 先跑通后端和 Swagger。
2. 用现有前端建一个低代码流程。
3. 看懂 `ThirdPartyAccountApplyFlowService`。
4. 仿照它做一个自己的 DIY 流程。
5. 给 DIY 流程加一个金额条件。
6. 替换 `IUserService` 和 `IRoleService`。
7. 最后再考虑自定义审批人规则、自定义条件、外部回调。

## 13. 常见问题

### Q1：Swagger 是前端吗？

不是。Swagger 只是后端 API 文档。完整前端需要从 README 提到的前端仓库获取。

### Q2：我只想加一个新业务审批，必须懂工作流引擎吗？

不必须。按 DIY 模式做时，你主要写一个 `XXXFlowService`，在几个钩子里处理业务数据。流程流转由框架处理。

### Q3：为什么发起流程后审批页查不到业务数据？

通常是 `OnSubmitData` 没有设置 `vo.BusinessId`，或者 `OnQueryData` 没有根据 `BusinessId` 查业务表并回填 VO。

### Q4：条件分支不生效怎么办？

检查：

- 流程模板是否已启用。
- `formCode` 是否一致。
- 条件字段 `columnDbname` 是否和 VO 属性匹配。
- 发起请求是否传了该字段。
- 字段值是否为 `null`。
- 是否设置了默认分支。

### Q5：审批人找不到怎么办？

检查：

- 指定人员或角色是否存在。
- `IUserService.QueryUserByIds` 是否能返回用户。
- `IRoleService` 是否能按角色返回用户。
- 发起人 ID 是否能查到组织关系。
- 是否配置了“审批人不存在时转交管理员”。

### Q6：我应该优先用低代码还是 DIY？

简单表单、没有复杂业务保存逻辑，用低代码。

复杂表单、要保存多张业务表、要审批完成后改业务状态，用 DIY。

