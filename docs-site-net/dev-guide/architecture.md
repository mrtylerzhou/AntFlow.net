# AntFlowCore .NET 架构设计

## 1. 概述

AntFlowCore 是一个基于 .NET 10 的轻量级工作流引擎，采用分层架构设计，支持低代码流程配置、灵活的审批人分配策略、多条件分支判断和多渠道通知。引擎核心设计思想是**不侵入业务系统**，通过适配器模式与外部业务系统无缝集成。

## 2. 项目结构

```
AntFlowCore/
├── AntFlowCore.Abstraction/        # 抽象层 - 定义核心接口和模型
├── AntFlowCore.AspNetCore/         # ASP.NET Core 配置（中间件、DI扩展）
├── AntFlowCore.Api/                # API 控制器层
├── AntFlowCore.Base/               # 基础层 - 实体、DTO、VO、工具类
├── AntFlowCore.Bpmn/               # BPMN 核心逻辑 - 节点处理、条件、通知适配器
├── AntFlowCore.Business/           # 业务服务实现
├── AntFlowCore.Core/               # 核心 VO 和共享模型
├── AntFlowCore.Engine/             # 引擎核心 - 流程操作、审批服务
├── AntFlowCore.Engine.Abstraction/ # 引擎抽象层
├── AntFlowCore.Persist/            # 持久化实现（FreeSQL）
├── AntFlowCore.Persist.api/        # 持久化接口定义
├── AntFlowCore.VirtualNode/        # 虚拟节点服务
└── AntFlowCore.Web/                # 入口项目（Program.cs）
```

## 3. 入口点与中间件管道

### 3.1 Program.cs 启动流程

`Program.cs` 是整个应用的入口点，负责构建 Web 应用、注册服务和配置中间件管道。

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        // 1. 初始化枚举类型
        EnumBase<LFFieldTypeEnum>.InitializeEnumBaseTypes();
        
        // 2. 创建 Web 应用构建器
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ServiceCollectionHolder.SetServiceCollection(builder.Services);
        ServiceProviderUtils.Initialize(builder.Services);
        
        // 3. 注册控制器和 JSON 序列化
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers().AddAFApplicationComponents();
        
        // 4. 注册 Swagger/OpenAPI
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // 5. 配置跨域
        builder.Services.AddCors(options => {
            options.AddPolicy("CorsPolicy", bd => bd
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });
        
        // 6. 配置 JSON 序列化（多种转换器处理兼容）
        builder.Services.AddControllers().AddJsonOptions(options => {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
            options.JsonSerializerOptions.Converters.Add(new StringOrIntConverter());
            // ... 更多类型转换器
        });
        
        // 7. 注册 FreeSQL ORM
        builder.Services.FreeSqlSet(builder.Configuration);
        builder.Services.AddFreeRepository();
        builder.Services.AddScoped<UnitOfWorkManager>();
        
        // 8. 注册 AntFlow 引擎核心服务
        builder.Services.AntFlowServiceSetUp(builder.Configuration);
        
        // 9. 构建应用
        WebApplication app = builder.Build();
        
        // 10. 配置中间件管道
        app.UseCors("CorsPolicy");
        app.UseMiddleware<HeaderMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<TransactionalMiddleware>();
        
        // 11. 映射路由
        app.MapControllers();
        app.MapOpenApi();
        
        app.Run();
    }
}
```

### 3.2 中间件管道

AntFlowCore 使用三个核心中间件（按执行顺序）：

| 中间件 | 作用 |
|--------|------|
| `HeaderMiddleware` | 解析请求头，提取租户信息、用户身份等上下文数据 |
| `GlobalExceptionMiddleware` | 全局异常捕获与处理，统一错误响应格式 |
| `TransactionalMiddleware` | 自动事务管理，支持 `[Transactional]` 特性标记的控制器方法 |

### 3.3 JSON 序列化策略

引擎注册了多种 JSON 转换器以处理前端可能传递的多种数据格式：

```csharp
options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());      // 日期时间
options.JsonSerializerOptions.Converters.Add(new StringOrIntConverter());        // 字符串或整数
options.JsonSerializerOptions.Converters.Add(new BooleanJsonConverter());        // 布尔值
options.JsonSerializerOptions.Converters.Add(new NullAbleBooleanJsonConverter()); // 可空布尔
options.JsonSerializerOptions.Converters.Add(new BooleanToIntJsonConverter());   // 布尔转整数
options.JsonSerializerOptions.Converters.Add(new GlobalNullableIntConverter());  // 全局可空整数
```

## 4. 核心实体模型

### 4.1 BpmnConf（流程配置）

流程模板的核心实体，代表一个审批流程的完整定义：

```csharp
public class BpmnConf
{
    public long Id { get; set; }                    // 自增ID
    public string BpmnCode { get; set; }            // 流程编码（唯一标识）
    public string BpmnName { get; set; }            // 流程名称
    public int? BpmnType { get; set; }              // 流程类型
    public string FormCode { get; set; }             // 关联表单编码
    public int? AppId { get; set; }                 // 应用ID
    public int? DeduplicationType { get; set; }      // 去重类型
    public int EffectiveStatus { get; set; }         // 生效状态（0=未生效，1=已生效）
    public int IsAll { get; set; }                   // 是否全员适用
    public int? IsOutSideProcess { get; set; }       // 是否第三方流程
    public int? IsLowCodeFlow { get; set; }          // 是否低代码流程
    public long? BusinessPartyId { get; set; }       // 业务方ID
    public string ConfConfigJson { get; set; }       // 流程级JSON配置
    // ... 审计字段（CreateUser, CreateTime, UpdateUser, UpdateTime, IsDel, TenantId）
}
```

**关键业务规则**：流程模板设计完成后默认**不生效**，管理员需手动点击"生效"按钮，防止配置错误直接上线。

### 4.2 BpmnNode（流程节点）

```csharp
public class BpmnNode
{
    public long Id { get; set; }
    public long ConfId { get; set; }                 // 所属流程配置ID
    public string NodeId { get; set; }               // 节点ID（BPMN元素ID）
    public int NodeType { get; set; }                // 节点类型
    public int NodeProperty { get; set; }            // 节点属性（审批人类型）
    public string NodeFrom { get; set; }             // 来源节点
    public int BatchStatus { get; set; }             // 批次状态
    public int ApprovalStandard { get; set; }        // 审批标准
    public string NodeName { get; set; }             // 节点名称
    public string NodeDisplayName { get; set; }       // 节点显示名称
    public int IsDeduplication { get; set; }         // 是否去重
    public int IsSignUp { get; set; }                // 是否加签
    public string NodeConfigJson { get; set; }       // 节点级JSON配置
    public bool? IsDynamicCondition { get; set; }    // 是否动态条件
    public bool? IsParallel { get; set; }            // 是否并行
    public int? IsOutSideProcess { get; set; }       // 是否外部流程
    public int? IsLowCodeFlow { get; set; }          // 是否低代码流程
}
```

### 4.3 BpmnNodeTo（节点连线关系）

```csharp
public class BpmnNodeTo
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }   // 节点ID
    public string NodeTo { get; set; }     // 目标节点ID
    public int IsDel { get; set; }         // 删除标记
}
```

### 4.4 BpmBusinessProcess（业务流程实例）

运行时流程实例记录，关联 Activiti 引擎和业务数据：

```csharp
public class BpmBusinessProcess
{
    public long Id { get; set; }
    public string ProcessinessKey { get; set; }   // 流程Key
    public string BusinessId { get; set; }         // 业务ID
    public string BusinessNumber { get; set; }     // 业务编号
    public string EntryId { get; set; }            // 入口ID
    public string Version { get; set; }            // 版本
    public int ProcessState { get; set; }          // 状态：1=已通过 2=审批中 3=已撤销
    public string ProcInstId { get; set; }         // Activiti 流程实例ID（关键关联字段）
    public string BackUserId { get; set; }         // 退回人ID
    public int IsOutSideProcess { get; set; }      // 是否外部流程
    public int IsLowCodeFlow { get; set; }         // 是否低代码流程
}
```

### 4.5 BpmVerifyInfo（审批记录）

记录每个节点的审批操作历史：

```csharp
public class BpmVerifyInfo
{
    public long Id { get; set; }
    public string RunInfoId { get; set; }       // 流程实例ID
    public string VerifyUserId { get; set; }    // 审批人ID
    public string VerifyUserName { get; set; }  // 审批人姓名
    public int VerifyStatus { get; set; }       // 审批状态：1=提交 2=同意 3=不同意
    public string VerifyDesc { get; set; }      // 审批意见
    public DateTime? VerifyDate { get; set; }   // 审批日期
    public string TaskName { get; set; }        // 任务名称
    public string TaskId { get; set; }          // 任务ID
    public string TaskDefKey { get; set; }      // 任务定义Key
    public string ProcessCode { get; set; }     // 流程编码
    public string OriginalId { get; set; }      // 原始审批人ID
    public string? AttachmentsJson { get; set; } // 附件JSON
}
```

## 5. API 层设计

### 5.1 BpmnConfController 核心 API

`BpmnConfController` 是流程配置的核心 API 控制器：

| API 端点 | 方法 | 说明 |
|----------|------|------|
| `POST /BpmnConf/Edit` | 新增/编辑流程模板 | 接收 BpmnConfVo，在事务中执行 |
| `POST /BpmnConf/listPage` | 流程模板列表 | 分页查询流程配置 |
| `GET /BpmnConf/detail/{id}` | 流程模板详情 | 查看完整流程配置（含节点） |
| `GET /BpmnConf/effectiveBpmn/{id}` | 生效流程模板 | 管理员手动激活流程 |
| `POST /BpmnConf/preview` | 流程预览 | 预览流程运行时的节点结构 |
| `POST /BpmnConf/startPagePreviewNode` | 发起页预览 | 发起页/任务页预览 |
| `POST /BpmnConf/process/buttonsOperation` | 流程操作核心 | 同意/拒绝/撤回/加签/变更处理人等 |
| `POST /BpmnConf/process/viewBusinessProcess` | 查看业务流程 | 获取业务数据与流程信息 |
| `POST /BpmnConf/process/listPage/{type}` | 待办/已办列表 | 按类型查询流程任务列表 |
| `GET /BpmnConf/getBpmVerifyInfoVos` | 审批路径查询 | 查看流程审批历史 |
| `GET /BpmnConf/todoList` | 统计信息 | 用户待办统计 |
| `POST /BpmnConf/loadNodeOperationUser` | 加载节点操作用户 | 获取节点可操作用户列表 |

### 5.2 流程操作核心方法

`ButtonsOperation` 是流程操作的统一入口，通过策略模板实现处理逻辑分发：

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

## 6. 依赖注入注册

`ServiceRegistration.AntFlowServiceSetUp` 是引擎的核心 DI 注册入口，包含以下服务注册：

### 6.1 核心服务注册

| 服务类型 | 说明 |
|----------|------|
| `IBpmnConfService` | 流程配置服务 |
| `IBpmnConfBizService` | 流程配置业务服务 |
| `IBpmnConfCommonService` | 流程配置公共服务 |
| `IProcessApprovalService` | 流程审批核心服务 |
| `IButtonOperationService` | 按钮操作服务 |
| `IFormOperationAdaptor<T>` | 表单操作适配器 |
| `IBpmnPersonnelProviderService` | 审批人提供者服务 |
| `IBpmnNodeConditionsAdaptor` | 节点条件适配器 |
| `IProcessNoticeAdaptor` | 流程通知适配器 |
| `IProcessOperationAdaptor` | 流程操作适配器 |
| `IConditionJudge` | 条件判断服务 |
| `IAFDeploymentService` | Activiti 部署服务 |
| `IAFTaskService` | Activiti 任务服务 |
| `IRepositoryService` | Activiti 仓库服务 |
| `IRuntimeService` | Activiti 运行时服务 |

### 6.2 审批人提供者注册

引擎内置了 14 种审批人提供者：

| 提供者 | 说明 |
|--------|------|
| `DirectLeaderPersonnelProvider` | 直属领导审批 |
| `CustomizePersonnelProvider` | 指定人员审批 |
| `HrbpPersonnelProvider` | HRBP 审批 |
| `LevelPersonnelProvider` | 级别审批 |
| `LoopPersonnelProvider` | 循环审批 |
| `OutSidePersonnelProvider` | 外部系统审批 |
| `RolePersonnelProvider` | 角色审批 |
| `StartUserPersonnelProvider` | 发起人审批 |
| `UserPointedPersonnelProvider` | 用户指定审批 |
| `BusinessTablePersonnelProvider` | 业务表字段审批 |
| `FormRelatedPersonnelProvider` | 表单关联审批 |
| `UDRPersonnelProvider` | 用户自定义审批 |
| `PrevNodeRelatedPersonnelProvider` | 上一节点关联审批 |
| `ApprovedUserPersonnelProvider` | 已审批人审批 |

## 7. 核心设计理念

### 7.1 分层架构

```
┌─────────────────────────────────────────────┐
│              API 层 (Controllers)            │
├─────────────────────────────────────────────┤
│           引擎核心层 (Engine/Core)           │
│  - ProcessApprovalService                    │
│  - ButtonOperationService                    │
│  - 各种 Processor                            │
├─────────────────────────────────────────────┤
│        BPMN 核心层 (Bpmn Adaptors)           │
│  - Personnel Adaptor (审批人)                │
│  - Condition Adaptor (条件)                  │
│  - Element Adaptor (节点元素)                │
│  - Notice Adaptor (通知)                     │
├─────────────────────────────────────────────┤
│         抽象层 (Abstraction)                  │
│  - 接口定义、DTO、VO、枚举                   │
├─────────────────────────────────────────────┤
│         持久层 (Persist)                      │
│  - FreeSQL Repository                        │
├─────────────────────────────────────────────┤
│         基础层 (Base)                         │
│  - 实体、常量、异常、工具类                  │
└─────────────────────────────────────────────┘
```

### 7.2 适配器模式

引擎的核心扩展机制，通过适配器实现与外部系统的解耦：

1. **Personnel Adaptor（审批人适配器）** - 确定节点的审批人
2. **Condition Adaptor（条件适配器）** - 判断分支条件是否满足
3. **Element Adaptor（节点元素适配器）** - 处理节点元素的添加和修改
4. **Process Notice Adaptor（通知适配器）** - 发送审批通知
5. **Process Operation Adaptor（流程操作适配器）** - 处理流程操作（同意、拒绝等）
6. **Form Operation Adaptor（表单操作适配器）** - 与业务表单交互

### 7.3 模板方法模式

抽象类提供骨架实现，具体业务类重写关键方法：

- `AbstractBpmnPersonnelAdaptor` - 审批人查找模板
- `AbstractMessageSendAdaptor<T>` - 消息发送模板
- `AbstractCommonBpmnElementAdaptor` - BPMN 元素处理模板

## 8. 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 运行时框架 |
| ASP.NET Core | 10.0 | Web API |
| FreeSQL | - | ORM（支持 MySQL、PostgreSQL、SQL Server 等） |
| Activiti | - | 工作流引擎 |
| Swagger/OpenAPI | - | API 文档 |
| System.Text.Json | - | JSON 序列化 |
