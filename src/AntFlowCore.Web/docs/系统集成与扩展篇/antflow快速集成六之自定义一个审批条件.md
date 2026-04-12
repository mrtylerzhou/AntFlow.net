## antflow快速集成到已有系统之DIY条件低代码化

> 条件需要添加的formId字段类型

| 名称                     | 编码  | 描述                                                                                |
| ------------------------ | ----- | ----------------------------------------------------------------------------------- |
| 通用字符串条件           | 10000 | 用于比较两个字符串类型值是否相等                                                    |
| 通用数字条件             | 10001 | 用于比较两个数字的等于，大于等于等关系比较，支持的符号查看后端枚举JudgeOperatorEnum |
| 通用无代码日期条件       | 10002 | 用于比较两个日期是否相等（后面会增加类似数字比较）                                  |
| 通用无代码带时间日期条件 | 10003 | 和10002一样，用于带时间的日期                                                       |
| 通用集合条件             | 10004 | 用于比较集合中的值是否包含用户表单中输入的值                                        |

### 1. 前端设计

自定义条件需要改动前端流程设计器里的条件参数，核心改动数据是columnId，值上上面定义的10000-10004之间的任一个，具体使用哪个要根据业务来定。

改造以后流程设计提交到后端的json数据中的conditionList如下:

```json
{
	"conditionList": [
		[
			{
				"formId": "1",
				"columnId": "10001",
				"showType": "3",
				"type": 2,
				"showName": "账户类型",
				"optType": 5,
				"zdy1": "1",
				"opt1": "<",
				"zdy2": "",
				"opt2": "<",
				"columnDbname": "accountType",
				"columnType": "String",
				"fieldTypeName": "checkbox",
				"multiple": false,
				"multipleLimit": 0,
				"fixedDownBoxValue": "[{\"key\":1,\"value\":\"百度云\"},{\"key\":2,\"value\":\"腾讯云\"},{\"key\":3,\"value\":\"中通云\"}]",
				"condRelation": false
			}
		]
	],
	"sort": 1,
	"isDefault": 0,
	"groupRelation": false
}
```

> 核心就是这个columnId，后端会根据它来找到对应的条件比较器。columnDbname也是一个比较关键的参数，即要比较字段的名字。它要和流程提交时用做条件的字段的名字一样

### 2.后端设计

diy 流程继承自AbstractLowFlowSpyFormOperationAdaptor 抽象类。就ok了。

> 需要注意的是实现类必须有[DIYFormServiceAnno]属性


> 这里需要注意的是，前端条件中定义的条件字段名（即columnDbname)在流程发起的表单中必须包含。仍然以三方账号申请为例。设计流程时条件字段是accountType,流程提交时，这个字段必须存在（这里说的必须存在是指业务上必须存在，如果没有的话条件判断结果恒为false）
