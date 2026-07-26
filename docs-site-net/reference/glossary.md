# 术语表

## A

### Activiti
开源工作流引擎，AntFlowCore 的 BPMN 执行参考实现之一。AntFlowCore 在 .NET 中实现了类似的执行语义。

### Adapter（适配器）
AntFlowCore 的核心设计模式。通过 `IFormOperationAdaptor`、`IUserEntrustService` 等接口，将引擎核心与业务实现解耦。

### AND/OR 条件
流程条件规则中的逻辑组合方式。AND 表示所有条件都满足，OR 表示任一条件满足。

### Approver（审批人）
流程节点中处理审批任务的人员。可以是具体的人、角色、或动态规则计算的结果。

### Assignee（任务处理人）
被分配处理某个审批任务的具体用户。

## B

### BPMN (Business Process Model and Notation)
业务流程模型与符号，工作流设计的国际标准。AntFlowCore 使用 BPMN 2.0 的子集进行流程建模。

### BpmnConf
流程配置（Process Configuration）的缩写。AntFlowCore 中存储流程模板的核心实体。

### BpmnConfLfFormdata
低代码表单数据实体。存储以 JSON 格式定义的低代码表单结构。

### BusinessDataVo
业务数据值对象。流程操作（同意/拒绝等）时传递的业务数据载体。

## C

### Callback（回调）
流程状态变更时通知外部系统的机制。AntFlowCore 通过 `ICallbackAdaptor` 接口实现回调。

### CodeFirst
FreeSql 的数据库开发模式。通过实体类定义自动生成数据库表结构。

### Condition（条件）
流程中用于判断分支路径的规则表达式。支持字段比较、逻辑组合等。

### Conf（Configuration）
配置（Configuration）的缩写。在 AntFlowCore 中通常指流程配置。

## D

### DIY（Do It Yourself）
自定义表单模式。与低代码表单相对，由开发者完全自主实现表单和逻辑。

### DictData
字典数据。AntFlowCore 使用字典表存储表单码、枚举值等配置数据。

### Distributed Cache
分布式缓存。通常使用 Redis 实现，用于跨实例共享缓存数据。

## E

### Effective（生效）
流程模板从草稿状态变为可用状态的操作。只有生效后的模板才能发起新流程。

### Exclusive Gateway（排他网关）
BPMN 中的决策节点。根据条件选择一条分支执行。

### Execution（执行）
流程实例的执行状态。记录流程在节点间的流转过程。

## F

### FormCode
表单码。低代码表单的唯一标识，用于关联流程与表单。

### Formdata
表单数据。低代码表单的 JSON 定义，存储在 `BpmnConfLfFormdata` 实体中。

### FreeSql
.NET 开源 ORM 框架。AntFlowCore 的数据访问层基于 FreeSql 构建。

## G

### Gateway（网关）
BPMN 中的流程控制节点。包括排他网关、并行网关等。

## H

### Historic Process Instance
历史流程实例。已完成流程的执行记录，用于审计和查询。

## I

### IAntFlowRepository
AntFlowCore 的仓储接口。定义数据访问的标准操作。

### IFormOperationAdaptor
表单操作适配器接口。DIY 表单需要实现此接口与引擎集成。

### Inform（通知）
流程事件的通知机制。支持邮件、短信、站内信等方式。

## J

### JWT (JSON Web Token)
JSON Web Token，AntFlowCore 使用的认证令牌格式。

## L

### LF（Low Code Form）
低代码表单的缩写。AntFlowCore 中 `LF` 前缀通常与低代码功能相关。

### LFFieldTypeEnum
低代码字段类型枚举。定义字符串、数字、日期等字段类型。

### Low Code Flow
低代码流程。通过可视化配置而非编码实现的工作流。

## M

### Multi-instance（多实例）
BPMN 中的会签概念。一个任务节点需要多人处理。

### MySQL
关系型数据库管理系统。AntFlowCore 支持的主要数据库之一。

## N

### Natasha
.NET 动态编译框架。AntFlowCore 用于运行时编译条件表达式。

### Node（节点）
流程中的基本单元。包括开始节点、审批节点、网关节点、结束节点等。

### NodeFormAssigneeProperty
节点表单审批人属性。定义审批人从表单中的哪个属性获取。

## O

### ORM (Object-Relational Mapping)
对象关系映射。FreeSql 是 AntFlowCore 使用的 ORM 工具。

### OutSideBpm
外部工作流。AntFlowCore 提供的供第三方系统接入的流程能力。

## P

### Parallel Gateway（并行网关）
BPMN 中的并行分支节点。所有分支同时执行，全部完成后汇合。

### PostgreSQL
开源关系型数据库。AntFlowCore 支持的数据库之一。

### ProcessDefinition（流程定义）
流程模板的运行时表示。包含节点、连线、条件等完整配置。

### ProcessInstance（流程实例）
流程定义的一次执行。每个实例有独立的状态和变量。

## R

### Repository（仓储）
数据访问层模式。AntFlowCore 通过仓储接口隔离业务逻辑与数据访问。

### Rule（规则）
审批人规则或条件规则。AntFlowCore 内置 14 种审批人规则。

## S

### Sequence Flow（顺序流）
BPMN 中连接节点的连线。定义流程的执行顺序。

### Serilog
.NET 结构化日志框架。AntFlowCore 使用 Serilog 进行日志记录。

### SQL Server
微软关系型数据库。AntFlowCore 支持的数据库之一。

### Start Event（开始事件）
BPMN 中的流程起点。每个流程实例从开始事件启动。

## T

### Task（任务）
流程中的审批工作单元。每个审批节点会生成一个或多个任务。

### Tenant（租户）
多租户架构中的隔离单位。不同租户的数据相互独立。

### Type（类型）
在 AntFlowCore 中常指任务类型（待办/已办/发起）或字段类型。

## U

### UDR（User Defined Rule）
用户自定义审批人规则。允许通过字典配置扩展审批人来源。

### UnitOfWork（工作单元）
事务管理单元。确保多个操作的原子性。

### UserEntrust（用户委托）
审批委托功能。用户可将自己的审批权限临时委托给他人。

## V

### Variable（变量）
流程变量。在流程执行过程中传递和共享的数据。

### Virtual Node（虚拟节点）
AntFlowCore 的核心创新。将流程流转逻辑抽象为虚拟节点，实现业务与引擎的解耦。

### vform
可视化表单设计器。AntFlowCore 低代码表单的前端设计工具。

## W

### Widget（控件）
表单中的字段组件。如文本框、下拉框、日期选择器等。

### Workflow（工作流）
自动化的业务流程。AntFlowCore 的核心管理对象。
