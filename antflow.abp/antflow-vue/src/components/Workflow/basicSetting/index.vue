<template>
    <div class="form-container">
        <el-form ref="ruleFormRef" :model="form" :rules="rules" label-width="auto"
            style="max-width: 600px;margin: auto;">
            <!-- <el-form-item label="流程分组" prop="bpmnType">
                    <el-select v-model="form.bpmnType" placeholder="请选择分组" :style="{ width: '100%' }">
                        <el-option v-for="(item, index) in bpmnTypeOptions" :key="index" :label="item.label"
                            :value="item.value"></el-option>
                    </el-select>
                </el-form-item> -->

            <!--  <el-form-item label="模板类型" prop="formCode">
                <el-select filterable v-model="form.formCode" placeholder="请选择模板类型" :style="{ width: '100%' }">
                    <el-option v-for="(item, index) in formCodeOptions" :key="index" :label="item.value" :value="item.key">
                        <span style="float: left">【{{ item.key }}】 {{ item.value }}</span> 
                    </el-option>
                </el-select>
            </el-form-item> -->


            <el-form-item v-if="!copyOpt" label="类型标识" prop="formCode">
                <el-input v-model="form.formCode" :disabled="true" :style="{ width: '100%' }" />
            </el-form-item>

            <el-form-item v-else label="类型标识" prop="formCode">
                <el-select filterable v-model="form.formCode" placeholder="请选类型标识" :style="{ width: '100%' }">
                    <el-option v-for="(item, index) in formCodeOptions" :key="index" :label="item.value"
                        :value="item.key">
                        <span style="float: left">【{{ item.key }}】 {{ item.value }}</span>
                    </el-option>
                </el-select>
            </el-form-item>

            <el-form-item v-if="!copyOpt" label="流程名称" prop="bpmnName">
                <el-input v-model="form.bpmnName" :disabled="true" :style="{ width: '100%' }" readonly />
            </el-form-item>

            <el-form-item v-else label="流程名称" prop="bpmnName">
                <template #label>
                    <span>
                        <el-tooltip content="同【模板类型】名称一致，不需手动输入" placement="top">
                            <el-icon><question-filled /></el-icon>
                        </el-tooltip>
                        流程名称
                    </span>
                </template>
                <el-input v-model="form.bpmnName" placeholder="请输入审批名称" :style="{ width: '100%' }" readonly />
            </el-form-item>

            <el-form-item label=" 审批人去重" prop="deduplicationType">
                <el-select v-model="form.deduplicationType" placeholder="请选择去重类型" :style="{ width: '100%' }">
                    <el-option v-for="(item, index) in duplicateOptions" :key="index" :label="item.label"
                        :value="item.value"></el-option>
                </el-select>
            </el-form-item>

            <!-- <el-form-item label="启用流程" prop="effectiveStatus">
                <el-switch v-model="form.effectiveStatus" />
            </el-form-item> -->
            <el-form-item label="发起人权限" prop="viewPageStart">
                <el-checkbox-group v-model="form.viewPageButtons.viewPageStart">
                    <el-checkbox v-for="item in viewPageButtons" :key="item.value" :label="item.label"
                        :value="item.value">
                        {{ item.label }}
                    </el-checkbox>
                </el-checkbox-group>
            </el-form-item>
            <el-form-item label=" 流程说明" prop="remark">
                <el-input v-model="form.remark" type="textarea" placeholder="请输入流程说明" :maxlength="100" show-word-limit
                    :autosize="{ minRows: 4, maxRows: 4 }" :style="{ width: '100%' }"></el-input>
            </el-form-item>
        </el-form>
    </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch, getCurrentInstance } from 'vue'
import { NodeUtils } from '@/utils/antflow/nodeUtils'
import { getDIYFromCodeData } from "@/api/workflow/index";
import { getLowCodeFlowFormCodes } from "@/api/workflow/lowcodeApi";
const { query } = useRoute();
const { proxy } = getCurrentInstance()
const emit = defineEmits(['nextChange'])
let loading = ref(false);
const copyOpt = query?.copy ?? 0 > 0 ? true : false;
let props = defineProps({
    flowType: {
        type: String,
        default: () => (''),
    },
    basicData: {
        type: Object,
        default: () => (null),
    }
});

const generatorID = "PROJECT_" + NodeUtils.idGenerator();
const ruleFormRef = ref(null);
const duplicateOptions = [{
    "label": "不去重",
    "value": 1
}, {
    "label": "前去重",
    "value": 2
}, {
    "label": "后去重",
    "value": 3
}];

const viewPageButtons = [{
    "label": "撤回",
    "value": 29
}, {
    "label": "作废",
    "value": 7
}];

let formCodeOptions = ref([]);
const form = reactive({
    bpmnName: '',
    bpmnCode: generatorID,
    bpmnType: 1,
    formCode: '',
    remark: '',
    effectiveStatus: false,
    deduplicationType: 1,
    viewPageButtons: {
        viewPageStart: [],
        viewPageOther: [],
    }
})
// 复制操作 监听formCode的变化
watch(() => form.formCode, (val) => {
    if (val) {
        formCodeOptions.value.forEach(item => {
            if (item.key == val) {
                form.bpmnName = item.value;
            }
        })
    }
});
onMounted(async () => {
    if (!proxy.isEmpty(props.basicData) && !proxy.isEmpty(props.basicData.formCode)) {
        form.bpmnName = props.basicData.bpmnName;
        form.bpmnCode = props.basicData.bpmnCode;
        form.formCode = props.basicData.formCode;
        form.remark = props.basicData.remark;
        form.deduplicationType = props.basicData.deduplicationType;
        form.viewPageButtons = props.basicData.viewPageButtons;
    }
    else {
        form.bpmnCode = generatorID;
        form.formCode = query.fc;
        form.bpmnName = decodeURIComponent(query.fcname ?? '');
    }
    if (props.flowType == 'DIY') {
        getDIYFromCodeList();
    } else if (props.flowType == 'LF') {
        getLFFromCodeList();
    }
});

/**获取全部DIY FromCode */
const getDIYFromCodeList = async () => {
    loading.value = true;
    await getDIYFromCodeData().then((res) => {
        loading.value = false;
        if (res.code == 200) {
            formCodeOptions.value = res.data;
        }
    });
}
/**获取全部LF FromCode */
const getLFFromCodeList = async () => {
    loading.value = true;
    await getLowCodeFlowFormCodes().then((res) => {
        loading.value = false;
        if (res.code == 200) {
            formCodeOptions.value = res.data;
        }
    });
}

const rules = {
    formCode: [{
        required: true,
        message: '请选择分类',
        trigger: 'blur'
    }],
    bpmnName: [{
        required: true,
        message: '请输入流程名称',
        trigger: 'change'
    }],
    bpmnCode: [{
        required: true,
        message: '请输入流程编号',
        trigger: 'blur'
    }],
};

// 给父级页面提供得获取本页数据得方法
const getData = () => {
    return new Promise((resolve, reject) => {
        proxy.$refs['ruleFormRef'].validate((valid, fields) => {
            if (!valid) {
                emit('nextChange', { label: "基础设置", key: "basicSetting" })
                reject({ valid: false });
            }
            form.effectiveStatus = form.effectiveStatus ? 1 : 0;
            resolve({ formData: form })  // TODO 提交表单
        })
    })
};
defineExpose({
    getData
})
</script>
<style scoped>
.form-container {
    background: white !important;
    padding: 10px;
    max-width: 750px;
    min-height: 80vh;
    left: 0;
    bottom: 0;
    right: 0;
    margin: auto;
}
</style>