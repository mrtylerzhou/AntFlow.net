<template>
    <el-container class="layout-setup" style="height: calc(100vh - 84px);">
        <el-aside class="layout-aside">
            <el-container>
                <el-header>
                    <div class="toolbar">
                        <el-input v-model="taskMgmtVO.processNumber" placeholder="请输入关键字" clearable
                            style="width: 225px">
                            <template #append>
                                <el-button icon="Search" @click="handleQuery" />
                            </template>
                        </el-input>
                        <el-badge v-if="total > 0" :value="total" :offset="[-10, 5]">
                            <el-button icon="Refresh" @click="getList"
                                style="width: 30px; height: 30px; margin: 0px 5px;" />
                        </el-badge>
                        <el-button v-else icon="Refresh" @click="getList"
                            style="width: 30px; height: 30px; margin: 0px 5px;" />
                    </div>
                </el-header>
                <el-main>
                    <el-scrollbar>
                        <div v-loading="loading" class="list-flex-cards">
                            <span v-if="dataList.length === 0" class="empty-text">暂无待办任务</span>
                            <el-card v-if="dataList.length > 0" v-for="(item, index) in dataList" :key="item.id"
                                @click="toggleFlowActive(item, index)"
                                :class="['item-card', { active: activeIndex === index }]">
                                <div class="card-content pointer">
                                    <div>
                                        <p class="card-title">
                                            [{{ item.isLowCodeFlow ? 'LF' : 'DIY' }}] {{ item.processTypeName }}
                                        </p>
                                        <p class="card-detail">
                                            <span style="width: 40px;">描述：</span>
                                            <span class="card-reason">{{ item.description }}</span>
                                        </p>
                                        <p class="card-detail">
                                            <span>流程编号：</span>
                                            <span class="card-reason">{{ substringHidden(item.processNumber) }}</span>
                                        </p>
                                        <p class="card-time">
                                            <span>审批状态：</span>
                                            <span class="card-time-value">
                                                <el-tag type="primary">{{ item.taskState }}</el-tag>
                                            </span>
                                        </p>
                                        <p class="card-time">
                                            <span>发起时间：</span>
                                            <span class="card-time-value">
                                                {{ parseTime(item.createTime, '{y}-{m}-{d} {h}: {i}') }}</span>
                                        </p>
                                    </div>
                                    <div class="card-user">
                                        <span class="card-username">
                                            <el-avatar v-if="item.userName" :size="18">
                                                {{ item.userName.substring(0, 1) }}
                                            </el-avatar>
                                            {{ item.userName }}
                                        </span>
                                        <span class="card-user-runtime">
                                            {{ getDateDiff(item.runTime) }}
                                        </span>
                                    </div>
                                </div>
                            </el-card>
                            <pagination v-show="total > pageDto.pageSize" :total="total" v-model:page="pageDto.page"
                                :pagerCount="5" :layout="layoutSize" v-model:limit="pageDto.pageSize"
                                @pagination="getList" />
                        </div>
                    </el-scrollbar>
                </el-main>
            </el-container>
        </el-aside>
        <el-container>
            <div class="layout-middle" id="fullscreen">
                <el-scrollbar>
                    <div style="min-width:650px;">
                        <el-empty v-if="!approveFormDataConfig || dataList.length === 0"
                            description="这里空空的,请点击左侧代办列表" />
                        <el-tabs v-else v-model="activeName" @tab-click="handleClick" class="content-tabs">
                            <el-tab-pane label="表单信息" name="baseTab">
                                <div v-if="activeName === 'baseTab'">
                                    <ApporveForm :approveFormData="approveFormDataConfig"
                                        @handleRefreshList="refreshList" />
                                </div>
                            </el-tab-pane>
                            <el-tab-pane label="审批记录" name="flowStep">
                                <div v-if="activeName === 'flowStep'">
                                    <FlowStepTable />
                                </div>
                            </el-tab-pane>
                            <el-tab-pane label="流程预览" name="flowReview">
                                <div v-if="activeName === 'flowReview'">
                                    <ReviewWarp />
                                </div>
                            </el-tab-pane>
                        </el-tabs>
                    </div>
                </el-scrollbar>
            </div>
        </el-container>
    </el-container>
</template>

<script setup>
import { ref } from 'vue';
import FlowStepTable from '@/components/Workflow/Preview/flowStepTable.vue';
import ReviewWarp from '@/components/Workflow/Preview/reviewWarp.vue';
import ApporveForm from "./components/approveForm.vue";
import { getPenddinglistPage } from "@/api/workflow/index";
import { getDateDiff } from "@/utils/antflow/hsharpUtils";
const { proxy } = getCurrentInstance();
import { useStore } from '@/store/modules/workflow';
let store = useStore();
let { setPreviewDrawerConfig } = store;
const activeIndex = ref(null);
const activeName = ref('baseTab');
const dataList = ref([]);
const loading = ref(true);
const total = ref(0);
const approveFormDataConfig = ref(null);
const data = reactive({
    form: {},
    pageDto: {
        page: 1,
        pageSize: 10
    },
    taskMgmtVO: {
        processNumber: undefined,
        processTypeName: undefined
    },
    rules: {
        bpmnCode: [{ required: true, message: "关键字不能为空", trigger: "blur" }],
        bpmnName: [{ required: true, message: "关键字不能为空", trigger: "blur" }],
    }
});
const { pageDto, taskMgmtVO } = toRefs(data);

const layoutSize = 'pager';
onMounted(async () => {
    await getList();
});
/** 搜索按钮操作 */
async function handleQuery() {
    pageDto.value.page = 1;
    await getList();
}
/** 查询代办列表 */
async function getList() {
    loading.value = true;
    console.log(pageDto.value, taskMgmtVO.value);
    await getPenddinglistPage(pageDto.value, taskMgmtVO.value).then(response => {
        dataList.value = response.data;
        total.value = response.pagination.totalCount;
        setTimeout(() => {
            loading.value = false;
        }, 300);
    }).catch((r) => {
        loading.value = false;
        console.log(r);
        proxy.$modal.msgError("加载列表失败:" + r.message);
    });
}
/**
 * 刷新列表
*/
const refreshList = async () => {
    await getList();
    toggleFlowActive(dataList.value[0], 0);
}
const toggleFlowActive = (data, index) => {
    if (proxy.isEmpty(data)) {
        return;
    }
    activeIndex.value = index;
    approveFormDataConfig.value = {
        ...approveFormDataConfig.value,
        formCode: data.processKey,
        processNumber: data.processNumber,
        taskId: data.taskId,
        isOutSideAccess: data.isOutSideProcess,
        isLowCodeFlow: data.isLowCodeFlow,
    };
    //console.log("approveFormDataConfig.value====", JSON.stringify(approveFormDataConfig.value));
    setPreviewDrawerConfig({ ...approveFormDataConfig.value });
    activeName.value = 'baseTab';
}

const handleClick = (tab, event) => {
    activeName.value = tab.paneName;
}
window.onload = function () {
    var fullscreen = document.getElementById("fullscreen");
    fullscreen.style.height = (window.innerHeight) + "px";
}
</script>

<style lang="scss" scoped>
* {
    margin: 0;
    padding: 0;
    border: 0;
    outline: 0;
    vertical-align: baseline;
    font-size: 12px;
    line-height: 2.0;
}


.empty-text {
    display: block;
    /* 让span独占一行 */
    text-align: center;
    /* 水平居中 */
    width: 100%;
    /* 占满父容器宽度 */
    color: #888;
    /* 可选，设置字体颜色 */
    margin: 20px 0;
    /* 可选，增加上下间距 */
}

.layout-setup .el-aside {
    width: 300px;
    color: var(--el-text-color-primary);
    background: #cccccc59;
}

.layout-aside .el-header {
    position: relative;
    color: var(--el-text-color-primary);
    border-radius: 5px;
}

.layout-aside .el-footer {
    position: relative;
    background-color: #f2f3f4f5;
    color: var(--el-text-color-primary);
    border-radius: 5px;
    margin-top: 10px;
}

.layout-aside .toolbar {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    right: 20px;
    margin: 0 5px;
}

.layout-middle {
    margin-right: 10px;
    margin-top: 10px;
    width: 100%;
    height: 90vh;
    background-color: #f2f3f4f5;
}

.layout-middle .content-tabs {
    padding: 10px;
    min-width: 680px;
    overflow: auto;
}

.list-flex-cards {
    display: flex;
    flex-wrap: wrap;
    gap: 5px;
    padding-top: 5px;
    padding-left: 5px;
    padding-right: 5px;
}

.item-card {
    width: 290px;
    box-shadow: var(--el-box-shadow-light);
}

.card-content {
    display: flex;
    flex-direction: column;
    border-radius: 20px;
    box-sizing: border-box;
    transition: box-shadow 0.2s;
}

.item-card:hover {
    border: 1px solid var(--current-color);
}

.active {
    border: 1px solid var(--current-color);
}

.card-title {
    font-size: 14px;
    font-weight: 600;
    color: #222;
    margin-bottom: 2px;
    letter-spacing: 1px;
    border-left: 3px solid var(--current-color);
    padding-left: 8px;
    max-width: 200px;
    /* 根据需要调整宽度 */
    white-space: nowrap;
    /* 不换行 */
    overflow: hidden;
    /* 超出隐藏 */
    text-overflow: ellipsis;
    /* 超出显示省略号 */
    margin-left: 4px;
    display: block;
    /* 或 inline-block，根据需要 */
}

.card-detail {
    display: flex;
    align-items: center;
    color: #222;
    font-size: 12px;
}

.card-detail .card-reason {
    color: #222;
    font-weight: 500;
    max-width: 200px;
    /* 根据需要调整宽度 */
    white-space: nowrap;
    /* 不换行 */
    overflow: hidden;
    /* 超出隐藏 */
    text-overflow: ellipsis;
    /* 超出显示省略号 */
    margin-left: 4px;
    display: block;
    /* 或 inline-block，根据需要 */
}

.card-time {
    font-size: 13px;
    color: #222;
    display: flex;
    align-items: center;
    gap: 4px;
    margin-bottom: 0;
}

.card-time-value {
    color: #222;
    font-weight: 500;
    margin-left: 2px;
}

.card-time-value .el-tag {
    margin-right: 8px;
}

.card-user {
    display: flex;
    align-items: center;
    justify-content: space-between;
}

.card-username {
    font-size: 12px;
    font-weight: 500;
    color: #222;
}

.card-user-runtime {
    font-size: 12px;
    font-weight: 500;
    color: #222;
    margin-left: auto;
    /* 添加这一行来将元素推到右边 */
}
</style>
