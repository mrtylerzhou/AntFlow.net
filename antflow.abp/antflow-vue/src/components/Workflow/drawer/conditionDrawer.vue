<!--
 * @Date: 2023-03-15 14:44:17
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-05-24 15:20:48
 * @FilePath: /ant-flow/src/components/drawer/conditionDrawer.vue
-->

<template>
    <el-drawer :append-to-body="true" title="条件设置" v-model="visible" class="set_condition" :with-header="false"
        :size="680">
        <span class="drawer-title">条件设置</span>
        <template #header="{ titleId, titleClass }">
            <h3 :id="titleId" :class="titleClass">条件设置</h3>
            <select v-model="originalConfigData.priorityLevel" class="priority_level">
                <option v-for="item in conditionsConfig.conditionNodes.length" :value="item" :key="item">优先级{{ item }}
                </option>
            </select>
        </template>
        <el-container>
            <el-main>
                <div class="demo-drawer__content">
                    <div class="condition_content drawer_content">
                        <p class="tip">当审批单满足以下条件时进入此流程
                            <el-text class="ml10" type="warning"
                                v-if="originalConfigData.conditionList.length > 1">条件组关系：
                                且<el-switch v-model="originalConfigData.groupRelation" />或
                            </el-text>
                        </p>
                        <div v-for="(conditionGroupArray, conditionGroupIdx) in originalConfigData.conditionList">
                            <el-card class="mb10" style="max-width: 680px">
                                <template #header>
                                    <div class="card-header">
                                        <div class="l">
                                            <span>条件组{{ conditionGroupIdx + 1 }}</span>
                                        </div>
                                        <div class="l pl10" v-if="conditionGroupArray.length > 1">
                                            <el-text class="ml10" type="warning">组内条件关系：
                                                且<el-switch
                                                    v-model="conditionGroupArray[conditionGroupIdx].condRelation" />或
                                            </el-text>
                                        </div>
                                        <div @click="deleteConditionGroup(conditionGroupIdx)" class="r clickable">
                                            <el-icon class="branch-delete-icon"><el-icon-delete /></el-icon>
                                        </div>
                                    </div>
                                </template>
                                <ul>
                                    <li v-for="(item, index) in conditionGroupArray" :key="index" class="l">
                                        <div v-if="item && item.fieldTypeName">
                                            <span class="ellipsis">{{ item.type == 1 ? '发起人' : item.showName }}：</span>
                                            <div v-if="item.type == 1">
                                                <p :class="originalConfigData.nodeApproveList.length > 0 ? 'selected_list' : ''"
                                                    @click.self="addConditionRole" style="cursor:text">
                                                    <span v-for="(item1, index1) in originalConfigData.nodeApproveList"
                                                        :key="index1">
                                                        {{ item1.name }}<img
                                                            src="@/assets/images/antflow/add-close1.png"
                                                            @click="$func.removeEle(originalConfigData.nodeApproveList, item1, 'targetId')">
                                                    </span>
                                                    <input type="text" placeholder="请选择具体人员/角色/部门"
                                                        v-if="originalConfigData.nodeApproveList.length == 0"
                                                        @click="addConditionRole">
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'input'">
                                                <p class="check_box">
                                                    <input v-model="item.optType" hidden>
                                                    <input type="text" :placeholder="'请输入' + item.showName"
                                                        v-model="item.zdy1">
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'date'">
                                                <p>
                                                    <el-select
                                                        :style="'width:' + (item.optType == 6 ? 350 : 105) + 'px'"
                                                        @change="changeOptType(item)" v-model="item.optType">
                                                        <el-option v-for="itemOpt in optTypes" :key="itemOpt.value"
                                                            :label="itemOpt.label" :value="itemOpt.value" />
                                                    </el-select>
                                                    <el-date-picker v-if="item.optType != 6" v-model="item.zdy1"
                                                        type="date" :placeholder="'请选择' + item.showName"
                                                        format="YYYY-MM-DD" />
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'time'">
                                                <p>
                                                    <el-select
                                                        :style="'width:' + (item.optType == 6 ? 350 : 105) + 'px'"
                                                        @change="changeOptType(item)" v-model="item.optType">
                                                        <el-option v-for="itemOpt in optTypes" :key="itemOpt.value"
                                                            :label="itemOpt.label" :value="itemOpt.value" />
                                                    </el-select>
                                                    <el-time-picker v-if="item.optType != 6" v-model="item.zdy1"
                                                        :placeholder="'请选择' + item.showName" />
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'switch'">
                                                <p class="check_box">
                                                    <el-switch v-model="item.zdy1" />
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'radio'">
                                                <p class="check_box">
                                                    {{ item.fieldTypeName }}
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'checkbox'">
                                                <p class="check_box">
                                                    <a :class="$func.toggleStrClass(item, item1.key) && 'active'"
                                                        @click="toStrChecked(item, item1.key)"
                                                        v-for="(item1, index1) in JSON.parse(item.fixedDownBoxValue)"
                                                        :key="index1">{{
                                                            item1.value }}</a>
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'select' && item.multiple">
                                                <p class="check_box" v-if="item.fixedDownBoxValue">
                                                    <el-select :placeholder="'请选择' + item.showName" v-model="item.zdy1"
                                                        multiple :multiple-limit="item.multipleLimit">
                                                        <el-option v-for="itemOpt in JSON.parse(item.fixedDownBoxValue)"
                                                            :key="itemOpt.key" :label="itemOpt.value"
                                                            :value="itemOpt.key" />
                                                    </el-select>
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'select' && !item.multiple">
                                                <p class="check_box" v-if="item.fixedDownBoxValue">
                                                    <el-select :placeholder="'请选择' + item.showName" v-model="item.zdy1">
                                                        <el-option v-for="itemOpt in JSON.parse(item.fixedDownBoxValue)"
                                                            :key="itemOpt.key" :label="itemOpt.value"
                                                            :value="itemOpt.key" />
                                                    </el-select>
                                                </p>
                                            </div>
                                            <div v-else-if="item.fieldTypeName == 'input-number'">
                                                <p>
                                                    <el-select
                                                        :style="'width:' + (item.optType == 6 ? 350 : 105) + 'px'"
                                                        @change="changeOptType(item)" v-model="item.optType">
                                                        <el-option v-for="itemOpt in optTypes" :key="itemOpt.value"
                                                            :label="itemOpt.label" :value="itemOpt.value" />
                                                    </el-select>
                                                    <input v-if="item.optType != 6" style="width:220px;" type="text"
                                                        :placeholder="'请输入' + item.showName" v-model="item.zdy1">
                                                </p>
                                                <p v-if="item.optType == 6">
                                                    <input type="text" style="width:75px;" class="mr10"
                                                        v-model="item.zdy1">
                                                    <el-select style="width:60px;" v-model="item.opt1">
                                                        <el-option v-for="itemOpt in opt1s" :key="itemOpt.value"
                                                            :label="itemOpt.label" :value="itemOpt.value" />
                                                    </el-select>
                                                    <span class="ellipsis"
                                                        style="display:inline-block;width:60px;vertical-align: text-bottom;">{{
                                                            item.showName }}</span>
                                                    <el-select style="width:60px;" class="ml10" v-model="item.opt2">
                                                        <el-option v-for="itemOpt in opt1s" :key="itemOpt.value"
                                                            :label="itemOpt.label" :value="itemOpt.value" />
                                                    </el-select>
                                                    <input type="text" style="width:75px;" v-model="item.zdy2">
                                                </p>
                                            </div>
                                            <div v-else>
                                                <p class="check_box">
                                                    <input v-model="item.optType" hidden>
                                                    <input type="text" :placeholder="'请输入' + item.showName"
                                                        v-model="item.zdy1">
                                                </p>
                                            </div>
                                            <a v-if="item.type == 1"
                                                @click="originalConfigData.nodeApproveList = []; $func.removeEle(originalConfigData.conditionList[conditionGroupIdx], item, 'formId')">删除</a>
                                            <a v-if="item.type == 2"
                                                @click="$func.removeEle(originalConfigData.conditionList[conditionGroupIdx], item, 'formId')">删除</a>
                                        </div>
                                    </li>
                                </ul>
                                <el-button type="primary" @click="addCondition(conditionGroupIdx)">添加条件</el-button>
                            </el-card>
                            <div v-if="originalConfigData.conditionList.length != conditionGroupIdx + 1">
                                <el-text class="ml10" type="warning"
                                    v-if="originalConfigData.groupRelation == false">且满足</el-text>
                                <el-text class="ml10" type="success"
                                    v-if="originalConfigData.groupRelation == true">或满足</el-text>
                            </div>
                        </div>
                        <el-button style="width: 100%" type="info" icon="el-icon-plus" text bg
                            @click="addConditionGroup">
                            添加条件组
                        </el-button>
                    </div>
                    <div class="demo-drawer__footer clear">
                        <el-button type="primary" @click="saveCondition">确 定</el-button>
                        <el-button @click="closeDrawer">取 消</el-button>
                    </div>
                </div>
            </el-main>
        </el-container>
    </el-drawer>
    <ConditionDialog v-model:visible="conditionVisible" :activeGroupIdx="activeGroupIdx" />
</template>
<script setup>
import { ref, watch, computed } from 'vue'
import ConditionDialog from "../dialog/selectConditionDialog.vue";
import { useStore } from '@/store/modules/workflow'
import { optTypes, opt1s } from '@/utils/antflow/const'
import $func from '@/utils/antflow/index'

const { proxy } = getCurrentInstance()
let store = useStore()
let { setCondition, setConditionsConfig } = store
let conditionVisible = ref(false)
let conditionRoleVisible = ref(false)
let conditionsConfig = ref(null)
let originalConfigData = ref({})
let priorityLevel = ref('')
let conditionsConfig1 = computed(() => store.conditionsConfig1)
let conditionDrawer = computed(() => store.conditionDrawer)
let activeGroupIdx = ref(0)
let visible = computed({
    get() {
        return conditionDrawer.value
    },
    set() {
        closeDrawer()
    }
})
watch(conditionsConfig1, (val) => {
    conditionsConfig.value = val.value;
    priorityLevel.value = val.priorityLevel
    originalConfigData.value = val.priorityLevel ? val.value.conditionNodes[val?.priorityLevel - 1] : { nodeApproveList: [], conditionList: [[]] }
    convertConditionNodeValue(originalConfigData.value.conditionList);
});
/**添加条件 */
const addCondition = async (index) => {
    activeGroupIdx.value = index;
    conditionVisible.value = true;
}
/**条件抽屉的确认 */
const saveCondition = () => {
    //console.log("conditionsConfig.value.conditionNodes=====", JSON.stringify(conditionsConfig.value.conditionNodes)); 
    var a = conditionsConfig.value.conditionNodes.splice(priorityLevel.value - 1, 1)//截取旧下标
    conditionsConfig.value.conditionNodes.splice(originalConfigData.value.priorityLevel - 1, 0, a[0])//填充新下标
    conditionsConfig.value.conditionNodes.map((item, index) => {
        console.log("item==item===", JSON.stringify(item.conditionList));
        item.priorityLevel = index + 1,
            convertConditionNodeValue(item.conditionList, false)
    });
    for (var i = 0; i < conditionsConfig.value.conditionNodes.length; i++) {
        const conditionNodeTitleStr = $func.conditionStr(conditionsConfig.value, i);
        conditionsConfig.value.conditionNodes[i].error = conditionNodeTitleStr.indexOf("请设置条件") >= 0 ? true : false;
        conditionsConfig.value.conditionNodes[i].nodeDisplayName = conditionNodeTitleStr;
        const defaultCond = i == conditionsConfig.value.conditionNodes.length - 1 &&
            conditionsConfig.value.conditionNodes[i].conditionList.flat().filter(
                (item) => item.columnId && item.columnId !== 0
            ).length == 0
        conditionsConfig.value.conditionNodes[i].isDefault = defaultCond ? 1 : 0
    }
    setConditionsConfig({
        value: conditionsConfig.value,
        flag: true,
        id: conditionsConfig1.value.id
    })
    closeDrawer()
}
/**添加条件角色 */
const addConditionRole = () => {
    conditionRoleVisible.value = true;
}
/**关闭抽屉 */
const closeDrawer = (val) => {
    setCondition(false)
}
/*添加条件组 */
const addConditionGroup = () => {
    originalConfigData.value.conditionList = originalConfigData.value.conditionList ?? [];
    originalConfigData.value.conditionList.push([]);
}
/*删除条件组 */

const deleteConditionGroup = (index) => {
    originalConfigData.value.conditionList.splice(index, 1)
}

/**值类型条件改变 */
const changeOptType = (item) => {
    if (item.optType == 1) {
        item.zdy1 = null;
    } else {
        item.zdy1 = null;
        item.zdy2 = null;
    }
}
/**checkbox控件选中效果 */
const toStrChecked = (item, key) => {
    let a = item.zdy1 ? item.zdy1.split(",") : []
    var isIncludes = $func.toggleStrClass(item, key);
    if (!isIncludes) {
        a.push(key)
        item.zdy1 = a.toString()
    } else {
        removeStrEle(item, key);
    }
}
/**删除数组元素 */
const removeStrEle = (item, key) => {
    let a = item.zdy1 ? item.zdy1.split(",") : []
    var includesIndex;
    a.map((item, index) => {
        if (item == key) {
            includesIndex = index
        }
    });
    a.splice(includesIndex, 1);
    item.zdy1 = a.toString()
}

/**格式化控件值 */
const convertConditionNodeValue = (data, isPreview = true) => {
    if (!data || proxy.isEmptyArray(data)) return;
    for (let itemArray of data) {
        let condRelationItem = itemArray[0]?.condRelation || false;//组内条件 或和且
        for (let item of itemArray) {
            if (proxy.isEmpty(item.fieldTypeName)) {
                continue;
            }
            item.condRelation = condRelationItem;
            if (item.fieldTypeName == 'radio') {//单选radio
                item.zdy1 = parseInt(item.zdy1)
            }
            if (item.fieldTypeName == 'select' && item.multiple) {//select多选 
                if (!Array.isArray(item.zdy1) && item.zdy1.includes('[')) {
                    if (isPreview) {
                        item.zdy1 = JSON.parse(item.zdy1)
                    }
                } else {
                    if (!isPreview) {
                        item.zdy1 = JSON.stringify(item.zdy1)
                    }
                }
            }
            if (item.fieldTypeName == 'select' && !item.multiple) {//select单选
                item.zdy1 = parseInt(item.zdy1)
            }
            if (item.fieldTypeName == 'date') {//日期控件
                item.zdy1 = proxy.parseTime(item.zdy1, '{y}-{m}-{d} {h}:{i}:{s}')
            }
            if (item.fieldTypeName == 'time') {//时间控件
                item.zdy1 = proxy.parseTime(item.zdy1, '{y}-{m}-{d} {h}:{i}:{s}')
            }
            /**适配后端设计 */
            if (item.optType == '6') {//数字控件
                //提交转换
                if (item.opt1 == '≤' && item.opt2 == '<') {
                    item.optType = '7'
                } else if (item.opt1 == '<' && item.opt2 == '≤') {
                    item.optType = '8'
                } else if (item.opt1 == '≤' && item.opt2 == '≤') {
                    item.optType = '9'
                } else {
                    item.optType = '6'
                }
            } else if (item.optType == '7' || item.optType == '8' || item.optType == '9') {
                item.optType = '6'//显示转换
            }
        }
    }
} 
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";

.set_condition {
    .priority_level {
        position: absolute;
        top: 11px;
        right: 30px;
        width: 100px;
        height: 32px;
        background: rgba(255, 255, 255, 1);
        border-radius: 4px;
        border: 1px solid rgba(217, 217, 217, 1);
        font-size: 12px;
    }

    .condition_content {
        padding: 5px 5px 0;

        p.tip {
            text-indent: 17px;
            line-height: 30px;
            background: rgba(241, 249, 255, 1);
            border: 1px solid rgba(64, 163, 247, 1);
            color: #46a6fe;
            font-size: 14px;
        }

        ul {
            max-height: 178px;
            overflow-y: scroll;
            margin-bottom: 20px;

            li {
                border-bottom: 1px solid #F2F2F2;

                div {
                    &>span {
                        float: left;
                        margin-right: 5px;
                        width: 100px;
                        line-height: 65px;
                        text-align: right;
                        color: #0857a1;
                        font-size: 14px;
                    }

                    &>div {
                        display: inline-block;
                        width: 350px;

                        &>p:not(:last-child) {
                            margin-bottom: 10px;
                        }
                    }

                    &:not(:last-child)>div>p {
                        margin-bottom: 20px;
                    }

                    &>a {
                        margin-left: 10px;
                        margin-top: 20px;
                        color: #46a6fe;
                        font-size: 14px;
                    }

                    select,
                    input {
                        width: 100%;
                        height: 32px;
                        background: rgba(255, 255, 255, 1);
                        border-radius: 4px;
                        border: 1px solid rgba(217, 217, 217, 1);
                    }

                    select+input {
                        width: 260px;
                    }

                    select {
                        margin-right: 10px;
                        width: 100px;
                    }

                    p.selected_list {
                        padding-left: 10px;
                        border-radius: 4px;
                        min-height: 32px;
                        border: 1px solid rgba(217, 217, 217, 1);
                        word-break: break-word;
                    }

                    p.check_box {
                        line-height: 32px;
                    }
                }
            }
        }

        .el-button {
            margin-bottom: 5px;
        }
    }
}
</style>