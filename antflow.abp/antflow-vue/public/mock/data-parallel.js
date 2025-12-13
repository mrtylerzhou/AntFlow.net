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
      nodeType: 1, //节点类型： 1 发起人节点 2路由 3条件节点 4审批人节点 6 抄送节点,7 并行审批
      priorityLevel: "", // 条件优先级
      setType: "5", //指定审批人类型，5-指定人员，4-指定角色，3-指定层级审批 ，13-直属领导，12-发起人自己，6-HRBP
      directorLevel: "", //第n层主管
      signType: "", //多人审批时采用的审批方式 1会签 2或签
      noHeaderAction: "", //审批人为空时 1自动审批通过/不允许发起 2转交给审核管理员
      ccSelfSelectFlag: "", //允许发起人自选抄送人
      conditionList: [], //当审批单同时满足以下条件时进入此流程
      nodeApproveList: [], //操作人
      childNode: {
        nodeName: "审核人",
        nodeType: 7,
        setType: 5,
        directorLevel: 1,
        signType: 1,
        noHeaderAction: 0,
        childNode: {
          nodeName: "并行审批路由节点",
          nodeType: 2,
          priorityLevel: 1,
          setType: 1,
          directorLevel: 1,
          signType: 1,
          noHeaderAction: 0,
          ccSelfSelectFlag: 1,
          conditionList: [],
          nodeApproveList: [],
          conditionNodes: [],
          parallelNodes: [
            {
              //并行审批节点
              nodeName: "审核人1",
              nodeType: 4,
              priorityLevel: 1,
              setType: 5,
              directorLevel: 1,
              signType: 1,
              noHeaderAction: 0,
              ccSelfSelectFlag: 1,
              isDefault: 0,
              nodeApproveList: [
                {
                  type: 5,
                  targetId: 1,
                  name: "张三",
                },
              ],
              childNode: null,
              conditionNodes: [],
              conditionList: [],
            },
            {
              nodeName: "审核人2",
              nodeType: 4,
              priorityLevel: 2,
              setType: 5,
              directorLevel: 1,
              signType: 1,
              noHeaderAction: 0,
              ccSelfSelectFlag: 1,
              conditionList: [],
              nodeApproveList: [
                {
                  type: 5,
                  targetId: 1,
                  name: "张三",
                },
              ],
              childNode: null,
              conditionNodes: [],
              isDefault: 0,
            },
          ],
          childNode: {
            nodeName: "抄送人",
            type: 6,
            ccSelfSelectFlag: 1,
            childNode: null,
            nodeApproveList: [],
          },
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
