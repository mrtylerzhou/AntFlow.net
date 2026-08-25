# DIY 表单审批人实现完整方案

## 功能需求
在 DIY 流程中，审批节点可以从自定义表单中获取某个字段（人员选择器）的值作为审批人。用户在表单中选择谁，审批节点就由谁来审批。

## 架构说明

### 现有的表单关联审批人机制（LF 表单）
AntFlowCore 已经为 LF（低代码）表单实现了表单关联审批人功能：
- **审批人类型枚举**：`PersonnelEnum.FORM_USERS_PERSONNEL = 16`
- **节点属性枚举**：`NodePropertyEnum.NODE_PROPERTY_FORM_RELATED = 16`
- **提供者**：`FormRelatedPersonnelProvider` - 从 `BusinessDataVo.Node2formRelatedAssignees` 获取人员 ID
- **数据传递**：`LowFlowApprovalService.ProcessFormRelatedUserConf()` 方法负责解析表单配置并填充 `Node2formRelatedAssignees`

### DIY 表单的差异
- DIY 表单使用 `AbstractLowFlowSpyFormOperationAdaptor<T>` 作为基类
- 每个 DIY 表单服务通过 `[DIYFormServiceAnno]` 注解标识
- 需要在 DIY 表单中实现类似 LF 表单的字段映射机制

---

## 实现方案

### 方案概述
我们将复用现有的 `FormRelatedPersonnelProvider`（审批人类型 16），但需要让 DIY 表单也能填充 `Node2formRelatedAssignees` 字段。

---

## 后端实现步骤

### 步骤 1：修改 `AbstractLowFlowSpyFormOperationAdaptor` 抽象类

**文件**：`src\AntFlowCore.Bpmn\adaptor\formoperation\AbstractLowFlowSpyFormOperationAdaptor.cs`

**已完成**：我们已经在 `PreviewSetCondition` 和 `LaunchParameters` 方法中添加了 `conditionsVo.BusinessDataVo = vo;`

现在需要添加处理表单关联人员的方法：

```csharp
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.vo;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using System.Text.Json;

namespace AntFlowCore.Bpmn.adaptor.formoperation;

public abstract class AbstractLowFlowSpyFormOperationAdaptor<T> : IFormOperationAdaptor<T> where T : BusinessDataVo
{
    // ... 现有的抽象方法 ...

    /// <summary>
    /// 可选：子类重写此方法来声明表单中的人员字段映射到哪些审批节点。
    /// 返回的 Dictionary key 是节点 ID，value 是表单字段名列表。
    /// </summary>
    protected virtual Dictionary<long, List<string>> GetFormRelatedAssigneesMapping()
    {
        return new Dictionary<long, List<string>>();
    }

    /// <summary>
    /// 处理表单关联人员配置（DIY 表单版本）
    /// 从业务数据对象中提取指定字段的值，填充到 Node2formRelatedAssignees
    /// </summary>
    protected void ProcessDIYFormRelatedAssignees(T businessDataVo, BpmnConfVo bpmnConfVo)
    {
        if (bpmnConfVo == null)
        {
            return;
        }

        int? extraFlags = bpmnConfVo.ExtraFlags;
        
        // 检查是否启用了表单关联审批人功能
        if (extraFlags != null && BpmnConfFlagsEnum.HasFlag(extraFlags, BpmnConfFlagsEnum.HAS_FORM_RELATED_ASSIGNEES))
        {
            // 获取子类定义的字段映射
            var mapping = GetFormRelatedAssigneesMapping();
            if (mapping == null || mapping.Count == 0)
            {
                return;
            }

            var node2formRelatedAssignees = new Dictionary<string, List<string>>();

            // 使用反射从业务数据对象中获取字段值
            var businessDataType = businessDataVo.GetType();
            
            foreach (var entry in mapping)
            {
                long nodeId = entry.Key;
                List<string> fieldNames = entry.Value;
                List<string> assigneeIds = new List<string>();

                foreach (string fieldName in fieldNames)
                {
                    var property = businessDataType.GetProperty(fieldName);
                    if (property == null)
                    {
                        continue;
                    }

                    var fieldValue = property.GetValue(businessDataVo);
                    if (fieldValue == null)
                    {
                        continue;
                    }

                    // 处理单个值（字符串）
                    if (fieldValue is string strValue && !string.IsNullOrEmpty(strValue))
                    {
                        assigneeIds.Add(strValue);
                    }
                    // 处理列表值
                    else if (fieldValue is System.Collections.IEnumerable enumerable && !(fieldValue is string))
                    {
                        foreach (var item in enumerable)
                        {
                            if (item != null)
                            {
                                assigneeIds.Add(item.ToString());
                            }
                        }
                    }
                }

                if (assigneeIds.Count > 0)
                {
                    node2formRelatedAssignees[nodeId.ToString()] = assigneeIds;
                }
            }

            businessDataVo.Node2formRelatedAssignees = node2formRelatedAssignees;
        }
    }

    public BpmnStartConditionsVo PreviewSetCondition(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        conditionsVo.BusinessDataVo = vo; // 传递完整的业务数据
        
        PreviewSetCondition(conditionsVo, vo);
        
        // 处理 DIY 表单关联审批人
        if (vo.BpmnConfVo != null)
        {
            ProcessDIYFormRelatedAssignees(vo, vo.BpmnConfVo);
        }
        
        return conditionsVo;
    }

    public BpmnStartConditionsVo LaunchParameters(T vo)
    {
        BpmnStartConditionsVo conditionsVo = new BpmnStartConditionsVo();
        conditionsVo.StartUserId = vo.StartUserId;
        conditionsVo.StartUserName = vo.StartUserName;
        conditionsVo.BusinessDataVo = vo; // 传递完整的业务数据
        
        LaunchParameters(conditionsVo, vo);
        
        // 处理 DIY 表单关联审批人（仅在启动时需要）
        if (vo.BpmnConfVo != null)
        {
            ProcessDIYFormRelatedAssignees(vo, vo.BpmnConfVo);
        }
        
        return conditionsVo;
    }

    public abstract void PreviewSetCondition(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public abstract void LaunchParameters(BpmnStartConditionsVo conditionsVo,T businessDataVo);
    public abstract void OnInitData(T vo);
    public abstract void OnQueryData(T vo);
    public abstract void OnSubmitData(T vo);
    public abstract void OnConsentData(T vo);
    public abstract void OnDisagreeData(T vo);
    public abstract void OnBackToModifyData(T vo);
    public abstract void OnCancellationData(T vo);
    public abstract void OnFinishData(BusinessDataVo vo);
}
```

### 步骤 2：在具体的 DIY 表单服务中使用

**示例**：修改 `ThirdPartyAccountApplyFlowService.cs`

```csharp
[DIYFormServiceAnno(SvcName = "DSFZH_WMA",Desc = "三方账号申请")]
public class ThirdPartyAccountApplyFlowService : AbstractLowFlowSpyFormOperationAdaptor<ThirdPartyAccountApplyVo>
{
    private readonly IThirdPartyAccountApplyService _thirdPartyAccountApplyService;

    public ThirdPartyAccountApplyFlowService(IThirdPartyAccountApplyService thirdPartyAccountApplyService)
    {
        _thirdPartyAccountApplyService = thirdPartyAccountApplyService;
    }

    /// <summary>
    /// 重写此方法来定义表单字段到审批节点的映射
    /// key: 审批节点 ID（在流程设计器中可以看到）
    /// value: 表单中的字段名列表（对应 ThirdPartyAccountApplyVo 的属性名）
    /// </summary>
    protected override Dictionary<long, List<string>> GetFormRelatedAssigneesMapping()
    {
        return new Dictionary<long, List<string>>
        {
            // 示例：节点 ID 为 123 的审批节点，从 ManagerUserId 字段获取审批人
            // { 123L, new List<string> { "ManagerUserId" } },
            
            // 实际使用时，根据具体的节点 ID 和字段名配置
        };
    }

    public override void PreviewSetCondition(BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo businessDataVo)
    {
        conditionsVo.AccountType = businessDataVo.AccountType;
        conditionsVo.StartUserId = businessDataVo.StartUserId;
    }

    public override void LaunchParameters(BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo businessDataVo)
    {
        conditionsVo.AccountType = businessDataVo.AccountType;
        conditionsVo.StartUserId = businessDataVo.StartUserId;
    }

    // ... 其他方法保持不变 ...
}
```

### 步骤 3：确保 BpmnConfVo 传递到业务数据对象

需要确保在调用表单服务时，`BpmnConfVo` 已经设置到业务数据对象中。

检查以下位置：
- 表单提交接口（通常在 Controller 层）
- 流程启动服务

**参考代码位置**：查找 `businessDataVo.BpmnConfVo =` 的赋值位置，确保在调用 `PreviewSetCondition` 和 `LaunchParameters` 之前已设置。

---

## 前端实现步骤

### 步骤 1：添加 DIY 表单审批人类型选项

**文件**：`H:\Antflow\antflow-vue\src\components\Workflow\drawer\approverDrawer.vue`

找到审批人类型的定义部分，添加新的选项：

```javascript
const setTypes = ref([
    { value: 12, label: '发起人自己' },
    { value: 13, label: '直属领导' },
    { value: 3, label: '主管' },
    { value: 2, label: '层层审批' },
    { value: 5, label: '指定人员' },
    { value: 4, label: '角色' },
    { value: 6, label: 'HRBP' },
    { value: 7, label: '发起人自选审批人' },
    { value: 14, label: '部门' },
    { value: 16, label: '表单关联（LF表单）' },  // 现有的
    { value: 20, label: 'DIY表单字段' },  // 新增：DIY 表单审批人
    { value: 17, label: '自定义审批规则' },
    { value: 18, label: '上一节点关联审批人' },
    { value: 19, label: '上一节点指定' },
]);
```

### 步骤 2：添加 DIY 表单字段选择UI

在 `approverDrawer.vue` 的模板中添加：

```vue
<!-- 在 displaySetType == 16 的配置区域后面添加 -->
<div class="approver_text" v-if="approverConfig.setType == 20">
    <div>
        <p><i style="color: red;">*</i>审批人类型:</p>
        <el-select v-model="approverConfig.property.formAssigneeProperty" placeholder="请选审批人类型"
            style="width: 300px">
            <el-option v-for="item in formUserOptionSet" :key="item.value" :label="item.label"
                :value="item.value" />
        </el-select>
    </div>
    <div>
        <p><i style="color: red;">*</i>DIY表单中的字段</p>
        <el-input v-model="diyFormFieldName" placeholder="请输入字段名称（如：ManagerUserId）" 
            style="width: 300px" clearable />
        <p class="tip">请输入DIY表单VO类中的属性名，例如：ManagerUserId、ApproverIds 等</p>
    </div>
</div>
```

### 步骤 3：添加字段绑定逻辑

在 `approverDrawer.vue` 的 `<script setup>` 部分添加：

```javascript
// 添加 DIY 表单字段名变量
const diyFormFieldName = ref('');

// 在 changeType 函数中添加对类型 20 的处理
const changeType = (val) => {
    if (val == PREV_NODE_APPOINTED_SET_TYPE) {
        return;
    }
    
    formInfoOptions.value = [];
    approverConfig.value.nodeApproveList = [];
    approverConfig.value.signType = 1;
    approverConfig.value.noHeaderAction = 0;
    checkedHRBP.value = '';
    
    // ... 其他现有逻辑 ...
    
    // 新增：处理 DIY 表单字段类型
    if (val == 20) {
        if (!approverConfig.value.property) {
            approverConfig.value.property = {};
        }
        // 初始化 formAssigneeProperty 为"表单中的人员"
        if (!approverConfig.value.property.formAssigneeProperty) {
            approverConfig.value.property.formAssigneeProperty = 1;
        }
        // 回填已保存的字段名
        diyFormFieldName.value = approverConfig.value.property.diyFormFieldName || '';
    }
}

// 监听字段名变化，保存到配置中
watch(diyFormFieldName, (newVal) => {
    if (approverConfig.value.setType == 20) {
        if (!approverConfig.value.property) {
            approverConfig.value.property = {};
        }
        approverConfig.value.property.diyFormFieldName = newVal;
    }
});
```

### 步骤 4：修改数据提交格式化

**文件**：`H:\Antflow\antflow-vue\src\utils\antflow\formatcommit_data.js`

找到审批人节点的数据格式化逻辑，添加对类型 20 的处理：

```javascript
// 在处理审批人节点的函数中
if (node.setType == 20) {
    // DIY 表单字段审批人
    approveObj.formAssigneeProperty = node.property.formAssigneeProperty;
    approveObj.diyFormFieldName = node.property.diyFormFieldName;
}
```

### 步骤 5：修改显示数据格式化

**文件**：`H:\Antflow\antflow-vue\src\utils\antflow\formatdisplay_data.js`

```javascript
// 在显示节点信息的函数中
if (node.setType == 20) {
    node.formAssigneeProperty = node?.property?.formAssigneeProperty;
    node.diyFormFieldName = node?.property?.diyFormFieldName;
}
```

---

## 使用说明

### 开发者使用步骤

#### 1. 在 DIY 表单 VO 中添加审批人字段

```csharp
public class YourFormVo : BusinessDataVo
{
    /// <summary>
    /// 审批人用户 ID
    /// </summary>
    [JsonPropertyName("managerUserId")]
    public string ManagerUserId { get; set; }

    /// <summary>
    /// 审批人姓名（可选，用于显示）
    /// </summary>
    [JsonPropertyName("managerUserName")]
    public string ManagerUserName { get; set; }

    // ... 其他业务字段 ...
}
```

#### 2. 在表单服务中配置字段映射

```csharp
[DIYFormServiceAnno(SvcName = "YOUR_FORM", Desc = "你的表单")]
public class YourFormService : AbstractLowFlowSpyFormOperationAdaptor<YourFormVo>
{
    // 重写映射方法（如果需要动态配置）
    protected override Dictionary<long, List<string>> GetFormRelatedAssigneesMapping()
    {
        // 方式1：硬编码（简单但不灵活）
        return new Dictionary<long, List<string>>
        {
            { 节点ID, new List<string> { "ManagerUserId" } }
        };
        
        // 方式2：从数据库或配置文件读取（推荐）
        // return LoadMappingFromDatabase();
    }
}
```

#### 3. 前端表单中添加人员选择组件

```vue
<template>
    <el-form-item label="审批人" prop="managerUserId">
        <UserSelector v-model="formData.managerUserId" 
            v-model:name="formData.managerUserName" />
    </el-form-item>
</template>
```

#### 4. 在流程设计器中配置

1. 添加审批节点
2. 点击审批节点，打开审批人设置
3. 选择审批人类型为 **"DIY表单字段"**
4. 选择审批人类型（表单中的人员/表单中的人员的直属领导等）
5. 输入字段名称：`ManagerUserId`
6. 保存配置

### 最终用户使用步骤

1. 打开表单填写页面
2. 在"审批人"字段中选择人员（例如：选择张三）
3. 提交表单
4. 系统自动将审批任务分配给张三
5. 张三收到待办任务并进行审批

---

## 优化建议

### 方案 A：配置化映射（推荐）

不在代码中硬编码节点 ID 和字段名的映射，而是：

1. **在数据库中创建配置表**

```sql
CREATE TABLE bpmn_diy_form_field_mapping (
    id BIGINT PRIMARY KEY,
    conf_id BIGINT NOT NULL COMMENT '流程配置ID',
    node_id BIGINT NOT NULL COMMENT '节点ID',
    form_code VARCHAR(50) NOT NULL COMMENT '表单编码',
    field_name VARCHAR(100) NOT NULL COMMENT '字段名',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_conf_node (conf_id, node_id)
);
```

2. **在流程设计器中保存配置时写入映射**

前端在保存审批人配置时，将字段名和节点 ID 的关系保存到配置表。

3. **运行时从配置表读取映射**

```csharp
protected override Dictionary<long, List<string>> GetFormRelatedAssigneesMapping()
{
    var confId = // 从当前上下文获取 confId
    return _mappingService.GetMappingByConfId(confId);
}
```

### 方案 B：使用节点配置 JSON

将字段名直接保存在节点的 `NodeConfigJson` 中，运行时解析：

```json
{
    "approverConf": {
        "diyFormRelatedConf": {
            "fieldName": "ManagerUserId",
            "assigneeProperty": 1
        }
    }
}
```

---

## 测试清单

### 后端测试

- [ ] 单人审批：表单选择一个人，审批任务正确分配
- [ ] 多人审批：表单选择多个人，生成多个审批任务
- [ ] 审批人类型：测试"表单中的人员"、"表单中人员的直属领导"等不同类型
- [ ] 字段为空：表单未选择审批人时，根据"审批人为空时"配置正确处理
- [ ] 多个节点：不同审批节点使用不同表单字段

### 前端测试

- [ ] UI 显示：审批人设置抽屉中正确显示 DIY 表单字段选项
- [ ] 字段输入：可以输入并保存字段名
- [ ] 配置回显：打开已保存的节点，配置正确回显
- [ ] 数据提交：保存流程时，配置数据正确提交到后端
- [ ] 流程预览：流程图中正确显示审批人配置信息

---

## 常见问题

### Q1: 如何获取节点 ID？
**A**: 在流程设计器中选中节点，在浏览器开发者工具的控制台输入 `$0.__vueParentComponent.ctx.nodeConfig.id` 查看。

### Q2: 字段名必须和 VO 属性完全一致吗？
**A**: 是的，必须完全一致（区分大小写），因为使用反射获取属性值。

### Q3: 可以支持 JSON 字段名吗？
**A**: 可以，但需要在代码中额外处理 `[JsonPropertyName]` 特性。

### Q4: 如何支持级联选择（如：部门 -> 人员）？
**A**: 可以扩展 `ProcessDIYFormRelatedAssignees` 方法，支持从嵌套对象中提取值。

---

## 附录

### 相关文件清单

**后端文件**：
- `src\AntFlowCore.Bpmn\adaptor\formoperation\AbstractLowFlowSpyFormOperationAdaptor.cs` - 抽象基类（需修改）
- `src\AntFlowCore.Bpmn\adaptor\personnel\provider\FormRelatedPersonnelProvider.cs` - 审批人提供者（复用）
- `src\AntFlowCore.Engine\service\formprocess\ThirdPartyAccountApplyFlowService.cs` - 示例实现

**前端文件**：
- `H:\Antflow\antflow-vue\src\components\Workflow\drawer\approverDrawer.vue` - 审批人设置抽屉
- `H:\Antflow\antflow-vue\src\utils\antflow\formatcommit_data.js` - 数据提交格式化
- `H:\Antflow\antflow-vue\src\utils\antflow\formatdisplay_data.js` - 数据显示格式化

### 参考文档
- `docs-site-net\dev-guide\extend-approver.md` - 审批人扩展指南
- `docs-site-net\dev-guide\integrate-existing.md` - 系统集成指南
