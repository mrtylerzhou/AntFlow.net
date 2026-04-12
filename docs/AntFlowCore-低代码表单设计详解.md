# AntFlowCore 低代码表单设计详解：权限控制到字段级

## 前言

AntFlowCore 原生支持低代码表单，你不需要写代码，在流程设计器中拖拽就能配置出一个审批表单，并且支持不同环节不同字段权限（隐藏/只读/可编辑）。本文深入解析低代码表单的设计原理和使用方法。

## 什么是低代码表单

简单来说：
- 你在流程设计器中可视化设计表单，选择字段类型（文本/数字/日期/多选框/下拉框...）
- 配置每个字段在不同审批环节的权限（隐藏/只读/可编辑）
- 用户发起申请的时候，前端根据你的配置动态渲染表单
- AntFlowCore 自动保存表单数据，你不需要建表写代码

就是这么简单，零代码实现一个审批表单。

## 整体架构设计

AntFlowCore 低代码表单分为三层：

```
流程设计器 → 表单配置 → 存储到数据库
        ↓
用户发起/审批 → 读取配置 → 前端动态渲染
        ↓
用户提交 → AntFlowCore 自动保存 → 绑定到流程实例
```

核心表结构：

| 表 | 作用 |
|----|------|
| `bpmn_conf_lfformdata` | 表单整体配置 |
| `bpmn_conf_lfformdata_field` | 字段配置（每个字段的类型、标签等） |
| `lf_main` | 表单实例（每次发起创建一个实例） |
| `lf_main_field` | 字段值（每个字段的值存储在这里） |

## 核心源码解析

我们来看核心处理类 `LowFlowApprovalService.cs`，这是低代码表单的核心处理器。

### 1. 提交表单：OnSubmitData

用户发起流程提交表单，核心代码：

```csharp
public void OnSubmitData(UDLFApplyVo vo)
{
    var lfFields = vo.LfFields;
    if (lfFields == null || lfFields.Count == 0)
    {
        throw new AFBizException("form data does not contain any field");
    }

    long confId = vo.BpmnConfVo.Id;
    string formCode = vo.FormCode;

    // 1. 创建表单实例
    var main = new LFMain
    {
        Id = SnowFlake.NextId(),
        ConfId = confId,
        FormCode = formCode,
        CreateUser = SecurityUtils.GetLogInEmpName(),
        TenantId = MultiTenantUtil.GetCurrentTenantId(),
    };
    _mainService.baseRepo.Insert(main);
    long mainId = main.Id;

    // 2. 获取字段配置缓存
    if (!allFieldConfMap.TryGetValue(confId, out var fieldConfMap))
    {
        Dictionary<string,BpmnConfLfFormdataField> name2SelfMap = 
            _lfformdataFieldService.QryFormDataFieldMap(confId);
        allFieldConfMap[confId] = name2SelfMap;
        fieldConfMap = name2SelfMap;
    }

    // 3. 解析用户提交的字段值，保存到数据库
    var mainFields = LFMainField.ParseFromMap(lfFields, fieldConfMap, mainId,formCode);
    _lfMainFieldService.baseRepo.Insert(mainFields);

    // 绑定业务ID
    vo.BusinessId = mainId.ToString();
    vo.ProcessDigest = vo.Remark;

    // 4. 调用自定义扩展钩子
    IEnumerable<ILFFormOperationAdaptor> adapters = 
        ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
    foreach (var adapter in adapters)
    {
        LFFormServiceAnnoAttribute? attr = adapter.GetType()
            .GetCustomAttribute<LFFormServiceAnnoAttribute>();
       
        if (attr != null && attr.SvcName.Equals(vo.FormCode))
        {
            adapter.OnSubmitData(vo);
        }
    }
}
```

设计要点：
- **配置缓存**：把字段配置缓存到静态字典 `allFieldConfMap`，不需要每次都查数据库
- **自动解析**：`LFMainField.ParseFromMap` 自动把用户提交的字典转换成实体列表保存
- **扩展钩子**：如果你有自定义业务逻辑，可以实现 `ILFFormOperationAdaptor` 注入自定义处理

### 2. 查询表单：OnQueryData

打开表单的时候查询数据，核心代码：

```csharp
public void OnQueryData(UDLFApplyVo vo)
{
    // 1. 查询表单实例
    LFMain lfMain = _mainService.baseRepo
        .Where(a => a.Id == long.Parse(vo.BusinessId))
        .First();
    
    long mainId = lfMain.Id;
    long confId = lfMain.ConfId.Value;
    string formCode = lfMain.FormCode;

    // 2. 缓存字段配置
    if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap))
    {
        lfFormdataFieldMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
        allFieldConfMap[confId] = lfFormdataFieldMap;
    }

    // 3. 查询所有字段值
    List<LFMainField> lfMainFields = _lfMainFieldService
        .baseRepo
        .Where(x => x.MainId == mainId)
        .ToList();

    // 4. 根据字段类型转换值，返回给前端
    Dictionary<string, object> fieldVoMap = new Dictionary<string, object>();
    Dictionary<String, List<LFMainField>> fieldName2SelfMap =
        lfMainFields.GroupBy(x => x.FieldId).ToDictionary(g => g.Key, g => g.ToList());

    foreach (var id2SelfEntry in fieldName2SelfMap)
    {
        string fieldName = id2SelfEntry.Key;
        if (!lfFormdataFieldMap.TryGetValue(fieldName, out var currentFieldProp))
        {
            throw new AFBizException($"field with name:{fieldName} has no property");
        }

        var fields = id2SelfEntry.Value;
        int valueLen = fields.Count;
        List<object> actualMultiValue = valueLen == 1 ? null : new List<object>(valueLen);

        foreach (var field in fields)
        {
            int fieldType = currentFieldProp.FieldType.Value;
            var fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldType);
            
            // 根据字段类型转换值
            object actualValue = null;
            switch (fieldTypeEnum)
            {
                case var ftype when ftype == LFFieldTypeEnum.STRING:
                    actualValue = field.FieldValue;
                    // 如果是JSON，自动反序列化
                    if (actualValue != null && actualValue.ToString().StartsWith("{"))
                    {
                        actualValue = JsonSerializer
                            .Deserialize<Dictionary<string, object>>(actualValue.ToString());
                    }
                    break;
                case var ftype when ftype == LFFieldTypeEnum.NUMBER:
                    actualValue = field.FieldValueNumber;
                    break;
                case var ftype when ftype == LFFieldTypeEnum.DATE_TIME:
                    actualValue = field.FieldValueDt?.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
                case var ftype when ftype == LFFieldTypeEnum.DATE:
                    actualValue = field.FieldValueDt?.ToString("yyyy-MM-dd");
                    break;
                case var ftype when ftype == LFFieldTypeEnum.TEXT:
                    actualValue = field.FieldValueText;
                    break;
                case var ftype when ftype == LFFieldTypeEnum.BOOLEAN:
                    actualValue = bool.Parse(field.FieldValue);
                    break;
            }

            if (valueLen == 1)
            {
                fieldVoMap[fieldName] = actualValue;
            }
            else
            {
                actualMultiValue.Add(actualValue);
                fieldVoMap[fieldName] = actualMultiValue;
            }
        }

        vo.LfFields = fieldVoMap;
    }

    // 加载表单配置给前端渲染
    var bpmnConfLfFormdataList = _lfformdataService.baseRepo
        .Where(x => x.BpmnConfId == confId)
        .ToList();
    vo.LfFormData = bpmnConfLfformdataList.First().Formdata;

    // 自定义扩展钩子
    IEnumerable<ILFFormOperationAdaptor> adapters = 
        ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
    foreach (var adapter in adapters)
    {
        LFFormServiceAnnoAttribute? attr = adapter.GetType()
            .GetCustomAttribute<LFFormServiceAnnoAttribute>();
       
        if (attr != null && attr.SvcName.Equals(vo.FormCode))
        {
            adapter.OnQueryData(vo);
        }
    }
}
```

设计要点：
- 根据字段类型自动转换值（字符串/数字/日期/布尔都处理好了）
- JSON 字符串自动反序列化成对象返回给前端
- 同样支持自定义扩展钩子

### 3. 审批修改：OnConsentData

审批人审批的时候，可以修改可编辑字段，核心逻辑：

```csharp
public void OnConsentData(UDLFApplyVo vo)
{
    var lfFields = vo.LfFields;
    if (lfFields == null || lfFields.Count == 0)
    {
        throw new AFBizException("form data does not contain any field");
    }

    // 1. 查询出原来的表单实例
    var lfMain = _mainService.baseRepo
        .Where(a => a.Id == long.Parse(vo.BusinessId))
        .First();
    long mainId = lfMain.Id;
    long confId = lfMain.ConfId.Value;

    // 2. 获取字段配置
    if (!allFieldConfMap.TryGetValue(confId, out var fieldConfMap))
    {
        fieldConfMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
        allFieldConfMap[confId] = fieldConfMap;
    }

    // 3. 更新字段值
    var submitLfFields = vo.LfFields;
    if (submitLfFields != null && submitLfFields.Any())
    {
        var mainFields = LFMainField.ParseFromMap(submitLfFields, fieldConfMap, mainId, vo.FormCode);
        
        // 去掉已经存在的，只更新新增/修改的
        List<LFMainField> existing = _lfMainFieldService
            .baseRepo
            .Where(a=>a.MainId==mainId)
            .ToList();
        mainFields.RemoveAll(mainField=>
            existing.Any(a=>a.FieldId==mainField.FieldId));
        
        if(mainFields.Any())
        {
            _lfMainFieldService.baseRepo.Insert(mainFields);
        }
    }

    // 4. 应用字段权限，隐藏字段不能更新
    List<LFFieldControlVO> fieldControls = _bpmnNodeLfFormdataFieldControlService
        .GetFieldControlByProcessNumberAndElementId(vo.ProcessNumber, vo.TaskDefKey);

    foreach (LFMainField field in existing)
    {
        LFFieldControlVO? control = fieldControls
            .FirstOrDefault(a=>a.FieldId==field.FieldId);
        
        if (control != null && 
            (control.Perm == StringConstants.HIDDEN_FIELD_PERMISSION || 
             control.Perm == StringConstants.READ_ONLY_FIELD_PERMISSION))
        {
            // 隐藏/只读字段不更新
            continue;
        }

        string newValue = vo.LfFields[field.FieldId]?.ToString();
        if (!StringConstants.HIDDEN_FIELD_VALUE.Equals(newValue))
        {
            field.FieldValue = newValue;
        }
    }

    // 批量更新
    _lfMainFieldService.baseRepo.Update(existing);

    // 自定义扩展钩子
    // ...
}
```

这里最关键的就是**字段权限控制**：

```csharp
if (control != null && 
    (control.Perm == HIDDEN_FIELD_PERMISSION || 
     control.Perm == READ_ONLY_FIELD_PERMISSION))
{
    // 隐藏/只读字段不更新
    continue;
}
```

就算前端被篡改，后端这里也会拦截，保证权限控制生效，非常安全。

## 字段权限控制：不同环节不同权限

AntFlowCore 支持**字段级权限控制**，你可以给每个字段在每个环节配置不同的权限：

| 权限 | 说明 |
|------|------|
| `hidden` | 隐藏，前端不显示，后端也不允许修改 |
| `readonly` | 只读，前端显示，但不能修改 |
| `edit` | 可编辑，前端显示，可以修改 |

使用场景举例：
- 发起人填写：所有字段可编辑
- 部门经理审批：金额字段只读，审批意见字段可编辑
- 总经理审批：所有字段只读，只有审批按钮可用

### 权限控制怎么工作

1. **设计时**：在流程设计器，给每个节点每个字段配置权限
2. **运行时**：查询当前节点的字段权限，返回给前端
3. **前端**：根据权限渲染（隐藏/禁用/可编辑）
4. **后端**：保存的时候再次校验权限，隐藏/只读字段不允许修改

前后端双重校验，保证安全。

### 源码：获取当前节点字段权限

```csharp
public List<LFFieldControlVO> GetFieldControlByProcessNumberAndElementId(
    string processNumber, 
    string elementId)
{
    // 查询当前节点的字段权限配置
    var list = _baseRepo.Where(a => 
        a.ProcessNumber == processNumber && 
        a.ElementId == elementId)
    .ToList();

    return list;
}
```

保存的时候后端再校验一次，就是我们在 `OnConsentData` 看到的那段权限检查代码。

## 支持的字段类型

AntFlowCore 支持这些常用字段类型：

| 类型 | 存储 | 说明 |
|------|------|------|
| 字符串 | `FieldValue` | 单行文本 |
| 文本 | `FieldValueText` | 多行文本 |
| 数字 | `FieldValueNumber` | 数字 |
| 日期 | `FieldValueDt` | 日期（yyyy-MM-dd） |
| 日期时间 | `FieldValueDt` | 日期时间（yyyy-MM-dd HH:mm:ss） |
| 布尔 | `FieldValue` | 勾选框 |
| 下拉框/多选框 | `FieldValue` | JSON 保存选中值 |

所有类型都有对应的处理，自动转换。

## 扩展自定义表单业务逻辑

如果你需要自定义业务逻辑，比如提交表单后发送通知、写业务表，可以扩展 `ILFFormOperationAdaptor`:

```csharp
public interface ILFFormOperationAdaptor
{
    // 初始化数据的时候调用
    void OnInitData(UDLFApplyVo vo);
    
    // 查询数据的时候调用
    void OnQueryData(UDLFApplyVo vo);
    
    // 提交数据的时候调用
    void OnSubmitData(UDLFApplyVo vo);
    
    // 审批同意的时候调用
    void OnConsentData(UDLFApplyVo vo);
    
    // 驳回修改的时候调用
    void OnBackToModifyData(UDLFApplyVo vo);
    
    // 取消的时候调用
    void OnCancellationData(UDLFApplyVo vo);
    
    // 流程完成的时候调用
    void OnFinishData(BusinessDataVo vo);
}
```

然后加上特性标记：

```csharp
[LFFormServiceAnno(SvcName = "my-custom-form")]
public class MyCustomFormAdaptor : ILFFormOperationAdaptor
{
    public void OnSubmitData(UDLFApplyVo vo)
    {
        // 自定义提交逻辑，比如写入你的业务表
        var businessId = vo.BusinessId;
        var formData = vo.LfFields;
        
        // 你的业务逻辑...
        _myBusinessService.Insert(new MyBusinessEntity
        {
            // ...
        });
    }
    
    // ... 其他钩子
}
```

注册到 DI：

```csharp
services.AddScoped<ILFFormOperationAdaptor, MyCustomFormAdaptor>();
```

AntFlowCore 会自动发现调用，非常方便。

## 缓存设计

AntFlowCore 对表单配置做了静态缓存：

```csharp
// key is confid,value is a map of field's name and its self
private static Dictionary<long, Dictionary<String, BpmnConfLfFormdataField>> allFieldConfMap =
    new Dictionary<long, Dictionary<string, BpmnConfLfFormdataField>>();
```

第一次查询配置后缓存到内存，后续直接用，不需要每次都查数据库，性能很好。

如果修改了表单配置，清空缓存就行了，下次访问自动加载新配置。

## 使用步骤

### 第一步：在设计器创建表单

1. 打开流程设计器
2. 进入"表单设计"
3. 拖拽添加字段，配置字段名称、类型
4. 保存

### 第二步：配置不同环节字段权限

1. 选中某个节点
2. 打开"字段权限"配置
3. 给每个字段选择权限（隐藏/只读/可编辑）
4. 保存

### 第三步：发起流程

前端自动根据配置渲染表单，用户填写提交，AntFlowCore 自动保存数据。

就是这么简单，零代码搞定一个审批表单。

## 常见问题

### 1. 低代码表单适合什么场景？

适合：
- 简单审批表单，不需要复杂业务逻辑
- 快速开发，需求经常变化
- 不需要和现有业务系统深度集成

不适合：
- 非常复杂的业务表单，有大量交互
- 需要和现有业务深度集成，这种情况你可以用自定义表单，AntFlowCore 只负责流程

### 2. 数据存在哪里？

低代码表单数据存在 `lf_main` 和 `lf_main_field` 表中，你随时可以查询使用，也可以在扩展钩子中同步到你的业务表。

### 3. 支持文件上传吗？

支持，文件上传后保存到服务器，表单中存储文件地址，和普通字段一样使用。

### 4. 修改表单配置后，已有数据怎么办？

不影响，已有数据按照原来的结构，新增字段按照新结构，兼容处理。

### 5. 可以和自定义业务结合吗？

完全可以，通过 `ILFFormOperationAdaptor` 钩子，你可以在各个生命周期插入你的自定义业务逻辑。

## 总结

AntFlowCore 低代码表单设计非常简洁：

- **配置驱动**：设计器拖拽配置，零代码生成表单
- **字段级权限**：不同环节不同权限，前后端双重校验
- **灵活扩展**：提供完整生命周期钩子，方便插入自定义业务逻辑
- **性能优秀**：配置缓存，减少数据库查询

不管你是快速开发简单审批表单，还是复杂场景用自定义表单，AntFlowCore 低代码表单都能满足你的需求。

---

**相关链接：**
- [AntFlowCore 事件系统详解](./AntFlowCore-事件系统详解-监听流程状态变化.md)
- [AntFlowCore 自定义按钮开发实战](./AntFlowCore-自定义按钮开发实战.md)
- [上一篇：撤回驳回回退实现差异详解](./AntFlowCore-撤回驳回回退-实现差异详解.md)
