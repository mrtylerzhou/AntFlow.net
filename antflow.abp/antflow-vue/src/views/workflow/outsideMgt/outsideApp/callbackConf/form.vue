<template>
  <!-- 添加审批人模板对话框 -->
  <el-dialog title="设置流程回调" v-model="dialogVisible" width="550px" append-to-body>
    <el-form :model="form" :rules="rules" ref="callbackConfRef" label-width="130px" style="margin: 0 20px;">
      <el-row>
        <el-col :span="24">
          <el-form-item label="租户名称" prop="businessPartyName">
            <el-input v-model="form.businessPartyName" :disabled=true placeholder="请输入租户名称" />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="应用名称" prop="applicationName">
            <el-input v-model="form.applicationName" :disabled=true placeholder="请输入应用名称" />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="流程回调URL" prop="bpmFlowCallbackUrl">
            <el-input v-model="form.bpmFlowCallbackUrl" placeholder="请输入流程流转回调URL(必须http或https开头)">
              <template #append>
                <el-tooltip class="box-item" effect="dark" content="检查流程流转回调URL是否连通" placement="bottom-end">
                  <el-button @click="handleCheckCallbackUrl">
                    <el-icon>
                      <CircleCheck />
                    </el-icon>
                  </el-button>
                </el-tooltip>
              </template>
            </el-input>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="ClientId" prop="apiClientId">
            <el-input v-model="form.apiClientId" placeholder="请输入ClientId" :disabled=true />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="ClientSecret" prop="apiClientSecret">
            <el-input v-model="form.apiClientSecret" placeholder="请输入ClientSecret" :disabled=true />
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="备注">
            <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitApproveTempForm">确 定</el-button>
        <el-button @click="closeDialog">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>
<script setup>
import { computed, reactive, getCurrentInstance, watch } from "vue";
import { callbackUrlConf } from "@/api/workflow/outsideApi";
import { getDynamicsList } from "@/api/workflow/mock";
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
const emits = defineEmits(["update:visible", "changeRefresh"]);

const data = reactive({
  form: {},
  rules: {
    businessPartyName: [{ required: true, message: '', trigger: 'blur' }],
    applicationName: [{ required: true, message: '', trigger: 'blur' }],
    apiClientId: [{ required: true, pattern: /^[^\u4e00-\u9fff]+$/, message: '请输入业务方唯一标识(不能输入中文)', trigger: 'blur' }],
    apiClientSecret: [{ required: true, pattern: /^[^\u4e00-\u9fff]+$/, message: '请输入业务方密钥(不能输入中文)', trigger: 'blur' }],
    bpmFlowCallbackUrl: [
      {
        required: true,
        pattern: /^https?:\/\//,
        message: '请输入正确的URL',
        trigger: 'blur'
      }]
  }
});
const { form, rules } = toRefs(data);

watch(() => props.bizformData, (val) => {
  form.value = val;
  form.value.apiClientId = "033AFA1C6C3545AD";
  form.value.apiClientSecret = "EF28AC4A539E4A6F8CFC17ECC2C863CC";
  form.value.bpmFlowCallbackUrl = "http://14.103.207.27:7001/user/getUser";
}, { deep: true });

/** 提交表单 */
function submitApproveTempForm() {
  proxy.$refs["callbackConfRef"].validate(async valid => {
    if (valid) {
      proxy.$modal.loading();
      await callbackUrlConf(form.value).then(res => {
        if (res && res.code == 200) {
          emits("update:visible", false);
          emits("changeRefresh", { paneName: 'callbackSet' });
          proxy.$modal.msgSuccess("添加成功");
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


function handleCheckCallbackUrl() {
  let url = form.value.bpmFlowCallbackUrl;
  if (!url) {
    proxy.$modal.msgError("流程流转回调URL不能为空");
    return;
  }
  else {
    const regex = /^https?:\/\//;
    if (!regex.test(url)) {
      proxy.$modal.msgError("请输入正确流程流转回调URL");
      return;
    }
    getDynamicsList(url).then((res) => {
      proxy.$modal.msgSuccess("流程流转回调URL链接成功");
    }).catch((res) => {
      proxy.$modal.msgError("请输入正确流程流转回调URL");
    });
  }
}
/** 取消操作添加条件模板表单 */
function closeDialog() {
  emits("update:visible", false);
  //reset();
}
/** 重置操作表单 */
function reset() {
  form.value = {};
}
</script>