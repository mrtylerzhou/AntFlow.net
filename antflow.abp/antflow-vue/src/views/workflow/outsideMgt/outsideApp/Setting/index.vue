<template>
    <div class="app-container">
        <div class="query-box">
            <el-row :gutter="10" class="mb8">
                <el-col :span="1.5">
                    <el-button type="warning" icon="Setting" @click="approveTempVisible = true">设置审批人</el-button>
                </el-col>

                <el-col :span="1.5">
                    <el-button type="primary" icon="Setting" @click="callbackConfVisible = true">设置流程回调</el-button>
                </el-col>
                <el-col :span="1.5" class="fr">
                    <el-button type="primary" plain @click="backPage()"><el-icon>
                            <Back />
                        </el-icon>返回 </el-button>
                </el-col>
            </el-row>
        </div>
        <div class="table-box">
            <el-tabs v-model="activeName" class="card-box" @tab-click="handleClickTab">
                <el-tab-pane label="审批人列表" name="approveSet">
                    <el-table v-loading="loading" :data="approveList">
                        <el-table-column label="应用标识" align="center">
                            {{ query.fc }}
                        </el-table-column>
                        <el-table-column label="模板类型" align="center" prop="approveTypeName" />
                        <el-table-column label="模板URL" align="center" prop="apiUrl" />
                        <el-table-column label="创建时间" align="center" prop="createTime">
                            <template #default="scope">
                                <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="220" align="center" class-name="small-padding fixed-width">
                            <template #default="scope">
                                <el-button link type="primary" icon="Delete"
                                    @click="handleDelete(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-tab-pane>

                <el-tab-pane label="流程回调列表" name="callbackSet">
                    <el-table v-loading="loading" :data="callbackList">
                        <el-table-column label="应用标识" align="center" prop="formCode" />
                        <el-table-column label="回调URL" align="center" prop="bpmFlowCallbackUrl" />
                        <el-table-column label="创建时间" align="center" prop="createTime">
                            <template #default="scope">
                                <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" width="220" align="center" class-name="small-padding fixed-width">
                            <template #default="scope">
                                <el-button link type="primary" icon="Delete"
                                    @click="handleDelete(scope.row)">删除</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-tab-pane>
            </el-tabs>
        </div>
        <approveForm v-model:visible="approveTempVisible" @changeRefresh="handleRefresh"
            v-model:bizformData="bizAppForm" />
        <callbackConf v-model:visible="callbackConfVisible" @changeRefresh="handleRefresh"
            v-model:bizformData="bizAppForm" />
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { getApproveTemplatelist, getCallbackUrlConfList } from "@/api/workflow/outsideApi";
import approveForm from "../approve/form.vue";
import callbackConf from "../callbackConf/form.vue";
const { proxy } = getCurrentInstance();
const { query } = useRoute();
const loading = ref(false);
const approveList = ref([]);
const callbackList = ref([]);
const activeName = ref('approveSet');
const approveTempVisible = ref(false);
const callbackConfVisible = ref(false);
let bizAppForm = ref({});
/** 加载审批人配置列表 */
function getApproveList() {
    loading.value = true;
    getApproveTemplatelist(query.appId).then(response => {
        approveList.value = response.data;
        loading.value = false;
    }).catch(() => {
        loading.value = false;
        proxy.$modal.msgError("加载审批人配置列表失败:" + err.message);
    });
}
/** 加载流程回调配置列表 */
function getCallbackList() {
    loading.value = true;
    getCallbackUrlConfList(query.fc).then(response => {
        callbackList.value = response.data;
        loading.value = false;
    }).catch(() => {
        loading.value = false;
        proxy.$modal.msgError("加载流程回调配置列表失败:" + err.message);
    });
}
onMounted(async () => {
    bizAppForm.value = {
        applicationId: query.appId,
        applicationName: decodeURIComponent(query.appName),
        formCode: query.fc,
        businessPartyId: query.pId,
        businessPartyName: decodeURIComponent(query.pName)
    };
    getApproveList();
})

/** 删除按钮操作 */
function handleDelete(row) {
    proxy.$modal.msgError("演示环境不允许删除操作！");
}

/**  返回按钮操作 */
const backPage = () => {
    const obj = { path: "outsideApp" };
    proxy.$tab.closeOpenPage(obj);
}
function handleClickTab(tab, event) {
    activeName.value = tab.paneName;
    if (tab.paneName == 'approveSet') {
        getApproveList();
    } else {
        getCallbackList();
    }
}
/**刷新list */
const handleRefresh = (data) => {
    handleClickTab(data);
}
</script>
<style lang="scss" scoped></style>