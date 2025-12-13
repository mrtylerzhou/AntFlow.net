<template>
    <div class="app-container">
        <el-scrollbar height="83vh">
            <el-row>
                <el-col :span="24">
                    <div class="mb10">
                        <el-descriptions title="流程信息" :column="3" border>
                            <el-descriptions-item label="流程类型">
                                {{ queryForm.processCode }}
                            </el-descriptions-item>
                            <el-descriptions-item label="流程名称">
                                {{ queryForm.processTypeName }}
                            </el-descriptions-item>
                            <el-descriptions-item label="流程编号">
                                {{ queryForm.processNumber }}
                            </el-descriptions-item>
                            <el-descriptions-item label="流程描述">
                                {{ queryForm.description }}
                            </el-descriptions-item>
                            <el-descriptions-item label="发起人">
                                {{ queryForm.actualName }}
                            </el-descriptions-item>
                            <el-descriptions-item label="发起时间">
                                {{ queryForm.createTime }}
                            </el-descriptions-item>
                            <!-- <el-descriptions-item label="流程状态" :span="2">
                                <el-tag type="success"> {{ queryForm.taskState }}</el-tag>
                            </el-descriptions-item> -->
                        </el-descriptions>
                    </div>
                </el-col>
            </el-row>
            <el-row :gutter="20" class="mb10">
                <el-col :span="10">
                    <div>
                        <ReviewWarp />
                    </div>
                </el-col>
                <el-col :span="14" style="background-color: #f5f5f5;">
                    <div class="p20">
                        <el-alert title="加/减/变更都是针对当前正在审批的节点,每次都能只处理一个人.不允许多个人同时操作." type="warning" show-icon
                            :closable="false" />
                        <el-empty v-if="checkedUserList.length === 0" description="请点击左侧审批人节点" />
                        <div v-else>
                            <el-form :inline="true">
                                <el-form-item label="节点名称">
                                    <el-input v-model="optNodeName" disabled style="width: 200px" />
                                </el-form-item>
                                <el-form-item>
                                    <el-button type="success" icon="CirclePlus"
                                        @click="addApproveUser">新增审批人</el-button>
                                </el-form-item>
                            </el-form>
                            <el-table v-loading="loading" :data="checkedUserList" class="mb10"
                                style="height: 400px; width: 97%">
                                <el-table-column prop="id" label="审批人ID" width="180" />
                                <el-table-column prop="name" label="审批人姓名" width="180" />
                                <el-table-column label="操作" fixed="right" align="left"
                                    class-name="small-padding fixed-width">
                                    <template #default="scope">
                                        <el-button link type="danger" icon="Delete"
                                            :disabled="scope.row.canDelete === false"
                                            @click="handleDeleteUser(scope.row)">删除</el-button>
                                    </template>
                                </el-table-column>
                            </el-table>
                            <el-button @click="handleCancel">返回</el-button>
                            <el-button type="warning" @click="handleReset">重置操作</el-button>
                            <el-button type="primary" @click="handleSubmit">提交修改</el-button>
                        </div>

                    </div>
                </el-col>
            </el-row>
        </el-scrollbar>
        <select-user-dialog v-model:visible="approverUserVisible" @change="sureUserApprover" />
        <label class="page-close-box" @click="handleCancel"><img src="@/assets/images/antflow/back-close.png"></label>
    </div>
</template>

<script setup>
import { provide, onBeforeMount } from 'vue';
import ReviewWarp from "@/components/Workflow/Preview/reviewWarp.vue"
import selectUserDialog from '@/components/Workflow/dialog/selectUserDialog.vue';
import { useStore } from '@/store/modules/workflow';
import { processOperation } from '@/api/workflow/index';
const { proxy } = getCurrentInstance();
const router = useRouter();
const route = useRoute()
const queryForm = route.query;
let store = useStore()
let { setPreviewDrawerConfig } = store
let loading = ref(false);
let optNodeName = ref(null);
let optFrom = ref({
    operationType: 25,
    formCode: queryForm.processKey,
    processNumber: queryForm.processNumber,
    taskDefKey: queryForm.taskName,
    nodeId: null,
    userInfos: []
});

let isChangedCount = 0;
let approverUserVisible = ref(false);
let checkedUserList = ref([]);

onBeforeMount(() => {
    setPreviewDrawerConfig({
        formCode: queryForm.processKey,
        processNumber: queryForm.processNumber,
        isOutSideAccess: queryForm.isOutSideProcess,
        isLowCodeFlow: queryForm.isLowCodeFlow,
        processState: queryForm.processState,
    })
})
/**选择审批人确认按钮 */
const sureUserApprover = (data) => {
    if (proxy.isEmptyArray(data)) {
        return;
    }
    if (data.length > 1) {
        proxy.$modal.msgError("每次最多添加一个审批人");
        return;
    }
    if (checkedUserList.value.length > isChangedCount) {
        proxy.$modal.msgError("每次最多添加一个审批人");
        return;
    }
    if (checkedUserList.value.some((c) => c.id == data[0].targetId)) {
        proxy.$modal.msgError("用户已被选中");
    } else {
        const checkedList = data.map(item => {
            return {
                id: item.targetId,
                name: item.name,
                canDelete: true
            }
        })
        checkedUserList.value.push(...checkedList)
        optFrom.value.userInfos = checkedList
    }
    approverUserVisible.value = false;
}
const clickNode = (data) => {
    if (data.beforeNodeIds.includes(data.nodeId) || data.params.isNodeDeduplication == 1) {
        proxy.$modal.msgWarning("该节点不能操作，请选择其他节点")
        return;
    }
    loading.value = true;
    optNodeName.value = data.nodeName;
    optFrom.value.nodeId = data.Id;
    optFrom.value.operationType = data.currentNodeId == data.nodeId ? 25 : 28; //当前节点加签 未来节点加签 
    isChangedCount = data.params?.assigneeList.length || 0;
    checkedUserList.value = data.params?.assigneeList
        .map(item => {
            return {
                id: item.assignee,
                name: item.assigneeName,
                canDelete: false
            }
        }) || [];
    //.filter((c) => c.isDeduplication == 0)
    setTimeout(() => {
        loading.value = false;
    }, 300);
}
provide("onClickNode", clickNode)
const handleDeleteUser = (row) => {
    checkedUserList.value = checkedUserList.value.filter(item => item.id != row.id)
    optFrom.value.userInfos = optFrom.value.userInfos.filter(item => item.id != row.id)
}

const addApproveUser = () => {
    approverUserVisible.value = true;
}

const handleCancel = () => {
    const obj = { path: "/workflow/instance/removeSign" };
    proxy.$tab.closeOpenPage(obj);
    router.push({
        path: "/workflow/instance",
    });
}

const handleReset = () => {
    location.reload()
}


const handleSubmit = async () => {
    proxy.$modal.loading();
    await processOperation(optFrom.value).then((res) => {
        if (res.code == 200) {
            proxy.$modal.msgSuccess("操作成功");
            //close();
        } else {
            proxy.$modal.msgError("操作失败:" + res.errMsg);
        }
    });
    proxy.$modal.closeLoading();
}
</script>
<style lang="scss" scoped>
.empty-text {
    display: block;
    text-align: center;
    width: 100%;
    color: #888;
    margin: 20px 0;
}
</style>