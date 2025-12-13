<template>
    <div style="text-align:center;">
        <section class="antflow-design" ref="antflowDesignRef">
            <div class="zoom" style="">
                <div class="zoom-out" @click="zoomOut" title="缩小"></div>
                <span>{{ nowVal }}%</span>
                <div class="zoom-in" @click="zoomIn" title="放大"></div>
                <!--刷新图标代码-->
                <div class="zoom-reset" @click="zoomReset" title="还原缩放比例">&#10227</div>
            </div>
            <div class="box-scale" ref="boxScaleRef">
                <LineWarp v-if="nodeConfig" v-model:nodeConfig="nodeConfig" />
                <div class="end-node">
                    <div class="end-node-circle"></div>
                    <div class="end-node-text">流程结束</div>
                </div>
            </div>
        </section>
    </div>
</template>
<script setup>
import LineWarp from '@/components/Workflow/Preview/lineWarp.vue';
import { getFlowPreview } from '@/api/workflow/index';
import { FormatPreviewUtils } from '@/utils/antflow/formatFlowPreview';
import { useStore } from '@/store/modules/workflow';
import { wheelZoomFunc, zoomInit, resetImage } from '@/utils/antflow/zoom.js';
const { proxy } = getCurrentInstance();
let store = useStore();
const antflowDesignRef = ref(null);
const boxScaleRef = ref(null);
let nowVal = ref(100);
let viewConfig = computed(() => store.instanceViewConfig1)
let props = defineProps({
    previewConf: {
        type: Object,
        default: () => (null)
    }
});
const nodeConfig = ref(null);
const getFlowPreviewList = async (objData) => {
    let param = {
        processNumber: objData.processNumber,
        isStartPreview: false,
        isOutSideAccessProc: objData.isOutSideAccess || false,
        isLowCodeFlow: objData.isLowCodeFlow || false
    };
    if (props.previewConf) {
        param = props.previewConf;
    }
    proxy.$modal.loading();
    await getFlowPreview(param)
        .then(res => {
            proxy.$modal.closeLoading();
            if (res.code != 200) {
                proxy.$modal.msgError(res.errMsg || '获取流程预览失败');
                return;
            } else {
                let formatData = FormatPreviewUtils.formatSettings(res.data);
                nodeConfig.value = formatData;
            }
        })
        .catch(err => {
            proxy.$modal.closeLoading();
            proxy.$modal.msgError(err.errMsg || '获取流程预览失败');
        });
}
onMounted(async () => {
    zoomInit(antflowDesignRef, boxScaleRef, (val) => {
        nowVal.value = val
    });
    await getFlowPreviewList(viewConfig.value);
});
/** 页面放大 */
function zoomIn() {
    wheelZoomFunc({ scaleFactor: parseInt(nowVal.value) / 100 + 0.1, isExternalCall: true })
}

/** 页面缩小 */
function zoomOut() {
    wheelZoomFunc({ scaleFactor: parseInt(nowVal.value) / 100 - 0.1, isExternalCall: true })
}
/** 还原缩放比例 */
function zoomReset() {
    resetImage()
}
</script>
<style lang="scss" scoped>
@use "@/assets/styles/antflow/workflow.scss";

.end-node-circle {
    width: 20px;
    height: 20px;
    margin: auto;
    border-radius: 50%;
    background: #dbdcdc
}
</style>