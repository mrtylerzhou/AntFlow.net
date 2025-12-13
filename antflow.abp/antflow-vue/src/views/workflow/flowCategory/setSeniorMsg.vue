<template>
    <!-- 设置通知消息 -->
    <el-dialog title="高级设置" v-model="dialogVisible" width="550px" append-to-body>
        <notice-conf @changeFlowMsgSet="handleFlowMsgSet" />
        <template #footer>
            <div class="dialog-footer">
                <el-button type="primary" @click="submitDialog">确认</el-button>
                <el-button @click="closeDialog">关闭</el-button>
            </div>
        </template>
    </el-dialog>
</template>
<script setup>
import { ref } from "vue";
import noticeConf from "@/components/Workflow/drawer/noticeConfig/index.vue";
const flowMasgVo = ref(null);

let props = defineProps({
    visible: {
        type: Boolean,
        default: false,
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
const emits = defineEmits(["update:visible", "changeSet"]);



/** 确认弹框 */
function submitDialog() {
    emits("changeSet", flowMasgVo.value);
    closeDialog()
}
/**消息设置 */
const handleFlowMsgSet = (data) => {
    flowMasgVo.value = data;
}

/** 取消操作添加条件模板表单 */
function closeDialog() {
    emits("update:visible", false);
} 
</script>