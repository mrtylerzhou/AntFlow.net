<template>
  <!-- 添加条件模板对话框 -->
  <el-dialog title="添加条件" v-model="dialogVisible" width="550px" append-to-body>
    <el-form :model="templateForm" :rules="templateRules" ref="conditionTemplateRef" label-width="130px"
      label-position="top" style="margin: 0 20px;">
      <el-row>
        <el-col :span="24">
          <el-form-item label="业务方名称" prop="businessPartyName">
            <el-input v-model="templateForm.businessPartyName" :disabled=true placeholder="请输入业务方名称" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="业务表单名称" prop="applicationName">
            <el-input v-model="templateForm.applicationName" :disabled=true placeholder="请输入业务表单名称" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="条件模板ID" prop="templateMark">
            <el-input v-model="templateForm.templateMark" placeholder="请输入条件模板唯一标识" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="条件模板名称" prop="name">
            <el-input v-model="templateForm.name" placeholder="请输入条件模板名称" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="备注">
            <el-input v-model="templateForm.remark" type="textarea" placeholder="请输入内容"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitConditionTempForm">确 定</el-button>
        <el-button @click="closeDialog">取 消</el-button>
      </div>
    </template>
  </el-dialog>

</template>
<script setup>
import { ref, watch } from "vue";
import { setConditionTemplate } from "@/api/workflow/outsideApi";
const { proxy } = getCurrentInstance();
let props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  bizformData: {
    type: Object,
    default: () => { },
  }
});

let dialogVisible = computed({
  get() {
    return props.visible
  },
  set() {
    closeDialog()
  }
})
const emits = defineEmits(["update:visible"]);

watch(() => props.bizformData, (val) => {
  templateForm.value = val;
}, { deep: true });

const data = reactive({
  templateForm: {},
  templateRules: {
    businessPartyName: [{ required: true, message: '', trigger: 'blur' }],
    applicationName: [{ required: true, message: '', trigger: 'blur' }],
    templateMark: [{ required: true, message: '请输入条件模板ID', trigger: 'blur' }],
    name: [{ required: true, message: '请输入条件模板名称', trigger: 'blur' }]
  }
});
const { templateForm, templateRules } = toRefs(data);

/** 提交条件模板表单 */
function submitConditionTempForm() {
  proxy.$refs["conditionTemplateRef"].validate(valid => {
    if (valid) {
      proxy.$modal.loading();
      setConditionTemplate(templateForm.value).then(res => {
        if (res && res.code == 200) {
          proxy.$modal.msgSuccess("添加成功");
          emits("update:visible", false);
        } else {
          proxy.$modal.msgError("添加失败" + res.errMsg);
        }
      }).catch(err => {
        console.log(err);
      });
      proxy.$modal.closeLoading();
    }
  });
}
/** 取消操作添加条件模板表单 */
function closeDialog() {
  emits("update:visible", false);
  reset();
}
/** 重置操作表单 */
function reset() {
  templateForm.value = {};
}

</script>