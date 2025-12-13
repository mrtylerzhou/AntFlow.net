<template>
    <el-drawer :append-to-body="true" title="审批人设置" v-model="visible" :with-header="false" :size="680">
        <div class="el-drawer__header">
            <span class="drawer-title">审批人</span>
        </div>
        <el-tabs v-model="activeName" @tab-click="handleTabClick">
            <el-tab-pane label="审批人设置" name="approverStep">
                <div v-if="approverStepShow">
                    <div class="approver_content">
                        <div>
                            <el-radio-group v-model="approverConfig.setType" class="clear" @change="changeType">
                                <el-radio v-for="({ value, label }) in setTypes" :value="value">{{ label
                                    }}</el-radio>
                            </el-radio-group>
                        </div>
                        <div v-show="approverConfig.setType == 5">
                            <el-button type="primary" plain icon="Plus" @click="addApprover">添加/修改人员</el-button>
                            <div class="gap-2">
                                <el-tag v-for="(item, index) in approverConfig.nodeApproveList" :key="item.targetId"
                                    size="large" closable
                                    @close="$func.removeEle(approverConfig.nodeApproveList, item, 'targetId')">
                                    {{ item.name }}
                                </el-tag>
                            </div>
                        </div>

                        <div v-show="approverConfig.setType == 4">
                            <el-button type="primary" plain icon="Plus" @click="addRoleApprover">添加/修改角色</el-button>
                            <div class="gap-2">
                                <el-tag v-for="(item, index) in approverConfig.nodeApproveList" :key="item.targetId"
                                    size="large" closable
                                    @close="$func.removeEle(approverConfig.nodeApproveList, item, 'targetId')">
                                    {{ item.name }}
                                </el-tag>
                            </div>
                        </div>

                        <div v-show="approverConfig.setType == 14">
                            <el-button type="primary" @click="addRoleApprover">添加/修改部门</el-button>
                            <div class="gap-2">
                                <el-tag v-for="(item, index) in approverConfig.nodeApproveList" :key="item.targetId"
                                    size="large" closable
                                    @close="$func.removeEle(approverConfig.nodeApproveList, item, 'targetId')">
                                    {{ item.name }}
                                </el-tag>
                            </div>
                        </div>

                        <div v-if="approverConfig.setType == 3">
                            <div>
                                <span>发起人的：</span>
                                <el-select v-model="approverConfig.directorLevel" placeholder="请选择"
                                    style="width: 300px">
                                    <el-option v-for="item in directorMaxLevel" :key="item" :value="item"
                                        :label="item == 1 ? '直接主管' : '第' + item + '级' + '主管'" />
                                </el-select>
                            </div>
                            <p class="tip">找不到主管时，由上级主管代审批</p>
                        </div>

                        <div v-if="approverConfig.setType == 6">
                            <div>
                                <span>HRBP选择：</span>
                                <el-select v-model="checkedHRBP" placeholder="请选择" style="width: 300px">
                                    <el-option v-for="item in hrbpOptions" :key="item.value" :label="item.label"
                                        :value="item.value" />
                                </el-select>
                            </div>
                        </div>
                        <div class="approver_text" v-if="approverConfig.setType == 12">
                            <p class="tip">该审批节点设置“发起人自己”后，审批人默认为发起人</p>
                        </div>
                        <div class="approver_text" v-if="approverConfig.setType == 13">
                            <p class="tip">该审批节点设置“直属领导”后，审批人默认为发起人的直属领导</p>
                        </div>
                        <div class="approver_text" v-if="approverConfig.setType == 7">
                            <p class="tip">该审批节点设置“发起人自选审批人”后，审批人在发起业务表单时由发起人选择</p>
                        </div>
                    </div>
                    <div class="approver_block">
                        <p>✍多人审批时采用的审批方式</p>
                        <el-radio-group v-model="approverConfig.signType" class="clear">
                            <el-radio :value="1">会签（需所有审批人同意，不限顺序）</el-radio>
                            <br />
                            <el-radio :value="2">或签（只需一名审批人同意或拒绝即可）</el-radio>
                            <br />
                            <el-radio :value="3" v-if="approverConfig.setType == 5">顺序会签（需要所有审批人同意，根据前端传入的顺序）</el-radio>
                        </el-radio-group>
                    </div>
                    <div class="approver_block">
                        <p>✍审批人为空时</p>
                        <el-radio-group v-model="approverConfig.noHeaderAction" class="clear">
                            <el-radio :value="0">不允许发起</el-radio>
                            <br />
                            <!--注意,这里的跳过指的是不生成审批任务节点,即流程图里没有当前缺失审批人的节点-->
                            <el-radio :value="1">跳过</el-radio>
                            <br />
                            <!--转给管理员需实现BpmnProcessAdminProvider接口-->
                            <el-radio :value="2">转交给审核管理员</el-radio>
                        </el-radio-group>
                    </div>
                </div>
            </el-tab-pane>
            <el-tab-pane lazy label="按钮权限设置" name="buttonStep">
                <p>【审批页面】按钮权限显示控制</p>
                <el-checkbox-group class="clear" v-model="checkApprovalPageBtns">
                    <el-checkbox style="margin: 6px 0;width: 100%;height: 45px;" border
                        v-for="opt in approvalPageButtons" :value="opt.value" :disabled="opt.type === 'default'"
                        @change="handleCheckedButtonsChange(opt.value)">
                        【{{ opt.label }}】
                        <span class="opt-description">
                            {{ opt.description }}
                        </span>
                    </el-checkbox>
                </el-checkbox-group>

                <div v-if="afterSignUpWayVisible">
                    <el-radio-group v-model="approvalBtnSubOption"
                        @change="handleApprovalBtnSubOption(approvalBtnSubOption)" class="clear">
                        <el-radio :value="1" class="auth-btn" border>
                            【顺序会签】
                            <span class="opt-description">
                                多个会签人员，依次进行审批
                            </span>
                        </el-radio>
                        <el-radio :value="2" class="auth-btn" border>
                            【会签】
                            <span class="opt-description">
                                多个会签人员，同步进行审批
                            </span>
                        </el-radio>

                        <el-radio :value="3" class="auth-btn" border>
                            【或签】
                            <span class="opt-description">
                                只需一名审批人同意或拒绝即可
                            </span>
                        </el-radio>
                        <el-radio :value="9" class="auth-btn" border>
                            【回到加批人】
                            <span class="opt-description">
                                只能是顺序会签，加批人审批完之后，会回到本节点的审批人再次审批
                            </span>
                        </el-radio>
                    </el-radio-group>
                </div>
            </el-tab-pane>
            <el-tab-pane lazy label="表单权限设置" name="formStep">
                <form-perm-conf v-if="formStepShow" default-perm="R" v-model:formItems="formItems"
                    @changePermVal="changePermVal" />
            </el-tab-pane>
            <el-tab-pane lazy label="通知设置" name="noticeStep">
                <notice-conf v-if="noticeStepShow" :formData="templateVos" @changeFlowMsgSet="handleFlowMsgSet" />
            </el-tab-pane>
        </el-tabs>
        <div class="demo-drawer__footer clear">
            <el-button type="primary" @click="saveApprover">确 定</el-button>
            <el-button @click="closeDrawer">取 消</el-button>
        </div>
        <select-user-dialog v-model:visible="approverUserVisible" :data="checkedUserList" @change="sureUserApprover" />
        <select-role-dialog v-model:visible="approverRoleVisible" :data="checkedRoleList" @change="sureRoleApprover" />
    </el-drawer>
</template>
<script setup>
import { ref, watch, computed } from 'vue';
import $func from '@/utils/antflow/index';
import { setTypes, hrbpOptions, approvalPageButtons } from '@/utils/antflow/const';
import { useStore } from '@/store/modules/workflow';
import selectUserDialog from '../dialog/selectUserDialog.vue';
import selectRoleDialog from '../dialog/selectRoleDialog.vue';
import formPermConf from "./permConfig/FormPermConf.vue";
import noticeConf from "./noticeConfig/index.vue";
const { proxy } = getCurrentInstance();
let store = useStore()
let props = defineProps({
    directorMaxLevel: {
        type: Number,
        default: 0
    }
});
let approverConfig = ref({});
let approverUserVisible = ref(false);
let approverRoleVisible = ref(false);
let checkedRoleList = ref([]);
let checkedUserList = ref([]);
let checkApprovalPageBtns = ref([]);
let checkedHRBP = ref('');
let approvalPageBtns = ref([]);
let afterSignUpWayVisible = computed(() => approverConfig.value?.isSignUp == 1);
let approvalBtnSubOption = ref(1);

let formItems = ref([]);
let templateVos = ref([]);
let activeName = ref('approverStep');
let approverStepShow = ref(true);
let formStepShow = ref(false);
let noticeStepShow = ref(false);
let approverConfig1 = computed(() => store.approverConfig1);
let approverDrawer = computed(() => store.approverDrawer);
let visible = computed({
    get() {
        handleTabClick({ paneName: "approverStep" })
        return approverDrawer.value
    },
    set() {
        closeDrawer()
    }
});
/**页面加载监听事件 */
watch(approverConfig1, (val) => {
    if (val.value.nodeType == 7) {//nodeType == 7 是并行审批
        let currParallel = val.value.parallelNodes[val.value.index]
        approverConfig.value = currParallel;
        formItems.value = currParallel.lfFieldControlVOs || [];
        templateVos.value = currParallel.templateVos || [];
        checkApprovalPageBtns.value = currParallel.buttons?.approvalPage;
    }
    else {
        approverConfig.value = val.value;
        formItems.value = val.value.lfFieldControlVOs || [];
        templateVos.value = val.value.templateVos || [];
        checkApprovalPageBtns.value = val.value.buttons?.approvalPage;
    }
});

/**监听 approverConfig 对象*/
watch(approverConfig, (val) => {
    approvalPageBtns.value = val.buttons?.approvalPage;
    if (val.nodeProperty == 6) {//nodeProperty == 6 指 HRBP
        checkedHRBP.value = val.property.hrbpConfType
    }
    if (approverConfig.value?.property?.afterSignUpWay == 1) {
        approvalBtnSubOption.value = 9;//审批完之后，会回到本节点的审批人再次审批
    } else {
        approvalBtnSubOption.value = approverConfig.value?.property?.signUpType;
    }
}, { deep: true });
/**处理HRBP选项 */
watch(checkedHRBP, (val) => {
    if (approverConfig.value.setType != 6) {
        return;
    }
    approverConfig.value.property.hrbpConfType = val;
    let labelName = hrbpOptions.find(item => item.value == val)?.label;
    if (labelName) {
        approverConfig.value.nodeApproveList = [{ "type": 6, "targetId": val, "name": labelName }];
    }
});
/**选择审批人类型更改事件 */
const changeType = (val) => {
    approverConfig.value.nodeApproveList = [];
    approverConfig.value.signType = 1;
    approverConfig.value.noHeaderAction = 0;
    checkedHRBP.value = '';
    if (val == 3) {
        approverConfig.value.directorLevel = 1;
    }
}
/**添加审批人 */
const addApprover = () => {
    approverUserVisible.value = true;
    checkedUserList.value = approverConfig.value.nodeApproveList
}
/**添加审批角色 */
const addRoleApprover = () => {
    approverRoleVisible.value = true;
    checkedRoleList.value = approverConfig.value.nodeApproveList
}
/**选择审批人确认按钮 */
const sureUserApprover = (data) => {
    approverConfig.value.nodeApproveList = data;
    approverUserVisible.value = false;
}
/**选择角色确认按钮 */
const sureRoleApprover = (data) => {
    approverConfig.value.nodeApproveList = data;
    approverRoleVisible.value = false;
}
/**处理权限按钮变更事件 */
const handleCheckedButtonsChange = (val) => {
    if (proxy.isEmpty(approvalPageBtns)) return;
    if (proxy.isEmptyArray(approvalPageBtns.value)) return;
    const index = approvalPageBtns.value.indexOf(val);
    index < 0 ? approvalPageBtns.value.push(val) : approvalPageBtns.value.splice(index, 1);
    const isAddStep = approvalPageBtns.value.indexOf(19);
    if (isAddStep >= 0) {
        approverConfig.value.isSignUp = 1;
    } else {
        approverConfig.value.isSignUp = 0;
    }
}

/**处理加批按钮 子操作 */
const handleApprovalBtnSubOption = (val) => {
    //signType 指的是当前节点审批方式 1:会签，2:或签，3:顺序会签
    //signUpType 指的是加批审批操作 1:顺序会签，2:会签，3:或签 
    //val加批类型 1:顺序会签，2:会签，3:或签 特别 9指: 回到加批人，则afterSignUpWay赋值为1，signUpType赋值为1
    approverConfig.value.property.afterSignUpWay = val && val == 9 ? 1 : 2;
    approverConfig.value.property.signUpType = val && val == 9 ? 1 : val;
}

/**条件抽屉的确认 */
const saveApprover = () => {
    approverConfig.value.nodeDisplayName = $func.setApproverStr(approverConfig.value);
    approverConfig.value.error = !$func.setApproverStr(approverConfig.value);
    console.log('保存审批人配置==', JSON.stringify(approverConfig1.value));
    store.setApproverConfig({
        value: approverConfig1.value.value,
        flag: true,
        id: approverConfig1.value.id
    })
    closeDrawer()
}
/**关闭抽屉 */
const closeDrawer = () => {
    store.setApprover(false)
}
/**低代码表单字段权限 */
const changePermVal = (data) => {
    approverConfig.value.lfFieldControlVOs = data;
}
/**消息设置 */
const handleFlowMsgSet = (data) => {
    approverConfig.value.templateVos = !proxy.isEmpty(data) ? [data] : [];
    store.setApproverConfig({
        value: approverConfig1.value.value,
        flag: true,
        id: approverConfig1.value.id
    })
}
/**
 * 切换tab
 * @param tab 当前tab
 * @param event 
 */
const handleTabClick = (tab, event) => {
    activeName.value = tab.paneName;
    if (tab.paneName == 'formStep') {
        formStepShow.value = true;
    } else {
        formStepShow.value = false;
    }
    if (tab.paneName == 'noticeStep') {
        noticeStepShow.value = true;
    } else {
        noticeStepShow.value = false;
    }
}

</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";

.el-drawer__header {
    margin-bottom: 5px !important;
}

.el-tabs {
    margin-left: 20px !important;
}

.approver_content {
    min-height: 250px;
    padding-bottom: 10px;
    border-bottom: 1px solid #f2f2f2;
    font-size: 14px;
}

.approver_content,
.approver_block {
    padding-top: 10px;

    .el-radio-group {
        display: unset;
    }

    .el-radio {
        width: 27%;
        margin-bottom: 20px;
        height: 16px;
    }
}

.tip {
    margin: 10px 0 22px 0;
    font-size: 12px;
    line-height: 16px;
    color: #f8642d;
    font-size: 16px;
}

.approver_text {
    padding: 28px 0px;
}

.opt-description {
    font-size: smaller;
    color: gray;
}

.auth-btn {
    margin-top: 6px;
    width: 95%;
    height: 45px;
}

.gap-2 {
    display: flex;
    gap: 0.5rem;
    flex-wrap: wrap;
    margin-top: 10px;
}
</style>