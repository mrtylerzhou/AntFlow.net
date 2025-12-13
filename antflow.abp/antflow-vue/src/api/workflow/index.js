/*
 * @Date:  2024-05-25 14:06:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-03-29 15:52:57
 * @FilePath: src\api\workflow.js
 * 工作流 相关接口
 */
//import http from "@/utils/axios";
import Cookies from "js-cookie";
let baseUrl = import.meta.env.VITE_APP_BASE_API;

import request from "@/utils/request";
const headers = {
  // Userid: Cookies.get("userId"),
  // Username: Cookies.get("userName"),
};
/**
 * 获取流程配置详情
 * @param { * } data
 * @returns
 */
export function getApiWorkFlowData(data) {
  
  //return http.get(`${baseUrl}/bpmnConf/detail/${data.id}`, { headers });
  return request({
    url: `/bpmnConf/detail/${data.id}`,
    method: "get",
  });
}

/**
 * 获取DIY FromCode
 * @returns
 */
export function getDIYFromCodeData() {
  //return http.get(`${baseUrl}/bpmnBusiness/getDIYFormCodeList`, { headers });
  return request({
    url: `/bpmnBusiness/getDIYFormCodeList`,
    method: "get",
  });
}

/**
 * 设置/添加审批流程配置详情
 * @param {*} data
 * @returns
 */
export function setApiWorkFlowData(data) {
  // return http.post(`${baseUrl}/bpmnConf/edit`, data, { headers });
  return request({
    url: `/bpmnConf/edit`,
    method: "post",
    data: data,
  });
}
/**
 * 获取代办事项
 * @returns
 */
export function getTodoList() {
  // return http.get(`${baseUrl}/bpmnConf/todoList`, { headers });
  return request({
    url: `/bpmnConf/todoList`,
    method: "get",
  });
}

/**
 * 获取抄送给我流程列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getCopyToMelistPage(pageDto, taskMgmtVO) {
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  return request({
    url: `/bpmnConf/process/listPage/9`,
    method: "post",
    data: data,
  });
}

/**
 * 获取所有实例列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getAllProcesslistPage(pageDto, taskMgmtVO) {
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  //return http.post(`${baseUrl}/bpmnConf/process/listPage/6`, data, { headers });
  return request({
    url: `/bpmnConf/process/listPage/6`,
    method: "post",
    data: data,
  });
}

/**
 * 获取用户代办数据列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getPenddinglistPage(pageDto, taskMgmtVO) {
  // let data = {
  //   pageDto: pageDto,
  //   taskMgmtVO: taskMgmtVO,
  // };
  // return http.post(`${baseUrl}/bpmnConf/process/listPage/5`, data, { headers });
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  return request({
    url: `/bpmnConf/process/listPage/5`,
    method: "post",
    data: data,
  });
}

/**
 * 获取用户已审批数据列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getApprovedlistPage(pageDto, taskMgmtVO) {
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  // return http.post(`${baseUrl}/bpmnConf/process/listPage/4`, data, { headers });
  return request({
    url: `/bpmnConf/process/listPage/4`,
    method: "post",
    data: data,
  });
}

/**
 * 获取我发起的流程列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getMyRequestlistPage(pageDto, taskMgmtVO) {
  // let data = {
  //   pageDto: pageDto,
  //   taskMgmtVO: taskMgmtVO,
  // };
  // return http.post(`${baseUrl}/bpmnConf/process/listPage/3`, data, { headers });
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO, 
  };
  return request({
    url: `/bpmnConf/process/listPage/3`,
    method: "post",
    data: data,
  });
}

/**
 * 获取流程配置数据列表
 * @param {*} data
 * @returns
 */
export function getBpmnConflistPage(pageDto, taskMgmtVO) {
  // let data = {
  //   pageDto: pageDto,
  //   entity: taskMgmtVO,
  // };
  // return http.post(`${baseUrl}/bpmnConf/listPage`, data, { headers });
  let data = {
    pageDto: pageDto,
    entity: taskMgmtVO,
  };
  return request({
    url: `/bpmnConf/listPage`,
    method: "post",
    data: data,
  });
}

/**
 * 审批,发起审批
 * @param {object} data
 * @param operationType 1 发起 2 重新提交 3 审批, 7 作废, 11 当前节点变更处理人,15 抄送, 24 当前节点减签,25 当前节点加签
 *  26 未来节点变更处理人, 27 未来节点减签,28 未来节点加签 29 撤回
 * @returns
 */
export function processOperation(data) {
  // return http.post(
  //   `${baseUrl}/bpmnConf/process/buttonsOperation?formCode=${data.formCode}`,
  //   data,
  //   { headers }
  // );
  return request({
    url: `/bpmnConf/process/buttonsOperation?formCode=${data.formCode}`,
    method: "post",
    data: data,
  });
}

/**
 * 获取审批进度数据
 * @param { object } param
 * @returns
 */
export function getBpmVerifyInfoVos(param) {
  // return http.get(
  //   `${baseUrl}/bpmnConf/getBpmVerifyInfoVos?processNumber=${param.processNumber}`,
  //   { headers }
  // );
  return request({
    url: `/bpmnConf/getBpmVerifyInfoVos?processNumber=${param.processNumber}`,
    method: "get",
  });
}

/**
 * 流程预览
 * @param {*} data
 * @returns
 */
export function getFlowPreview(data) {
  // let paramA = {
  //   "formCode": "DSFZH_WMA",
  //    "accountType":1
  // }
  // return http.post(`${baseUrl}/bpmnConf/startPagePreviewNode`, data, {
  //   headers,
  // });
  return request({
    url: `/bpmnConf/startPagePreviewNode`,
    method: "post",
    data: data,
  });
}

/**
 * 流程节点当前操作人
 * @param {*} data
 * @returns
 */
export function loadNodeOperationUser(data) {
  // let paramA = {
  //   "formCode": "DSFZH_WMA",
  //    "accountType":1
  // }
  // return http.post(`${baseUrl}/bpmnConf/loadNodeOperationUser`, data, {
  //   headers,
  // });
  return request({
    url: `/bpmnConf/loadNodeOperationUser`,
    method: "post",
    data: data,
  });
}

/**
 * 流程生效
 * @param {*} data
 * @returns
 */
export function getEffectiveBpmn(data) {
  return request({
    url: `/bpmnConf/effectiveBpmn/${data.id}`,
    method: "get",
  });
}

/**
 * 获取审批页面按钮权限
 * @param {*} data
 * @returns
 */
export function getViewBusinessProcess(data) {
  // return http.post(
  //   `${baseUrl}/bpmnConf/process/viewBusinessProcess?formCode=${data.formCode}`,
  //   data,
  //   { headers }
  // );
  return request({
    url: `/bpmnConf/process/viewBusinessProcess?formCode=${data.formCode}`,
    method: "post",
    data: data,
  });
}

/**
 * 获取审批页面 审批人配置类型
 * @returns
 */
export function getApproveNodeProperties() {
  return request({
    url: `/bpmnBusiness/listNodeProperties`,
    method: "get",
  });
}

/**
 * 获取委托列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getUserEntrustListPage(pageDto, taskMgmtVO) {
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  return request({
    url: `/bpmnBusiness/entrustlist/1`,
    method: "post",
    data: data,
  });
}
/**
 * 获取委托列表
 * @param {*} pageDto
 * @param {*} taskMgmtVO
 * @returns
 */
export function getEntrustListPage(pageDto, taskMgmtVO) {
  let data = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  return request({
    url: `/bpmnBusiness/entrustlist/2`,
    method: "post",
    data: data,
  });
}
/**
 *  委托详情
 * @param {Number} id
 * @returns
 */
export function entrustDetail(id) {
  // return http.get(`${baseUrl}/bpmnBusiness/entrustDetail/${id}`, { headers });
  return request({
    url: `/bpmnBusiness/entrustDetail/${id}`,
    method: "get",
  });
}
/**
 * 设置委托
 * @param {*} data
 * @returns
 */
export function setEntrust(data) {
  return request({
    url: `/bpmnBusiness/editEntrust`,
    method: "post",
    data: data,
  });
}
/**
 * 获取流程自选审批人节点
 * @param {*} formCode
 * @returns
 */
export function getStartUserChooseModules(formCode) {
  return request({
    url: `/bpmnBusiness/getStartUserChooseModules?formCode=${formCode}`,
    method: "get",
  });
}
