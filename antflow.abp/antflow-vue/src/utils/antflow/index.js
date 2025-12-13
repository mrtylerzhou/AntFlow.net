import { parseTime } from "@/utils/ruoyi";
import { isEmpty, isEmptyArray } from "@/utils/antflow/ObjectUtils";
function All() {}
All.prototype = {
  arrToStr(arr) {
    if (arr) {
      return arr
        .map((item) => {
          return item.name;
        })
        .toString();
    }
  },
  toggleClass(arr, elem, key = "id") {
    //判断数组中是否包含某个元素
    if (isEmptyArray(arr)) return false;
    if (arr && arr.length > 0) {
      return arr.some((item) => {
        return !isEmpty(item) && item[key] == elem[key];
      });
    }
  },
  toChecked(arr, elem, key = "id") {
    var isIncludes = this.toggleClass(arr, elem, key);
    !isIncludes ? arr.push(elem) : this.removeEle(arr, elem, key);
  },
  removeEle(arr, elem, key = "id") {
    var includesIndex;
    arr.map((item, index) => {
      if (item[key] == elem[key]) {
        includesIndex = index;
      }
    });
    arr.splice(includesIndex, 1);
  },
  setApproverStr(nodeConfig) {
    if (!nodeConfig) return;
    if (nodeConfig.setType == 5) {
      if (nodeConfig.nodeApproveList.length == 1) {
        return nodeConfig.nodeApproveList[0].name;
      } else if (nodeConfig.nodeApproveList.length > 1) {
        if (nodeConfig.signType == 2) {
          return this.arrToStr(nodeConfig.nodeApproveList);
        } else if (nodeConfig.signType == 1) {
          return (
            nodeConfig.nodeApproveList.length +
            "人(" +
            this.arrToStr(nodeConfig.nodeApproveList) +
            ")会签"
          );
        } else if (nodeConfig.signType == 3) {
          return (
            nodeConfig.nodeApproveList.length +
            "人(" +
            this.arrToStr(nodeConfig.nodeApproveList) +
            ")顺序会签"
          );
        }
      }
    } else if (nodeConfig.setType == 3) {
      const levelMap = {
        1: "直接主管",
        2: "第二主管",
        3: "第三主管",
      };
      let level =
        levelMap[nodeConfig.directorLevel] ||
        `第${nodeConfig.directorLevel}级主管`;
      if (nodeConfig.signType == 2) {
        return level + "会签";
      } else {
        return level;
      }
    } else if (nodeConfig.setType == 4) {
      if (nodeConfig.nodeApproveList.length > 0) {
        return "指定 (" + this.arrToStr(nodeConfig.nodeApproveList) + ") 角色";
      }
      return "";
    } else if (nodeConfig.setType == 6) {
      if (nodeConfig.nodeApproveList.length > 0) {
        return "指定 (" + this.arrToStr(nodeConfig.nodeApproveList) + ")";
      }
      return "";
    } else if (nodeConfig.setType == 14) {
      return "指定部门";
    } else if (nodeConfig.setType == 12) {
      return "发起人自己";
    } else if (nodeConfig.setType == 13) {
      return "直属领导";
    } else if (nodeConfig.setType == 7) {
      return "由发起人自选审批人";
    } else {
      return "";
    }
  },
  getCheckboxStr(str, obj) {
    if (!obj) return;
    let arr = [];
    let list = str.split(",");
    for (var elem in obj) {
      list.map((item) => {
        if (item == obj[elem].key) {
          arr.push(obj[elem].value);
        }
      });
    }
    return arr.join("或");
  },
  // select 为单选
  getSelectStr(index, obj) {
    if (!obj) return;
    let ret = obj.filter((c) => c.key == index).map((x) => x.value);
    if (ret) {
      return ret;
    }
    return "";
  },
  // select 为多选
  getMultipleSelectStr(keys, obj) {
    if (!Array.isArray(keys) && keys.includes("[")) {
      keys = JSON.parse(keys);
    }
    if (!obj || isEmptyArray(keys)) return;
    let ret = obj.filter((c) => keys.includes(c.key)).map((x) => x.value);
    return ret;
  },

  conditionStr(nodeConfig, index) {
    var { conditionList, nodeApproveList, groupRelation } =
      nodeConfig.conditionNodes[index];

    const flatArray = conditionList
      .reduce((acc, val) => acc.concat(val), [])
      .filter((item) => item.columnId && item.columnId !== 0);
    if (flatArray.length == 0) {
      return index == nodeConfig.conditionNodes.length - 1 &&
        nodeConfig.conditionNodes[index].conditionList
          .reduce((acc, val) => acc.concat(val), [])
          .filter((item) => item.columnId && item.columnId !== 0).length == 0
        ? "其他条件进入此流程"
        : "请设置条件";
    }
    let str = "";
    for (let i = 0; i < conditionList.length; i++) {
      str = str + "条件组" + (i + 1) + "：【";
      str = str + this.getConditionStr(conditionList[i]);
      str = str + "】  ";
      if (i + 1 != conditionList.length) {
        str = str + (groupRelation == false ? " 且 " : " 或 ");
      }
    }
    return str;
  },

  getConditionStr(conditionArray) {
    let str = "";
    for (let condition of conditionArray) {
      var {
        columnId,
        showName,
        optType,
        zdy1,
        opt1,
        zdy2,
        opt2,
        fieldTypeName,
        fixedDownBoxValue,
        condRelation, //条件关系
      } = condition;
      const relationTip = condRelation == false ? " 且 " : " 或 ";
      if (fieldTypeName == "input") {
        if (zdy1) {
          str += showName + "：" + zdy1 + relationTip;
        }
      } else if (fieldTypeName == "switch") {
        str += showName + "：" + zdy1 + relationTip;
      } else if (fieldTypeName == "radio") {
        // if (zdy1) {
        //     str += showName + '：' + zdy1 + " 并且 "
        // }
      } else if (fieldTypeName == "checkbox") {
        if (!fixedDownBoxValue) {
          str += nodeConfig.conditionNodes[index].nodeDisplayName + "     ";
        } else {
          if (zdy1) {
            str +=
              showName +
              "属于：" +
              this.getCheckboxStr(zdy1, JSON.parse(fixedDownBoxValue)) +
              relationTip;
          }
        }
      } else if (fieldTypeName == "select") {
        if (!fixedDownBoxValue) {
          str += nodeConfig.conditionNodes[index].nodeDisplayName + "     ";
        } else {
          if (zdy1) {
            if (!isNaN(Number(zdy1))) {
              str +=
                showName +
                "：" +
                this.getSelectStr(zdy1, JSON.parse(fixedDownBoxValue)) +
                relationTip;
            } else {
              str +=
                showName +
                "：" +
                this.getMultipleSelectStr(zdy1, JSON.parse(fixedDownBoxValue)) +
                relationTip;
            }
          }
        }
      } else if (fieldTypeName == "date") {
        if (zdy1) {
          var optTypeStr = ["", "≥", ">", "≤", "<", "="][optType];
          str += `${showName} ${optTypeStr} ${parseTime(
            zdy1,
            "{y}-{m}-{d}"
          )} ${relationTip} `;
        }
      } else if (fieldTypeName == "time") {
        if (zdy1) {
          var optTypeStr = ["", "≥", ">", "≤", "<", "="][optType];
          str += `${showName} ${optTypeStr} ${parseTime(
            zdy1,
            "{h}:{i}:{s}"
          )} ${relationTip} `;
        }
      } else if (fieldTypeName == "input-number") {
        if (optType < 6 && zdy1) {
          var optTypeStr = ["", "≥", ">", "≤", "<", "="][optType];
          str += `${showName} ${optTypeStr} ${zdy1} ${relationTip} `;
        } else if (optType >= 6 && zdy1 && zdy2) {
          str += `${zdy1} ${opt1} ${showName} ${opt2} ${zdy2} ${relationTip} `;
        }
      } else {
        str += null;
      }
    }
    str = this.removeLastIndexOfAnd(str);
    str = this.removeLastIndexOfOR(str);
    return str && str.length > 0 ? str : "请设置条件";
  },
  copyerStr(nodeConfig) {
    if (nodeConfig.nodeApproveList.length != 0) {
      return this.arrToStr(nodeConfig.nodeApproveList);
    } else {
      // if (nodeConfig.ccSelfSelectFlag == 1) {
      //   return "发起人自选";
      // }
      return "";
    }
  },
  toggleStrClass(item, key) {
    let a = item.zdy1 ? item.zdy1.split(",") : [];
    return a.some((item) => {
      return item == key;
    });
  },

  removeLastIndexOfAnd(str) {
    let lastIndexOfAnd = str.lastIndexOf("且");
    if (lastIndexOfAnd !== -1) {
      str = str.slice(0, lastIndexOfAnd) + str.slice(lastIndexOfAnd + 1);
    }
    return str;
  },

  removeLastIndexOfOR(str) {
    let lastIndexOfOR = str.lastIndexOf("或");
    if (lastIndexOfOR !== -1) {
      str = str.slice(0, lastIndexOfOR) + str.slice(lastIndexOfOR + 1);
    }
    return str;
  },
};

export default new All();
