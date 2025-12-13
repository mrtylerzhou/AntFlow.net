// import { FormatUtils } from '@/utils/formatFlowPreview'
import { isEmpty, isEmptyArray } from "@/utils/antflow/ObjectUtils";
export class FormatPreviewUtils {
  /**
   * 对基础设置,高级设置等设置页内容进行格式化
   * @param param
   */
  // 静态方法，用于格式化设置参数
  static formatSettings(param) {
    let nodeList = [];
    if (!param || isEmptyArray(param.bpmnNodeList)) return nodeList;
    for (let node of param.bpmnNodeList) {
      node.currentNodeId = param.currentNodeId;
      node.beforeNodeIds = param.beforeNodeIds;
      node.afterNodeIds = param.afterNodeIds;
      nodeList.push(this.createNewNode(node));
    }
    return this.depthConverterNodes(nodeList);
  }

  /**
   * 展平树结构
   * @param {Object} treeData  - 节点数据
   * @returns Array - 节点数组
   */
  static createNewNode(node) {
    if (isEmpty(node)) return null;
    let newNode = {
      Id: node.id,
      nodeId: node.nodeId,
      nodeType: node.nodeType,
      nodeName: node.nodeName,
      nodeDisplayName: this.arrToStr(node),
      nodeFrom: node.nodeFrom,
      nodeTo: node.nodeTo,
      params: node.params,
      isNodeDeduplication: node.isDeduplication,
      parallelChildNode: 0,
      currentNodeId: node.currentNodeId,
      beforeNodeIds: node.beforeNodeIds,
      afterNodeIds: node.afterNodeIds,
    };
    return newNode;
  }
  static arrToStr(nodeObj) {
    if (isEmpty(nodeObj)) return "未获取到审批人信息";
    let arr = nodeObj.params?.assigneeList;
    let nodeNameStr = isEmpty(nodeObj.nodeDisplayName)
      ? nodeObj.nodePropertyName
      : nodeObj.nodeDisplayName;
    if (nodeObj.nodeType == 1) return nodeObj.nodeDisplayName;
    if (isEmptyArray(arr)) return nodeNameStr;
    let strApprovers = arr
      .map((item) => {
        let _item_displayName = isEmpty(item.assigneeName)
          ? item.elementName
          : item.assigneeName;
        if (item.isDeduplication == 1) {
          return (
            '<del  style="background-color: red !important;font-weight: 100"><em>' +
            _item_displayName +
            "</em></del>  "
          );
        } else {
          return item.assigneeName;
        }
      })
      .join(",");

    return isEmpty(strApprovers) == true
      ? nodeNameStr
      : nodeObj.nodePropertyName + ":" + strApprovers;
  }
  /**
   *  List 转成tree结构
   * @param {*} parmData
   * @returns
   */
  static depthConverterNodes(parmData) {
    if (isEmptyArray(parmData)) return;
    //普通审批和并行审批有相同之处，这里偷懒分开处理
    if (!parmData.some((c) => c.nodeType == 7)) {
      //判断是否包含并行网关
      return this.depthConverterToTree(parmData);
    } else {
      return this.depthConverterToTreeForParallelway(parmData);
    }
  }
  /**
   * List 转成tree结构（不包含并行网关）
   */
  static depthConverterToTree(parmData) {
    let nodesGroup = {},
      startNode = {};
    for (let t of parmData) {
      if (isEmpty(t.nodeFrom)) continue;
      if (nodesGroup.hasOwnProperty(t.nodeFrom)) {
        nodesGroup[t.nodeFrom].push(t);
      } else {
        nodesGroup[t.nodeFrom] = [t];
      }
    }
    for (let node of parmData) {
      if (1 == node.nodeType) {
        startNode = node;
      }
      let currNodeId = node.nodeId;
      if (nodesGroup.hasOwnProperty(currNodeId)) {
        let itemNodes = nodesGroup[currNodeId];
        for (let itemNode of itemNodes) {
          if (4 == itemNode.nodeType || 6 == itemNode.nodeType) {
            Object.assign(node, { childNode: itemNode });
          }
        }
      }
    }
    //console.log('startNode====', JSON.stringify(startNode))
    return startNode;
  }
  /**
   * List 转成tree结构（包含并行网关）
   */
  static depthConverterToTreeForParallelway(parmData) {
    let nodesGroup = {},
      startNode = {};
    for (let t of parmData) {
      if (isEmpty(t.nodeFrom)) continue;
      if (nodesGroup.hasOwnProperty(t.nodeFrom)) {
        nodesGroup[t.nodeFrom].push(t);
      } else {
        nodesGroup[t.nodeFrom] = [t];
      }
    }
    for (let node of parmData) {
      if (1 == node.nodeType) {
        startNode = node;
      }
      let currNodeId = node.nodeId;
      if (nodesGroup.hasOwnProperty(currNodeId)) {
        let itemNodes = nodesGroup[currNodeId];
        for (let itemNode of itemNodes) {
          if (4 == itemNode.nodeType) {
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
              itemNode.parallelChildNode = 1;
              node.parallelNodes.push(itemNode);
            }
          } else {
            node.childNode = itemNode;
          }
        }
      }
    }
    //console.log('startNode====', JSON.stringify(startNode))
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
}
