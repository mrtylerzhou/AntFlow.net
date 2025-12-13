<template>
    <div class="drawer-div">
        <div style="margin:10px auto; text-align:center;">
            <el-button type="success" v-print="printOption">打 印</el-button>
            <el-button @click="closeWin()">关 闭</el-button>
        </div>
        <div id="printTable">
            <div v-if="formCode == 'LEAVE_WMA'">
                <leveTimeTable />
            </div>
            <div v-else>
                <el-card style="margin-bottom: 10px;">
                    <template v-slot:header>
                        <div class="clearfix">
                            <span>表单信息</span>
                        </div>
                    </template>
                    <el-container>
                        <el-main>
                            <flow-biz-form />
                        </el-main>
                    </el-container>
                </el-card>
                <el-card>
                    <template v-slot:header>
                        <div class="clearfix">
                            <span>审批记录</span>
                        </div>
                    </template>
                    <el-container>
                        <el-main>
                            <flow-step-table />
                        </el-main>
                    </el-container>
                </el-card>
            </div>
        </div>

    </div>
</template>
<script setup>
import flowBizForm from "./components/flowBizForm.vue";
import flowStepTable from "./components/flowStepTable.vue";
import leveTimeTable from "./components/leveTimeTable.vue";
const { query } = useRoute();
const { formCode } = query;
const printOption = {
    id: 'printTable', // 打印元素的id 不需要携带#号
    // preview: true, // 开启打印预览
    // previewTitle: '打印预览', // 打印预览标题
    // previewPrintBtnLabel: '确认打印', // 打印预览打印按钮文字
    //popTitle: '流程信息', // 页眉标题 默认浏览器标题 空字符串时显示undefined 使用html语言
    // 头部文字 默认空 在节点中添加 DOM 节点， 并用,(Print local range only)分隔多个节点
    //extraHead: '测试',
    // 新的 CSS 样式表， 并使用,（仅打印本地范围）分隔多个节点
    //extraCss: '<meta http-equiv="Content-Language"content="zh-cn"/>',
    previewBeforeOpenCallback: () => {
        console.log("触发打印预览打开前回调");
    },
    previewOpenCallback: () => {
        console.log("触发打开打印预览回调");
    },
    beforeOpenCallback: () => {
        console.log("触发打印工具打开前回调");
    },
    openCallback: () => {
        console.log("触发打开打印工具回调");
    },
    closeCallback: () => {
        console.log("触发关闭打印工具回调");
    },
    clickMounted: () => {
        console.log("触发点击打印回调");
    }
}
const closeWin = () => {
    window.close();
}
</script>
<style lang="scss" scoped>
.drawer-div {
    width: 800px;
    margin: 0 auto;
    border-radius: 4px;
    background: #fff;
    display: block;
}

@page {
    size: auto;
    margin: 0mm;
}

body,
html,
div {
    height: auto !important;
}

@media print {
    #printTable {
        width: 800px;
        zoom: 0.95;
        margin: 0 auto;
    }
}
</style>