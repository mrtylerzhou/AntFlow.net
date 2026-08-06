# AntFlowCore .NET 系统集成指南

## 1. 概述

AntFlowCore 的核心设计目标是**不侵入业务系统**。引擎通过适配器模式与外部业务系统集成，业务系统只需实现少量接口即可接入工作流能力。

## 2. 集成方式总览

AntFlowCore 提供两种集成方式：

| 方式 | 适用场景 | 复杂度 |
|------|---------|--------|
| **表单操作适配器** (`IFormOperationAdaptor<T>`) | 自定义业务表单，需要控制数据的增删改查 | 中 |
| **低代码表单** (`LowFlowApprovalService`) | 使用内置低代码表单引擎，无需编码 | 低 |

## 3. 使用表单操作适配器集成

### 3.1 实现步骤

```
1. 创建业务数据VO类（继承 BusinessDataVo）
   ↓
2. 创建业务数据实体类
   ↓
3. 实现 IFormOperationAdaptor<T> 接口
   ↓
4. 注册服务到 DI 容器
   ↓
5. 在流程管理后台配置表单编码（formCode）
   ↓
6. 设计流程模板并关联 formCode
```

### 3.2 示例：第三方账户申请流程

#### Step 1: 创建业务数据 VO

```csharp
public class ThirdPartyAccountApplyVo : BusinessDataVo
{
    /// <summary>
    /// 账户类型
    /// </summary>
    public int AccountType { get; set; }
    
    /// <summary>
    /// 账户所有者名称
    /// </summary>
    public string AccountOwnerName { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}
```

#### Step 2: 创建业务实体

```csharp
public class ThirdPartyAccountApply
{
    public int Id { get; set; }
    public int AccountType { get; set; }
    public string AccountOwnerName { get; set; }
    public string Remark { get; set; } = "";
}
```

#### Step 3: 实现表单操作适配器

```csharp
[DIYFormServiceAnno(SvcName = "DSFZH_WMA", Desc = "三方账号申请")]
public class ThirdPartyAccountApplyFlowService 
    : AbstractLowFlowSpyFormOperationAdaptor<ThirdPartyAccountApplyVo>
{
    private readonly IThirdPartyAccountApplyService _service;

    public ThirdPartyAccountApplyFlowService(IThirdPartyAccountApplyService service)
    {
        _service = service;
    }

    /// <summary>
    /// 预览条件设置
    /// </summary>
    public override void PreviewSetCondition(
        BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo vo)
    {
        conditionsVo.AccountType = vo.AccountType;
        conditionsVo.StartUserId = vo.StartUserId;
    }

    /// <summary>
    /// 启动参数设置（流程发起时调用）
    /// </summary>
    public override void LaunchParameters(
        BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo vo)
    {
        conditionsVo.AccountType = vo.AccountType;
        conditionsVo.StartUserId = vo.StartUserId;
    }

    /// <summary>
    /// 初始化数据（打开表单时调用）
    /// </summary>
    public override void OnInitData(ThirdPartyAccountApplyVo vo)
    {
        // 初始化表单默认值
    }

    /// <summary>
    /// 查询数据（查看已发起流程时调用）
    /// </summary>
    public override void OnQueryData(ThirdPartyAccountApplyVo vo)
    {
        var entity = _service._repository
            .Find(a => a.Id == Convert.ToInt32(vo.BusinessId))
            .FirstOrDefault();
            
        vo.AccountType = entity.AccountType;
        vo.AccountOwnerName = entity.AccountOwnerName;
        vo.Remark = entity.Remark;
    }

    /// <summary>
    /// 提交数据（流程发起时调用）
    /// </summary>
    public override void OnSubmitData(ThirdPartyAccountApplyVo vo)
    {
        var entity = vo.MapToEntity();
        _service._repository.Add(entity);
        
        // 保存业务ID供后续使用
        vo.BusinessId = entity.Id.ToString();
        vo.ProcessTitle = "第三方账号申请";
        vo.ProcessDigest = vo.Remark;
        vo.EntityName = nameof(ThirdPartyAccountApply);
    }

    /// <summary>
    /// 同意审批回调
    /// </summary>
    public override void OnConsentData(ThirdPartyAccountApplyVo vo)
    {
        // 仅在重新提交时更新数据
        if (vo.OperationType == (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT)
        {
            var entity = vo.MapToEntity();
            int id = Convert.ToInt32(vo.BusinessId);
            entity.Id = id;
            _service._repository.Update(entity);
        }
    }

    /// <summary>
    /// 退回修改回调
    /// </summary>
    public override void OnBackToModifyData(ThirdPartyAccountApplyVo vo)
    {
        // 退回时通常无需额外操作
    }

    /// <summary>
    /// 取消流程回调
    /// </summary>
    public override void OnCancellationData(ThirdPartyAccountApplyVo vo)
    {
        // 取消时可能需要回滚数据
    }

    /// <summary>
    /// 流程结束回调
    /// </summary>
    public override void OnFinishData(BusinessDataVo vo)
    {
        // 流程完成后的收尾工作
    }
}
```

#### Step 4: 注册服务

```csharp
// 在 ServiceRegistration 或 Startup 中注册
services.AddSingleton<IThirdPartyAccountApplyService, ThirdPartyAccountApplyService>();
services.AddSingleton<IFormOperationAdaptor<ThirdPartyAccountApplyVo>, ThirdPartyAccountApplyFlowService>();
```

### 3.3 适配器生命周期

```
用户发起流程
     │
     ▼
┌─────────────────┐
│ PreviewSetCondition │ 设置预览条件（用于流程预览）
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ LaunchParameters   │ 设置启动参数（流程正式启动）
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ OnSubmitData       │ 保存业务数据（生成 businessId）
└────────┬────────┘
         │
         ▼
     审批流程中...
         │
    ┌────┴────┐
    │         │
    ▼         ▼
┌────────┐ ┌────────────┐
│OnQuery │ │OnConsent   │ 同意/重新提交时
│Data    │ │Data        │
└────────┘ └────────────┘
                 │
                 ▼
         ┌───────────────┐
         │OnBackToModify │ 退回修改时
         │Data           │
         └───────────────┘
                 │
                 ▼
         ┌───────────────┐
         │OnCancellation │ 取消流程时
         │Data           │
         └───────────────┘
                 │
                 ▼
         ┌───────────────┐
         │OnFinishData   │ 流程完成时
         └───────────────┘
```

### 3.4 DIYFormServiceAnno 特性

使用 `[DIYFormServiceAnno]` 特性标记自定义表单服务：

```csharp
[DIYFormServiceAnno(SvcName = "DSFZH_WMA", Desc = "三方账号申请")]
public class ThirdPartyAccountApplyFlowService : ...
```

| 参数 | 说明 |
|------|------|
| `SvcName` | 服务编码（需与流程配置中的 formCode 一致） |
| `Desc` | 服务描述 |

## 4. 使用低代码表单集成

### 4.1 概述

低代码表单（`LowFlowApprovalService`）是 AntFlowCore 内置的表单引擎，支持通过可视化配置生成表单，无需编写后端代码。

### 4.2 低代码表单数据流

```
┌──────────────────┐     ┌──────────────────────────┐
│ 前端低代码表单    │────▶│ UDLFApplyVo              │
│ (LF表单设计器)   │     │ - FormCode               │
│                  │     │ - LfFields (表单字段字典) │
└──────────────────┘     └────────────┬─────────────┘
                                      │
                                      ▼
                             ┌─────────────────┐
                             │ LowFlowApproval │
                             │ Service          │
                             │ - OnSubmitData   │
                             │ - OnQueryData    │
                             │ - OnConsentData  │
                             └────────┬────────┘
                                      │
                                      ▼
                             ┌─────────────────┐
                             │ LFMain          │
                             │ LFMainField     │
                             │ (低代码数据存储) │
                             └─────────────────┘
```

### 4.3 低代码表单 VO

```csharp
public class UDLFApplyVo : BusinessDataVo
{
    /// <summary>
    /// 表单编码
    /// </summary>
    public string FormCode { get; set; }
    
    /// <summary>
    /// 表单字段字典（key=字段名, value=字段值）
    /// </summary>
    public Dictionary<string, object> LfFields { get; set; }
    
    /// <summary>
    /// 低代码表单HTML（用于渲染）
    /// </summary>
    public string LfFormData { get; set; }
    
    /// <summary>
    /// 节点关联审批人配置
    /// </summary>
    public Dictionary<string, List<string>> Node2formRelatedAssignees { get; set; }
    
    /// <summary>
    /// 低代码条件列表
    /// </summary>
    public List<LfConditionVo> LfConditions { get; set; }
}
```

### 4.4 低代码表单字段类型

| 类型枚举 | 说明 | 存储字段 |
|---------|------|---------|
| `LFFieldTypeEnum.STRING` | 字符串 | `FieldValue` |
| `LFFieldTypeEnum.NUMBER` | 数值 | `FieldValueNumber` |
| `LFFieldTypeEnum.DATE` | 日期 | `FieldValueDt` |
| `LFFieldTypeEnum.DATE_TIME` | 日期时间 | `FieldValueDt` |
| `LFFieldTypeEnum.TEXT` | 文本 | `FieldValueText` |
| `LFFieldTypeEnum.BOOLEAN` | 布尔值 | `FieldValue` |

## 5. 低代码表单操作适配器（ILFFormOperationAdaptor）

如果低代码表单内置功能不足，可以实现 `ILFFormOperationAdaptor` 进行扩展：

```csharp
public interface ILFFormOperationAdaptor
{
    void OnInitData(UDLFApplyVo vo);
    void OnQueryData(UDLFApplyVo vo);
    void OnSubmitData(UDLFApplyVo vo);
    void OnConsentData(UDLFApplyVo vo);
    void OnBackToModifyData(UDLFApplyVo vo);
    void OnCancellationData(UDLFApplyVo vo);
    void OnFinishData(BusinessDataVo vo);
}
```

示例实现：

```csharp
[LFFormServiceAnno(SvcName = "MY_FORM")]
public class MyFormOperationAdaptor : ILFFormOperationAdaptor
{
    public void OnInitData(UDLFApplyVo vo) { /* 初始化 */ }
    public void OnQueryData(UDLFApplyVo vo) { /* 查询 */ }
    public void OnSubmitData(UDLFApplyVo vo) { /* 提交 */ }
    public void OnConsentData(UDLFApplyVo vo) { /* 同意 */ }
    public void OnBackToModifyData(UDLFApplyVo vo) { /* 退回 */ }
    public void OnCancellationData(UDLFApplyVo vo) { /* 取消 */ }
    public void OnFinishData(BusinessDataVo vo) { /* 结束 */ }
}
```

## 6. 流程操作 API 调用

### 6.1 发起流程

```javascript
// 前端调用示例
fetch('/BpmnConf/process/buttonsOperation?formCode=DSFZH_WMA', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        operationType: 1,  // BUTTON_TYPE_START
        businessDataVo: {
            startUserId: 'user001',
            accountType: 1,
            accountOwnerName: '张三',
            remark: '申请开通第三方账号'
        }
    })
});
```

### 6.2 同意审批

```javascript
fetch('/BpmnConf/process/buttonsOperation?formCode=DSFZH_WMA', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        operationType: 2,  // BUTTON_TYPE_AGREE
        businessId: '12345',
        verifyDesc: '同意申请',
        businessDataVo: { /* 更新后的业务数据 */ }
    })
});
```

### 6.3 流程预览

```javascript
fetch('/BpmnConf/preview', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        bpmnConfVo: { /* 流程配置 */ },
        startUserId: 'user001',
        accountType: 1
    })
});
```

## 7. 按钮操作类型枚举

| 枚举值 | 编码 | 说明 |
|--------|------|------|
| `BUTTON_TYPE_START` | 1 | 发起流程 |
| `BUTTON_TYPE_AGREE` | 2 | 同意 |
| `BUTTON_TYPE_DISAGREE` | 3 | 不同意 |
| `BUTTON_TYPE_RESUBMIT` | 4 | 重新提交 |
| `BUTTON_TYPE_BACK_TO_MODIFY` | 5 | 退回修改 |
| `BUTTON_TYPE_CANCEL` | 6 | 撤销流程 |
| `BUTTON_TYPE_FORWARD` | 7 | 转发 |
| `BUTTON_TYPE_ADD_ASSIGNEE` | 8 | 加签 |
| `BUTTON_TYPE_REMOVE_ASSIGNEE` | 9 | 减签 |
| `BUTTON_TYPE_CHANGE_ASSIGNEE` | 10 | 变更处理人 |
| `BUTTON_TYPE_TRANSFER` | 11 | 转交 |
| `BUTTON_TYPE_UNDERTAKE` | 12 | 承办 |

## 8. 自定义自动节点条件

如果需要在自动节点中执行自定义条件判断，重写 `AutoCondition` 方法：

```csharp
public override bool? AutoCondition(ThirdPartyAccountApplyVo vo)
{
    // 自定义条件：金额大于10000自动通过
    if (vo.LfFields.TryGetValue("amount", out var amountObj) 
        && decimal.TryParse(amountObj?.ToString(), out var amount))
    {
        return amount > 10000;
    }
    
    // 返回 null 表示使用默认条件评估逻辑
    return null;
}

public override void AutomaticAction(ThirdPartyAccountApplyVo vo, bool? conditionResult)
{
    if (conditionResult == true)
    {
        // 自动节点通过时的自定义动作
        // 例如：记录日志、发送通知等
    }
}
```

## 9. 最佳实践

### 9.1 表单设计

1. **formCode 命名规范**：使用大写字母和下划线，如 `LEAVE_APPLY`、`PURCHASE_ORDER`
2. **字段命名**：保持前后端一致，使用 camelCase
3. **条件字段**：确保表单字段名与条件配置中的参数名一致

### 9.2 适配器实现

1. **OnSubmitData**：必须设置 `BusinessId` 和 `ProcessTitle`
2. **OnQueryData**：必须填充所有需要展示的字段
3. **OnConsentData**：根据 `OperationType` 区分同意和重新提交
4. **错误处理**：使用 `AFBizException` 抛出业务异常

### 9.3 事务管理

引擎使用 `TransactionalMiddleware` 自动管理事务。如需手动控制事务：

```csharp
[HttpPost("myAction")]
[Transactional]
public Result MyAction([FromBody] MyRequest request)
{
    // 方法将在事务中执行
    // 发生异常自动回滚
}
```

## 10. 常见问题

| 问题 | 解决方案 |
|------|---------|
| 表单数据未保存 | 检查 `OnSubmitData` 中是否设置了 `BusinessId` |
| 条件分支不生效 | 检查 `PreviewSetCondition` 中是否设置了条件字段值 |
| 审批人计算为空 | 确认 `IBpmnPersonnelProviderService` 实现是否正确注册 |
| 流程预览失败 | 检查 `PreviewSetCondition` 的参数名是否与条件配置一致 |
| 通知未发送 | 确认 `InformationTemplate` 配置和通知适配器是否注册 |
