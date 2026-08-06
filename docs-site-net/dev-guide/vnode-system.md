# AntFlowCore .NET 虚拟节点模式详解

## 1. 概述

虚拟节点（VirtualNode）模式是 AntFlowCore 中一种重要的服务封装模式。它位于 `AntFlowCore.VirtualNode` 项目中，为核心引擎服务提供了一层轻量级的业务逻辑封装，主要职责包括：

- 协调多个核心服务完成复杂业务操作
- 处理运行时数据的读写（如变量配置、加签人员）
- 管理应用配置 URL 的获取
- 封装节点与元素之间的映射关系

## 2. 项目结构

```
AntFlowCore.VirtualNode/
├── AntFlowCore.VirtualNode.csproj
└── service/
    ├── BpmnConfService.cs              # 流程配置服务
    ├── BpmnNodeToService.cs            # 节点连线关系服务
    ├── BpmProcessAppApplicationService.cs  # 应用URL配置服务
    ├── BpmVariableMultiplayerPersonnelService.cs  # 多人会签人员服务
    ├── BpmVariableMultiplayerService.cs  # 多人会签变量服务
    ├── BpmVariableService.cs            # 变量映射服务
    └── BpmVariableSignUpPersonnelService.cs    # 加签人员服务
```

## 3. 核心虚拟节点服务详解

### 3.1 BpmnConfService - 流程配置服务

`BpmnConfService` 封装了流程配置的查询和生效操作，是流程模板管理的核心虚拟节点。

```csharp
public class BpmnConfService : IBpmnConfService
{
    private readonly IBpmnConfRepository _repository;

    public BpmnConfService(IBpmnConfRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取最大流程编码（用于生成新编码）
    /// </summary>
    public string GetMaxBpmnCode(string bpmnCodeParts)
    {
        return _repository.GetMaxBpmnCode(bpmnCodeParts);
    }

    /// <summary>
    /// 校验流程编码是否唯一
    /// </summary>
    public string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode)
    {
        return _repository.ReCheckBpmnCode(bpmnCodeParts, bpmnCode);
    }

    /// <summary>
    /// 分页查询流程配置列表
    /// </summary>
    public List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)
    {
        return _repository.SelectPageList(page, vo);
    }

    /// <summary>
    /// 生效流程配置（管理员手动激活）
    /// </summary>
    public void EffectiveBpmnConf(int id)
    {
        _repository.EffectiveBpmnConf(id);
    }
}
```

**设计要点**：
- 通过构造函数注入仓储接口，实现依赖倒置
- `EffectiveBpmnConf` 方法执行后流程才可在运行时使用
- 编码自动生成机制确保 `BpmnCode` 的唯一性

### 3.2 BpmnNodeToService - 节点连线关系服务

```csharp
public class BpmnNodeToService : IBpmnNodeToService
{
    private readonly IBpmnNodeToRepository _repository;

    public BpmnNodeToService(IBpmnNodeToRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 编辑节点连线关系
    /// </summary>
    public void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        _repository.EditNodeTo(bpmnNodeVo, bpmnNodeId);
    }
}
```

节点连线关系决定了流程的流转方向。`EditNodeTo` 方法在流程模板保存时更新节点之间的连接关系。

### 3.3 BpmVariableService - 变量映射服务

`BpmVariableService` 负责管理流程运行时变量，特别是节点ID与BPMN元素ID之间的映射关系。

```csharp
public class BpmVariableService : IBpmVariableService
{
    private readonly IBpmVariableRepository _repository;

    public BpmVariableService(IBpmVariableRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 根据元素ID获取节点ID列表
    /// </summary>
    public List<string> GetNodeIdsByeElementId(string processNumber, string elementId)
    {
        return _repository.GetNodeIdsByeElementId(processNumber, elementId);
    }

    /// <summary>
    /// 根据节点ID获取元素ID列表
    /// </summary>
    public List<string> GetElementIdsdByNodeId(string processNumber, string nodeId)
    {
        return _repository.GetElementIdsdByNodeId(processNumber, nodeId);
    }

    /// <summary>
    /// 根据元素ID获取节点与元素映射DTO
    /// </summary>
    public NodeElementDto GetNodeIdByElementId(string processNumber, string elementId)
    {
        return _repository.GetNodeIdByElementId(processNumber, elementId);
    }

    /// <summary>
    /// 根据节点ID获取元素映射DTO
    /// </summary>
    public NodeElementDto GetElementIdByNodeId(string processNumber, string nodeId)
    {
        return _repository.GetElementIdByNodeId(processNumber, nodeId);
    }

    /// <summary>
    /// 批量根据元素IDs获取节点IDs
    /// </summary>
    public List<string> GetNodeIdByElementIds(string processNumber, List<string> elementIds)
    {
        return _repository.GetNodeIdByElementIds(processNumber, elementIds);
    }

    /// <summary>
    /// 获取当前多人会签节点信息
    /// </summary>
    public BpmVariableMultiplayer GetCurrentMultiPlayerNode(
        string processNumber, string elementId, string nodeId)
    {
        return _repository.GetCurrentMultiPlayerNode(processNumber, elementId, nodeId);
    }

    /// <summary>
    /// 使节点审批人失效（用于动态变更审批人场景）
    /// </summary>
    public void InvalidNodeAssignees(List<string> assigneeIds, string processNumber, bool isSingle)
    {
        _repository.InvalidNodeAssignees(assigneeIds, processNumber, isSingle);
    }
}
```

**关键概念映射**：

| 方法 | 作用 |
|------|------|
| `GetNodeIdsByeElementId` | BPMN元素ID → 内部节点ID |
| `GetElementIdsdByNodeId` | 内部节点ID → BPMN元素ID |
| `GetCurrentMultiPlayerNode` | 获取多人会签节点配置 |
| `InvalidNodeAssignees` | 动态失效指定审批人 |

### 3.4 BpmVariableSignUpPersonnelService - 加签人员服务

加签是审批流中的常见场景：当前审批人可以将其他人加入审批链。`BpmVariableSignUpPersonnelService` 负责管理加签人员的持久化和读取。

```csharp
public class BpmVariableSignUpPersonnelService : IBpmVariableSignUpPersonnelService
{
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;

    public BpmVariableSignUpPersonnelService(
        IBpmVariableService bpmVariableService,
        IBpmBusinessProcessService bpmBusinessProcessService)
    {
        _bpmVariableService = bpmVariableService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }

    /// <summary>
    /// 写入加批人员到 variable_config_json
    /// </summary>
    public void InsertSignUpPersonnel(
        string processNumber, 
        string taskTaskDefinitionKey, 
        string assignee, 
        List<BaseIdTranStruVo> signUpUsers)
    {
        if (signUpUsers == null || signUpUsers.Count == 0) return;

        // 1. 查找流程变量
        BpmVariable bpmVariable = _bpmVariableService._repository.FindByProcessNum(processNumber);
        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
            return;

        // 2. 反序列化配置JSON
        VariableConfigJson config = JsonSerializer.Deserialize<VariableConfigJson>(
            bpmVariable.VariableConfigJson, JsonConfUtil.Options);
        if (config?.SignUps == null || config.SignUps.Count == 0) return;

        // 3. 找到对应的加签配置项
        VariableSignUpItem signUp = config.SignUps
            .FirstOrDefault(s => taskTaskDefinitionKey == s.ElementId);
        if (signUp == null || string.IsNullOrEmpty(signUp.SubElements)) return;

        // 4. 解析子元素列表
        List<BpmnConfCommonElementVo> subElementVos = JsonSerializer
            .Deserialize<List<BpmnConfCommonElementVo>>(signUp.SubElements, JsonConfUtil.Options);

        // 5. 正向加批节点（isBackSignUp == 0）
        BpmnConfCommonElementVo signUpElement = subElementVos
            .FirstOrDefault(o => o.IsBackSignUp == 0) ?? new BpmnConfCommonElementVo();

        // 6. 构建加签人员列表
        List<VariablePersonnelItem> signUpPersonnel = signUpUsers
            .Select(o => new VariablePersonnelItem 
            { 
                Assignee = o.Id, 
                AssigneeName = o.Name 
            })
            .ToList();

        signUp.PersonnelByElement[signUpElement.ElementId] = signUpPersonnel;

        // 7. 加批后回到加批人（afterSignUpWay == 1）
        if (signUp.AfterSignUpWay != null && signUp.AfterSignUpWay == 1)
        {
            BpmnConfCommonElementVo backSignUpElement = subElementVos
                .FirstOrDefault(o => o.IsBackSignUp == 1) ?? new BpmnConfCommonElementVo();
            signUp.PersonnelByElement[backSignUpElement.ElementId] = new List<VariablePersonnelItem>
            {
                new VariablePersonnelItem
                {
                    Assignee = assignee,
                    AssigneeName = SecurityUtils.GetLogInEmpName()
                }
            };
        }

        // 8. 序列化并保存
        bpmVariable.VariableConfigJson = JsonSerializer.Serialize(config, JsonConfUtil.Options);
        _bpmVariableService._repository.Update(bpmVariable);
    }

    /// <summary>
    /// 读取加批节点人员映射
    /// </summary>
    public List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(
        string procInstId, string elementId)
    {
        var result = new List<KeyValuePair<string, string>>();
        
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService
            .GetBpmBusinessProcessByProcInstId(procInstId);
        if (bpmBusinessProcess == null) return result;

        BpmVariable bpmVariable = _bpmVariableService._repository
            .FindByProcessNum(bpmBusinessProcess.BusinessNumber);
        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
            return result;

        VariableConfigJson config = JsonSerializer.Deserialize<VariableConfigJson>(
            bpmVariable.VariableConfigJson, JsonConfUtil.Options);
        if (config?.SignUps == null) return result;

        foreach (VariableSignUpItem signUp in config.SignUps)
        {
            if (signUp.PersonnelByElement != null && 
                signUp.PersonnelByElement.TryGetValue(elementId, out var personnelList))
            {
                foreach (VariablePersonnelItem p in personnelList)
                {
                    result.Add(new KeyValuePair<string, string>(p.Assignee, p.AssigneeName));
                }
                break;
            }
        }
        return result;
    }
}
```

### 3.5 BpmProcessAppApplicationService - 应用URL配置服务

管理流程关联的应用URL配置（查看URL、提交URL、条件URL）：

```csharp
public class BpmProcessAppApplicationService : IBpmProcessAppApplicationService
{
    private const int appCommonId = 2;   // 移动端通用ID
    private const int pcCommonId = 1;    // PC端通用ID

    private readonly IBpmProcessAppApplicationRepository _repository;

    /// <summary>
    /// 根据业务编码和流程Key获取应用URL配置
    /// </summary>
    public BpmProcessAppApplicationVo GetApplicationUrl(string businessCode, string processKey)
    {
        if (string.IsNullOrEmpty(businessCode) && string.IsNullOrEmpty(processKey))
            return null;

        List<BpmProcessAppApplication> list = _repository.GetApplicationUrl(businessCode, processKey);
        if (ObjectUtils.IsEmpty(list)) return null;

        BpmProcessAppApplication application = list[0];
        BpmProcessAppApplicationVo vo = application.MapToVo();

        // HTML解码URL
        if (!string.IsNullOrEmpty(vo.LookUrl))
            vo.LookUrl = HttpUtility.HtmlDecode(vo.LookUrl);
        if (!string.IsNullOrEmpty(vo.SubmitUrl))
            vo.SubmitUrl = HttpUtility.HtmlDecode(vo.SubmitUrl);
        if (!string.IsNullOrEmpty(vo.ConditionUrl))
            vo.ConditionUrl = HttpUtility.HtmlDecode(vo.ConditionUrl);

        return vo;
    }
}
```

## 4. 虚拟节点模式设计思想

### 4.1 为什么要引入虚拟节点？

```
┌──────────────────────────────────────────────┐
│              核心引擎层                        │
│  (AntFlowCore.Engine / AntFlowCore.Bpmn)      │
│  - 流程定义解析                               │
│  - 节点遍历                                   │
│  - 条件评估                                   │
│  - 审批人计算                                 │
└──────────────────┬───────────────────────────┘
                   │ 调用
                   ▼
┌──────────────────────────────────────────────┐
│           虚拟节点层                           │
│       (AntFlowCore.VirtualNode)              │
│  - 运行时数据读写                             │
│  - 变量配置管理                               │
│  - 加签人员管理                               │
│  - URL配置获取                                │
└──────────────────┬───────────────────────────┘
                   │ 调用
                   ▼
┌──────────────────────────────────────────────┐
│            持久层                              │
│       (AntFlowCore.Persist)                  │
│  - 数据库操作                                 │
│  - 事务管理                                   │
│  - 仓储实现                                   │
└──────────────────────────────────────────────┘
```

虚拟节点层的核心价值：

1. **解耦**：将运行时数据管理从核心引擎逻辑中分离
2. **可替换**：可以轻松替换持久化实现（如从 FreeSQL 切换到 EF Core）
3. **可测试**：每个虚拟节点服务都可以独立进行单元测试
4. **扩展**：新增业务功能只需添加新的虚拟节点服务

### 4.2 数据流示意

```
流程启动/审批操作
       │
       ▼
┌─────────────┐     ┌─────────────────┐     ┌──────────────┐
│   API 层    │────▶│  引擎核心服务    │────▶│  虚拟节点服务 │
│ Controller  │     │ ApprovalService  │     │ BpmVariable  │
└─────────────┘     └─────────────────┘     └──────┬───────┘
                                                   │
                                                   ▼
                                            ┌──────────────┐
                                            │   仓储层      │
                                            │ Repository   │
                                            └──────────────┘
```

## 5. DI 注册

虚拟节点服务在 `ServiceRegistration.AntFlowServiceSetUp` 中统一注册：

```csharp
// VirtualNode services are registered via their interfaces
// Note: VirtualNode services are registered in the Engine.Abstraction project's
// ServiceRegistration through the following registrations:
services.AddSingleton<BpmnConfService>();
services.AddSingleton<IBpmnConfService, BpmnConfService>();
services.AddSingleton<BpmnNodeToService>();
services.AddSingleton<IBpmnNodeToService, BpmnNodeToService>();
```

## 6. 使用示例

### 6.1 获取节点元素映射

```csharp
public class MyWorkflowService
{
    private readonly IBpmVariableService _bpmVariableService;

    public MyWorkflowService(IBpmVariableService bpmVariableService)
    {
        _bpmVariableService = bpmVariableService;
    }

    public void ProcessNodeMapping(string processNumber, string elementId)
    {
        // 获取节点ID列表
        List<string> nodeIds = _bpmVariableService
            .GetNodeIdsByeElementId(processNumber, elementId);
        
        // 获取当前会签节点信息
        BpmVariableMultiplayer multiplayer = _bpmVariableService
            .GetCurrentMultiPlayerNode(processNumber, elementId, nodeIds.First());
    }
}
```

### 6.2 获取应用URL配置

```csharp
public class MyFormService
{
    private readonly IBpmProcessAppApplicationService _appService;

    public MyFormService(IBpmProcessAppApplicationService appService)
    {
        _appService = appService;
    }

    public string GetFormUrl(string businessCode, string processKey)
    {
        BpmProcessAppApplicationVo vo = _appService
            .GetApplicationUrl(businessCode, processKey);
        return vo?.SubmitUrl;
    }
}
```

## 7. 总结

虚拟节点模式是 AntFlowCore 架构中的关键设计，它：

1. 作为核心引擎与持久层之间的桥梁
2. 封装了运行时数据的复杂读写逻辑
3. 支持加签、会签等高级审批场景
4. 通过接口注入实现松耦合设计
5. 便于单元测试和功能扩展
