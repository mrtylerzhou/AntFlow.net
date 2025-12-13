<template>
  <el-dialog v-model="visibleDialog" style="width: 680px !important;" :show-close="false" append-to-body>
    <template #header>
      {{ title }}
      <div style="float: right;">
        <el-button @click="$emit('update:visible', false)">关 闭</el-button>
        <el-button type="primary" @click="handleCopy">复 制</el-button>
      </div>
    </template>
    <div style="min-height: 250px !important;">
      <el-text id="jsonTextId" class="mx-1" type="primary"> {{ modelValue }}</el-text>
    </div>

  </el-dialog>
</template>

<script setup>
import { computed, getCurrentInstance } from 'vue';
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
  modelValue: {
    type: Object,
    default: () => { }
  }
});
let emits = defineEmits(['update:visible'])
let visibleDialog = computed({
  get() {
    return props.visible
  },
  set() {
    closeDialog()
  }
});

/**
 * 关闭弹窗
 */
const closeDialog = () => {
  emits('update:visible', false)
}
const handleCopy = () => {
  const jsonELE = document.getElementById('jsonTextId');
  if (!jsonELE) return;
  const jsonText = jsonELE.innerText;
  if (navigator.clipboard) {
    navigator.clipboard.writeText(jsonText).then(() => {
      proxy.$modal.msgSuccess("复制成功");
    }, () => {
      proxy.$modal.msgSuccess("复制失败");
    });
  }
  else {
    jsonELE.focus();
    document.execCommand('copy');
    proxy.$modal.msgSuccess("复制成功");
  }
}  
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";
</style>