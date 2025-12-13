/*
 * @Date:  2024-05-25 14:06:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2024-06-29 15:52:57
 * @FilePath: src/api/workflow/mock.js
 * 静态数据模拟接口
 */
import http from "@/utils/axios";
let baseUrl = import.meta.env.BASE_URL;
let baseApiUrl = import.meta.env.VITE_APP_BASE_API;
let workflowUrl = import.meta.env.VITE_APP_BASE_API;

/**
 * 用户登录
 */
export function login(username, password, code, uuid) {
  return http.get(`${baseUrl}mock/login.json`);
}

// 退出方法
export function logout() {
  return http.get(`${baseUrl}mock/logout.json`);
}

/**
 * 获取登录用户信息
 */
export function getInfo() {
  return http.get(`${baseUrl}mock/userinfo.json`);
}

/**
 * 获取登录用户信息
 */
export function getUserProfile() {
  return http.get(`${baseUrl}mock/profile.json`);
}
/**
 * 获取菜单
 */
export function getRouters() {
  return http.get(`${baseUrl}mock/menu.json`);
}

/**
 * 查询岗位列表
 */
export function listPost(query) {
  return http.get(`${baseUrl}mock/post.json`);
}

/**
 * 获取角色
 * @param {*} data
 * @returns
 */
export function getRoles(data) {
  return http.get(`${baseUrl}mock/roles.json`, { params: data });
}

/**
 * 获取部门
 * @param {*} data
 * @returns
 */
export function getDepartments(data) {
  return http.get(`${baseUrl}mock/departments.json`, { params: data });
}

/**
 * 获取职员
 * @param {*} data
 * @returns
 */
export function getEmployees(data) {
  return http.get(`${baseUrl}mock/employees.json`, { params: data });
}

/**
 * 获取条件字段
 * @param {*} data
 * @returns
 */
export function getConditions(data) {
  return http.get(`${baseUrl}mock/conditions.json`, { params: data });
}

/**
 * 获取审批数据
 * @param {*} data
 * @returns
 */
export function getWorkFlowData(data) {
  return http.get(`${baseUrl}mock/data.json`, { params: data });
}
/**
 * 获取电子签名
 * @returns
 */
export function getSignatureData() {
  return http.get(`${baseUrl}mock/signature.json`);
}

/**
 * 获取全部用户信息
 * @param {*} data
 * @returns
 */
export function getUsers(data) {
  let headers = {
    Userid: "1",
    Username: "%E5%BC%A0%E4%B8%89",
  };
  return http.get(`${workflowUrl}/user/getUser`, { headers });
}
/**
 * 获取用户分页信息
 * @param {*} data
 * @returns
 */
export function getUserPageList(pageDto, qVO) {
  let headers = {
    Userid: "1",
    Username: "%E5%BC%A0%E4%B8%89",
  };
  let data = {
    pageDto: pageDto,
    taskMgmtVO: qVO,
  };
  return http.post(`${workflowUrl}/user/getUserPageList`, data, { headers });
}
/**
 * 三方接入 获取数据根据url
 * @returns
 */
export function getDynamicsList(url) {
  let headers = {
    Userid: "1",
    Username: "%E5%BC%A0%E4%B8%89",
  };
  return http.get(url, { headers });
}

/**
 * 获取全部角色信息
 * @returns
 */
export function getRoleList() {
  let headers = {
    Userid: "1",
    Username: "%E5%BC%A0%E4%B8%89",
  };
  return http.get(`${workflowUrl}/user/getRoleInfo`, { headers });
}
