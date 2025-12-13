/*
 * @Date:  2024-10-10 20:00:00
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2024-10-10 20:00:00
 * @FilePath: src\api\outsideApi.js
 * 三方接入模块 相关接口
 */
import http from "@/utils/axios";
import Cookies from "js-cookie";
let baseUrl = import.meta.env.VITE_APP_BASE_API;
const headers = {
  Userid: Cookies.get("userId"),
  Username: Cookies.get("userName"),
};

/** 三方接入模块 流程设计 * / 
 * 
 /**
 * 获取OutSide FormCode Page List 模板列表使用
 * @param { page } 分页
 * @param { vo } 条件
 * @returns
 */
export function getOutSideFormCodePageList(page, vo) {
  let data = {
    pageDto: page,
    entity: vo,
  };
  return http.post(`${baseUrl}/outSide/getOutSideFormCodePageList`, data, {
    headers,
  });
}

/**
 * 获取审批数据
 * @param {*} data
 * @returns
 */
export function getApiWorkFlowData(data) {
  return http.get(`${baseUrl}/bpmnConf/detail/${data.id}`, { headers });
}

/**
 * 设置审批数据
 * @param {*} data
 * @returns
 */
export function setApiWorkFlowData(data) {
  return http.post(`${baseUrl}/bpmnConf/edit`, data, { headers });
}

/**
 * 获取Sass租户列表
 * @param { page } 分页
 * @param { vo } 条件
 * @returns
 */
export function getBusinessPartyList(page, vo) {
  let data = {
    pageDto: page,
    entity: vo,
  };
  return http.post(`${baseUrl}/outSideBpm/businessParty/listPage`, data, {
    headers,
  });
}
/**
 * 获取租户详情
 * @param { Number } id
 * @returns
 */
export function getBusinessPartyDetail(id) {
  return http.get(`${baseUrl}/outSideBpm/businessParty/detail/${id}`, {
    headers,
  });
}

/**
 * 编辑/新增租户
 * @param {*} param
 * @returns
 */
export function setBusinessParty(data) {
  return http.post(`${baseUrl}/outSideBpm/businessParty/edit`, data, {
    headers,
  });
}
/**
 * 获取应用列表
 * @param {*} page
 * @param {*} vo
 * @returns
 */
export function getApplicationsPageList(page, vo) {
  let data = {
    pageDto: page,
    entity: vo,
  };
  return http.post(
    `${baseUrl}/outSideBpm/businessParty/applicationsPageList`,
    data,
    { headers }
  );
}

/**
 * 添加业应用
 * @param {*} data
 * @returns
 */
export function addApplication(data) {
  return http.post(
    `${baseUrl}/outSideBpm/businessParty/addBpmProcessAppApplication`,
    data,
    { headers }
  );
}
/**
 * 获取注册应用详情
 * @param {*} data
 * @returns
 */
export function getApplicationDetail(id) {
  return http.get(
    `${baseUrl}/outSideBpm/businessParty/applicationDetail/${id}`,
    { headers }
  );
}

/** 应用关联条件模板 * /

/**
 * Obsoleted
 * 获取条件模板列表
 * @param {*} param 
 * @returns 
 */
export function getConditionTemplatelist(applicationId) {
  return http.get(
    `${baseUrl}/outSideBpm/conditionTemplate/selectListByTemp/${applicationId}`,
    { headers }
  );
}

/**
 * Obsoleted
 * 添加条件模板
 * @param {*} param
 * @returns
 */
export function setConditionTemplate(data) {
  return http.post(`${baseUrl}/outSideBpm/conditionTemplate/edit`, data, {
    headers,
  });
}
/**
 * Obsoleted
 * 审批人模板列表
 * @param {*} param
 * @returns
 */
export function getApproveTemplatePageList(page, vo) {
  let data = {
    page: page,
    vo: vo,
  };
  return http.post(`${baseUrl}/outSideBpm/approveTemplate/listPage`, data, {
    headers,
  });
}
/**
 * 获取审批人列表
 * @param {*} param
 * @returns
 */
export function getApproveTemplatelist(applicationId) {
  return http.get(
    `${baseUrl}/outSideBpm/approveTemplate/selectListByTemp/${applicationId}`,
    { headers }
  );
}
/**
 * 审批人模板详情
 * @param {*} param
 * @returns
 */
export function getApproveTemplateDetail(id) {
  return http.post(`${baseUrl}/outSideBpm/approveTemplate/detail/${id}`, {
    headers,
  });
}
/**
 * 添加审批人模板
 * @param {*} param
 * @returns
 */
export function setApproveTemplate(data) {
  return http.post(`${baseUrl}/outSideBpm/approveTemplate/edit`, data, {
    headers,
  });
}

/** 三方业务发起 * /

 /**
 * 三方业务发起
 * @param {*} data 
 * @returns 
 */
export function processSubmit(data) {
  return http.post(`${baseUrl}/outSide/processSubmit`, data, { headers });
}
/**
 * 回调地址配置
 * @param {*} data
 * @returns
 */
export function callbackUrlConf(data) {
  return http.post(`${baseUrl}/outSideBpm/callbackUrlConf/edit`, data, {
    headers,
  });
}
/**
 * 回调地址列表
 * @param {*} applicationId
 * @returns
 */
export function getCallbackUrlConfList(formCode) {
  return http.get(`${baseUrl}/outSideBpm/callbackUrlConf/list/${formCode}`, {
    headers,
  });
}
