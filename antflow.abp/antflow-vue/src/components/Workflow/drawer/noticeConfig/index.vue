<template>
    <div>
        <el-form ref="templateRef" label-width="130px" label-position="top"
            style="margin-bottom: 10px;border:1px solid #bfcbd9;padding:20px;">
            <el-row>
                <el-col :span="24">
                    <el-form-item label="通知类型" prop="checkedMsgSendTypeList">
                        <el-checkbox-group v-model="checkedMsgSendTypeList">
                            <el-checkbox style="margin: 5px;" v-for="(item, index) in messageSendTypeList"
                                :value="item.id" :key="item.id" border>
                                {{ item.name }}
                                <msgIcon v-model:iconValue="item.id" viewValue="primary" />
                            </el-checkbox>
                        </el-checkbox-group>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="事件类型" prop="event">
                        <el-select v-model="templateForm.event" placeholder="请选择事件类型" style="width: 350px">
                            <el-option v-for="item in eventOptions" :key="item.id" :label="item.name"
                                :value="item.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="消息模板" prop="templateId">
                        <el-button type="primary" plain icon="Plus" @click="dialogMsgVisible = true">选择消息模板</el-button>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <el-form-item>
                        <p v-for="tag in selectValues">
                            <el-tag v-if="tag.name" :key="tag.id" type="warning" size="large">
                                {{ tag.num }} {{ tag.name }}
                            </el-tag>
                            <el-tooltip v-if="tag.name" content="查看消息模板详情" placement="top">
                                <el-button icon="Search" circle plain type="warning"
                                    @click="handleReverwTemplate(tag.id)" />
                            </el-tooltip>
                        </p>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item label="通知人选择" prop="name">
                        <el-radio-group v-model="noticeUserType" class="clear" @change="changeUserType">
                            <el-radio v-for="({ value, label }) in noticeUserList" :value="value">{{ label }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="24">
                    <el-form-item prop="name">
                        <div v-show="noticeUserType == 5">
                            <el-button type="primary" plain icon="Plus"
                                @click="chooseUserVisible = true">添加/修改人员</el-button>
                        </div>
                        <div v-show="noticeUserType == 6">
                            <el-button type="primary" plain icon="Plus"
                                @click="chooseRoleVisible = true">添加/修改角色</el-button>
                        </div>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <div class="gap-2">
                        <el-tag v-for="userTag in templateForm.empList" :key="userTag.targetId" type="success"
                            size="large" closable @close="handleRemoveUser(userTag)">
                            {{ userTag.name }}
                        </el-tag>

                        <el-tag v-for="roleTag in templateForm.roleList" :key="roleTag.targetId" type="success"
                            size="large" closable @close="handleRemoveRole(roleTag)">
                            {{ roleTag.name }}
                        </el-tag>
                    </div>
                </el-col>
            </el-row>
        </el-form>
        <el-button type="primary" plain icon="Refresh" @click="resetForm">重置通知</el-button>
        <flow-msg-templete v-model:visible="dialogMsgVisible" v-model:checkedData="selectValues"
            @change="saveFlowMsgTempDialog" />
        <select-user-dialog v-model:visible="chooseUserVisible" :data="templateForm.empList" @change="sureUserDialog" />
        <select-role-dialog v-model:visible="chooseRoleVisible" :data="templateForm.roleList"
            @change="sureRoleDialog" />
        <msg-view-dialog v-model:visible="dialogMsgViewVisible" v-model:formData="selectTemplateForm" />
    </div>
</template>
<script setup>
import { onMounted, ref, watchEffect, onBeforeMount } from "vue";
import { getAllNoticeTypes, getProcessEvents, getInformationTemplateById } from "@/api/workflow/flowMsgApi";
import flowMsgTemplete from "./flowMsgTemplateDialog.vue";
import msgViewDialog from "./msgViewDialog.vue";
import selectUserDialog from '../../dialog/selectUserDialog.vue';
import selectRoleDialog from '../../dialog/selectRoleDialog.vue';
import msgIcon from '../../components/msgIcon.vue';
import { messageSendTypeList, noticeUserList } from '@/utils/antflow/const';
const { proxy } = getCurrentInstance();
const notifyTypeList = ref([]);
const eventOptions = ref([]);


const dialogMsgVisible = ref(false);
const dialogMsgViewVisible = ref(false);
const chooseUserVisible = ref(false);
const chooseRoleVisible = ref(false);
const noticeUserType = ref("1");

const selectValues = ref([]);
const selectTemplateForm = ref(null);
const checkedMsgSendTypeList = ref([]);
const templateForm = ref({
    nodeId: "",
    messageSendTypeList: [],
    event: "",
    informIdList: [],
    empList: [],
    roleList: [],
    templateId: ""
});

let props = defineProps({
    formData: {
        type: Array,
        default: [],
    }
});
const emits = defineEmits(["update:visible", "changeFlowMsgSet"]);
//加载的时候判断，赋默认值
onBeforeMount(() => {
    templateForm.value = Array.isArray(props.formData) && props.formData.length > 0 && props.formData[0].event > 0 ? props.formData[0] : templateForm.value;
    checkedMsgSendTypeList.value = templateForm.value.messageSendTypeList.map(item => {
        return item.id;
    });
    selectValues.value = [{
        id: templateForm.value.templateId,
        name: templateForm.value.templateName
    }]
    noticeUserType.value = templateForm.value.informIdList[0];
    templateForm.value.informList = []
})

watchEffect(() => {
    templateForm.value.messageSendTypeList = checkedMsgSendTypeList.value.map(item => {
        return {
            id: item
        }
    });

    const propsToCheck = ['messageSendTypeList', 'event', 'templateId', 'informIdList'];
    if (proxy.hasEmptyValue(templateForm.value, propsToCheck)) {
        emits('changeFlowMsgSet')
    } else {
        emits('changeFlowMsgSet', templateForm.value)
    }
})

onMounted(() => {
    //getAllNoticeTypesList();
    getProcessEventsList();
})

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

const getSelectTemplateById = (id) => {
    getInformationTemplateById(id).then(res => {
        if (res && res.code == 200) {
            selectTemplateForm.value = res.data;
            dialogMsgViewVisible.value = true;
        } else {
            proxy.$modal.msgError("获取消息模板失败" + res.errMsg);
        }
    }).catch(err => {
        console.log(err);
    });
}

/**选择审批人类型更改事件 */
const changeUserType = (val) => {
    templateForm.value.informIdList = [val];
    templateForm.value.empList = [];
    templateForm.value.roleList = [];
}
/** 消息模板选择 */
const saveFlowMsgTempDialog = (data) => {
    selectValues.value = data;
    templateForm.value.templateId = data[0]?.id;
    templateForm.value.templateName = data[0]?.name;
}
/**
 * 选择人员
 * @param data 
 */
const sureUserDialog = (data) => {
    templateForm.value.empList = data.map(item => {
        return {
            id: item.targetId,
            name: item.name
        }
    });
}
/**
 * 选择角色
 * @param data 
 */
const sureRoleDialog = (data) => {
    templateForm.value.roleList = data.map(item => {
        return {
            id: item.targetId,
            name: item.name
        }
    });
}
/**
 * 移除人员
 */
const handleRemoveUser = (data) => {
    templateForm.value.empList = templateForm.value.empList
        .filter(item => item.id != data.id);
}
/**
 * 移除角色
 */
const handleRemoveRole = (data) => {
    templateForm.value.roleList = templateForm.value.roleList
        .filter(item => item.id != data.id);
}

const handleReverwTemplate = (id) => {
    getSelectTemplateById(id);
}

const resetForm = () => {
    checkedMsgSendTypeList.value = [];
    selectValues.value = [];
    noticeUserType.value = 1;
    templateForm.value = {
        nodeId: undefined,
        messageSendTypeList: [],
        event: undefined,
        informIdList: [],
        empList: [],
        roleList: [],
        templateId: undefined
    };
} 
</script>

<style lang="scss" scoped>
.gap-2 {
    display: flex;
    gap: 0.5rem;
}
</style>