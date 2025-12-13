<template>
  <div class="card-box" style="padding: 10px;">
    <div class="task-title">
      <span class="task-title-text">流程详情预览</span>
    </div>
    <div style="background-color: #f5f5f7;padding: 15px;">
      <el-tabs v-model="activeTab" class="demo-tabs">
        <el-tab-pane label="流程基本信息" name="flowForm"></el-tab-pane>
        <el-tab-pane label="业务表单预览" name="formRender"></el-tab-pane>
        <el-tab-pane label="流程模板预览" name="flow"></el-tab-pane>
      </el-tabs>
      <el-row>
        <el-col :span="24" v-if="activeTab === 'flowForm'" class="item">
          <div v-if="processConfig">
            <BasicSetting ref="basicSetting" :basicData="processConfig" />
          </div>
        </el-col>
        <el-col :span="24" v-if="activeTab === 'formRender'" class="item">
          <div v-if="processConfig" class="component">
            <component v-if="componentLoaded" :is="loadedComponent" :lfFormData="lfFormDataConfig"
              :lfFieldPerm="lfFieldControlVOs" :isPreview="true">
            </component>
          </div>
        </el-col>
        <el-col :span="24" v-if="activeTab === 'flow'" class="item">
          <div v-if="nodeConfig" class="flow">
            <Process ref="processDesign" :processData="nodeConfig" />
          </div>
        </el-col>
      </el-row>
    </div>
    <label class="page-close-box" @click="close()"><img src="@/assets/images/antflow/back-close.png"></label>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import { getApiWorkFlowData } from "@/api/workflow/index";
import BasicSetting from "@/components/Workflow/basicSetting/index.vue";
import Process from "@/components/Workflow/Process/index.vue";
import { FormatDisplayUtils } from '@/utils/antflow/formatdisplay_data';
import { loadDIYComponent, loadLFComponent } from '@/views/workflow/components/componentload.js';

const { proxy } = getCurrentInstance();
const route = useRoute();
const activeTab = ref('flowForm')
let processConfig = ref(null)
let lfFormDataConfig = ref(null)
let lfFieldControlVOs = ref(null)
let nodeConfig = ref(null)
let title = ref('')
let id = route.query?.id
let loadedComponent = ref(null)
let componentLoaded = ref(null)

/** 关闭按钮 */
function close() {
  proxy.$tab.closePage();
};
onMounted(async () => {
  proxy.$modal.loading();
  await init();
  proxy.$modal.closeLoading();
});
const init = async () => {
  let mockjson = await getApiWorkFlowData({ id });
  if (mockjson.code != 200) {
    proxy.$modal.msgError(mockjson.errMsg);
    return;
  }
  let data = FormatDisplayUtils.getToTree(mockjson.data);
  processConfig.value = data;
  title.value = data?.bpmnName;
  nodeConfig.value = data?.nodeConfig;
  if (data.isLowCodeFlow == '1') {//低代码表单
    lfFormDataConfig.value = data?.lfFormData
    lfFieldControlVOs.value = JSON.stringify(data.processRecordInfo?.lfFieldControlVOs);
    loadedComponent.value = await loadLFComponent();
    componentLoaded.value = true;
  } else {//自定义表单
    loadedComponent.value = await loadDIYComponent(data.formCode).catch((err) => {
      proxy.$modal.msgError(err);
    });
    componentLoaded.value = true;
  }
}

</script>

<style scoped lang="scss">
.task-title {
  display: flex;
  justify-content: space-between;
  padding-bottom: 6px;
  //border-bottom: 2px solid #e8e8e8;
  background-color: #f5f5f7;
}

.task-title-text {
  line-height: 28px;
  font-weight: 600;
  font-size: 16px;
  color: #383838;
}

.component {
  background: white !important;
  padding: 30px !important;
  max-width: 720px !important;
  left: 0 !important;
  right: 0 !important;
  margin: auto !important;
}
</style>
