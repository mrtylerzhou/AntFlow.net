// import { FormatUtils } from '@/utils/antflowformatcommit_data'
import { isEmpty, isEmptyArray } from "@/utils/antflow/ObjectUtils";
export class FormatCommitUtils {
  /**
   * 对基础设置,高级设置等设置页内容进行格式化
   * @param params
   */
  static formatSettings = (param) => {
    let treeList = this.flattenMapTreeToList(param);
    let combinationList = this.getEndpointNodeId(treeList);
    let finalList = this.cleanNodeList(combinationList);
    let fomatList = this.adapterActivitiNodeList(finalList);
    return fomatList;
  };
  /**
   * 展平树结构
   * @param {Object} treeData  - 节点数据
   * @returns Array - 节点数组
   */
  static flattenMapTreeToList = (treeData) => {
    let nodeData = [];
    function traverse(node) {
      if (!node && !node.hasOwnProperty("nodeType")) {
        return nodeData;
      }
      if (node.nodeType == 2) {
        if (node.childNode) {
          node.childNode.nodeFrom = node.nodeId;
          traverse(node.childNode);
        }
        if (!isEmptyArray(node.conditionNodes)) {
          for (let child of node.conditionNodes) {
            child.nodeFrom = node.nodeId;
            traverse(child);
          }
          node.nodeTo = node.conditionNodes.map((item) => item.nodeId);
          delete node.conditionNodes;
        }
      } else if (node.nodeType == 7) {
        if (node.childNode) {
          node.childNode.nodeFrom = node.nodeId;
          traverse(node.childNode);
        }
        if (!isEmptyArray(node.parallelNodes)) {
          for (let child of node.parallelNodes) {
            child.nodeFrom = node.nodeId;
            traverse(child);
          }
          node.nodeTo = node.parallelNodes.map((item) => item.nodeId);
          delete node.parallelNodes;
        }
      } else if (node.childNode) {
        node.nodeTo = [node.childNode.nodeId];
        node.childNode.nodeFrom = node.nodeId;
        traverse(node.childNode);
      }
      delete node.childNode;
      nodeData.push(node);
    }
    traverse(treeData);
    return nodeData;
  };
  /**
   * 递归处理网关节点下属子节点的nodeTo数据
   * @param { Array } parmData -节点关系数组
   * @returns
   */
  static getEndpointNodeId = (parmData) => {
    if (isEmptyArray(parmData)) return parmData;
    let nodesGroup = {};
    for (let t of parmData) {
      if (isEmpty(t.nodeFrom)) continue;
      if (nodesGroup.hasOwnProperty(t.nodeFrom) && !isEmpty(t.nodeFrom)) {
        nodesGroup[t.nodeFrom].push(t);
      } else {
        nodesGroup[t.nodeFrom] = [t];
      }
    }
    //console.log("nodesGroup===========",JSON.stringify(nodesGroup));
    //处理审批人并行网关
    handleApproverParallelGetway(nodesGroup, parmData);
    //处理条件网关
    handleConditionGetway(nodesGroup, parmData);
    return parmData;
  };

  /**
   * 清理节点数据
   * @param { Array } arr -节点数组
   * @returns
   */
  static cleanNodeList = (arr) => {
    let nodeIds = arr.map((c) => {
      return c.nodeId;
    });
    for (const node of arr) {
      node.nodeTo = Array.from(new Set(node.nodeTo));
      if (!isEmptyArray(node.nodeTo)) {
        node.nodeTo = node.nodeTo.filter((key) => {
          return nodeIds.indexOf(key) > -1;
        });
      }
    }
    return arr;
  };

  /**
   * 格式化node数据，对接api接口
   * @param {Array} nodeList
   * @returns
   */
  static adapterActivitiNodeList = (nodeList) => {
    for (let node of nodeList) {
      if (node.hasOwnProperty("id")) {
        delete node.id;
      }
      if (node.nodeType == 3) {
        let conditionObj = {
          conditionList: node.conditionList,
          sort: node.priorityLevel,
          isDefault: node.isDefault,
          groupRelation: node.groupRelation,
        };
        Object.assign(node, {
          property: {},
        });
        node.property = conditionObj;
        delete node.conditionList;
        delete node.isDefault;
        delete node.groupRelation;
      }

      if (node.nodeType == 4 || node.nodeType == 6) {
        let approveObj = {
          emplIds: [],
          emplList: [],
          roleIds: [],
          roleList: [],
          hrbpConfType: 0,
          assignLevelGrade: 0,
          signType: node.signType,
          signUpType: 1,
          afterSignUpWay: 2,
        };

        if (node.nodeApproveList && !isEmptyArray(node.nodeApproveList)) {
          if (node.setType == 4) {
            for (let approve of node.nodeApproveList) {
              let role = {};
              role.id = approve.targetId;
              role.name = approve.name;
              approveObj.roleIds.push(approve.targetId);
              approveObj.roleList.push(role);
            }
          } else if (node.setType == 5) {
            for (let approve of node.nodeApproveList) {
              let emp = {};
              emp.id = approve.targetId;
              emp.name = approve.name;
              approveObj.emplIds.push(approve.targetId);
              approveObj.emplList.push(emp);
            }
          } else if (node.setType == 6) {
            for (let approve of node.nodeApproveList) {
              approveObj.hrbpConfType = approve.targetId;
            }
          }
        } else if (node.setType == 3) {
          approveObj.assignLevelGrade = node.directorLevel;
        }
        approveObj.afterSignUpWay = node.property?.afterSignUpWay ?? 2;
        approveObj.signUpType = node.property?.signUpType ?? 1;
        node.nodeProperty = node.setType;
        node.property = approveObj;
        delete node.nodeApproveList;
      }
    }
    return nodeList;
  };
}

/** 处理审批人并行网关
 * @param {Object} nodesGroup - 节点关系对象
 * @param {Object} parmData - 节点关系数组
 * @returns
 * */
const handleApproverParallelGetway = (nodesGroup, parmData) => {
  let parallelgetwayList = parmData.filter((c) => {
    return c.nodeType == 7;
  });
  if (!isEmptyArray(parallelgetwayList)) {
    //处理并行审批网关
    for (let parallel of parallelgetwayList) {
      if (nodesGroup.hasOwnProperty(parallel.nodeId)) {
        let itemNodes = nodesGroup[parallel.nodeId];
        if (isEmptyArray(itemNodes)) continue;
        let childParallelList = itemNodes.filter((c) => {
          //并行子分支
          return parallel.nodeTo.includes(c.nodeId);
        });
        if (isEmptyArray(childParallelList)) continue;
        let parallelWayChild = itemNodes.find((c) => {
          //并行聚合节点
          return !parallel.nodeTo.includes(c.nodeId);
        });
        for (let itemNode of childParallelList) {
          function internalTraverse(info) {
            if (!info) return;
            if (info.nodeType == 7) {
              //并行审批嵌套
              let parallelCilds = nodesGroup[info.nodeId];
              let parallelComboNode = parallelCilds.find((c) => {
                //并行聚合节点递归
                return !info.nodeTo.includes(c.nodeId);
              });
              internalTraverse(parallelComboNode);
            } else {
              if (
                !nodesGroup[info.nodeId] &&
                !isEmpty(parallelWayChild) &&
                info.nodeId != parallelWayChild.nodeId
              ) {
                info.nodeTo = [parallelWayChild.nodeId];
              } else {
                let tempNode = nodesGroup[info.nodeId];
                if (Array.isArray(tempNode)) {
                  for (let t_item of tempNode) {
                    internalTraverse(t_item);
                  }
                } else {
                  internalTraverse(tempNode);
                }
              }
            }
          }
          internalTraverse(itemNode);
        }
      }
    }
  }
};
/** * 处理条件网关
 * @param {Object} nodesGroup - 节点关系对象
 * @param {Object} parmData - 节点关系数组
 * @returns
 * */
const handleConditionGetway = (nodesGroup, parmData) => {
  let getwayList = parmData
    .filter((c) => {
      return c.nodeType == 2;
    })
    .reverse();

  if (!isEmptyArray(getwayList)) {
    //处理条件网关
    for (let getway of getwayList) {
      if (nodesGroup.hasOwnProperty(getway.nodeId)) {
        let itemNodes = nodesGroup[getway.nodeId];
        let comNode = itemNodes.find((c) => {
          return c.nodeType != 3;
        });
        if (!comNode) continue;
        let conditionList = itemNodes.filter((c) => {
          return c.nodeId != comNode.nodeId;
        });
        for (let itemNode of conditionList) {
          function internalTraverse(info) {
            if (!info) return;
            if (info.nodeType == 7) {
              let condition_parallelNodes = nodesGroup[info.nodeId];
              if (isEmptyArray(condition_parallelNodes)) return;
              let condition_parallelWayChild = condition_parallelNodes.find(
                (c) => {
                  //并行聚合节点
                  return !info.nodeTo.includes(c.nodeId);
                }
              );
              condition_parallelWayChild.nodeTo = [comNode.nodeId];
              return;
            }
            if (!nodesGroup[info.nodeId]) {
              info.nodeTo = [comNode.nodeId];
            } else {
              let tempNode = nodesGroup[info.nodeId];
              if (Array.isArray(tempNode)) {
                for (let t_item of tempNode) {
                  internalTraverse(t_item);
                }
              } else {
                internalTraverse(tempNode);
              }
            }
          }
          internalTraverse(itemNode);
        }
      }
    }
  }
};
