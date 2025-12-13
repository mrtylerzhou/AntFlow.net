<template>
    <div>
        <el-table v-loading="loading" :data="activityList">
            <!-- <el-table-column label="序号" align="center" prop="id" /> -->
            <el-table-column label="流程环节" align="center" prop="taskName" />
            <el-table-column label="执行人" align="center" prop="verifyUserName">
                <template #default="scope">
                    <el-avatar v-if="scope.row.verifyUserName" size="small">
                        {{ scope.row.verifyUserName.substring(0, 1) }}</el-avatar>
                    {{ scope.row.verifyUserName }}
                </template>
            </el-table-column>
            <el-table-column label="操作" align="center" prop="type">
                <template #default="item">
                    <el-tag style="display: flex;" :type="item.row.type" :size="item.row.size">{{
                        item.row.verifyStatusName }}</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="审批意见" align="center" prop="remark" />
            <el-table-column label="处理时间" align="center" prop="verifyDate" width="180">
                <template #default="scope">
                    <span>{{ parseTime(scope.row.verifyDate) }}</span>
                </template>
            </el-table-column>

        </el-table>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { approveButtonColor } from '@/utils/antflow/const';
import { getBpmVerifyInfoVos } from '@/api/workflow/index';
let activityList = ref(null);
let loading = ref(false);
const { query } = useRoute();
const { formCode, processNumber, isLowCodeFlow } = query;

const param = {
    formCode: formCode,
    processNumber: processNumber,
    isLowCodeFlow: isLowCodeFlow || false,
}
onMounted(async () => {
    loading.value = true;
    await getPreviewData();
})
const getPreviewData = async () => {
    const resData = await getBpmVerifyInfoVos(param);
    loading.value = false;
    if (resData.code == 200) {
        activityList.value = resData.data.map(c => {
            return {
                ...c,
                type: approveButtonColor[c.verifyStatus],
                size: c.verifyStatus == 99 ? 'large' : 'default',
                verifyStatusName: c.verifyStatusName ? c.verifyStatusName : (c.taskName != '流程结束' ? '未处理' : '结束'),
                remark: c.verifyDesc
            }
        })
    };
};

</script>