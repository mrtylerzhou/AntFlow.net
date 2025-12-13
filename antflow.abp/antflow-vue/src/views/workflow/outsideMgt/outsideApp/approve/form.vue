<template>
  <!-- 添加审批人模板对话框 -->
  <el-dialog title="审批人配置" v-model="dialogVisible" width="550px" append-to-body>
    <el-form :model="form" :rules="rules" ref="approveTemplateRef" label-width="130px" style="margin: 0 20px;">
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
          <el-form-item label="审批人类型" prop="approveTypeId">
            <el-select v-model="form.approveTypeId" @change="selectTypeChanged" placeholder="请选择账户类型"
              :style="{ width: '100%' }">
              <el-option v-for="(item, index) in approveTypeOptions" :key="index" :label="item.label"
                :value="item.value"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="24">
          <el-form-item label="审批人模板URL" prop="apiUrl">
            <el-input v-model="form.apiUrl" placeholder="请输入审批人模板URL(必须http或https开头)" :disabled=true>
              <template #append>
                <el-tooltip class="box-item" effect="dark" content="检查审批人模板URL是否连通" placement="bottom-end">
                  <el-button @click="handleCheckUserUrl">
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
          <el-form-item label="Token" prop="apiToken">
            <el-input v-model="form.apiToken" placeholder="请输入Token" :disabled=true />
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
import { setApproveTemplate } from "@/api/workflow/outsideApi";
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
const approveTypeOptions = [
  { label: "指定人员", value: 1 },
  { label: "指定角色", value: 2 }
];
const data = reactive({
  form: {},
  rules: {
    businessPartyName: [{ required: true, message: '', trigger: 'blur' }],
    applicationName: [{ required: true, message: '', trigger: 'blur' }],
    approveTypeId: [{ required: true, message: '请选择审批人类型', trigger: 'change' }],
    apiClientId: [{ required: true, pattern: /^[^\u4e00-\u9fff]+$/, message: '请输入业务表单唯一标识(不能输入中文)', trigger: 'blur' }],
    apiClientSecret: [{ required: true, pattern: /^[^\u4e00-\u9fff]+$/, message: '请输入业务表单密钥(不能输入中文)', trigger: 'blur' }],
    apiToken: [{ required: true, pattern: /^[^\u4e00-\u9fff]+$/, message: '请输入业务表单唯一标识(不能输入中文)', trigger: 'blur' }],
    apiUrl: [
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
  form.value.apiToken = "AA2BB0F7647D408992333672A8551E96";
  form.value.apiClientId = "033AFA1C6C3545AD";
  form.value.apiClientSecret = "EF28AC4A539E4A6F8CFC17ECC2C863CC";
}, { deep: true });

function selectTypeChanged() {
  if (form.value.approveTypeId == 1) {
    form.value.approveTypeName = "指定人员";
    form.value.apiUrl = "http://14.103.207.27:7001/user/getUser";
  } else {
    form.value.approveTypeName = "指定角色";
    form.value.apiUrl = "http://14.103.207.27:7001/user/getRoleInfo";
  }
}

/** 提交表单 */
function submitApproveTempForm() {
  proxy.$refs["approveTemplateRef"].validate(async valid => {
    if (valid) {
      proxy.$modal.loading();
      await setApproveTemplate(form.value).then(res => {
        if (res && res.code == 200) {
          emits("update:visible", false);
          emits("changeRefresh", { paneName: 'approveSet' });
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


function handleCheckUserUrl() {
  let url = form.value.apiUrl;
  if (!url) {
    proxy.$modal.msgError("审批人模板URL不能为空");
    return;
  }
  else {
    const regex = /^https?:\/\//;
    if (!regex.test(url)) {
      proxy.$modal.msgError("请输入正确审批人模板URL");
      return;
    }
    getDynamicsList(url).then((res) => {
      proxy.$modal.msgSuccess("审批人模板URL链接成功");
    }).catch((res) => {
      proxy.$modal.msgError("请输入正确审批人模板URL");
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