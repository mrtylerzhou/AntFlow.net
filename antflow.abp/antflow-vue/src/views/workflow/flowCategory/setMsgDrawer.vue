<template>
    <!-- 设置通知消息 -->
    <el-drawer :append-to-body="true" v-model="dialogVisible" :size="620">
        <template #header>
            <h3 style="margin-bottom: -25px;">设置通知消息</h3>
        </template>

        <template #default>
            <div>
                <el-radio-group v-model="tabPosition" style="margin-bottom: 30px">
                    <el-radio-button value="defaultNotice">默认通知设置</el-radio-button>
                    <el-radio-button value="seniorNotice">高级通知设置</el-radio-button>
                </el-radio-group>
            </div>
            <div v-if="tabPosition === 'defaultNotice'">
                <el-form ref="templateRef" label-width="130px" label-position="top" style="margin: 0 20px;">
                    <el-form-item label="默认通知" prop="checkedMsgSendTypeList">
                        <template #label>
                            <span>
                                默认通知
                                <el-tooltip :content="tooltipContent" placement="top">
                                    <el-icon><question-filled /></el-icon>
                                </el-tooltip>
                            </span>
                        </template>
                        <el-checkbox-group v-model="checkedMsgSendTypeList">
                            <el-checkbox style="margin: 5px;" v-for="(item, index) in messageSendTypeList"
                                :value="item.id" :key="item.id" border>
                                {{ item.name }}
                                <msgIcon v-model:iconValue="item.id" viewValue="primary" />
                            </el-checkbox>
                        </el-checkbox-group>
                    </el-form-item>
                </el-form>
            </div>
            <div v-if="tabPosition === 'seniorNotice'">
                <el-row class="mb8">
                    <el-col>
                        <el-button type="primary" plain icon="CirclePlus" @click="openSeniorSet">新增</el-button>
                    </el-col>
                </el-row>
                <el-row class="mb8">
                    <el-col>
                        <el-table :data="msgTableData" style="width: 100%;height: 40vh;">
                            <el-table-column prop="messageSendTypeList" label="通知类型" width="120">
                                <template #default="item">
                                    <span v-for="(rowItem, index) in item.row.messageSendTypeList" :key="index">
                                        <msgIcon :iconValue="rowItem.id" viewValue="primary" />
                                    </span>
                                </template>
                            </el-table-column>
                            <el-table-column prop="event" label="事件类型" width="120">
                                <template #default="item">
                                    {{ getEventType(item.row.event) }}
                                </template>
                            </el-table-column>
                            <el-table-column prop="templateName" label="模板名称" width="220" />
                            <el-table-column prop="informIdList" label="通知人">
                                <template #default="item">
                                    {{ getNoticeUserType(item.row.informIdList) }}
                                </template>
                            </el-table-column>
                            <el-table-column label="操作" fixed="right" width="150" align="center"
                                class-name="small-padding fixed-width">
                                <template #default="scope">
                                    <el-button link type="success" @click="handleDelete(scope.row)">删除</el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-col>
                </el-row>
            </div>

        </template>
        <template #footer>
            <div style="flex: auto">
                <el-button @click="closeDialog">取消</el-button>
                <el-button type="primary" @click="submitDialog">确认</el-button>
            </div>
        </template>
    </el-drawer>
    <set-senior-msg v-if="noticeSeniorSetShow" v-model:visible="noticeSeniorSetShow"
        @changeSet="handleFlowMsgSet"></set-senior-msg>
</template>
<script setup>
import { ref, watch } from "vue";
import msgIcon from '@/components/Workflow/components/msgIcon.vue';
import { saveTaskMgmt } from "@/api/workflow/flowMsgApi";
import SetSeniorMsg from './setSeniorMsg.vue';
import { noticeUserList, messageSendTypeList, eventTypeList } from '@/utils/antflow/const';
const { proxy } = getCurrentInstance();
const checkedMsgSendTypeList = ref([]);
const noticeSeniorSetShow = ref(false);
const tabPosition = ref('defaultNotice');

let props = defineProps({
    visible: {
        type: Boolean,
        default: false,
    },
    formMsgData: {
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
const emits = defineEmits(["update:visible", "refresh"]);
const msgTableData = ref([]);
const tooltipContent = ref('注：默认通知将会采用默认消息模板，审批人为默认通知人。如果有高级设置，则以高级设置为主');
watch(() => dialogVisible.value, (val) => {
    if (val) {
        checkedMsgSendTypeList.value = props.formMsgData.processNotices.filter(c => c.active).map(item => {
            return item.id;
        });
        msgTableData.value = props.formMsgData.templateVos || [];
    }
});

function submitDialog() {
    let params = {
        processKey: props.formMsgData.formCode,
        notifyTypeIds: checkedMsgSendTypeList.value,
        templateVos: msgTableData.value
    }
    saveTaskMgmt(params).then(res => {
        if (res && res.code == 200) {
            proxy.$modal.msgSuccess("保存成功");
            emits("refresh", props.formMsgData.flowType);
            closeDialog();
        } else {
            proxy.$modal.msgError("保存失败" + res.errMsg);
        }
    }).catch(err => {
        console.log(err);
    });
}

function openSeniorSet() {
    noticeSeniorSetShow.value = true;
}
/**消息设置 */
const handleFlowMsgSet = (data) => {
    if (proxy.isEmpty(data)) return;

    const propsToCheck = ['messageSendTypeList', 'event', 'templateId', 'informIdList'];
    if (proxy.hasEmptyValue(data, propsToCheck)) {
        proxy.$modal.msgError("请选择完整的消息设置");
        return;
    }
    const index = msgTableData.value.findIndex(item => item.event === data.event);
    if (index !== -1) {
        // 如果找到具有相同 event 的条目，则更新该条目
        msgTableData.value[index] = { ...msgTableData.value[index], ...data };
    } else {
        // 如果没有找到，则添加新的条目
        msgTableData.value.push(data);
    }
}

/**消息设置 */
const handleDelete = (row) => {
    msgTableData.value = msgTableData.value.filter(x => x.event !== row.event)
}

const getNoticeUserType = (param) => {
    if (proxy.isEmptyArray(param)) return '';
    let ret = noticeUserList.filter(item => {
        return param.includes(item.value);
    })?.map(item => item.label).join(', ');
    return ret;
}

const getEventType = (param) => {
    if (proxy.isEmptyArray(param)) return '';
    return eventTypeList.filter(item => {
        return item.id == param;
    })?.map(item => item.name).join(', ');
}

/** 取消操作添加条件模板表单 */
function closeDialog() {
    emits("update:visible", false);
}

</script>