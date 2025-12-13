<template>
    <div class="lf-container">
        <div class="fd-nav">
            <div class="fd-nav-left">
                <div class="fd-nav-title">
                    <el-icon>
                        <HomeFilled />
                    </el-icon>
                    {{ title }}
                </div>
            </div>
            <div class="fd-nav-center">
                <div class="step-tab">
                    <div v-for="(item, index) in steps" :key="index" class="step"
                        :class="[activeStep == item.key ? 'active' : '']" @click="changeSteps(item)">
                        <span class="step-index">{{ index + 1 }}</span>
                        {{ item.label }}
                    </div>
                </div>
            </div>
            <div class="fd-nav-right">
                <button type="button" class="fd-btn button-publish" @click="previewJson">
                    <span>预览json</span>
                </button>
                <button type="button" class="fd-btn button-publish" @click="publish">
                    <span>发 布</span>
                </button>
            </div>
        </div>
        <div v-if="processConfig" v-show="activeStep === 'basicSetting'">
            <BasicSetting ref="basicSetting" :basicData="processConfig" @nextChange="changeSteps" :flowType="'LF'" />
        </div>
        <div v-show="activeStep === 'formDesign'" aria-hidden="true">
            <DynamicForm ref="formDesign" :lfFormData="lfFormDataConfig" />
        </div>
        <div v-if="nodeConfig" v-show="activeStep === 'processDesign'">
            <Process ref="processDesign" :processData="nodeConfig" @nextChange="changeSteps" />
        </div>
        <jsonDialog v-model:visible="viewJson" :title="jsonTitle" :modelValue="nodeConfig" />
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from 'vue-router';
import { getApiWorkFlowData, setApiWorkFlowData } from '@/api/workflow/index';
import { FormatCommitUtils } from '@/utils/antflow/formatcommit_data';
import { FormatDisplayUtils } from '@/utils/antflow/formatdisplay_data';
import { NodeUtils } from '@/utils/antflow/nodeUtils';
import BasicSetting from "@/components/Workflow/basicSetting/index.vue";
import Process from "@/components/Workflow/Process/index.vue";
import DynamicForm from "@/components/Workflow/DynamicForm/index.vue";
import jsonDialog from "@/components/Workflow/dialog/jsonDialog.vue";
const { proxy } = getCurrentInstance()
const route = useRoute();
const basicSetting = ref(null);
const processDesign = ref(null);
const formDesign = ref(null);
let activeStep = ref("basicSetting"); // 激活的步骤面板
let steps = ref([
    { label: "基础设置", key: "basicSetting" },
    { label: "表单设计", key: "formDesign" },
    { label: "流程设计", key: "processDesign" },
]);
const changeSteps = (item) => {
    activeStep.value = item.key;
};
let processConfig = ref(null);
let nodeConfig = ref(null);
let lfFormDataConfig = ref(null);
let title = ref('');

let viewJson = ref(false);
let jsonTitle = ref('');

onMounted(async () => {
    let mockjson = {};
    proxy.$modal.loading();
    if (route.query.id) {
        mockjson = await getApiWorkFlowData({ id: route.query.id });
        if (!mockjson.code || mockjson.code != 200) {
            proxy.$modal.closeLoading();
            proxy.$modal.msgError("获取API数据失败" + JSON.stringify(mockjson.errMsg));
            return;
        }
    } else {
        mockjson = NodeUtils.createStartNode();
    }
    let data = FormatDisplayUtils.getToTree(mockjson.data);
    proxy.$modal.closeLoading();
    processConfig.value = data;
    title.value = proxy.isEmpty(data?.bpmnName) ? decodeURIComponent(route.query.fcname ?? '') : data?.bpmnName;
    nodeConfig.value = data?.nodeConfig;
    lfFormDataConfig.value = data?.lfFormData;
    //console.log("lfFormDataConfig==========", JSON.stringify(lfFormDataConfig.value));
});


const publish = () => {
    const step1 = basicSetting.value.getData();
    const step2 = formDesign.value.getData();
    const step3 = processDesign.value.getData();
    proxy.$modal.loading();
    Promise.all([step1, step2, step3])
        .then((res) => {
            //proxy.$modal.msgSuccess("设置成功,F12控制台查看数据");
            let basicData = res[0].formData;
            basicData.isLowCodeFlow = 1; // 1代表低代码表单 
            let lowcodeformData = res[1].formData;
            Object.assign(basicData, { lfFormData: JSON.stringify(lowcodeformData) });
            var nodes = FormatCommitUtils.formatSettings(res[2].formData);
            Object.assign(basicData, { nodes: nodes });
            return basicData;
        })
        .then((data) => {
            //console.log("提交到API===success=", JSON.stringify(data));
            setApiWorkFlowData(data).then((resLog) => {
                proxy.$modal.closeLoading();
                if (resLog.code == 200) {
                    proxy.$modal.msgSuccess("设置成功,F12控制台查看数据");
                    let obj = { path: "flow-version", query: { formCode: data.formCode } };
                    proxy.$tab.closeOpenPage(obj);
                } else {
                    proxy.$modal.msgError("提交到API返回失败" + JSON.stringify(resLog.errMsg));
                }
            })
        })
        .catch((err) => {
            proxy.$modal.closeLoading();
            if (err) {
                console.log("设置失败" + JSON.stringify(err));
            }
        });
};
const previewJson = () => {
    viewJson.value = true;
    jsonTitle.value = "预览JSON";
}
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";

.lf-container {
    position: relative;
    background-color: #f5f5f7;
    min-height: calc(100vh - 85px);
    padding-top: 15px;
    overflow: auto;
}

.step-tab {
    display: flex;
    justify-content: center;
    position: relative;
    height: 60px;
    font-size: 14px;
    border-right: 0px solid #1583f2;
    text-align: center;
    cursor: pointer
}

.fd-nav .step {
    width: 140px;
    line-height: 100%;
    padding-left: 30px;
    padding-right: 30px;
    line-height: 60px;
    cursor: pointer;
    position: relative;
}

.fd-nav .step:hover {
    background: #5af
}

.fd-nav .step:active {
    background: #1583f2
}

.fd-nav .active {
    background: #5af;
}

.fd-nav .step-index {
    display: inline-block;
    width: 18px;
    height: 18px;
    border: 1px solid #fff;
    border-radius: 8px;
    line-height: 18px;
    text-align: center;
    box-sizing: border-box;
}
</style>