<!--
 * @Date:  2024-05-25 14:05:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-05-24 15:20:53
 * @FilePath: /ant-flow/src/components/drawer/copyerDrawer.vue
-->
<template>
    <el-drawer :append-to-body="true" title="抄送人设置" v-model="visible" class="set_copyer" :with-header="false"
        destory-on-close :size="680">
        <div class="el-drawer__header">
            <span class="drawer-title">抄送人</span>
        </div>
        <el-tabs v-model="activeName" @tab-click="handleTabClick">
            <el-tab-pane label="抄送人设置" name="copyStep">
                <div class="copyer_content drawer_content" v-if="copyStepShow">
                    <el-button type="primary" @click="addCopyer">添加成员</el-button>
                    <p class="selected_list">
                        <span v-for="(item, index) in copyerConfig.nodeApproveList" :key="index">{{ item.name }}
                            <img src="@/assets/images/antflow/add-close1.png"
                                @click="$func.removeEle(copyerConfig.nodeApproveList, item, 'targetId')">
                        </span>
                        <a v-if="copyerConfig.nodeApproveList && copyerConfig.nodeApproveList.length != 0"
                            @click="copyerConfig.nodeApproveList = []">清除</a>
                    </p>
                    <!-- <el-checkbox-group v-model="ccSelfSelectFlag" class="clear">
                        <el-checkbox :value="1">允许发起人自选抄送人</el-checkbox>
                    </el-checkbox-group> -->
                </div>
            </el-tab-pane>
            <el-tab-pane lazy label="表单权限设置" name="formStep">
                <div class="drawer_content">
                    <form-perm-conf v-if="formStepShow" default-perm="R" v-model:formItems="formItems"
                        @changePermVal="changePermVal" />
                </div>
            </el-tab-pane>
        </el-tabs>
        <div class="demo-drawer__footer clear">
            <el-button type="primary" @click="saveCopyer">确 定</el-button>
            <el-button @click="closeDrawer">取 消</el-button>
        </div>
        <select-user-dialog v-model:visible="copyerVisible" :data="checkedList" @change="sureCopyer" />
    </el-drawer>
</template>
<script setup>
import { ref, watch, computed } from 'vue'
import selectUserDialog from '../dialog/selectUserDialog.vue'
import FormPermConf from "./permConfig/FormPermConf.vue"
import $func from '@/utils/antflow/index'
import { useStore } from '@/store/modules/workflow'
let copyerConfig = ref({})
let ccSelfSelectFlag = ref([])
let copyerVisible = ref(false)
let checkedList = ref([])
let formItems = ref([])

let activeName = ref('copyStep')
let copyStepShow = ref(true)
let formStepShow = ref(false)

//let testObj = JSON.parse("{\"lfFieldControlVOs\":[{\"fieldId\":\"input12931\",\"fieldName\":\"发件人姓名\",\"perm\":\"R\"},{\"fieldId\":\"switch96070\",\"fieldName\":\"是否保密\",\"perm\":\"E\"},{\"fieldId\":\"input23031\",\"fieldName\":\"发件人号码\",\"perm\":\"H\"}]}");

let store = useStore()
let { setCopyerConfig, setCopyer } = store
let copyerDrawer = computed(() => store.copyerDrawer)
let copyerConfig1 = computed(() => store.copyerConfig1)
let visible = computed({
    get() {
        handleTabClick({ paneName: "copyStep" })
        return copyerDrawer.value
    },
    set() {
        closeDrawer()
    }
})
watch(copyerConfig1, (val) => {
    copyerConfig.value = val.value;
    formItems.value = copyerConfig.value.lfFieldControlVOs || [];
    //console.log("copyerConfig.value========", JSON.stringify(copyerConfig.value))
    ccSelfSelectFlag.value = copyerConfig.value.ccSelfSelectFlag == 0 ? [] : [copyerConfig.value.ccSelfSelectFlag]
})

const addCopyer = () => {
    copyerVisible.value = true;
    checkedList.value = copyerConfig.value.nodeApproveList
}
const sureCopyer = (data) => {
    copyerConfig.value.nodeApproveList = data;
    copyerVisible.value = false;
}
const saveCopyer = () => {
    copyerConfig.value.ccSelfSelectFlag = ccSelfSelectFlag.value.length == 0 ? 0 : 1;
    copyerConfig.value.error = !$func.copyerStr(copyerConfig.value);
    setCopyerConfig({
        value: copyerConfig.value,
        flag: true,
        id: copyerConfig1.value.id
    })
    closeDrawer();
}
const closeDrawer = () => {
    //console.log("copyerConfig.value.lfFieldControlVOs========", JSON.stringify(copyerConfig.value))
    setCopyer(false)
}
const handleTabClick = (tab, event) => {
    activeName.value = tab.paneName;
    if (tab.paneName == 'copyStep') {
        copyStepShow.value = true;
        formStepShow.value = false;
    }
    if (tab.paneName == 'formStep') {
        copyStepShow.value = false;
        formStepShow.value = true;
    }
}
const changePermVal = (data) => {
    copyerConfig.value.lfFieldControlVOs = data;
    //console.log("copyerConfig.value.lfFieldControlVOs========",JSON.stringify(copyerConfig.value))
}

</script>

<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";

.selected_list {
    margin-bottom: 20px;
    line-height: 30px;
}

.selected_list span {
    margin-right: 10px;
    padding: 3px 6px 3px 9px;
    line-height: 12px;
    white-space: nowrap;
    border-radius: 2px;
    border: 1px solid rgba(220, 220, 220, 1);
}

.selected_list img {
    margin-left: 5px;
    width: 7px;
    height: 7px;
    cursor: pointer;
}

.selected_list a {
    font-size: 10 !important;
    color: #46a6fe;
    cursor: pointer;
}

.set_copyer {
    .copyer_content {
        padding: 20px 20px 0;

        .el-button {
            margin-bottom: 20px;
        }

        .el-checkbox {
            margin-bottom: 20px;
        }
    }
}
</style>