// import { FormatDisplayUtils } from '@/utils/antflowformatdisplay_data.js'
import { hrbpOptions } from "@/utils/antflow/const";
import { isEmpty, isEmptyArray } from "@/utils/antflow/ObjectUtils";
export class FormatDisplayUtils {
  /**
   * 格式化显示数据
   * @param {Array} parmData
   * @returns Object
   */
  static getToTree(parmData) {
    if (isEmptyArray(parmData)) return;
    let node = this.createNodeDisplay(parmData);
    if (!node) return;
    let formatList = this.formatDisplayStructNodeList(parmData?.nodes);
    node.nodeConfig = this.depthConverterNodes(formatList); //parmData.nodes
    return node;
  }

  /**
   * 创建Node Data 数据
   * @param { Object } nodeData - 源节点数据
   * @returns Object
   */
  static createNodeDisplay(nodeData) {
    if (!nodeData) return;
    if (isEmptyArray(nodeData)) return;
    let displayObj = {
      tableId: nodeData?.id,
      bpmnCode: nodeData.bpmnCode,
      bpmnName: nodeData.bpmnName, //name 改成 bpmnName 其他的都是添加的
      bpmnType: nodeData.bpmnType,
      formCode: nodeData.formCode,
      appId: nodeData.appId,
      isOutSideProcess: nodeData.isOutSideProcess,
      businessPartyId: nodeData.businessPartyId,
      businessPartyName: nodeData.businessPartyName,
      businessPartyMark: nodeData.businessPartyMark,
      deduplicationType: nodeData.deduplicationType, //2去重,1不去重
      effectiveStatus: nodeData.effectiveStatus == 1 ? true : false,
      isLowCodeFlow: nodeData.isLowCodeFlow,
      lfFormData: nodeData.lfFormData,
      lfFormDataId: nodeData.lfFormDataId,
      viewPageButtons: nodeData.viewPageButtons,
      property: nodeData.property,
      remark: nodeData.remark,
      isDel: 0,
      directorMaxLevel: 3,
      nodeConfig: {},
      templateVos: nodeData.templateVos || [],
    };
    return displayObj;
  }
  /**
   * List 转成tree结构
   * @param {Array} parmData
   * @returns
   */
  static depthConverterNodes(parmData) {
    if (!parmData) return;
    if (isEmptyArray(parmData)) return;
    let nodesGroup = {};
    for (let t of parmData) {
      if (isEmpty(t.nodeFrom)) continue;
      if (nodesGroup.hasOwnProperty(t.nodeFrom) && !isEmpty(t.nodeFrom)) {
        nodesGroup[t.nodeFrom].push(t);
      } else {
        nodesGroup[t.nodeFrom] = [t];
      }
    }
    if (!parmData.some((c) => c.nodeType == 7)) {
      //判断是否包含并行网关
      return this.depthConverterToTree(nodesGroup, parmData);
    } else {
      return this.depthConverterToTreeForParallelway(nodesGroup, parmData);
    }
  }
  /**
   * List 转成tree结构（不包含并行网关）
   */
  static depthConverterToTree(nodesGroup, parmData) {
    let startNode = {};
    for (let node of parmData) {
      if (1 == node.nodeType) {
        startNode = node;
      }
      Object.assign(node, { conditionNodes: [] });
      let currNodeId = node.nodeId;
      if (nodesGroup.hasOwnProperty(currNodeId)) {
        let itemNodes = nodesGroup[currNodeId];
        for (let itemNode of itemNodes) {
          if (3 == itemNode.nodeType) {
            node.conditionNodes.push(itemNode);
          } else {
            node.childNode = itemNode;
          }
        }
      }
    }
    return startNode;
  }
  /**
   * List 转成tree结构（包含并行网关）
   */
  static depthConverterToTreeForParallelway(nodesGroup, parmData) {
    let startNode = {};
    for (let node of parmData) {
      if (1 == node.nodeType) {
        startNode = node;
      }
      Object.assign(node, { conditionNodes: [] });
      let currNodeId = node.nodeId;
      if (nodesGroup.hasOwnProperty(currNodeId)) {
        let itemNodes = nodesGroup[currNodeId];
        for (let itemNode of itemNodes) {
          if (3 == itemNode.nodeType) {
            node.conditionNodes.push(itemNode);
          } else if (4 == itemNode.nodeType) {
            let isTrueParallelNode = this.isParallelChildNode(
              itemNode,
              parmData
            );
            if (isTrueParallelNode == false) {
              node.childNode = itemNode;
            } else {
              if (!node.hasOwnProperty("parallelNodes")) {
                Object.assign(node, { parallelNodes: [] });
              }
              node.parallelNodes.push(itemNode);
            }
          } else {
            node.childNode = itemNode;
          }
        }
      }
    }
    return startNode;
  }
  /**
   * 判断是否是并行节点下的 分支子节点
   *
   * 1、当前节点的父节点 nodeType=7
   * 2、当前节点的nodeId,存在于父节点的nodeTo中
   * 以上两个条件同时满足，则该节点为并行节点下的分支子节点
   *
   * 只满足条件 1 ，则 并行路由的下一个普通审批人节点
   *
   * @param {*} currentNode
   * @param {*} parmData
   * @returns
   */
  static isParallelChildNode(currentNode, parmData) {
    for (let node of parmData) {
      if (currentNode.nodeFrom == node.nodeId) {
        if (node.nodeType != 7) {
          return false;
        } else {
          return node.nodeTo.includes(currentNode.nodeId) ? true : false;
        }
      }
    }
    return false;
  }
  /**
   * 格式api返回的数组
   * @param {*} nodeList
   * @returns
   */
  static formatDisplayStructNodeList(nodeList) {
    if (!nodeList) return;
    if (isEmptyArray(nodeList)) return nodeList;
    for (let node of nodeList) {
      if (node.nodeType == 3) {
        node.priorityLevel = node.property.sort;
        node.isDefault = node.property.isDefault;
        node.groupRelation = node.property.groupRelation;
        Object.assign(node, { conditionList: [] });
        node.conditionList = node.property.conditionList
          ? node.property.conditionList
          : [];
        delete node.property;
      }

      if (node.nodeType == 4 || node.nodeType == 6) {
        let empList = [];
        if (node.nodeProperty == 6) {
          let approveObj = {
            type: 5,
            targetId: node.property.hrbpConfType || 0,
            name: hrbpOptions.find(
              (item) => item.value == node.property.hrbpConfType
            )?.label,
          };
          empList.push(approveObj);
        } else if (
          node.nodeProperty == 4 &&
          !isEmptyArray(node.property.roleList)
        ) {
          for (let role of node.property.roleList) {
            let r = {
              type: 3,
              targetId: role.id,
              name: role.name,
            };
            empList.push(r);
          }
        } else if (
          node.nodeProperty == 5 &&
          !isEmptyArray(node.property.emplList)
        ) {
          for (let emp of node.property.emplList) {
            let approveObj = {
              type: 5,
              targetId: emp.id,
              name: emp.name,
            };
            empList.push(approveObj);
          }
        } else if (node.nodeProperty == 3) {
          node.directorLevel = node.property.assignLevelGrade;
        }
        Object.assign(node, { signType: node.property?.signType });
        node.setType = node.nodeProperty;
        Object.assign(node, { nodeApproveList: [] });

        node.nodeApproveList = empList;
        //delete node.property;
      }
    }
    return nodeList;
  }
}
