<template>
    <div v-if="activityList">
        <div>
            <div style="margin-bottom: 25px;display: flex; justify-content: center;align-items: Center;">
                <p style="margin-left: 10px;"><span class="dotPrimary"></span> 通过</p>
                <p style="margin-left: 10px;"><span class="dotDanger"></span> 拒绝</p>
                <p style="margin-left: 10px;"><span class="dotSuccess"></span> 当前节点</p>
                <p style="margin-left: 10px;"><span class="dotInfo"></span> 未处理</p>
                <p style="margin-left: 10px;"><span class="dotInfo"></span> 结束</p>
            </div>
            <el-timeline>
                <el-timeline-item v-for="(activity, index) in activityList" :key="index" :type="activity.type"
                    :size="activity.size">
                    <el-collapse>
                        <el-collapse-item :title="activity.taskName">
                            <el-card>
                                <p v-if="activity.verifyUserName">审批人: {{ activity.verifyUserName }}</p>
                                <p v-if="activity.verifyStatusName">审批结果: {{ activity.verifyStatusName }}</p>
                                <p v-if="activity.verifyDesc">审批备注: {{ activity.verifyDesc }}</p>
                                <p v-if="activity.verifyDate">操作时间: {{ activity.verifyDate }}</p>
                            </el-card>
                        </el-collapse-item>
                    </el-collapse>
                </el-timeline-item>
            </el-timeline>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { approveButtonColor } from '@/utils/antflow/const';
import { getBpmVerifyInfoVos } from '@/api/workflow/index';

let activityList = ref(null);

onMounted(async () => {
    await getPreviewData();
})
const getPreviewData = async () => {
    let param = {
        "processNumber": "DSFZH_WMA_9",
    }
    let resData = await getBpmVerifyInfoVos(param);
    if (resData.code == 200) {
        activityList.value = resData.data.map(c => {
            return {
                ...c,
                type: approveButtonColor[c.verifyStatus],
                size: c.verifyStatus == 99 ? 'large' : 'normal',
                remark: c.verifyStatus == 0 ? '流程结束' : c.verifyDesc
            }
        })
    };
};

</script>
<style lang="scss">
.el-timeline {
    --el-timeline-node-size-normal: 25px !important;
    --el-timeline-node-size-large: 25px !important;
}

.el-timeline-item {
    padding-bottom: 0px !important;
}

.el-timeline-item__node--normal {
    left: -8px !important;
}

.el-timeline-item__node--large {
    left: -8px !important;
}

.el-timeline-item__wrapper {
    top: -10px !important;
}

.el-tabs--border-card {
    min-width: 800px !important;
    min-height: 550px !important;
}

.dotPrimary {
    height: 15px;
    width: 15px;
    background-color: #46A6FE;
    border-radius: 50%;
    display: inline-block;
}

.dotDanger {
    height: 15px;
    width: 15px;
    background-color: #ff4949;
    border-radius: 50%;
    display: inline-block;
}

.dotSuccess {
    height: 15px;
    width: 15px;
    background-color: #13ce66;
    border-radius: 50%;
    display: inline-block;
}

.dotInfo {
    height: 15px;
    width: 15px;
    background-color: #bbb;
    border-radius: 50%;
    display: inline-block;
}
</style>