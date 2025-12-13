<template>
    <section class="antflow-design" ref="antflowDesignRef">
        <div class="zoom">
            <div class="zoom-out" @click="zoomOut" title="缩小"></div>
            <span>{{ nowVal }}%</span>
            <div class="zoom-in" @click="zoomIn" title="放大"></div>
            <!--刷新图标代码-->
            <div class="zoom-reset" @click="zoomReset" title="还原缩放比例">&#10227</div>
        </div>
        <div class="box-scale" ref="boxScaleRef">
            <nodeWrap v-model:nodeConfig="nodeConfig" />
            <div class="end-node">
                <div class="end-node-circle"></div>
                <div class="end-node-text">流程结束</div>
            </div>
        </div>
    </section>
    <errorDialog v-model:visible="tipVisible" :list="tipList" />
    <promoterDrawer />
    <approverDrawer :directorMaxLevel="directorMaxLevel" />
    <copyerDrawer />
    <conditionDrawer />
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useStore } from '@/store/modules/workflow'
import { nodeTypeList } from '@/utils/antflow/const'
import errorDialog from "@/components/Workflow/dialog/errorDialog.vue";
import promoterDrawer from "@/components/Workflow/drawer/promoterDrawer.vue";
import approverDrawer from "@/components/Workflow/drawer/approverDrawer.vue";
import copyerDrawer from "@/components/Workflow/drawer/copyerDrawer.vue";
import conditionDrawer from "@/components/Workflow/drawer/conditionDrawer.vue";
import { wheelZoomFunc, zoomInit, resetImage } from "@/utils/antflow/zoom.js";
import { flattenMapTreeToList } from '@/utils/antflow/nodeUtils'
const { proxy } = getCurrentInstance();
let { setIsTried } = useStore()
const emit = defineEmits(['nextChange'])
let props = defineProps({
    processData: {
        type: Object,
        default: () => (null),
    }
});
const antflowDesignRef = ref(null);
const boxScaleRef = ref(null);
let tipList = ref([]);
let tipVisible = ref(false);
let nowVal = ref(100);
let nodeConfig = ref({});
let directorMaxLevel = ref(3);
onMounted(async () => {
    zoomInit(antflowDesignRef, boxScaleRef, (val) => {
        nowVal.value = val
    });
    if (props.processData) {
        nodeConfig.value = props.processData;
    }
});

/**
 * 判断流程中是否有审批节点 Demo 预览需要，项目中不使用可以去掉这步验证
 * @param treeNode 
 */
const validateIsExistApproveNode = (treeNode) => {
    if (!treeNode) return { isSuccess: false, msg: "至少配置一个有效节点，实际项目中不需要可以去掉" };
    const nodeArray = flattenMapTreeToList(treeNode);
    const isExist = nodeArray.some(node => {
        return node.nodeType == 4 || node.nodeType == 6 || node.nodeType == 7;
    });
    if (!isExist) {
        return { isSuccess: false, msg: "至少配置一个有效节点，实际项目中不需要可以去掉" };
    }
    else {
        return { isSuccess: true, msg: "" };
    }
}
/**
 * 并行审批节点验证
 * 判断存在并行审批就必须有聚合节点
 * @param treeNode 
 */
const validateParallelApproveNode = (treeNode) => {
    if (proxy.isEmpty(treeNode)) return { isSuccess: true, msg: "" };
    if (treeNode.nodeType == 7) {
        if (proxy.isEmpty(treeNode.childNode) || treeNode.childNode.nodeType != 4) {
            return { isSuccess: false, msg: "并行审批下必须有一个审批人节点作为聚合节点" };
        } else {
            return validateParallelApproveNode(treeNode.childNode);
        }
    }
    else {
        return validateParallelApproveNode(treeNode.childNode);
    }
}

/**
 * 条件并行 节点验证
 * 判断存在条件并行节点就必须有聚合节点
 * @param treeNode 
 */
const validateParallelConditionNode = (treeNode) => {
    if (proxy.isEmpty(treeNode)) return { isSuccess: true, msg: "" };
    if (treeNode.nodeType == 2 && treeNode.isParallel == true) {
        if (proxy.isEmpty(treeNode.childNode) || treeNode.childNode.nodeType != 4) {
            return { isSuccess: false, msg: "条件并行节点下必须有一个审批人节点作为聚合节点" };
        } else {
            return validateParallelConditionNode(treeNode.childNode);
        }
    }
    else {
        return validateParallelConditionNode(treeNode.childNode);
    }
}

// 节点验证 Set集合
const nodeVerifyMap = new Set([validateIsExistApproveNode, validateParallelApproveNode, validateParallelConditionNode]);

/**
 * 节点必填校验
 * @param childNode 
 */
const reErr = ({ childNode }) => {
    if (childNode) {
        let { nodeType, error, nodeName, conditionNodes, parallelNodes } = childNode;
        if (nodeType == 1) {
            reErr(childNode);
        }
        else if (nodeType == 2) {
            reErr(childNode);
            for (var i = 0; i < conditionNodes.length; i++) {
                if (conditionNodes[i].error == true) {
                    tipList.value.push({ name: conditionNodes[i].nodeName, nodeType: "条件" });
                }
                reErr(conditionNodes[i]);
            }
        }
        else if (nodeType == 3) {
            reErr(childNode);
        }
        else if (nodeType == 4 || nodeType == 6) {
            if (error) {
                tipList.value.push({
                    name: nodeName,
                    nodeType: nodeTypeList[nodeType],
                });
            }
            reErr(childNode);
        }
        else if (nodeType == 7) {
            reErr(childNode);
            for (var i = 0; i < parallelNodes.length; i++) {
                if (parallelNodes[i].error) {
                    tipList.value.push({ name: parallelNodes[i].nodeName, nodeType: "审批人" });
                }
                reErr(parallelNodes[i]);
            }
        }
    } else {
        childNode = null;
    }
};

const getJson = () => {
    setIsTried(true);
    tipList.value = [];
    reErr(nodeConfig.value);
    if (tipList.value.length != 0) {
        emit('nextChange', { label: "流程设计", key: "processDesign" });
        tipVisible.value = true;
        return false;
    }

    for (const handleVerifyFunc of nodeVerifyMap) {
        const { isSuccess, msg } = handleVerifyFunc(nodeConfig.value);
        if (!isSuccess) {
            proxy.$modal.msgError(msg);
            emit('nextChange', { label: "流程设计", key: "processDesign" });
            return false;
        }
    }
    let submitData = JSON.parse(JSON.stringify(nodeConfig.value));
    return submitData;
};

// 给父级页面提供得获取本页数据得方法
const getData = () => {
    let resData = getJson();
    return new Promise((resolve, reject) => {
        if (!resData) {
            reject({ formData: null });
        }
        resolve({ formData: resData })
    })
};
/** 页面放大 */
function zoomIn() {
    wheelZoomFunc({ scaleFactor: parseInt(nowVal.value) / 100 + 0.1, isExternalCall: true })
}

/** 页面缩小 */
function zoomOut() {
    wheelZoomFunc({ scaleFactor: parseInt(nowVal.value) / 100 - 0.1, isExternalCall: true })
}
/** 还原缩放比例 */
function zoomReset() {
    resetImage()
}
defineExpose({
    getData
})
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";
</style>