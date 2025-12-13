// ObjectUtils.isObjectChanged(tar1, tar2)
// ObjectUtils.isObjectChangedSimple(tar1, tar2)
//import {  ObjectUtils } from '@/utils/antflow/ObjectUtils'
export const isEmpty = (data) =>
  data === null ||
  data === undefined ||
  data == "" ||
  data == {} ||
  data == "{}" ||
  data == "[]" ||
  data == "null";
export const isEmptyArray = (data) =>
  Array.isArray(data)
    ? data.length === 0
    : data === null ||
      data === undefined ||
      data == "" ||
      data == [] ||
      data == "{}" ||
      data == "null"
    ? true
    : false;

// 检查对象中指定属性是否有空值
export const hasEmptyValue = (obj, props) => {
  if (isEmpty(obj)) {
    return Object.values(obj).some((value) => {
      return isEmpty(value);
    });
  } else {
    return props.some((prop) => {
      const value = obj[prop];
      return isEmpty(value);
    });
  }
};
// 判断字段是否为“真”
export const isTrue = (val) => {
  return (
    val === true ||
    val === "true" ||
    val === "True" ||
    val === "TRUE" ||
    val === 1 ||
    val === "1"
  );
};
/**
 * 字符串中间部分隐藏
 * @param {*} str
 * @returns
 */
export function substringHidden(str) {
  let frontLen = 6;
  let endLen = 6;
  if (str == null || str == undefined || str == "") {
    return str;
  }
  if (str.length <= 18) {
    return str;
  }
  var xing = "******";
  return str.substring(0, frontLen) + xing + str.substring(str.length - endLen);
}
export function boolToString(obj) {
  if (Array.isArray(obj)) {
    return obj.map(boolToString);
  } else if (obj && typeof obj === "object") {
    const newObj = {};
    for (const key in obj) {
      if (typeof obj[key] === "boolean") {
        newObj[key] = obj[key] ? "true" : "false";
      } else if (typeof obj[key] === "object") {
        newObj[key] = boolToString(obj[key]);
      } else {
        newObj[key] = obj[key];
      }
    }
    return newObj;
  }
  return obj;
}
/**
 * 比较两个对象是否相等
 * @param {*} source
 * @param {*} comparison
 * @returns true 不相等 false 相等
 */
export const isObjectChanged = (source, comparison) => {
  if (!iterable(source)) {
    throw new Error(
      `source should be a Object or Array , but got ${getDataType(source)}`
    );
  }
  if (getDataType(source) !== getDataType(comparison)) {
    return true;
  }
  const sourceKeys = Object.keys(source);
  const comparisonKeys = Object.keys({ ...source, ...comparison });
  if (sourceKeys.length !== comparisonKeys.length) {
    return true;
  }
  return comparisonKeys.some((key) => {
    if (iterable(source[key])) {
      return isObjectChanged(source[key], comparison[key]);
    } else {
      return source[key] !== comparison[key];
    }
  });
};
/**
 * 简单比较两个对象是否相等
 * @param {*} source
 * @param {*} comparison
 * @returns true 不相等 false 相等
 */
export const isObjectChangedSimple = (source, comparison) => {
  const _source = JSON.stringify(source);
  const _comparison = JSON.stringify({ ...source, ...comparison });
  return _source !== _comparison;
};
/*以下是私有方法 */
/**
 * 定义一个函数，用于获取数据的类型
 * @param {*} data
 * @returns
 */
const getDataType = (data) => {
  const temp = Object.prototype.toString.call(data);
  const type = temp.match(/\b\w+\b/g);
  return type.length < 2 ? "Undefined" : type[1];
};
/**
 * 判断数据是否为可迭代对象
 * @param {*} data
 * @returns true 可迭代 false 不可迭代
 */
const iterable = (data) => {
  return ["Object", "Array"].includes(getDataType(data));
};
