<template>
  <el-dialog :title="title" v-model="visibleDialog" style="width: 680px !important; " append-to-body
    class="promoter_person">
    <div style="min-height: 250px !important;">
      <el-row style="padding-left: -5px;padding-right: -5px;">
        <el-col :span="24" class="my-col">
          <el-form ref="formRef" :model="form" :rules="rules" label-position="top" class="my-form">
            <el-form-item label="选择人员" prop="selectList">
              <TagUserSelect v-model:list="form.selectList" :multiple="isMultiple" placeholder="请选择人员" />
            </el-form-item>
            <el-form-item label="备注/说明" prop="remark">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" :maxlength="100" show-word-limit
                :autosize="{ minRows: 4, maxRows: 4 }" :style="{ width: '100%' }"></el-input>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
    </div>
    <template #footer>
      <el-button @click="$emit('update:visible', false)">取 消</el-button>
      <el-button type="primary" @click="saveDialog">确 定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { computed, watch, ref, getCurrentInstance } from 'vue';
import TagUserSelect from "@/components/BizSelects/TagUserSelect/index.vue";
const { proxy } = getCurrentInstance()
let props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: ''
  },
  isMultiple: {
    type: Boolean,
    default: false
  },
});
let formRef = ref(null)
let form = ref({
  selectList: [],
  remark: ''
})
let emits = defineEmits(['update:visible', 'change'])
let visibleDialog = computed({
  get() {
    return props.visible
  },
  set() {
    closeDialog()
  }
});
watch(() => props.visible, (val) => {
  if (val) {
    reset();
  }
});
let rules = {
  selectList: [{ required: true, message: '', trigger: ['change', 'blur'] }],
  remark: [{ required: true, message: '请输入备注', trigger: ['change', 'blur'] }]
};
/**
 * 保存
 */
let saveDialog = () => {
  let resFrom = {};
  proxy.$refs['formRef'].validate((valid) => {
    if (valid) {
      if (!props.isMultiple) {
        resFrom.selectList = [{
          id: form.value.selectList[0].id,
          name: form.value.selectList[0].name
        }]
      } else {
        resFrom.selectList = form.value.selectList;
      }
      resFrom.remark = form.value.remark
      emits('change', resFrom)
    }
  });
}
/**
 * 关闭弹窗
 */
const closeDialog = () => {
  emits('update:visible', false)
}
/** 重置操作表单 */
function reset() {
  form.value = {
    selectList: [],
    remark: ''
  };
  proxy.resetForm("formRef");
};
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";
</style>