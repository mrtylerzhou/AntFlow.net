<template>
    <div class="app-container">
        <div class="card-box" style="padding-top: 10px;background-color: #fff;">
            <div class="flex gap-2">
                <el-tag type="primary">{{ formCode }}</el-tag>
                <el-tag type="success" style="margin-left: 5px;">{{ processNumber }}</el-tag>
            </div>
            <el-tabs v-model="activeName" class="set-tabs">
                <el-tab-pane label="表单信息" name="baseTab">
                    <div v-if="activeName === 'baseTab'">
                        <ApporveForm v-if="loadComplete" :approveFormData="approveFormDataConfig" />
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
    </div>
</template>

<script setup>
import { nextTick, ref } from 'vue';
import FlowStepTable from '@/components/Workflow/Preview/flowStepTable.vue';
import ReviewWarp from '@/components/Workflow/Preview/reviewWarp.vue';
import ApporveForm from "./components/approveForm.vue";
const { query } = useRoute();
import { useStore } from '@/store/modules/workflow';
let store = useStore();
let { setPreviewDrawerConfig } = store;
const formCode = query?.formCode;
const processNumber = query?.processNumber;
const activeName = ref('baseTab');
let approveFormDataConfig = ref(null);
let loadComplete = ref(false);
nextTick(() => {
    const queryData = {
        formCode: formCode,
        processNumber: processNumber,
        taskId: query?.taskId,
        isOutSideAccess: query?.isOutSideAccess,
        isLowCodeFlow: query?.isLowCodeFlow,
    };
    approveFormDataConfig.value = { ...queryData };
    setPreviewDrawerConfig({ ...queryData });
    loadComplete.value = true;
}) 
</script>
<style lang="scss" scoped></style>