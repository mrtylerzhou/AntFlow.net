export default {
  code: "200",
  msg: "success",
  data: {
    tableId: 1, //审批id
    workFlowDef: {
      name: "合同审批", //审批名称
    },
    directorMaxLevel: 4, //审批主管最大层级
    flowPermission: [], //发起人
    nodeConfig: {
      nodeName: "发起人", //节点名称
      nodeType: 1, //节点类型： 1 发起人节点 2路由 3条件节点 4审批人节点 6 抄送节点
      priorityLevel: "", // 条件优先级
      setType: "5", //指定审批人类型，5-指定人员，4-指定角色，3-指定层级审批 ，13-直属领导，12-发起人自己，6-HRBP
      directorLevel: "", //第n层主管
      signType: "", //多人审批时采用的审批方式 1会签 2或签
      noHeaderAction: "", //审批人为空时 0不允许发起  1 跳过 2转交给审核管理员
      ccSelfSelectFlag: "", //允许发起人自选抄送人
      conditionList: [], //当审批单同时满足以下条件时进入此流程
      nodeApproveList: [], //操作人
      childNode: {
        nodeName: "审核人",
        nodeType: 4,
        setType: 5,
        directorLevel: 1,
        signType: 1,
        noHeaderAction: 0,
        childNode: {
          nodeName: "路由",
          nodeType: 2,
          priorityLevel: 1,
          setType: 1,
          directorLevel: 1,
          signType: 1,
          noHeaderAction: 0,
          ccSelfSelectFlag: 1,
          conditionList: [],
          nodeApproveList: [],
          childNode: {
            nodeName: "抄送人",
            type: 6,
            ccSelfSelectFlag: 1,
            childNode: null,
            nodeApproveList: [],
          },
          conditionNodes: [
            {
              //条件节点
              nodeName: "条件1",
              nodeType: 3,
              priorityLevel: 1,
              setType: 1,
              directorLevel: 1,
              signType: 1,
              noHeaderAction: 0,
              ccSelfSelectFlag: 1,
              isDefault: 0,
              conditionList: [
                {
                  //当前条件
                  formId: 0, //条件表单的唯一Id
                  columnId: 0, //用于条件判断的字段Id，与后端对应
                  type: 1, //1 发起人 2其他
                  optType: "", //["", "<", ">", "≤", "=", "≥","介于两数之间"][optType]
                  zdy1: "", //左侧自定义内容
                  zdy2: "", //右侧自定义内容
                  opt1: "", //左侧符号 < ≤
                  opt2: "", //右侧符号 < ≤
                  columnDbname: "", //条件字段名称
                  columnType: "", //条件字段类型
                  showType: "", //3多选 其他
                  showName: "", //展示名
                  fixedDownBoxValue: "", //多选数组
                },
              ],
              nodeApproveList: [],
              childNode: {
                nodeName: "审核人",
                nodeType: 4, //节点类型： 1 发起人节点 2路由 3条件节点 4审批人节点 6 抄送节点
                priorityLevel: 1, //条件优先级
                setType: 5, //指定审批人类型，5-指定人员，4-指定角色，3-指定层级审批 ，13-直属领导，12-发起人自己，6-HRBP
                directorLevel: 1, //当 setType= 3时，代表指定第几级领导审批
                signType: 1, //多人审批时采用的审批方式 1-会签 2-或签
                noHeaderAction: 0,
                ccSelfSelectFlag: 1, //允许发起人自选
                conditionList: [],
                nodeApproveList: [
                  {
                    type: 5, //指定审批人类型，5-指定人员，4-指定角色，3-指定层级审批 ，13-直属领导，12-发起人自己，6-HRBP
                    targetId: 2,
                    name: "李四",
                  },
                ],
                childNode: null,
                conditionNodes: [],
                templateVos: [], //消息通知设置
              },
              conditionNodes: [],
            },
            {
              nodeName: "条件2",
              nodeType: 3,
              priorityLevel: 2,
              setType: 1,
              directorLevel: 1,
              signType: 1,
              noHeaderAction: 0,
              ccSelfSelectFlag: 1,
              conditionList: [],
              nodeApproveList: [],
              childNode: null,
              conditionNodes: [],
              isDefault: 1,
            },
          ],
        },
        nodeApproveList: [
          {
            type: 5,
            targetId: 1,
            name: "张三",
          },
        ],
      },
      conditionNodes: [],
    },
  },
};
