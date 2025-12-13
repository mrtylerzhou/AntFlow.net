<template>
    <!-- 查看消息模板对话框 -->
    <el-dialog title="查看消息模板" v-model="dialogVisible" width="650px" append-to-body>
        <el-form :model="templateForm" ref="templateRef" :disabled="true" label-width="130px" label-position="top"
            style="margin: 0 20px;">
            <el-row>
                <el-col :span="24">
                    <el-form-item label="事件类型" prop="event">
                        <el-select v-model="templateForm.event" placeholder="请选择事件类型" style="width: 240px">
                            <el-option v-for="item in eventOptions" :key="item.id" :label="item.name"
                                :value="item.id" />
                        </el-select>

                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="模板名称" prop="name">
                        <el-input v-model="templateForm.name" placeholder="请输入唯一模板名称" />
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="主题" prop="systemTitle">
                        <el-input v-model="templateForm.systemTitle" type="textarea" placeholder="请输入主题"
                            :autosize="{ minRows: 3, maxRows: 3 }" style="height: 55px;" />
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="通知内容" prop="systemContent">
                        <el-input v-model="templateForm.systemContent" type="textarea" placeholder="请输入通知内容"
                            :autosize="{ minRows: 5, maxRows: 5 }" style="height: 120px;" />
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="跳转页面" prop="jumpUrl">
                        <el-radio-group v-model="templateForm.jumpUrl">
                            <el-radio-button :value=1>流程审批页</el-radio-button>
                            <el-radio-button :value=2>流程查看页</el-radio-button>
                            <el-radio-button :value=3>流程待办页</el-radio-button>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>
        </el-form>
        <template #footer>
            <div class="dialog-footer">
                <el-button @click="closeDialog">关闭</el-button>
            </div>
        </template>
    </el-dialog>

</template>
<script setup>
import { ref, watch } from "vue";
import { getAllNoticeTypes, getProcessEvents } from "@/api/workflow/flowMsgApi";
const { proxy } = getCurrentInstance();
const notifyTypeList = ref([]);
const eventOptions = ref([]);
let props = defineProps({
    visible: {
        type: Boolean,
        default: false,
    },
    formData: {
        type: Object,
        default: () => { },
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
const emits = defineEmits(["update:visible"]);
const data = reactive({
    templateForm: {
        systemTitle: "工作流名称是:",
        systemContent: "",
        event: null,
        name: "",
    }
});
const { templateForm } = toRefs(data);

watch(() => dialogVisible.value, (val) => {
    if (val) {
        reset();
        templateForm.value = props.formData;
        getAllNoticeTypesList();
        getProcessEventsList();
    }
});

/** 获取所有通知类型列表 */
const getAllNoticeTypesList = () => {
    getAllNoticeTypes().then(res => {
        if (res && res.code == 200) {
            notifyTypeList.value = res.data;
        } else {
            proxy.$modal.msgError("获取通知类型失败" + res.errMsg);
        }
    }).catch(err => {
        console.log(err);
    });
};
/** 获取事件列表 */
const getProcessEventsList = () => {
    getProcessEvents().then(res => {
        if (res && res.code == 200) {
            eventOptions.value = res.data;
        } else {
            proxy.$modal.msgError("获取事件列表失败" + res.errMsg);
        }
    }).catch(err => {
        console.log(err);
    });
};
/** 取消操作添加条件模板表单 */
function closeDialog() {
    reset();
    emits("update:visible", false);
}
/** 重置操作表单 */
function reset() {
    templateForm.value = {
        systemTitle: null,
        systemContent: null,
        event: null,
        name: null,
    };
}
</script>