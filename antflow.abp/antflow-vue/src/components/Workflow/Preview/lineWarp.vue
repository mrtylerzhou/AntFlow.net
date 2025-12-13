<!--
 * @Date: 2022-09-21 14:41:53
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-05-24 15:20:24
 * @FilePath: /src/components/Workflow/Preview/lineWarp.vue
-->
<template>
    <div class="node-wrap" v-if="nodeConfig.nodeType != 7 && nodeConfig.parallelChildNode == 0">
        <div class="node-wrap-box" :class="(nodeConfig.nodeType == 1 ? 'start-node ' : '')"
            :data-node-key="nodeConfig.nodeId" @click="clickNodeBtn(nodeConfig)">
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
                            <div class="node-wrap-box" :data-node-key="item.nodeId" @click="clickNodeBtn(item)">
                                <div class="title" :style="`background: rgb(${bgColors[4]});`">
                                    <span class="editable-title">{{ item.nodeName }}</span>
                                </div>
                                <div class="content">
                                    <div v-html="item.nodeDisplayName" class="text"></div>
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
import { inject, onMounted } from 'vue';
import { bgColors } from '@/utils/antflow/const'
let props = defineProps({
    nodeConfig: {
        type: Object,
        default: () => ({}),
    }
});
onMounted(() => {
    const elementList = document.getElementsByClassName("node-wrap-box");
    for (let element of elementList) {
        element.classList.remove("current-node");
        const customNodeKey = element.getAttribute('data-node-key');
        if (customNodeKey == 'Gb2') continue;
        if (customNodeKey == props.nodeConfig.currentNodeId) {
            element.classList.toggle("current-node");
        }
    }
});

const handleClickNode = inject("onClickNode", true);
const clickNodeBtn = (data) => {
    if (data.beforeNodeIds && data.beforeNodeIds.includes(data.nodeId)) {
        return;
    }
    const elementList = document.getElementsByClassName("node-wrap-box");
    for (let element of elementList) {
        element.classList.remove("active-node");
        const customNodeKey = element.getAttribute('data-node-key');
        if (customNodeKey == data.nodeId) {
            element.classList.toggle("active-node");
        }
    }
    if (typeof handleClickNode === 'function') {
        handleClickNode(data);
    }
};

//console.log("props.nodeConfig==============",JSON.stringify(props.nodeConfig)) 
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

.testtt {
    width: 2px;
    border-style: solid;
    border-width: 8px 6px 4px;
    border-color: #cacaca transparent transparent;
    background: #f5f5f7;
}

.current-node {
    border: 3px solid #1890ff;
    border-radius: 7px;
    box-shadow: 3px 3px 5px #1890ff;
}

.active-node {
    border: 3px solid #30b08f;
    border-radius: 7px;
    box-shadow: 3px 3px 5px #30b08f;
}
</style>