<template>
    <!-- 添加消息模板对话框 -->
    <el-dialog title="添加消息模板" v-model="dialogVisible" width="650px" append-to-body>
        <el-form :model="templateForm" :rules="templateRules" ref="templateRef" label-width="130px" label-position="top"
            style="margin: 0 20px;">
            <el-row>
                <el-col :span="24">
                    <el-form-item label="模板名称" prop="name">
                        <el-input v-model="templateForm.name" placeholder="请输入唯一模板名称" />
                    </el-form-item>
                </el-col>
            </el-row>
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
                    <el-form-item label="通知内容" prop="systemContent">
                        <el-input v-model="templateForm.systemContent" type="textarea" placeholder="请输入通知内容"
                            :autosize="{ minRows: 5, maxRows: 5 }" style="height: 120px;" />
                    </el-form-item>
                </el-col>
            </el-row>
            <div class="mb-4">
                <el-button style="margin: 5px;" type="success" link v-for="btnTxt in wildcardList"
                    @click="templateForm.systemContent += btnTxt">
                    {{ btnTxt }}
                </el-button>
            </div>
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
                <el-button type="primary" @click="submitFormBtn">确 定</el-button>
                <el-button @click="closeDialog">取 消</el-button>
            </div>
        </template>
    </el-dialog>

</template>
<script setup>
import { ref, watch } from "vue";
import { getAllNoticeTypes, saveInformationTemp, getWildcardCharacter, getProcessEvents } from "@/api/workflow/flowMsgApi";
const { proxy } = getCurrentInstance();
const notifyTypeList = ref([]);
const wildcardList = ref([]);
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
    templateForm: {},
    templateRules: {
        name: [{ required: true, message: '', trigger: 'blur' }],
        systemTitle: [{ required: true, message: '', trigger: 'blur' }],
        systemContent: [{ required: true, message: '', trigger: 'blur' }],
        event: [{ required: true, message: '请选择事件类型', trigger: 'blur' }],
        jumpUrl: [{ required: true, message: '请选择跳转页面', trigger: 'blur' }],
    }
});
const { templateForm, templateRules } = toRefs(data);

watch(() => dialogVisible.value, (val) => {
    if (val) {
        reset();
        templateForm.value = props.formData;
        getAllNoticeTypesList();
        getWildcardCharacterList();
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
/** 获取所有通知类型列表 */
const getWildcardCharacterList = () => {
    getWildcardCharacter().then(res => {
        if (res && res.code == 200) {
            wildcardList.value = res.data.map(x => {
                return x.desc
            });
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



/** 提交条件模板表单 */
function submitFormBtn() {
    proxy.$refs["templateRef"].validate(valid => {
        if (valid) {
            proxy.$modal.loading();
            saveInformationTemp(templateForm.value).then(res => {
                if (res && res.code == 200) {
                    proxy.$modal.msgSuccess("添加成功");
                    emits("update:visible", false);
                } else {
                    proxy.$modal.msgError("添加失败" + res.errMsg);
                }
            }).catch(err => {
                console.log(err);
            });
            proxy.$modal.closeLoading();
        }
    });
}
/** 取消操作添加条件模板表单 */
function closeDialog() {
    reset();
    emits("update:visible", false);
}
/** 重置操作表单 */
function reset() {
    templateForm.value = {
        notifyType: [1],
        systemTitle: "工作流名称是:",
        systemContent: "",
    };
}
</script>