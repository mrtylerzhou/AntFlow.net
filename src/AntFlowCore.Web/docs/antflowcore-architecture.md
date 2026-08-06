# AntFlowCore Technical Architecture Document

> **Version:** 1.0
> **Date:** 2026-07-26
> **Target Runtime:** .NET 10.0
> **Database:** MySQL (via FreeSql ORM)
> **Framework:** ASP.NET Core Web API

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [Solution Structure & Layered Architecture](#3-solution-structure--layered-architecture)
4. [Core Domain Model](#4-core-domain-model)
5. [API Endpoints](#5-api-endpoints)
6. [Dependency Injection Registration](#6-dependency-injection-registration)
7. [Middleware Pipeline](#7-middleware-pipeline)
8. [BPMN Adaptor System](#8-bpmn-adaptor-system)
9. [Key Design Patterns](#9-key-design-patterns)
10. [Database Schema Overview](#10-database-schema-overview)
11. [Configuration](#11-configuration)

---

## 1. Project Overview

AntFlowCore is an open-source BPMN (Business Process Model and Notation) workflow engine built in C# / .NET. It is a .NET port of the Java-based [AntFlow](https://gitee.com/antswarm/antflowcore) project. The engine enables designing, configuring, and running approval workflows (审批流) without code changes — process templates are created through a visual designer and stored as JSON configuration.

### Key Capabilities

- **Visual process design** with BPMN 2.0-style node modeling
- **Multiple sign/approval modes**: sequential sign (会签), OR sign (或签), ordered sign (顺序会签)
- **Configurable approver resolution**: 14 built-in personnel provider strategies (direct leader, HRBP, role-based, loop, etc.)
- **External system integration**: embedded and API-based third-party BPM access
- **Low-code form integration** (LF = LowCode Flow)
- **Draft, forward, entrust, reassign** runtime operations
- **Email / SMS / App Push notifications**
- **Multi-tenancy** support

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10.0 |
| Web Framework | ASP.NET Core 10.0 Web API |
| ORM | [FreeSql](https://github.com/dotnetcore/FreeSql) (v3.x) with MySQL provider |
| API Documentation | Swashbuckle.AspNetCore (Swagger/OpenAPI 3.0) |
| AOP / Interceptors | [Rougamo.Fody](https://github.com/inversionhourglass/Rougamo.Fody) (IL weaver) |
| Serialization | System.Text.Json with custom converters |
| Email | MailKit / MimeKit |
| Dynamic Compilation | Natasha (Roslyn-based runtime compilation) |
| Logging | Microsoft.Extensions.Logging |

---

## 3. Solution Structure & Layered Architecture

### Project Layout (13 class libraries + 1 entry point)

```
antflowcore/
├── src/
│   ├── AntFlowCore.Web              ← Entry point (Program.cs, middleware wiring)
│   ├── AntFlowCore.Api              ← Controllers (REST API endpoints)
│   ├── AntFlowCore.AspNetCore       ← Middleware, DI extensions, MVC configuration
│   ├── AntFlowCore.Abstraction      ← Service interfaces, AOP base types
│   ├── AntFlowCore.Abstraction.Orm  ← Repository/ORM abstractions
│   ├── AntFlowCore.Base             ← Entities, enums, VOs, BPMN model, utilities
│   ├── AntFlowCore.Bpmn             ← BPMN adaptor system (core extensibility layer)
│   ├── AntFlowCore.Business         ← Business-specific service implementations
│   ├── AntFlowCore.Engine           ← Service implementations (biz services, processors)
│   ├── AntFlowCore.Engine.Abstraction ← Engine DI registration (ServiceRegistration)
│   ├── AntFlowCore.Persist          ← FreeSql repository implementations
│   ├── AntFlowCore.Persist.api      ← Persistence interfaces (repositories, biz interfaces)
│   └── AntFlowCore.VirtualNode      ← Virtual node runtime services
└── script/
    └── bpm_init_db_mysql.sql        ← Full DB schema (80+ tables)
```

### Layered Architecture Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│                    AntFlowCore.Web (Entry Point)                  │
│   Program.cs  ──►  Middleware Pipeline  ──►  Swagger / OpenAPI    │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│                   AntFlowCore.Api (Controllers)                   │
│   BpmnConfController, LowCodeFlowController, UserController,     │
│   OutSideBpmAccessController, ProcessControlController, ...      │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│               AntFlowCore.Abstraction (Interfaces)                │
│   IBpmnConfService, IBpmBusinessProcessService,                  │
│   IBpmnPersonnelProviderService, IProcessOperationAdaptor, ...  │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│            AntFlowCore.Engine / AntFlowCore.Business              │
│   Service implementations, processors, biz logic                 │
│   BpmnConfBizService, ProcessApprovalService,                   │
│   ButtonOperationService, NextNodeTaskProcessor, ...             │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│              AntFlowCore.Bpmn (Adaptor System)                    │
│   Personnel adaptors, condition judges, flow element adaptors,   │
│   process operation adaptors, variable subscribers               │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│         AntFlowCore.VirtualNode / AntFlowCore.Base               │
│   BPMN model (AfUserTask, AFSequenceFlow, gateways),             │
│   entities, enums, value objects, utilities                      │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│   AntFlowCore.Persist / AntFlowCore.Persist.api / Abstraction.Orm │
│   FreeSql repositories, UnitOfWork, ORM context                  │
└──────────────────────────────┬───────────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────────┐
│                        MySQL Database                             │
└──────────────────────────────────────────────────────────────────┘
```

---

## 4. Core Domain Model

### 4.1 Entity Relationship Overview

```
                        ┌─────────────────────┐
                        │     BpmnConf         │  (t_bpmn_conf)
                        │  Process Template    │
                        └──────────┬──────────┘
                                   │ 1:N
                        ┌──────────▼──────────┐
                        │     BpmnNode         │  (t_bpmn_node)
                        │  Process Node        │
                        └──────────┬──────────┘
                                   │ 1:N
                        ┌──────────▼──────────┐
                        │     BpmnNodeTo       │  (t_bpmn_node_to)
                        │  Node Transitions    │
                        └─────────────────────┘

┌──────────────────────┐       ┌──────────────────────────────┐
│  BpmBusinessProcess   │       │  BpmVariable                  │
│  (bpm_business_process)│       │  (t_bpm_variable)             │
│  Runtime Instance     │       │  Process Variables            │
└──────────────────────┘       └──────────────┬───────────────┘
                                              │ 1:N
                               ┌──────────────▼───────────────┐
                               │  BpmVariableMultiplayer       │
                               │  (t_bpm_variable_multiplayer) │
                               └──────────────┬───────────────┘
                                              │ 1:N
                               ┌──────────────▼───────────────┐
                               │  BpmVariableMultiplayerPersonnel│
                               │  (t_bpm_variable_multiplayer_  │
                               │   personnel)                   │
                               └──────────────────────────────┘
```

### 4.2 Key Entities

#### `BpmnConf` (t_bpmn_conf) — Process Template

| Field | Type | Description |
|---|---|---|
| `Id` | long | Auto-increment PK |
| `BpmnCode` | string | Unique process code (e.g., "00001") |
| `BpmnName` | string | Process display name |
| `BpmnType` | int? | Process type |
| `FormCode` | string | Business form code (links to app) |
| `DeduplicationType` | int? | 1=none, 2=forward dedup, 3=backward dedup |
| `EffectiveStatus` | int | 0=inactive, 1=active |
| `IsAll` | int | 0=not for all, 1=for all users |
| `IsOutSideProcess` | int? | Third-party process flag |
| `IsLowCodeFlow` | int? | Low-code flow flag |
| `ConfConfigJson` | string | Consolidated process-level JSON config |

#### `BpmnNode` (t_bpmn_node) — Process Node

| Field | Type | Description |
|---|---|---|
| `Id` | long | PK |
| `ConfId` | long | FK → BpmnConf |
| `NodeId` | string | BPMN node identifier |
| `NodeType` | int | See `NodeTypeEnum` |
| `NodeProperty` | int | Approver resolution rule (`NodePropertyEnum`) |
| `NodeFrom` | string | Previous node ID |
| `NodeFroms` | string | All previous node IDs |
| `IsDeduplication` | int | Dedup flag |
| `IsSignUp` | int | Sign-up allowed |
| `IsDynamicCondition` | bool? | Dynamic condition node |
| `IsParallel` | bool? | Parallel gateway |
| `NodeConfigJson` | string | Consolidated node-level JSON config |

#### `BpmBusinessProcess` (bpm_business_process) — Runtime Process Instance

| Field | Type | Description |
|---|---|---|
| `Id` | long | PK |
| `ProcessinessKey` | string | Process key |
| `BusinessId` | string | Business record ID |
| `BusinessNumber` | string | Process number |
| `ProcInstId` | string | Activiti process instance ID |
| `ProcessState` | int | 1=approving, 2=approved, 3=canceled, 6=rejected |
| `IsOutSideProcess` | int | External process flag |
| `IsLowCodeFlow` | int | Low-code flow flag |

### 4.3 Key Enums

#### `ProcessStateEnum`

| Value | Name | Description |
|---|---|---|
| 1 | HANDLING_STATE | 审批中 (In Progress) |
| 2 | HANDLE_STATE | 审批通过 (Approved) |
| 3 | END_STATE | 作废 (Canceled) |
| 6 | REJECT_STATE | 审批拒绝 (Rejected) |

#### `SignTypeEnum`

| Value | Name | Description |
|---|---|---|
| 1 | SIGN_TYPE_SIGN | 会签 (All must approve, any order) |
| 2 | SIGN_TYPE_OR_SIGN | 或签 (Only one needs to decide) |
| 3 | SIGN_TYPE_SIGN_IN_ORDER | 顺序会签 (All must approve, in order) |

### 4.4 BPMN Model Class Hierarchy

```
AFAbstractBaseElement
  └── AbstractFlowElement
        ├── AbstractFlowNode
        │     ├── AfExclusiveGateway
        │     ├── AFParallelGateway
        │     ├── AbstractActivity
        │     │     ├── AbstractTask
        │     │     │     ├── AfUserTask
        │     │     │     └── UserTask
        │     │     └── (service tasks, etc.)
        │     ├── StartEvent
        │     └── EndEvent
        └── AFSequenceFlow
```

---

## 5. API Endpoints

### 5.1 BpmnConfController (`/BpmnConf`)

| Method | Route | Description |
|---|---|---|
| POST | `/BpmnConf/Edit` | Create or update a process template |
| POST | `/BpmnConf/process/buttonsOperation` | Core: approve/reject/transfer/etc. (strategy pattern) |
| POST | `/BpmnConf/listPage` | Paginated process template list |
| POST | `/BpmnConf/preview` | Preview process node tree |
| POST | `/BpmnConf/startPagePreviewNode` | Start page preview (start vs task) |
| GET | `/BpmnConf/getBpmVerifyInfoVos` | Get approval path info |
| POST | `/BpmnConf/process/viewBusinessProcess` | View business process info |
| POST | `/BpmnConf/process/listPage/{type}` | Todo/done/CC list (type-based) |
| GET/POST | `/BpmnConf/detail/{id}` | Process template detail |
| GET | `/BpmnConf/effectiveBpmn/{id}` | Activate a process template |
| GET | `/BpmnConf/todoList` | Process statistics for current user |
| POST | `/BpmnConf/loadNodeOperationUser` | Load node operation users |

### 5.2 LowCodeFlowController (`/lowcode`)

| Method | Route | Description |
|---|---|---|
| POST | `/lowcode/createLowCodeFormCode` | Create low-code form code |
| GET | `/lowcode/getLowCodeFlowFormCodes` | List all low-code form codes |
| POST | `/lowcode/getLFFormCodePageList` | Paginated LF form codes |
| POST | `/lowcode/getLFActiveFormCodePageList` | Paginated active LF form codes |
| GET | `/lowcode/getformDataByFormCode` | Get LF form data by form code |

### 5.3 BpmBusinessController (`/bpmnBusiness`)

| Method | Route | Description |
|---|---|---|
| GET | `/bpmnBusiness/getDIYFormCodeList` | Get DIY form code list |
| POST | `/bpmnBusiness/entrustlist/{type}` | Entrust list (paginated) |
| GET | `/bpmnBusiness/entrustDetail/{id}` | Entrust detail |
| POST | `/bpmnBusiness/editEntrust` | Edit entrust |
| GET | `/bpmnBusiness/getStartUserChooseModules` | Get nodes with custom approver property |

### 5.4 UserController (`/user`)

| Method | Route | Description |
|---|---|---|
| GET | `/user/getUser` | Get all users |
| GET | `/user/getRoleInfo` | Get all roles |
| POST | `/user/getUserPageList` | Paginated user list |

### 5.5 OutSideBpmAccessController (`/outSide`)

| Method | Route | Description |
|---|---|---|
| POST | `/outSide/processSubmit` | External workflow submission |
| POST | `/outSide/getOutSideFormCodePageList` | Paginated external form codes |
| POST | `/outSide/processBreak` | Break/terminate external process |
| GET | `/outSide/outSideProcessRecord` | External process records |

### 5.6 OutSideBpmBusinessController (`/outSideBpm`)

| Method | Route | Description |
|---|---|---|
| POST | `/outSideBpm/businessParty/listPage` | Business party list |
| GET | `/outSideBpm/businessParty/detail/{id}` | Business party detail |
| POST | `/outSideBpm/businessParty/edit` | Edit business party |
| POST | `/outSideBpm/businessParty/applicationsPageList` | Applications list |
| POST | `/outSideBpm/businessParty/addBpmProcessAppApplication` | Add application |
| GET | `/outSideBpm/businessParty/applicationDetail/{id}` | Application detail |
| GET | `/outSideBpm/conditionTemplate/listPage` | Condition templates |
| GET | `/outSideBpm/conditionTemplate/selectListByTemp/{applicationId}` | Conditions by app |
| POST | `/outSideBpm/conditionTemplate/edit` | Edit condition template |
| GET | `/outSideBpm/conditionTemplate/delete/{id}` | Delete condition template |
| GET | `/outSideBpm/approveTemplate/listPage` | Approve templates |
| GET | `/outSideBpm/approveTemplate/selectListByTemp/{applicationId}` | Approve templates by app |
| POST | `/outSideBpm/approveTemplate/edit` | Edit approve template |
| GET | `/outSideBpm/approveTemplate/detail/{id}` | Approve template detail |

### 5.7 OutSideBpmCallbackUrlConfController (`/outSideBpm`)

| Method | Route | Description |
|---|---|---|
| GET | `/outSideBpm/callbackUrlConf/list/{formCode}` | Callback configs by form code |
| GET | `/outSideBpm/callbackUrlConf/detail/{id}` | Callback config detail |
| POST | `/outSideBpm/callbackUrlConf/edit` | Edit callback config |

### 5.8 InformationTemplateController (`/informationTemplates`)

| Method | Route | Description |
|---|---|---|
| POST | `/informationTemplates/listPage` | Message template list |
| GET | `/informationTemplates/getInformationTemplateById` | Template by ID |
| POST | `/informationTemplates/updateById` | Update template |
| POST | `/informationTemplates/save` | Save template |
| POST | `/informationTemplates/deleteById` | Delete template |
| GET | `/informationTemplates/listByName` | Templates by name |
| GET | `/informationTemplates/defaultTemplates` | Default templates |
| POST | `/informationTemplates/defaultTemplates` | Set default templates |
| GET | `/informationTemplates/getWildcardCharacte` | Wildcard characters |
| GET | `/informationTemplates/getProcessEvents` | All process events |
| GET | `/informationTemplates/getAllNoticeTypes` | All notice types |

### 5.9 ProcessDraftController (`/processDraft`)

| Method | Route | Description |
|---|---|---|
| GET | `/processDraft/loadDraft` | Load saved draft for current user |

### 5.10 ProcessControlController (`/taskMgmt`)

| Method | Route | Description |
|---|---|---|
| POST | `/taskMgmt/taskMgmt` | Save process notices config |
| GET | `/taskMgmt/getFormRelatedOptions` | Form-related assignee options |
| GET | `/taskMgmt/getUDROptions` | User-defined rule options |

---

## 6. Dependency Injection Registration

All service registrations are centralized in `ServiceRegistration.AntFlowServiceSetUp()` (`AntFlowCore.Engine.Abstraction`). The method registers **100+ services** as singletons.

### Registration Categories

#### Core Services
- `IBpmnConfService`, `IBpmnConfBizService`, `IBpmnConfCommonService`
- `IBpmnNodeService`, `IBpmnNodeToService`
- `IBpmBusinessProcessService`, `IBpmProcessAppApplicationService`
- `IProcessApprovalService`, `IButtonOperationService`
- `ITaskMgmtService`, `ITaskService`

#### Personnel Providers (14 strategies)
All registered as `IBpmnPersonnelProviderService`:
- `DirectLeaderPersonnelProvider` — 直属领导
- `HrbpPersonnelProvider` — HRBP
- `LevelPersonnelProvider` — 层级审批
- `LoopPersonnelProvider` — 循环审批
- `RolePersonnelProvider` — 角色审批
- `StartUserPersonnelProvider` — 发起人
- `UserPointedPersonnelProvider` — 指定人员
- `CustomizePersonnelProvider` — 自定义审批人
- `OutSidePersonnelProvider` — 外部审批人
- `BusinessTablePersonnelProvider` — 业务表人员
- `FormRelatedPersonnelProvider` — 表单相关人员
- `UDRPersonnelProvider` — 用户自定义规则
- `PrevNodeRelatedPersonnelProvider` — 上一节点相关人员
- `ApprovedUserPersonnelProvider` — 已审批人员

#### Personnel Adaptors (14 matching)
Registered as `AbstractBpmnPersonnelAdaptor` — one-to-one with providers.

#### Process Operation Adaptors (20+)
Registered as `IProcessOperationAdaptor`:
- `SubmitProcessService`, `ResubmitProcessService`, `EndProcessService`
- `ChangeAssigneeProcessService`, `TransferAssigneeProcessService`
- `AddAssigneeProcessService`, `RemoveAssigneeProcessService`
- `AddFutureAssigneeProcessService`, `RemoveFutureAssigneeProcessService`
- `ChangeFutureAssigneeProcessService`
- `UndertakeProcessService`, `BackToModifyService`
- `ProcessForwardService`, `FastForwardProcessService`
- `RemoveCurrentNodeProcessService`, `RemoveFutureNodeProcessService`
- `InsertNodeAfterCurrentOrFutureService`
- `SaveDraftProcessService`, `TaskRecoverProcessSerivce`
- `OutSideAccessSubmitProcessService`

#### Flow Element Adaptors (5)
Registered as `IBpmnAddFlowElementAdaptor`:
- `BpmnAddFlowElementSingleAdaptor`
- `BpmnAddFlowElementLoopAdaptor`
- `BpmnAddFlowElementMultOrSignAaptor`
- `BpmnAddFlowElementSignUpSerialAdaptor`
- `BpmnAddFlowElementMultSignAdaptor`

#### Condition Judges (10)
Registered as `IConditionJudge`:
- `ThirdAccountJudgeService`, `AskLeaveJudge`, `PurchaseTotalMoneyJudge`
- `NumberOperatorJudgeService`, `BpmnTemplateMarkJudge`
- `LFStringConditionJudge`, `LFNumberFormatJudge`
- `LFDateConditionJudge`, `LFDateTimeConditionJudge`
- `LFCollectionConditionJudge`

#### Node Adaptors (15+)
Registered as `IAdaptorService`:
- `NodePropertyPersonnelAdaptor`, `NodePropertyBusinessTableAdaptor`
- `NodePropertyDirectLeaderAdaptor`, `NodePropertyHrbpAdaptor`
- `NodePropertyLevelAdaptor`, `NodePropertyLoopAdaptor`
- `NodePropertyOutSideAccessAdaptor`, `NodePropertyRoleAdaptor`
- `NodePropertyStartUserAdaptor`, `NodePropertyCustomizeAdaptor`
- `NodePropertyFormRelatedAdaptor`, `NodePropertyUDRAdaptor`
- `NodePropertyPrevNodeAdaptor`, `NodePropertyApprovedUsersAdaptor`
- `NodeTypeConditionsAdaptor`

#### BPMN Element Adaptors (14)
Also registered as `IAdaptorService`:
- `BpmnElementBusinessTableAdaptor`, `BpmnElementCustomizeAdaptor`
- `BpmnElementDirectLeaderAdaptor`, `BpmnElementHrbpAdaptor`
- `BpmnElementLevelAdaptor`, `BpmnElementLoopAdaptor`
- `BpmnElementOutSideAccessAdaptor`, `BpmnElementPersonnelAdaptor`
- `BpmnElementRoleAdaptor`, `BpmnElementStartUserAdaptor`
- `BpmnElementPrevNodeAdaptor`, `BpmnElementFormRelatedAdp`
- `BpmnElementUDRAdp`

#### Variable Subscribers (3)
Registered as `IBpmnInsertVariableSubs`:
- `BpmnInsertVariableSubsMultiplayerOrSignAdaptor`
- `BpmnInsertVariableSubsMultiplayerSignAdaptor`
- `BpmnInsertVariableSubsSingleAdaptor`

#### Process Notice Adaptors (3)
Registered as `IProcessNoticeAdaptor`:
- `EmailSendAdaptor`
- `AppPushAdaptor`
- `SMSSendAdaptor`

#### Repositories (30+)
All registered as `Fs*Repository` implementing `I*Repository` interfaces via FreeSql.

#### Activiti Engine Services
- `IAFDeploymentService` → `AFDeploymentService`
- `IAfTaskInstService` → `AfTaskInstService`
- `IAFTaskService` → `AFTaskService`
- `IAFExecutionService` → `AFExecutionService`
- `RepositoryService`, `RuntimeService`

#### Listeners
- `ITaskListener` → `BpmnTaskListener`
- `IExecutionListener` → `BpmnExecutionListener`
- `IWorkflowButtonOperationHandler` → `AntFlowOperationListener`

#### Next-Node Processors
- `INextNodeTaskProcessor` → `NextNodeLabelsProcessor`, `NextNodeForwardProcessor`, `NextNodeProcessNoticeSendProcessor`

#### Post-Processors
- `IAntFlowOrderPostProcessor<BusinessDataVo>` → `AntFlowButtonsOperationPostProcessor`
- `IAntFlowOrderPostProcessor<BpmnConfVo>` → `LFFieldControlPostProcessor`, `NodeLabelsPostProcessor`

---

## 7. Middleware Pipeline

The middleware pipeline is configured in `Program.cs` in the following order:

```
Incoming Request
      │
      ▼
┌─────────────────────────┐
│  TransactionalMiddleware │  Sets ServiceProvider for TransactionalAttribute AOP
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│    HeaderMiddleware      │  Extracts userId/userName from headers → ThreadLocal
│                          │  Extracts tenantId → ITenantIdHolder
│                          │  Cleans ThreadLocal on response completion
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│ GlobalExceptionMiddleware│  Catches all unhandled exceptions
│                          │  Returns JSON Result<object>.NewFailureResult
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│   Controllers (MVC)      │  Routing → Controller action
└─────────────────────────┘
```

### Middleware Details

#### `TransactionalMiddleware`
- Sets `context.RequestServices` into `TransactionalAttribute` for AOP-based transaction management
- Enables the `[Transactional]` attribute pattern via Rougamo.Fody IL weaving

#### `HeaderMiddleware`
- Reads `userId` / `userName` from HTTP headers (case-insensitive fallback)
- Falls back to `IUserService.GetById()` if userName not in header
- Stores current user in `ThreadLocalContainer` (request-scoped)
- Reads `tenantId` header into `ITenantIdHolder` for multi-tenancy
- Cleans `ThreadLocalContainer` on response completion

#### `GlobalExceptionMiddleware`
- Wraps entire pipeline in try/catch
- Returns all exceptions as JSON: `Result<object>` with code "200" and error message
- HTTP status is always 200 (errors communicated in response body)

---

## 8. BPMN Adaptor System

The adaptor system is the core extensibility mechanism of AntFlowCore. It uses a **registry + factory** pattern where multiple implementations of an interface are registered as singletons, then resolved at runtime by key.

### 8.1 Adaptor Categories

```
AntFlowCore.Bpmn/adaptor/
├── bpmnelementadp/       ← Flow element creation (single, loop, multi-sign, serial)
├── bpmnnodeadp/          ← Node property resolution (15 node types)
├── bpmnprocessnotice/    ← Notification dispatch (email, SMS, push)
├── nodetypecondition/    ← Condition evaluation (account type, purchase, LF fields)
│   └── judge/            ← Individual condition judges
├── personnel/            ← Approver resolution strategies
│   ├── provider/         ← Data providers (query users from DB/org)
│   ├── provideradp/      ← Adaptors wrapping providers
│   └── loopsign/         ← Sequential/ordered sign logic
├── processoperation/     ← Runtime process operations (submit, transfer, reassign...)
├── variable/             ← Variable subscription strategies
└── formoperation/        ← Low-code form operation adaptors
```

### 8.2 Adaptor Resolution Flow

```
Request → Controller
             │
             ▼
    AdaptorFactory (IAdaptorFactory)
             │
             ▼
    Resolve by key (e.g., nodeType + nodeProperty)
             │
             ▼
    Specific Adaptor implementation
             │
             ▼
    Execute → Return result
```

### 8.3 Key Adaptor Interfaces

| Interface | Purpose |
|---|---|
| `IBpmnPersonnelProviderService` | Resolve approver list for a node |
| `AbstractBpmnPersonnelAdaptor` | Adaptor wrapping a provider with common logic |
| `IProcessOperationAdaptor` | Execute a runtime process operation |
| `IBpmnAddFlowElementAdaptor` | Create BPMN flow elements at runtime |
| `IBpmnNodeConditionsAdaptor` | Evaluate node conditions |
| `IConditionJudge` | Judge a single condition expression |
| `IBpmnInsertVariableSubs` | Subscribe variables for multi-instance nodes |
| `IProcessNoticeAdaptor` | Send notifications |
| `IAdaptorService` | General node/element property adaptor |

### 8.4 Adaptor Factory

`AdaptorFactoryProxy.GetProxyInstance()` creates a proxy factory registered as singleton. The factory resolves adaptors by type at runtime, enabling the strategy pattern without direct coupling.

---

## 9. Key Design Patterns

### 9.1 Strategy Pattern (Adaptor System)
The most pervasive pattern. Each personnel provider, process operation, condition judge, and flow element creator is a strategy implementation resolved at runtime by key.

### 9.2 Template Method
`AbstractOrderedSignNodeAdp`, `AbstractNodeAssigneeVoProvider`, `AbstractComparableJudge`, `AbstractLFConditionJudge` define skeletons; subclasses override specific steps.

### 9.3 Chain of Responsibility (Next-Node Processors)
`INextNodeTaskProcessor` implementations (`NextNodeLabelsProcessor`, `NextNodeForwardProcessor`, `NextNodeProcessNoticeSendProcessor`) form a chain executed after node creation.

### 9.4 Post-Processor Pipeline
`IAntFlowOrderPostProcessor<T>` implementations run after main operations (e.g., `LFFieldControlPostProcessor`, `NodeLabelsPostProcessor`, `AntFlowButtonsOperationPostProcessor`).

### 9.5 Proxy Pattern
`AdaptorFactoryProxy` creates a dynamic proxy for `IAdaptorFactory`. Rougamo.Fody provides AOP proxy for transactional methods.

### 9.6 Factory Pattern
`FormFactory`, `BpmnStartFormatFactory`, `BpmnRemoveConfFormatFactory`, `ProcessorFactory`, `ThirdPartyCallbackFactory` encapsulate object creation.

### 9.7 Repository Pattern
All data access goes through `I*Repository` interfaces implemented by `Fs*Repository` classes using FreeSql.

### 9.8 Unit of Work
`UnitOfWorkManager` from FreeSql provides transaction management across repositories.

### 9.9 Observer / Listener Pattern
`ITaskListener`, `IExecutionListener`, `IWorkflowButtonOperationHandler`, `IBpmVariableMessageListenerService` provide lifecycle event hooks.

### 9.10 ThreadLocal Storage
`ThreadLocalContainer` stores per-request user context (set by `HeaderMiddleware`, cleaned on response completion).

---

## 10. Database Schema Overview

The schema contains **40+ active tables** (many legacy tables migrated to JSON columns). Key tables:

### Core Configuration Tables
| Table | Entity | Description |
|---|---|---|
| `t_bpmn_conf` | `BpmnConf` | Process template header |
| `t_bpmn_node` | `BpmnNode` | Process nodes |
| `t_bpmn_node_to` | `BpmnNodeTo` | Node transitions (edges) |
| `t_bpmn_conf_lf_formdata` | `BpmnConfLfFormdata` | Low-code form data |
| `t_bpmn_conf_lf_formdata_field` | `BpmnConfLfFormdataField` | Low-code form fields |

### Runtime Tables
| Table | Entity | Description |
|---|---|---|
| `bpm_business_process` | `BpmBusinessProcess` | Process instance ↔ business record |
| `bpm_af_deployment` | — | Activiti deployment cache |
| `bpm_af_task` | — | Activiti task runtime |
| `bpm_af_taskinst` | — | Activiti task history |
| `bpm_af_execution` | — | Activiti execution runtime |
| `bpm_verify_info` | `BpmVerifyInfo` | Approval records |
| `bpm_process_node_submit` | — | Node submit tracking |
| `bpm_process_forward` | — | Forward records |
| `bpm_flowrun_entrust` | — | Entrust/forward records |
| `bpm_business_draft` | — | Process drafts |

### Variable Tables
| Table | Description |
|---|---|
| `t_bpm_variable` | Process variables |
| `t_bpm_variable_multiplayer` | Multi-instance variable elements |
| `t_bpm_variable_multiplayer_personnel` | Multi-instance assignees |

### External Integration Tables
| Table | Description |
|---|---|
| `t_out_side_bpm_business_party` | External business parties |
| `t_out_side_bpm_access_business` | External process records |
| `t_out_side_bpm_callback_url_conf` | Callback URL configs |
| `t_out_side_bpm_admin_personnel` | External admins |
| `t_out_side_bpm_approve_template` | External approve templates |
| `t_out_side_bpm_conditions_template` | External condition templates |
| `t_out_side_bpm_call_back_record` | Callback delivery records |

### Organization Tables
| Table | Description |
|---|---|
| `t_user` | Users |
| `t_role` | Roles |
| `t_user_role` | User-role mapping |
| `t_department` | Department tree |
| `t_user_entrust` | User entrust configs |
| `t_user_message` | In-app messages |
| `t_user_message_status` | Message preferences |

### Supporting Tables
| Table | Description |
|---|---|
| `t_information_template` | Notification templates |
| `t_dict_data` | Dictionary data |
| `t_lf_main` | Low-code form main tables (sharded) |
| `t_lf_main_field` | Low-code form field values (sharded) |
| `bpm_process_app_application` | Process applications |
| `bpm_process_permissions` | Process permissions |
| `bpm_process_category` | Process categories |
| `t_op_log` | Operation audit log |
| `t_quick_entry` | Quick entry shortcuts |
| `t_sys_version` | App version management |
| `t_method_replay` | Method replay records |

### JSON Consolidation Pattern
Many formerly separate tables have been **consolidated into JSON columns** (`conf_config_json`, `node_config_json`, `variable_config_json`, etc.). The SQL file contains comments like `-- REMOVED: t_bpmn_node_business_table_conf (migrated to JSON)` documenting this evolution.

---

## 11. Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "server=localhost;userid=root;pwd=123456;port=3306;database=antflow.net-next;sslmode=none;Charset=utf8mb4"
  },
  "MailSettings": {
    "Host": "smtp.163.com",
    "Port": 465,
    "Account": "antflow@163.com",
    "Password": "HHVZDETFJMCATUGS"
  }
}
```

### Key Configuration Points

| Setting | Usage |
|---|---|
| `ConnectionStrings:MySqlConnection` | FreeSql database connection |
| `MailSettings` | SMTP configuration for email notifications |
| `FreeSqlSet()` extension | Configures FreeSql from connection string |
| `AddFreeRepository()` | Registers FreeSql repository pattern |
| `AntFlowServiceSetUp()` | Registers all AntFlow services |
| `AddAFApplicationComponents()` | Custom MVC controller configuration |
| `AddCors("CorsPolicy")` | Allows all origins, methods, headers |

### JSON Serialization Converters
Custom converters registered globally:
- `CustomDateTimeConverter`
- `StringOrIntConverter`
- `BooleanJsonConverter`
- `NullAbleBooleanJsonConverter`
- `BooleanToIntJsonConverter`
- `BooleanToNullableIntJsonConverter`
- `GlobalNullableIntConverter`

---

## Appendix: Project Dependency Graph

```
AntFlowCore.Web
  └── AntFlowCore.Api
        └── AntFlowCore.AspNetCore
              └── AntFlowCore.Abstraction
                    └── AntFlowCore.Abstraction.Orm
                          └── AntFlowCore.Persist.api
        └── AntFlowCore.Engine.Abstraction
              └── AntFlowCore.Persist.api
        └── AntFlowCore.Engine
              └── AntFlowCore.Bpmn
                    └── AntFlowCore.Base
              └── AntFlowCore.VirtualNode
                    └── AntFlowCore.Base
                    └── AntFlowCore.Persist.api
        └── AntFlowCore.Business
              └── AntFlowCore.Engine.Abstraction
        └── AntFlowCore.Persist
              └── AntFlowCore.Persist.api
```

---

*Document generated from source code analysis of AntFlowCore .NET project.*
