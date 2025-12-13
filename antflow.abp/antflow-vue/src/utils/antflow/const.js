/*
 * @Date: 2024-09-21 22:05:32
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2024-09-21 22:05:32
 * @FilePath: /src/utils/antflow/const.js
 */

export let bgColors = [
  "192,192,192",
  "87, 106, 149",
  "255，97，0",
  "65，105，225",
  "255, 148, 62",
  "50, 150, 250",
  "50, 150, 250",
]; // '灰色, 蓝色, 橙色, 黄色, 黄色'
export let placeholderList = [
  "",
  "发起人",
  "",
  "条件",
  "审核人",
  "",
  "抄送人",
  "审核人",
];
export let nodeTypeList = [
  "未知",
  "发起人",
  "网关",
  "条件",
  "审核人",
  "未知",
  "抄送人",
  "并行审批",
];
export let signTypeObj = {
  1: "会签",
  2: "或签", //
  3: "顺序会签", //拒绝
};
export let setTypes = [
  { value: 5, label: "指定人员" },
  { value: 4, label: "指定角色" },
  { value: 6, label: "HRBP" },
  { value: 13, label: "直属领导" },
  // { value: 2, label: '层层审批' },
  { value: 3, label: "指定层级审批" },
  // { value: 8, label: '关联业务表' },
  { value: 12, label: "发起人自己" },
  { value: 7, label: "发起人自选审批人" },
  // { value: 14, label: '指定部门' },
];
export let hrbpOptions = [
  { value: 1, label: "HRBP" },
  { value: 2, label: "HRBP Leader" },
];
export let optTypes = [
  { value: 1, label: "大于等于" },
  { value: 2, label: "大于" },
  { value: 3, label: "小于等于" },
  { value: 4, label: "小于" },
  { value: 5, label: "等于" },
  { value: 6, label: "介于两个数之间" },
];

export let opt1s = [
  { value: "<", label: "<" },
  { value: "≤", label: "≤" },
];
/**审批按钮lable-value */
export class approvalButtonConf {
  static preview = 0; //预览
  static submit = 1; //提交
  static resubmit = 2; //重新提交
  static agree = 3; //同意
  static noAgree = 4; //拒绝
  static repulse = 6; //退回上节点修改
  static invalid = 7; //作废
  static print = 8; //打印
  static undertake = 10; //承办
  static changeApprove = 11; //变更处理人
  static terminate = 12; //终止
  static forward = 15; //转发
  static repulse = 18; //退回
  static addApproval = 19; //加批
  static transfer = 21; //转办
  static selectApprove = 22; //自选审批人
  static backToAnyNode = 23; //退回任意节点
  static currentNodeReduceSign = 24; //当前节点减签
  static currentNodeAddSign = 25; //当前节点加签
  static futureNodeChangeApprove = 26; //未来节点变更处理人
  static futureNodeReduceSign = 27; //未来节点减签
  static futureNodeAddSign = 28; //未来节点加签
  static withdraw = 29; //流程撤回
  static inApproval = 99; //处理中
  static completed = 100; //已完成

  static buttonsObj = {
    0: "预览",
    1: "提交", //提交
    2: "重新提交", //
    3: "同意", //拒绝
    4: "不同意", //拒绝
    6: "退回上节点修改", //退回上节点修改
    7: "作废", //
    8: "打印", //打印
    10: "承办", //承办
    11: "当前节点变更处理人", //变更处理人
    12: "终止", //终止
    13: "添加审批人", //添加审批人
    15: "转发", //转发
    18: "退回", //退回修改
    19: "加批", //加批
    21: "转办", //转办
    22: "自选审批人", //自选审批人
    23: "退回任意节点", //退回任意节点
    24: "当前节点减签",
    25: "当前节点加签",
    26: "未来节点变更处理人",
    27: "未来节点减签",
    28: "未来节点加签",
    29: "流程撤回",
  };
}
/**
 * 流程设计审批按钮显示
 */
export let approvalPageButtons = [
  {
    value: approvalButtonConf.agree,
    label: "同意",
    description: "审批通过，流转到下一个节点",
    type: "default",
  },
  {
    value: approvalButtonConf.noAgree,
    label: "不同意",
    description: "当不同意任务时，当前任务被终止，并结束整个流程",
    type: "default",
  },
  {
    value: approvalButtonConf.repulse,
    label: "退回",
    description: "退回到(发起人或任意节点)，流程重新开始或者回到当前审批人",
  },

  {
    value: approvalButtonConf.transfer,
    label: "转办",
    description:
      "审批页面显示【转办】按钮，转办后选择审批人，转办后自己将不再进行审批",
  },
  {
    value: approvalButtonConf.addApproval,
    label: "加批",
    description:
      "在当前任务上额外添加新人员，以处理相关事项或提供必要的审批或意见",
  },
];
export let startPageButtons = [
  { value: approvalButtonConf.submit, label: "提交", type: "default" },
  { value: approvalButtonConf.resubmit, label: "重新提交", type: "default" },
  { value: approvalButtonConf.terminate, label: "终止" },
];
export let viewPageButtons = [
  { value: approvalButtonConf.preview, label: "预览", type: "default" },
  { value: approvalButtonConf.print, label: "打印" },
  { value: approvalButtonConf.forward, label: "转发" },
];
/**
 * 自定义表单路径与FormCode映射
 */
export const bizFormMaps = new Map([
  ["DSFZH_WMA", "/forms/form1.vue"],
  ["LEAVE_WMA", "/forms/form2.vue"],
  ["UCARREFUEl_WMA", "/forms/form3.vue"],
  ["PURCHASE_WMA", "/forms/form4.vue"],
  ["BXSP_WMA", "/forms/form5.vue"],
]);

/**审批按钮颜色显示 */
export let approveButtonColor = {
  0: "info",
  1: "primary", //
  2: "primary", //
  3: "success", //同意
  4: "danger", //拒绝
  5: "danger", //
  6: "danger", //
  7: "primary", //
  8: "danger", //
  10: "warning", //承办
  13: "primary", //
  18: "warning", //退回
  19: "success", //加批
  21: "primary", //转办
  23: "warning", //驳回
  99: "success", //处理中
  100: "info",
};

/**
 * 1、控件对应后端api的判断类型
 * 2、用于条件节点 对接 流程引擎中 条件判断
 * 3、与后端约定的值
 */
export const condition_columnTypeMap = new Map([
  ["input", "10000"], //"int/fload/double/string" input
  ["input-number", "10001"], //"Double"
  ["select", "10000"], //"string" select
  ["checkbox", "10004"], //"string" checkbox
  ["radio", "10001"],
  ["switch", "10001"],
  ["time", "10002"],
  ["time-range", "10003"],
  ["data-range", "10002"],
  ["date", "10002"],
]);

/**
 * 1、控件是在条件节点 选择条件时候否显示
 * 2、对应后端数据解析 与后端约定的值
 * Mapping: 1-string 2-int 3-date 4-time 5-text/长字符串 6-boolean 7-二进制/byte
 */
export const condition_filedTypeMap = new Map([
  ["input", "1"], //"String"
  ["input-number", "4"], //"time"
  ["select", "2"], //"int" select
  ["checkbox", "1"], //"String" checkbox
  //['radio', '2'], //  int radio
  ["switch", "6"], // boolean switch
  ["time", "1"],
  // ['time-range', '1'],
  // ['data-range', '1'],
  ["date", "1"],
]);
/**
 * 判断控件的值的类型
 */
export const condition_filedValueTypeMap = new Map([
  ["input", "String"], //"Double"
  ["input-number", "String"], //"Double"
  ["select", "Int"], //"Int" select
  ["checkbox", "String"], //checkbox 对应 VForm 是Array
  ["radio", "Int"],
  ["switch", "Boolean"],
  ["time", "String"],
  ["time-range", "String"],
  ["data-range", "String"],
  ["date", "String"],
]);

export const noticeUserList = [
  {
    value: "1",
    label: "申请人",
  },
  {
    value: "2",
    label: "所有已审批人",
  },
  {
    value: "3",
    label: "当前节点审批人",
  },
  {
    value: "4",
    label: "被转发人",
  },
  {
    value: "5",
    label: "指定人员",
  },
  {
    value: "6",
    label: "指定角色",
  },
];
export const messageSendTypeList = [
  {
    active: false,
    id: 1,
    name: "邮件",
  },
  {
    active: false,
    id: 2,
    name: "短信",
  },
  {
    active: false,
    id: 3,
    name: "app推送",
  },
  {
    active: false,
    id: 5,
    name: "企微",
  },
  {
    active: false,
    id: 6,
    name: "钉钉",
  },
  {
    active: false,
    id: 7,
    name: "飞书",
  },
];
export const eventTypeList = [
  {
    active: false,
    id: 1,
    name: "流程发起",
  },
  {
    active: false,
    id: 2,
    name: "作废操作",
  },
  {
    active: false,
    id: 3,
    name: "流程流转至当前节点",
  },
  {
    active: false,
    id: 4,
    name: "同意操作",
  },
  {
    active: false,
    id: 5,
    name: "不同意操作",
  },
  {
    active: false,
    id: 6,
    name: "加批操作",
  },
  {
    active: false,
    id: 7,
    name: "退回修改操作",
  },
  {
    active: false,
    id: 8,
    name: "转发操作",
  },
  {
    active: false,
    id: 9,
    name: "流程结束",
  },
];
