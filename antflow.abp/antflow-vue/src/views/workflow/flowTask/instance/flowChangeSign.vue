<template>
    <div>
        <common ref="commonRef" @clickNodeOpt="handleClickNode" :currentOptId="11" :afterOptId="26">
            <template #userChoose>
                <el-empty v-if="checkedUserList.length === 0" description="请点击左侧审批人节点" />
                <div v-else>
                    <el-form :inline="true">
                        <el-form-item label="当前操作节点名称">
                            <el-input v-model="optFrom.nodeName" disabled style="width: 200px" />
                        </el-form-item>
                    </el-form>
                    <el-table v-loading="loading" :data="checkedUserList" class="mb10"
                        style="height: 400px; width: 97%">
                        <el-table-column prop="id" label="审批人ID" width="180" />
                        <el-table-column prop="name" label="审批人姓名" width="180" />
                        <el-table-column label="操作" fixed="right" align="left" class-name="small-padding fixed-width">
                            <template #default="scope">
                                <el-button link type="success" icon="Switch" :disabled="scope.row.canChange === false"
                                    @click="addApproveUser(scope.row)">选择人员</el-button>
                            </template>
                        </el-table-column>
                    </el-table>
                    <el-button @click="handleCancel">返回</el-button>
                    <el-button type="warning" @click="handleReset">重置操作</el-button>
                    <el-button type="primary" @click="handleSubmit" :disabled="isCanSubmit">提交修改</el-button>
                </div>
            </template>
        </common>
        <select-user-dialog v-model:visible="approverUserVisible" @change="sureUserApprover" />
    </div>
</template>

<script setup>
import { ref, watch, useTemplateRef } from 'vue';
import common from "./components/common.vue"
import selectUserDialog from '@/components/Workflow/dialog/selectUserDialog.vue';
const { proxy } = getCurrentInstance();
const commonRef = useTemplateRef("commonRef");
let loading = ref(false);
let approverUserVisible = ref(false);
let nodeUserList = ref([]);
let checkedUserList = ref([]);
let changeUserId = ref(null);
let optFrom = ref(null)
let isCanSubmit = ref(true);

watch(() => optFrom.value, (newVal) => {
    if (newVal) {
        isCanSubmit.value = newVal.userInfos?.length == 0;
    }
}, { deep: true });
watch(() => nodeUserList.value, (newVal) => {
    if (newVal) {
        checkedUserList.value = [...newVal];
    }
});
/**点击流程图节点回调*/
const handleClickNode = (data, nodeUsers) => {
    loading.value = true;
    optFrom.value = data.value;
    checkedUserList.value = nodeUsers.map(item => {
        return {
            ...item,
            canChange: item.isDeduplication !== 1,
        }
    });
    setTimeout(() => {
        loading.value = false;
    }, 300);
}

/**选择审批人确认按钮 */
const sureUserApprover = (data) => {
    if (proxy.isEmptyArray(data)) {
        return;
    }
    if (data.length > 1) {
        proxy.$modal.msgError("每次最多添加一个审批人");
        return;
    }
    if (checkedUserList.value.some((c) => c.id == data[0].targetId)) {
        proxy.$modal.msgError("用户已被选中");
    } else {
        const checkedList = data.map(item => {
            return {
                id: item.targetId,
                name: item.name
            }
        })

        const idx = checkedUserList.value.findIndex(item => item.id == changeUserId.value)
        if (idx !== -1) {
            checkedUserList.value.splice(idx, 1, ...checkedList)
        }
        optFrom.value.userInfos = [...checkedUserList.value.map(item => {
            return {
                id: item.id,
                name: item.name
            }
        })];
    }
    approverUserVisible.value = false;
}

const addApproveUser = (row) => {
    changeUserId.value = row.id;
    approverUserVisible.value = true;
}
const handleSubmit = () => {
    commonRef.value.handleSubmit(optFrom.value);
}
const handleCancel = () => {
    commonRef.value.handleCancel();
}
const handleReset = () => {
    commonRef.value.loadNodeOperationUserList();
    //loading.value = true;
    //optFrom.value = { ...commonRef.value.optFrom, userInfos: [] };
    //nodeUserList.value = [...commonRef.value.originalNodeUserList];
    // setTimeout(() => {
    //     loading.value = false;
    // }, 300);
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