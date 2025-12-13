<template>
  <el-dialog :title="title" v-model="visibleDialog" style="width: 680px !important; " append-to-body
    class="promoter_person">
    <div style="min-height: 250px !important;">
      <el-row style="padding-left: -5px;padding-right: -5px;">
        <el-col :span="24" class="my-col">
          <el-form ref="formRef" :model="form" :rules="rules" label-position="top" class="my-form">
            <el-form-item label="选择人员" prop="selectList">
              <TagUserSelect v-model:value="form.selectList">
                <template #append>
                  <el-button class="append-add" type="default" icon="Plus" @click="userDialogVisible = true" />
                </template>
              </TagUserSelect>
            </el-form-item>
            <el-form-item label="备注/说明" prop="remark">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" :maxlength="100" show-word-limit
                :autosize="{ minRows: 4, maxRows: 4 }" :style="{ width: '100%' }"></el-input>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
      <selectUser ref="selectUserRef" v-model:visible="userDialogVisible" :multiple="isMultiple"
        v-model:checkedData="form.selectList" @change="saveUserDialog" />
    </div>
    <template #footer>
      <el-button @click="$emit('update:visible', false)">取 消</el-button>
      <el-button type="primary" @click="saveDialog">确 定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { computed, onMounted, ref, getCurrentInstance } from 'vue';
import TagUserSelect from "@/components/BizSelects/TagUserSelect/index.vue";
import selectUser from '@/components/BizSelects/userListDialog.vue';
import { getUsers } from "@/api/workflow/mock.js";
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
let list = ref([])
let userDialogVisible = ref(false);

let emits = defineEmits(['update:visible', 'change'])
let visibleDialog = computed({
  get() {
    reset();
    return props.visible
  },
  set() {
    closeDialog()
  }
});

let rules = {
  selectList: [{
    required: true,
    message: '请选择人员',
    trigger: 'change'
  }],
  remark: [{
    required: true,
    message: '请输入备注',
    trigger: 'blur'
  }]
};
const saveUserDialog = (data) => {
  form.value.selectList = data;
}
/**
 * 关闭弹窗
 */
const closeDialog = () => {
  emits('update:visible', false)
}
reset();
onMounted(async () => {
  reset();
  await getUserList();
});

const getUserList = async () => {
  await getUsers().then(res => {
    if (res.code == 200) {
      list.value = res.data.map(item => {
        return {
          label: item.name,
          value: item.id
        }
      });
    }
  });
}

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
      //console.log('resFrom.value==========', JSON.stringify(resFrom))
      emits('change', resFrom)
    }
  });
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