<!--
 * @Date:  2024-05-25 14:05:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-03-29 16:05:54
 * @FilePath: /ant-flow/src/components/dialog/errorDialog.vue
-->
<template>
  <el-dialog title="错误提示" v-model="visibleDialog" :width="520">
    <div class="fd-confirm-body">
      <i class="anticon anticon-close-circle" style="color: #f00;"></i>
      <span class="fd-confirm-title">当前无法发布</span>
      <div class="fd-confirm-content">
        <div>
          <p class="error-modal-desc">以下内容不完善，需进行修改</p>
          <div class="error-modal-list">
            <div class="error-modal-item" v-for="(item, index) in list" :key="index">
              <div class="error-modal-item-label">流程设计</div>
              <div class="error-modal-item-content">【{{ item.name }}】 未选择{{ item.nodeType }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <template #footer>
      <el-button @click="visibleDialog = false">我知道了</el-button>
      <el-button type="primary" @click="visibleDialog = false">前往修改</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { computed } from 'vue'
let props = defineProps({
  list: {
    type: Array,
    default: () => []
  },
  visible: {
    type: Boolean,
    default: false
  }
})
let emits = defineEmits(['update:visible'])

let visibleDialog = computed({
  get() {
    return props.visible
  },
  set(val) {
    emits('update:visible', val)
  }
})
</script>

<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";

.fd-confirm-body .fd-confirm-title {
  color: rgba(0, 0, 0, .85);
  font-weight: 500;
  font-size: 16px;
  line-height: 1.4;
  padding-left: 2px;
  display: block;
  overflow: hidden
}

.fd-confirm-body .fd-confirm-content {
  margin-left: 40px;
  font-size: 14px;
  color: rgba(0, 0, 0, .65);
  margin-top: 8px
}

.fd-confirm-body>.anticon {
  font-size: 22px;
  margin-right: 15px;
  margin-left: 40px;
  float: left
}
</style>