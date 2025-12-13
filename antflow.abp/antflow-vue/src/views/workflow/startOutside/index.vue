<template>
    <div class="app-container">
        <div class="card-box">
            <el-tabs class="demo-tabs" v-model="activeName" @tab-click="handleClick">
                <el-tab-pane name="createFrom">
                    <template #label>
                        填写表单
                    </template>
                    <div>
                        <component ref="formRef" v-if="componentLoaded" :is="loadedComponent" :lfFormData="lfFormData"
                            :isPreview="false" :showSubmit="true" @handleBizBtn="handleSubmit">
                        </component>
                    </div>
                </el-tab-pane>

                <el-tab-pane name="flowFromReview" label="流程预览">
                    <div v-if="reviewWarpShow">
                        <ReviewWarp v-model:previewConf="previewConf" />
                    </div>
                </el-tab-pane>
            </el-tabs>
            <label class="page-close-box" @click="close()"><img src="@/assets/images/antflow/back-close.png"></label>
        </div>
    </div>
</template>

<script setup>
import { ref, getCurrentInstance, onMounted } from 'vue';
import ReviewWarp from '@/components/Workflow/Preview/reviewWarp.vue';
import { getLowCodeFromCodeData } from '@/api/workflow/lowcodeApi';
import { loadDIYComponent, loadLFComponent } from '@/views/workflow/components/componentload.js';
import { processSubmit } from "@/api/workflow/outsideApi";
import Cookies from "js-cookie";
const { proxy } = getCurrentInstance();
const route = useRoute();
const activeName = ref("createFrom");

const formRef = ref(null);
const reviewWarpShow = ref(false);
const previewConf = ref({});
let componentLoaded = ref(false);
let loadedComponent = ref(null);
let lfFormData = ref(null);
const isLFFlow = true;//route.query?.formType == 'LF';
const flowCode = route.query?.fc;
onMounted(async () => {
    await adapFlowType();
})
const adapFlowType = async () => {
    if (isLFFlow && isLFFlow == true) {
        await getLowCodeFromCodeData(flowCode).then((res) => {
            if (res.code == 200) {
                lfFormData.value = res.data
            }
        });
        loadedComponent.value = await loadLFComponent();
        componentLoaded.value = lfFormData.value ? true : false;
    } else {
        loadedComponent.value = await loadDIYComponent(flowCode)
            .catch((err) => { console.log('err=', err); proxy.$modal.msgError(err); componentLoaded.value = false; });
        componentLoaded.value = true;
    }
}
const handleSubmit = (param) => {
    if (componentLoaded.value != true) { return; }
    startTest(param);
}
/**
 * 点击tab切换
 * @param tab 
 * @param event 
 */
const handleClick = async (tab, event) => {
    if (componentLoaded.value != true) { return; }
    if (tab.paneName != 'flowFromReview') {
        reviewWarpShow.value = false;
        return;
    }
    if (formRef.value.hasOwnProperty('handleValidate') == false) {
        proxy.$modal.msgError("未定义表单组件");
        return;
    }
    await formRef.value.handleValidate().then(async (isValid) => {
        if (!isValid) {
            activeName.value = "createFrom";
        } else {
            const _formData = await formRef.value.getFromData();
            if (isLFFlow && isLFFlow == true) {
                let lfFormdata = JSON.parse(_formData);
                previewConf.value.approversList = lfFormdata.approversList;
                previewConf.value.approversValid = lfFormdata.approversValid;
                delete lfFormdata.approversList;
                delete lfFormdata.approversValid;
                previewConf.value.lfFields = JSON.parse(_formData);
            } else {
                previewConf.value = JSON.parse(_formData);
            }
            previewConf.value.formCode = flowCode || '';
            previewConf.value.isStartPreview = true;
            previewConf.value.isLowCodeFlow = isLFFlow;
            previewConf.value.isOutSideAccess = true;//三方发起
            reviewWarpShow.value = true;
        }
    }).catch((r) => {
        //console.log('errormsg',r);
        activeName.value = "createFrom";
        //proxy.$modal.msgError("加载失败:" + r.message);
    });
}
/**
 * 发起流程
 * @param param 
 */
const startTest = (param) => {
    let bizFrom = JSON.parse(param);
    bizFrom.formCode = flowCode || '';
    bizFrom.operationType = 1;//operationType 1发起 3 审批 
    bizFrom.isLowCodeFlow = true;
    bizFrom.lfFields = null;
    if (isLFFlow && isLFFlow == true) {
        bizFrom = {};
        bizFrom.formCode = flowCode || '';
        bizFrom.operationType = 1;//operationType 1发起 3 审批 
        bizFrom.isLowCodeFlow = true;

        let lfFormdata = JSON.parse(param);
        bizFrom.approversList = lfFormdata.approversList;
        bizFrom.approversValid = lfFormdata.approversValid;
        delete lfFormdata.approversList;
        delete lfFormdata.approversValid;
        bizFrom.lfFields = lfFormdata;
    }
    bizFrom.userId = Cookies.set('userId');
    proxy.$modal.loading();
    processSubmit(bizFrom).then((res) => {
        if (res.code == 200) {
            proxy.$modal.msgSuccess("发起流程成功");
            const obj = { path: "/flowTask/mytask" };
            proxy.$tab.closeOpenPage(obj);
        } else {
            proxy.$modal.msgError("发起流程失败" + res.errMsg);
        }
        proxy.$modal.closeLoading();
    });
}
/** 关闭按钮 */
function close() {
    proxy.$tab.closePage();
}
</script>
<style scoped lang="scss">
.task-title-text {
    line-height: 28px;
    font-weight: 600;
    font-size: 16px;
    color: #383838;
}
</style>