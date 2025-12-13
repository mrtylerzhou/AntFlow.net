<!--
 * @Date: 2025-03-08 15:20
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2025-03-08 15:20
 * @FilePath: /src/views/flowTask/pendding/components/chooseFlowNode/lineWarp.vue
-->
<template>
    <div class="node-wrap" v-if="nodeConfig.nodeType != 7 && nodeConfig.parallelChildNode == 0">
        <div class="node-wrap-box" :class="(nodeConfig.nodeType == 1 ? 'start-node not-allowed' : '')"
            :data-node-key="nodeConfig.nodeId" @click="handleChecked(nodeConfig)">
            <div class="title"
                :style="(nodeConfig.isNodeDeduplication == 1 ? `background: rgb(${bgColors[0]});` : `background: rgb(${bgColors[nodeConfig.nodeType]});`)">
                <span>{{ nodeConfig.nodeName }}</span>
            </div>
            <div class="content">
                <div v-html="nodeConfig.nodeDisplayName" class="text"></div>
            </div>
        </div>
        <div class="pixel-line"></div>
    </div>
    <!--并行审批分支-->
    <div class="branch-wrap" v-if="nodeConfig.nodeType == 7">
        <div class="branch-box-wrap">
            <div class="branch-box">
                <button class="add-branch">并行审批</button>
                <div class="col-box" v-for="(item, index) in nodeConfig.parallelNodes" :key="index">
                    <div class="condition-node">
                        <div class="condition-node-box">
                            <div class="node-wrap-box" :data-node-key="item.nodeId" @click="handleChecked(item)">
                                <div class="title" :style="`background: rgb(${bgColors[4]});`">
                                    <span class="iconfont"></span>
                                    <span class="editable-title">{{ item.nodeName }}</span>
                                </div>
                                <div class="content">
                                    <div class="text">
                                        {{ item.nodeDisplayName }}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <LineWarp v-if="item.childNode" v-model:nodeConfig="item.childNode" />
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
            <div class="pixel-line"></div>
        </div>
        <div class="pixel-line"></div>
    </div>
    <LineWarp v-if="nodeConfig.childNode" v-model:nodeConfig="nodeConfig.childNode" />
</template>
<script setup>
import { onMounted } from 'vue';
import { bgColors } from '@/utils/antflow/const'
import { useStore } from '@/store/modules/workflow';
let store = useStore();
let { setApproveChooseFlowNodeConfig } = store;
let props = defineProps({
    nodeConfig: {
        type: Object,
        default: () => ({}),
    }
});

onMounted(() => {
    const elementList = document.getElementsByClassName("node-wrap-box");
    for (let element of elementList) {
        const customNodeKey = element.getAttribute('data-node-key');
        if ('Gb2' == customNodeKey) {
            element.classList.remove("not-allowed");
            element.classList.toggle("not-allowed");
            continue;
        }
        element.classList.remove("not-allowed");
        element.classList.remove("current-node");
        element.classList.remove("checked-node");
        if (props.nodeConfig.afterNodeIds.indexOf(customNodeKey) > -1) {
            element.classList.toggle("not-allowed");
            continue;
        }
        if (customNodeKey == props.nodeConfig.currentNodeId) {
            element.classList.toggle("not-allowed");
            element.classList.toggle("current-node");
            continue;
        }
    }
});

const handleChecked = (item) => {
    const elementList = document.getElementsByClassName("node-wrap-box");
    if (props.nodeConfig.afterNodeIds.indexOf(item.nodeId) > -1) {
        return;
    }
    if (props.nodeConfig.currentNodeId == item.nodeId) {
        return;
    }
    if ('Gb2' == item.nodeId) {
        for (let element of elementList) {
            const customNodeKey = element.getAttribute('data-node-key');
            if ('Gb2' == customNodeKey) {
                element.classList.remove("not-allowed");
                element.classList.toggle("not-allowed");
                break;
            }
        }
        return;
    }
    for (let element of elementList) {
        element.classList.remove("not-allowed");
        element.classList.remove("checked-node");
        const customNodeKey = element.getAttribute('data-node-key');
        if (element.classList.contains("not-allowed")) {
            continue;
        }
        if (customNodeKey == item.nodeId) {
            element.classList.toggle("checked-node");
        } else {
            element.classList.toggle("not-allowed");
            continue;
        }
    }
    setApproveChooseFlowNodeConfig({
        visible: false,
        nodeId: String(item.Id),
        nodeName: item.nodeName,
        nodeDisplayName: item.nodeDisplayName,
    });
}
//console.log("props.nodeConfig==============",JSON.stringify(props.nodeConfig)) 
// active
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";

.pixel-line {
    width: 2px;
    height: 50px;
    background-color: #cacaca;
}

.end-node-circle {
    width: 20px;
    height: 20px;
    margin: auto;
    border-radius: 50%;
    background: #dbdcdc
}

.line-through {
    text-decoration: line-through
}

.checked-node {
    border: 5px solid #13ce66;
    border-radius: 7px;
    box-shadow: 3px 3px 5px #13ce66;
}

.current-node {
    border: 3px solid #1890ff;
    border-radius: 7px;
    box-shadow: 3px 3px 5px #1890ff;
}

.not-allowed {
    cursor: not-allowed;
    opacity: 0.9;
    background-color: #cacaca;
}
</style>