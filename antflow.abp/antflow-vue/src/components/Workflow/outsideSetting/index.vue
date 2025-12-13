<template>
  <div class="form-container">
    <el-form ref="ruleFormRef" :model="form" label-width="auto" style="max-width: 600px; margin: auto">
      <el-form-item label="租户标识" prop="businessPartyMark">
        <el-input v-model="form.businessPartyMark" :disabled="true" :style="{ width: '100%' }" />
      </el-form-item>
      <el-form-item label="租户名称" prop="businessPartyName">
        <el-input v-model="form.businessPartyName" :disabled="true" :style="{ width: '100%' }" />
      </el-form-item>
      <el-form-item label="应用标识" prop="formCode">
        <template #label>
          <span>
            <el-tooltip content="注：Saas租户管理中应用标识" placement="top">
              <el-icon><question-filled /></el-icon>
            </el-tooltip>
            应用标识
          </span>
        </template>
        <el-input v-model="form.formCode" :disabled="true" :style="{ width: '100%' }" />
      </el-form-item>
      <el-form-item label="应用名称" prop="bpmnName">
        <template #label>
          <span>
            <el-tooltip content="注：Saas租户管理中应用名称" placement="top">
              <el-icon><question-filled /></el-icon>
            </el-tooltip>
            应用名称
          </span>
        </template>
        <el-input v-model="form.bpmnName" :disabled="true" :style="{ width: '100%' }" />
      </el-form-item>
      <el-form-item label="审批人去重" prop="deduplicationType">
        <el-select v-model="form.deduplicationType" placeholder="请选择去重类型" :style="{ width: '100%' }">
          <el-option v-for="(item, index) in duplicateOptions" :key="index" :label="item.label"
            :value="item.value"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="流程说明" prop="remark">
        <el-input v-model="form.remark" type="textarea" placeholder="请输入流程说明" :maxlength="100" show-word-limit
          :autosize="{ minRows: 4, maxRows: 4 }" :style="{ width: '100%' }"></el-input>
      </el-form-item>

    </el-form>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, getCurrentInstance } from "vue";
import { getApproveTemplatelist } from "@/api/workflow/outsideApi";
import { NodeUtils } from "@/utils/antflow/nodeUtils";
import { useStore } from "@/store/modules/outsideflow";
let store = useStore();
const { query } = useRoute();
const { proxy } = getCurrentInstance();
const emit = defineEmits(["nextChange"]);
let props = defineProps({
  basicData: {
    type: Object,
    default: () => null,
  },
});

const generatorID = "PROJECT_" + NodeUtils.idGenerator();
const ruleFormRef = ref(null);
let duplicateOptions = [
  {
    label: "不去重",
    value: 1,
  },
  {
    label: "前去重",
    value: 2,
  },
  {
    label: "后去重",
    value: 3,
  },
];
let form = reactive({
  bpmnCode: generatorID,
  bpmnName: undefined,
  businessPartyId: undefined,
  appId: undefined,
  isOutSideProcess: 1,
  bpmnType: 1,
  formCode: undefined,
  remark: undefined,
  effectiveStatus: false,
  deduplicationType: 1,
});
onMounted(async () => {
  if (!proxy.isEmpty(props.basicData) && !proxy.isEmpty(props.basicData.formCode)) {
    form.bpmnName = props.basicData.bpmnName;
    form.bpmnCode = props.basicData.bpmnCode;
    form.formCode = props.basicData.formCode;
    form.businessPartyId = props.basicData.businessPartyId;
    form.appId = props.basicData.appId;
    form.businessPartyName = props.basicData.businessPartyName;
    form.businessPartyMark = props.basicData.businessPartyMark;
    form.remark = props.basicData.remark;
    form.deduplicationType = props.basicData.deduplicationType;
  }
  else {
    form.bpmnCode = generatorID;
    form.appId = query.appid;
    form.formCode = query.fc;
    form.bpmnName = decodeURIComponent(query.fcname);
    form.businessPartyId = query.bizid;
    form.businessPartyName = decodeURIComponent(query.bizname);
    form.businessPartyMark = query.bizcode;
    form.deduplicationType = 1;
  }
});

watch(() => form.appId, (newVal) => {
  getApproveTemplatelist(newVal).then(response => {
    store.setBasideFormConfig({
      appdId: newVal,
      formCode: form.formCode,
      configList: response.data,
    });
  }).catch(() => {
    console.log(`获取${formCode}审批模板配置信息失败`);
  });
});

// 给父级页面提供得获取本页数据得方法
const getData = () => {
  return new Promise((resolve, reject) => {
    proxy.$refs["ruleFormRef"].validate((valid, fields) => {
      if (!valid) {
        emit("nextChange", { label: "基础设置", key: "basicSetting" });
        reject({ valid: false });
      }
      form.effectiveStatus = form.effectiveStatus ? 1 : 0;
      resolve({ formData: form }); // TODO 提交表单
    });
  });
};

defineExpose({
  getData,
});
</script>
<style scoped>
.form-container {
  background: white !important;
  padding: 10px;
  max-width: 600px;
  min-height: 520px;
  left: 0;
  bottom: 0;
  right: 0;
  margin: auto;
}
</style>
