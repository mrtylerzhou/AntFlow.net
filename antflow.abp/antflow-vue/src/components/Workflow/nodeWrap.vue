<!--
 * @Date: 2022-09-21 14:41:53
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-05-24 15:20:24
 * @FilePath: /ant-flow/src/components/nodeWrap.vue
-->
<template>
    <!--审批人，抄送人分支-->
    <div class="node-wrap" v-if="nodeConfig.nodeType != 2 && nodeConfig.nodeType != 7">
        <div class="node-wrap-box"
            :class="(nodeConfig.nodeType == 1 ? 'start-node ' : '') + (isTried && nodeConfig.error ? 'active error' : '')">
            <div class="title" :style="`background: rgb(${bgColors[nodeConfig.nodeType]});`">
                <span v-if="nodeConfig.nodeType == 1">{{ nodeConfig.nodeName }}</span>
                <template v-else>
                    <svg-icon icon-class="copy-user" class="iconfont" v-if="nodeConfig.nodeType == 6" />
                    <svg-icon icon-class="approve" class="iconfont" v-else />
                    <input v-if="isInput" type="text" class="fd-input editable-title-input" @blur="blurEvent()"
                        @focus="$event.currentTarget.select()" v-focus v-model="nodeConfig.nodeName"
                        :placeholder="defaultText" />
                    <span v-else class="editable-title" @click="clickEvent()">{{ nodeConfig.nodeName }}</span>
                    <i class="anticon anticon-close close" @click="delNode()"></i>
                    <i v-if="noticeIconShow" class="anticon anticon-notice notice"></i>
                </template>
            </div>
            <div class="content" @click="setNodeInfo">
                <div class="text">
                    <span class="placeholder" v-if="!showText">请选择{{ defaultText }}</span>
                    {{ showText }}
                </div>
                <i class="anticon anticon-right arrow"></i>
            </div>
            <div class="error_tip" v-if="isTried && nodeConfig.error">
                <i class="anticon anticon-exclamation-circle"></i>
            </div>
        </div>
        <addNode v-model:childNodeP="nodeConfig.childNode" />
    </div>
    <!--条件分支-->
    <div class="branch-wrap" v-if="nodeConfig.nodeType == 2">
        <div class="branch-box-wrap">
            <div class="branch-box">
                <button class="add-branch" @click="addTerm">添加条件</button>
                <div class="col-box" v-for="(item, index) in nodeConfig.conditionNodes" :key="index">
                    <div class="condition-node">
                        <div class="condition-node-box">
                            <div class="auto-judge" :class="isTried && item.error ? 'error active' : ''">
                                <div class="sort-left" v-if="index != 0" @click="arrTransfer(index, -1)">&lt;</div>
                                <div class="title-wrapper">
                                    <svg-icon icon-class="dynamic-condition" class="iconfont"
                                        v-if="nodeConfig.isDynamicCondition == true" />
                                    <svg-icon icon-class="parallel-condition" class="iconfont"
                                        v-else-if="nodeConfig.isParallel == true" />
                                    <svg-icon icon-class="condition" class="iconfont" v-else />
                                    <input v-if="isInputList[index]" type="text" class="fd-input editable-title-input"
                                        @blur="blurEvent(index)" @focus="$event.currentTarget.select()" v-focus
                                        v-model="item.nodeName" />
                                    <span v-else class="editable-title" @click="clickEvent(index)">{{ item.nodeName
                                    }}</span>
                                    <span class="priority-title" @click="setNodeInfo(item.priorityLevel)">优先级{{
                                        item.priorityLevel }}</span>
                                    <i class="anticon anticon-close close" @click="delTerm(index)"></i>
                                </div>
                                <div class="sort-right" v-if="index != nodeConfig.conditionNodes.length - 1"
                                    @click="arrTransfer(index)">&gt;</div>
                                <div class="content" @click="setNodeInfo(item.priorityLevel)">
                                    {{ item.nodeDisplayName || $func.conditionStr(nodeConfig, index) }}</div>
                                <div class="error_tip" v-if="isTried && item.error">
                                    <i class="anticon anticon-exclamation-circle"></i>
                                </div>
                            </div>
                            <addNode v-model:childNodeP="item.childNode" />
                        </div>
                    </div>
                    <nodeWrap v-if="item.childNode" v-model:nodeConfig="item.childNode" />
                    <template v-if="index == 0">
                        <div class="top-left-cover-line"></div>
                        <div class="bottom-left-cover-line"></div>
                    </template>
                    <template v-if="index == nodeConfig.conditionNodes.length - 1">
                        <div class="top-right-cover-line"></div>
                        <div class="bottom-right-cover-line"></div>
                    </template>
                </div>
            </div>
            <addNode v-model:childNodeP="nodeConfig.childNode" />
        </div>
    </div>
    <!--并行审批分支-->
    <div class="branch-wrap" v-if="nodeConfig.nodeType == 7">
        <div class="branch-box-wrap">
            <div class="branch-box">
                <button class="add-branch" @click="addTerm">添加审批节点</button>
                <div class="col-box" v-for="(item, index) in nodeConfig.parallelNodes" :key="index">
                    <div class="condition-node">
                        <div class="condition-node-box">
                            <div class="node-wrap-box" :class="isTried && item.error ? 'error active' : ''">
                                <div class="title" :style="`background: rgb(${bgColors[4]});`">
                                    <svg-icon icon-class="approve" class="iconfont" />
                                    <input v-if="isInputList[index]" type="text" class="fd-input editable-title-input"
                                        @blur="blurEvent(index)" @focus="$event.currentTarget.select()" v-focus
                                        v-model="item.nodeName" />
                                    <span v-else class="editable-title" @click="clickEvent(index)">{{ item.nodeName
                                    }}</span>
                                    <i class="anticon anticon-close close" @click="delTerm(index)"></i>
                                    <i v-if="item.templateVos && item.templateVos.length > 0"
                                        class="anticon anticon-notice notice"></i>
                                </div>

                                <div class="content" @click="setNodeInfo(index)">
                                    <div class="text">
                                        <span class="placeholder" v-if="!item.nodeDisplayName">请选择{{ defaultText
                                        }}</span>
                                        {{ item.nodeDisplayName }}
                                    </div>
                                    <i class="anticon anticon-right arrow"></i>
                                </div>
                                <div class="error_tip" v-if="isTried && item.error">
                                    <i class="anticon anticon-exclamation-circle"></i>
                                </div>
                            </div>
                            <addNode v-model:childNodeP="item.childNode" />
                        </div>
                    </div>
                    <nodeWrap v-if="item.childNode" v-model:nodeConfig="item.childNode" />
                    <template v-if="index == 0">
                        <div class="top-left-cover-line"></div>
                        <div class="bottom-left-cover-line"></div>
                    </template>
                    <template v-if="index == nodeConfig.parallelNodes.length - 1">
                        <div class="top-right-cover-line"></div>
                        <div class="bottom-right-cover-line"></div>
                    </template>
                </div>
            </div>
            <addNode v-model:childNodeP="nodeConfig.childNode" />
        </div>
    </div>
    <nodeWrap v-if="nodeConfig.childNode" v-model:nodeConfig="nodeConfig.childNode" />
</template>
<script setup>
import { onMounted, ref, watch, getCurrentInstance, computed } from "vue";
import $func from "@/utils/antflow/index";
import { useStore } from '@/store/modules/workflow'
import { bgColors, placeholderList } from '@/utils/antflow/const'
import { NodeUtils } from '@/utils/antflow/nodeUtils'
const { proxy } = getCurrentInstance();
let _uid = getCurrentInstance().uid;
let props = defineProps({
    nodeConfig: {
        type: Object,
        default: () => ({}),
    },
    flowPermission: {
        type: Object,
        default: () => [],
    },
});

let isInputList = ref([]);
let isInput = ref(false);

let emits = defineEmits(["update:flowPermission", "update:nodeConfig"]);
let store = useStore();
let {
    setPromoter,
    setApprover,
    setCopyer,
    setCondition,
    setFlowPermission,
    setApproverConfig,
    setCopyerConfig,
    setConditionsConfig,
} = store;
let isTried = computed(() => store.isTried)
let flowPermission1 = computed(() => store.flowPermission1)
let approverConfig1 = computed(() => store.approverConfig1)
let copyerConfig1 = computed(() => store.copyerConfig1)
let conditionsConfig1 = computed(() => store.conditionsConfig1)

let defaultText = computed(() => {
    return placeholderList[props.nodeConfig.nodeType]
});

let noticeIconShow = computed(() => {
    return !proxy.isEmptyArray(props.nodeConfig.templateVos);
});

/**节点名称展示 */
let showText = computed(() => {
    if (!props.nodeConfig.nodeType) return '';
    if (props.nodeConfig.nodeType == 1) return $func.arrToStr(props.flowPermission) || '发起人';
    if (props.nodeConfig.nodeType == 4) return $func.setApproverStr(props.nodeConfig);
    if (props.nodeConfig.nodeType == 6) return $func.copyerStr(props.nodeConfig);
});
/**
* 重置条件节点错误状态和展示名称
*/
const resetConditionNodesErr = () => {
    for (var i = 0; i < props.nodeConfig.conditionNodes.length; i++) {
        let conditionTitle = $func.conditionStr(props.nodeConfig, i);
        props.nodeConfig.conditionNodes[i].error = conditionTitle.indexOf("请设置条件") > -1;
        const defaultCond = i == props.nodeConfig.conditionNodes.length - 1 && props.nodeConfig.conditionNodes[i].conditionList.flat().filter(
            (item) => item.columnId && item.columnId !== 0
        ).length == 0;
        props.nodeConfig.conditionNodes[i].isDefault = defaultCond ? 1 : 0;
        props.nodeConfig.conditionNodes[i].nodeDisplayName = proxy.isEmpty(conditionTitle) ? props.nodeConfig.conditionNodes[i].nodeDisplayName : conditionTitle;
    }
    let maxLen = props.nodeConfig.conditionNodes.length - 1;
    let node = props.nodeConfig.conditionNodes[maxLen];
    if (node && node.conditionList.length <= 0) {
        node.isDefault = 1;
        node.error = false;
    }
}
/**
 * 重置并行节点错误状态和展示名称
 */
const resetParallelNodesErr = () => {
    if (!props.nodeConfig.parallelNodes) return;
    for (var i = 0; i < props.nodeConfig.parallelNodes.length; i++) {
        let parallTitle = $func.setApproverStr(props.nodeConfig.parallelNodes[i]);
        props.nodeConfig.parallelNodes[i].error = (props.nodeConfig.setType == 4 || props.nodeConfig.setType == 5)
            && proxy.isEmptyArray(props.nodeConfig.parallelNodes[i].nodeApproveList);
        props.nodeConfig.parallelNodes[i].nodeDisplayName = parallTitle;
    }
}
onMounted(() => {
    if (props.nodeConfig.nodeType == 2) {
        resetConditionNodesErr();
    } else if (props.nodeConfig.nodeType == 7) {
        resetParallelNodesErr();
    }
});
/**权限暂未实现 */
watch(flowPermission1, (flow) => {
    if (flow.flag && flow.id === _uid) {
        emits("update:flowPermission", flow.value);
    }
});
/**审批人节点监听 */
watch(approverConfig1, (approver) => {
    if (approver.flag && approver.id === _uid) {
        emits("update:nodeConfig", approver.value);
    }
});
/**抄送人节点监听 */
watch(copyerConfig1, (copyer) => {
    if (copyer.flag && copyer.id === _uid) {
        emits("update:nodeConfig", copyer.value);
    }
});
/**条件节点监听 */
watch(conditionsConfig1, (condition) => {
    if (condition.flag && condition.id === _uid) {
        emits("update:nodeConfig", condition.value);
    }
});
/**
 * 点击节点名称
 * 点击事件
 * @param index 条件索引
 */
const clickEvent = (index) => {
    if (index || index === 0) {
        isInputList.value[index] = true;
    } else {
        isInput.value = true;
    }
};
/**
 * 修改节点名称
 * 失焦事件
 * @param index 条件索引
 */
const blurEvent = (index) => {
    if (index || index === 0) {
        isInputList.value[index] = false;
        if (props.nodeConfig.nodeType == 2) {
            props.nodeConfig.conditionNodes[index].nodeName = props.nodeConfig.conditionNodes[index].nodeName || "条件";
        } else if (props.nodeConfig.nodeType == 7) {
            props.nodeConfig.parallelNodes[index].nodeName = props.nodeConfig.parallelNodes[index].nodeName || "审批人";
        }
    } else {
        isInput.value = false;
        props.nodeConfig.nodeName = props.nodeConfig.nodeName || defaultText
    }
};

/**
 * 添加网关下节点
 */
const addTerm = () => {
    if (props.nodeConfig.nodeType == 2) {
        const len = props.nodeConfig.conditionNodes.length;
        const fistConditionNode = props.nodeConfig;
        //console.log('props.nodeConfig==', JSON.stringify(props.nodeConfig))
        const n_name = resetConditionNodesTitle(fistConditionNode, len);
        props.nodeConfig.conditionNodes.push(NodeUtils.createConditionNode(n_name, null, len, 0));
        resetConditionNodesErr()
    } else if (props.nodeConfig.nodeType == 7) {
        const len = props.nodeConfig.parallelNodes.length + 1;
        const n_name = '并行审核人' + len;
        props.nodeConfig.parallelNodes.push(NodeUtils.createParallelNode(n_name, null, len, 0));
        resetParallelNodesErr();
    }
    emits("update:nodeConfig", props.nodeConfig);
};

/**
 * 删除普通审批人或抄送人节点
 */
const delNode = () => {
    emits("update:nodeConfig", props.nodeConfig.childNode);
};

/**
 * 删除网关下节点
 * @param index 条件索引
 */
const delTerm = (index) => {
    if (props.nodeConfig.nodeType == 2) {
        delConditionNodeTerm(index);
    } else if (props.nodeConfig.nodeType == 7) {
        delParallelNodeTerm(index);
    }
};
/**
 * 删除条件
 * @param index 条件索引
 */
const delConditionNodeTerm = (index) => {
    props.nodeConfig.conditionNodes.splice(index, 1);
    props.nodeConfig.conditionNodes.map((item, index) => {
        item.priorityLevel = index + 1;
        item.nodeName = resetConditionNodesTitle(props.nodeConfig, index);
    });
    resetConditionNodesErr()
    emits("update:nodeConfig", props.nodeConfig);

    if (props.nodeConfig.conditionNodes.length == 1) {
        if (props.nodeConfig.childNode) {
            if (props.nodeConfig.conditionNodes[0].childNode) {
                reData(props.nodeConfig.conditionNodes[0].childNode, props.nodeConfig.childNode);
            } else {
                props.nodeConfig.conditionNodes[0].childNode = props.nodeConfig.childNode;
            }
        }
        emits("update:nodeConfig", props.nodeConfig.conditionNodes[0].childNode);
    }
}
/**
 * 删除并行审批节点
 * @param index 条件索引
 */
const delParallelNodeTerm = (index) => {
    props.nodeConfig.parallelNodes.splice(index, 1);
    props.nodeConfig.parallelNodes.map((item, index) => {
        item.priorityLevel = index + 1;
        item.nodeName = `审批人${index + 1}`;
    });
    resetParallelNodesErr();
    emits("update:nodeConfig", props.nodeConfig);
    if (props.nodeConfig.parallelNodes.length == 1) {
        if (props.nodeConfig.childNode) {
            if (props.nodeConfig.parallelNodes[0].childNode) {
                reData(props.nodeConfig.parallelNodes[0].childNode, props.nodeConfig.childNode);
            } else {
                props.nodeConfig.parallelNodes[0].childNode = props.nodeConfig.childNode;
            }
        }
        emits("update:nodeConfig", props.nodeConfig.parallelNodes[0].childNode);
    }
}
/**重置子节点 */
const reData = (data, addData) => {
    if (!data.childNode) {
        data.childNode = addData;
    } else {
        reData(data.childNode, addData);
    }
};

/**删除或添加条件节点 重置节点标题 */
const resetConditionNodesTitle = (conditionNode, len) => {
    if (!conditionNode) {
        return `条件`;
    }
    let isDynamicCondition = conditionNode.isDynamicCondition;
    let isParallel = conditionNode.isParallel;
    if (isDynamicCondition == true) {
        return `动态条件${len + 1}`;
    }
    if (isParallel == true) {
        return `并行条件${len + 1}`;
    }
    return `条件${len + 1}`;
}

/**
 * 设置节点信息
 */
const setNodeInfo = (index) => {
    var { nodeType } = props.nodeConfig;
    if (nodeType == 1) {
        setPromoter(true);
        setFlowPermission({
            value: props.flowPermission,
            flag: false,
            id: _uid,
        });
    } else if (nodeType == 4) {
        setApprover(true);
        setApproverConfig({
            value: {
                ...JSON.parse(JSON.stringify(props.nodeConfig))
            },
            flag: false,
            id: _uid,
        });
    } else if (nodeType == 6) {
        setCopyer(true);
        setCopyerConfig({
            value: JSON.parse(JSON.stringify(props.nodeConfig)),
            flag: false,
            id: _uid,
        });
    }
    else if (nodeType == 7) {
        setApprover(true);
        setApproverConfig({
            value: {
                ...JSON.parse(JSON.stringify(props.nodeConfig)),
                index: index,
            },
            flag: false,
            id: _uid,
        });
    }
    else {
        setCondition(true);
        setConditionsConfig({
            value: JSON.parse(JSON.stringify(props.nodeConfig)),
            priorityLevel: index,
            flag: false,
            id: _uid,
        });
    }
};
/**
 * 条件排序
 * @param index 条件索引
 * @param type 排序类型
 */
const arrTransfer = (index, type = 1) => {
    //向左-1,向右1
    props.nodeConfig.conditionNodes[index] = props.nodeConfig.conditionNodes.splice(
        index + type,
        1,
        props.nodeConfig.conditionNodes[index]
    )[0];
    props.nodeConfig.conditionNodes.map((item, index) => {
        item.priorityLevel = index + 1;
    });
    resetConditionNodesErr()
    emits("update:nodeConfig", props.nodeConfig);
};
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";

.error_tip {
    position: absolute;
    top: 0px;
    right: 0px;
    transform: translate(150%, 0px);
    font-size: 24px;
}

.promoter_person .el-dialog__body {
    padding: 10px 20px 14px 20px;
}

.selected_list {
    margin-bottom: 20px;
    line-height: 30px;
}

.selected_list span {
    margin-right: 10px;
    padding: 3px 6px 3px 9px;
    line-height: 12px;
    white-space: nowrap;
    border-radius: 2px;
    border: 1px solid rgba(220, 220, 220, 1);
}

.selected_list img {
    margin-left: 5px;
    width: 7px;
    height: 7px;
    cursor: pointer;
}
</style>