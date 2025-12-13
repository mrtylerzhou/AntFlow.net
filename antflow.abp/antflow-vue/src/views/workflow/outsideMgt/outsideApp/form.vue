<template>
  <!-- 添加或修改对话框 -->
  <el-dialog :title="dialogTitle" v-model="dialogVisible" width="550px" append-to-body>
    <el-form :model="form" :rules="rules" ref="formRef" label-width="130px" label-position="top"
      style="margin: 0 20px;">
      <el-row>
        <el-col :span="24">
          <el-form-item label="所属项目" prop="businessName">
            <el-input v-model="form.businessName" placeholder="请输入应用名称" :disabled="true" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="应用名称" prop="title">
            <el-input v-model="form.title" placeholder="请输入应用名称" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="应用类型" prop="applyType">
            <el-radio-group v-model="form.applyType">
              <el-radio value="1" :disabled=true>流程</el-radio>
              <el-radio value="2" :disabled=true>应用</el-radio>
              <el-radio value="3" :disabled=true>父级应用</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="24">
          <el-form-item label="子应用" prop="isSon">
            <el-radio-group v-model="form.isSon">
              <el-radio value="1" :disabled=true>是</el-radio>
              <el-radio value="2" :disabled=true>否</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>

      <!-- <el-row>
                <el-col :span="24">
                  <el-form-item label="应用URL" prop="applicationUrl">
                    <el-input v-model="form.applicationUrl" placeholder="请输入应用URL" />
                  </el-form-item>
                </el-col>
              </el-row> -->
      <!-- <el-row>
                <el-col :span="24">
                  <el-form-item label="icon图(app)" prop="effectiveSource">
                    <ImageUpload :limit="1" :fileSize="3" :fileType="['jpg']"/>
                  </el-form-item>
                </el-col>
              </el-row> -->
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="cancel">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>
<script setup>
import { addApplication } from "@/api/workflow/outsideApi";
import { computed } from "vue";
const { proxy } = getCurrentInstance();
let props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  dialogTitle: {
    type: String,
    default: '新增应用',
  },
  appformData: {
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
const emits = defineEmits(["update:visible", "refresh"]);
const form = computed(() => props.appformData);
const rules = {
  businessCode: [{ required: true, message: '请选择业务方', trigger: 'blur' }],
  title: [
    { required: true, message: '请填写应用名称', trigger: 'blur' },
    { pattern: /^.{2,10}$/, message: '长度必须在2到10位之间', trigger: 'blur' }
  ],
}
/** 提交表单 */
function submitForm() {
  proxy.$refs["formRef"].validate(valid => {
    if (valid) {
      proxy.$modal.loading();
      if (form.value.id != undefined) {
        addApplication(form.value).then(response => {
          open.value = false;
          proxy.$modal.closeLoading();
          proxy.$modal.msgSuccess("修改成功");
          emits("refresh");
          emits("update:visible", false);
        });
      } else {
        addApplication(form.value).then(response => {
          proxy.$modal.closeLoading();
          if (response.code != 200) {
            proxy.$modal.msgError("注册失败");
            return;
          }
          open.value = false;
          proxy.$modal.msgSuccess("注册成功");
          emits("refresh");
          emits("update:visible", false);
        });
      }

    }
  });
}
/** 取消操作添加条件模板表单 */
function closeDialog() {
  emits("update:visible", false);
}

/** 取消操作表单 */
function cancel() {
  emits("update:visible", false);
}
</script>