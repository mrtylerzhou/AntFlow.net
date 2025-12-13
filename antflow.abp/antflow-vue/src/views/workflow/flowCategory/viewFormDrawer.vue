<template>
    <!-- 设置通知消息 -->
    <el-drawer :append-to-body="true" v-model="drawerVisible" :size="620">
        <template #header>
            <h3 style="margin-bottom: -25px;">查看表单</h3>
        </template>

        <template #default>
            <div class="component">
                <component v-if="componentLoaded" :is="loadedComponent" :lfFormData="lfFormDataConfig"
                    :isPreview="true">
                </component>
            </div>
        </template>
        <template #footer>
            <div style="flex: auto">
                <el-button @click="closeDialog">取消</el-button>
            </div>
        </template>
    </el-drawer>
</template>
<script setup>
import { ref } from "vue";
import { getLowCodeFromCodeData } from '@/api/workflow/lowcodeApi';
import { loadDIYComponent, loadLFComponent } from '@/views/workflow/components/componentload.js';
const { proxy } = getCurrentInstance();
let lfFormDataConfig = ref(null);
let loadedComponent = ref(null);
let componentLoaded = ref(null);

let props = defineProps({
    visible: {
        type: Boolean,
        default: false,
    },
    viewFormData: {
        type: Object,
        default: () => { },
    }
});

let drawerVisible = computed({
    get() {
        return props.visible
    },
    set() {
        closeDialog()
    }
})
const emits = defineEmits(["update:visible"]);
watch(() => drawerVisible.value, (val) => {
    if (val) {
        handleLFTemp(props.viewFormData);
    }
});
/** 查看表单操作 */
const handleLFTemp = async (row) => {
    loadedComponent.value = null;
    proxy.$modal.loading();
    if (row.type == 'LF') {//低代码表单
        await getLowCodeFromCodeData(row.key).then(async (res) => {
            if (res.code == 200) {
                lfFormDataConfig.value = res.data
                loadedComponent.value = await loadLFComponent();
                componentLoaded.value = true;
            } else {
                proxy.$modal.msgWarning("未定义业务表单组件");
            }
        }).catch((err) => {
            proxy.$modal.msgWarning("未定义业务表单组件");
        });
    } else {//自定义表单
        loadedComponent.value = await loadDIYComponent(row.key)
            .catch((err) => { proxy.$modal.msgWarning(err); });
        componentLoaded.value = true;
    }
    proxy.$modal.closeLoading();
}

/** 取消操作添加条件模板表单 */
function closeDialog() {
    emits("update:visible", false);
}

</script>