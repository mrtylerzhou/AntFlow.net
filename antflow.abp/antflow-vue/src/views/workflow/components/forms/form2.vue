<template>
    <div class="app-container">
        <el-row :gutter="20">
            <el-col :span="hasChooseApprove == 'true' ? 16 : 24">
                <div class="form-container"
                    :style="hasChooseApprove == 'true' ? {} : { maxWidth: '80vw', margin: '0 auto' }">
                    <div class="el-main">
                        <el-form ref="ruleFormRef" :model="form" :rules="rules"
                            style="max-width: 600px;min-height: 100px; margin: auto;">
                            <el-row :class="{ disableClss: props.isPreview }">
                                <el-col :span="24">
                                    <el-form-item label="请假类型" prop="leaveType">
                                        <el-select v-model="form.leaveType" placeholder="请选择请假类型"
                                            :style="{ width: '220px' }">
                                            <el-option v-for="(item, index) in leaveTypeOptions" :key="index"
                                                :label="item.label" :value="item.value"></el-option>
                                        </el-select>
                                    </el-form-item>
                                </el-col>
                                <el-col :span="24">
                                    <el-form-item label="开始时间" prop="beginDate">
                                        <el-date-picker :disabled-date="disabledBeginDateDate" v-model="form.beginDate"
                                            type="datetime" placeholder="请选择开始时间" format="YYYY/MM/DD HH:mm" />
                                    </el-form-item>
                                </el-col>
                                <el-col :span="24">
                                    <el-form-item label="结束时间" prop="endDate">
                                        <el-date-picker :disabled-date="disabledEndDate" v-model="form.endDate"
                                            type="datetime" placeholder="请选择结束时间" format="YYYY/MM/DD HH:mm" />
                                    </el-form-item>
                                </el-col>
                                <el-col :span="24">
                                    <el-form-item label="请假时长" prop="leaveHour">
                                        <el-input-number v-model="form.leaveHour" style="width: 220px;" :min="1"
                                            :max="100" placeholder="请输入时长" />
                                    </el-form-item>
                                </el-col>
                                <el-col :span="24">
                                    <el-form-item label="请假说明" prop="remark">
                                        <el-input v-model="form.remark" type="textarea" placeholder="请填写请假理由"
                                            :maxlength="100" show-word-limit :autosize="{ minRows: 4, maxRows: 4 }"
                                            :style="{ width: '100%' }"></el-input>
                                    </el-form-item>
                                </el-col>
                            </el-row>
                        </el-form>
                    </div>
                    <div class="el-footer" v-if="!props.isPreview && props.showSubmit">
                        <el-button type="primary" @click="handleSubmit">提交</el-button>
                    </div>
                </div>
            </el-col>
            <el-col :span="8" v-if="hasChooseApprove == 'true'">
                <TagApproveSelect v-model:formCode="formCode" @chooseApprove="chooseApprovers" />
            </el-col>
        </el-row>
    </div>
</template>

<script setup>
import { ref, reactive, getCurrentInstance } from 'vue';
import TagApproveSelect from "@/components/BizSelects/TagApproveSelect/index.vue";
const { proxy } = getCurrentInstance();
const route = useRoute();
const formCode = route.query?.formCode ?? '';
const hasChooseApprove = route.query?.hasChooseApprove ?? 'false';
/**传参不需要修改*/
let props = defineProps({
    previewData: {
        type: Object,
        default: () => ({}),
    },
    showSubmit: {//是否显示提交按钮
        type: Boolean,
        default: false,
    },
    isPreview: {
        type: Boolean,
        default: true,
    }
});
const ruleFormRef = ref(null)
let leaveTypeOptions = [{
    "label": "年假",
    "value": 1
}, {
    "label": "事假",
    "value": 2
}, {
    "label": "婚嫁",
    "value": 3
}, {
    "label": "病假",
    "value": 4
}];
/**定义表单字段和预览，根据实际业务表单修改*/

const { leaveType, beginDate, endDate, leaveHour, userId, userName, remark } = props.previewData;
const form = reactive({ leaveType, beginDate, endDate, leaveHour, userId, userName, remark });
/**表单字段验证，根据实际业务表单修改*/
let rules = {
    beginDate: [{
        required: true,
        message: '请选择开始时间',
        trigger: 'blur'
    }],
    endDate: [{
        required: true,
        message: '请选择结束时间',
        trigger: 'blur'
    }],
    leaveHour: [{
        required: true,
        message: '请填写请假时长',
        trigger: 'blur'
    }],
    remark: [{
        required: true,
        message: '请填写请假理由',
        trigger: 'blur'
    }],
    leaveType: [{
        required: true,
        message: '请选择请假类型',
        trigger: 'change'
    }],
};
const disabledBeginDateDate = (time) => {
    return time.getTime() > new Date(form.endDate);
}
const disabledEndDate = (time) => {
    return time.getTime() < new Date(form.beginDate);
}
/**以下是通用方法不需要修改 views/startFlow/index.vue中调用*/
/**自选审批人 */
const chooseApprovers = (data) => {
    form.approversList = data.approvers;
    form.approversValid = data.nodeVaild;
}

const getFromData = () => {
    return new Promise((resolve, reject) => {
        try {
            resolve(JSON.stringify(form));
        } catch (error) {
            reject(error);
        }
    });
}
const handleSubmit = () => {
    handleValidate().then((isValid) => {
        if (isValid) {
            proxy.$emit("handleBizBtn", JSON.stringify(form))
        }
    });
}

const handleValidate = () => {
    return proxy.$refs['ruleFormRef'].validate((valid) => {
        if (!valid) {
            return Promise.reject(false);
        }
        else if (hasChooseApprove == 'true') {
            if (!form.approversValid || form.approversValid == false) {
                proxy.$modal.msgError('请选择自选审批人');
                return Promise.reject(false);
            }
        }
        else {
            return Promise.resolve(true);
        }
    });
}
defineExpose({
    handleValidate,
    getFromData
})
</script>
<style scoped lang="scss">
.disableClss {
    pointer-events: none;
}

.form-container {
    /* 新增父级定位 */
    display: flex;
    flex-direction: column;
    margin: auto;
    background: #eee !important;
}

.form-container .el-main {
    background-color: #fff;
    flex: 1 1 auto;
}

.form-container .el-footer {
    background-color: #fff;
    border-top: 2px solid #f0f0f0;
    box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
    display: flex;
    justify-content: flex-end;
    align-items: center;
    padding-right: 24px;
}
</style>