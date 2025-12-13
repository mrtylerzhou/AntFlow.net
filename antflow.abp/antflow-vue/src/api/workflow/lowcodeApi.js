/*
 * @Date:  2024-05-25 14:06:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-03-29 15:52:57
 * @FilePath: src\api\lowcodeApi.js
 * 低代码表单模块 相关接口
 */
import http from "@/utils/axios";
import Cookies from "js-cookie";
let baseUrl = import.meta.env.VITE_APP_BASE_API;
const headers = {
  Userid: Cookies.get("userId"),
  Username: Cookies.get("userName"),
};
import request from "@/utils/request";
/**
 * Obsoleted
 * 获取全部 LF FormCodes 在流程设计时选择使用
 * @param { String } formCode
 * @returns
 */
export function getLowCodeFlowFormCodes() {
  return request({
    url: `/lowcode/getLowCodeFlowFormCodes`,
    method: "get",
  });
}
/**
 * 获取LF FormCode Page List 模板列表使用
 * @returns
 */
export function getLFFormCodePageList(pageDto, taskMgmtVO) {
  let paramDto = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  // return http.post(`${baseUrl}/lowcode/getLFFormCodePageList`, paramDto, {
  //   headers,
  // });
  return request({
    url: `/lowcode/getLFFormCodePageList`,
    method: "post",
    data: paramDto,
  });
}
/**
 * 获取 已设计流程并且启用的 LF FormCode Page List 发起页面使用
 * @returns
 */
export function getLFActiveFormCodePageList(pageDto, taskMgmtVO) {
  let paramDto = {
    pageDto: pageDto,
    taskMgmtVO: taskMgmtVO,
  };
  // return http.post(`${baseUrl}/lowcode/getLFActiveFormCodePageList`, paramDto, {
  //   headers,
  // });
  return request({
    url: `/lowcode/getLFActiveFormCodePageList`,
    method: "post",
    data: paramDto,
  });
}

/**
 * 获取低代码表单根据FromCode
 * @param { String } formCode
 * @returns
 */
export function getLowCodeFromCodeData(formCode) {
  // return http.get(
  //   `${baseUrl}/lowcode/getformDataByFormCode?formCode=${formCode}`,
  //   { headers }
  // );
  return request({
    url: `/lowcode/getformDataByFormCode?formCode=${formCode}`,
     method: "get",
  });
}

/**
 * 新增低代码表单FormCode
 * @param {*} data
 * @returns
 */
export function createLFFormCode(data) {
  // return http.post(`${baseUrl}/lowcode/createLowCodeFormCode`, data, {
  //   headers,
  // });
  return request({
    url: `/lowcode/createLowCodeFormCode`,
    method: "post",
    data: data,
  });
}
