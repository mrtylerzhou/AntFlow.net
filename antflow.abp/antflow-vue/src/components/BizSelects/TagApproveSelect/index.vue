<template>
    <div v-if="canShowComponent">
        <el-row>
            <el-col :span="24">
                <div class="optional-approver-title">
                    <el-icon style="color: orange">
                        <Avatar />
                    </el-icon>自选审批人
                </div>
                <div class="optional-approver-content">
                    <div v-for="node in approvaNodeList" :key="node.id" class="optional-node-title">
                        {{ node.nodeName }}
                        <div style="display: block;">
                            <el-tag v-for="item in node.approversList" :key="item.id" style="margin: 0 3px;"
                                size="large" closable effect="light" @close="onDeleteTag(node, item)">
                                {{ item.name }}
                            </el-tag>
                            <el-button type="success" size="default" icon="Plus" circle @click="openUserDialog(node)"
                                v-show="canShowaddButton(node)" />
                        </div>
                    </div>
                </div>
            </el-col>
        </el-row>
        <chooseApproveUser v-model:visible="userDialogVisible" v-model:checkedData="selectApproveNode" :multiple="true"
            :multiplelimit="multiplelimit" @change="saveUserDialog" />
    </div>
</template>

<script setup>
import { ref } from 'vue';
import { getStartUserChooseModules } from '@/api/workflow/index';
import chooseApproveUser from './chooseApproveDialog.vue';
const emits = defineEmits(['chooseApprove']);
const props = defineProps({
    formCode: {
        type: String,
        default: ''
    }
});
const multiplelimit = 3;
const approvaNodeList = ref([]);
let userDialogVisible = ref(false);
let selectApproveNode = ref([]);//

const canShowComponent = computed(() => {
    return approvaNodeList.value.length > 0;
});

onMounted(async () => {
    await getStartUserChooseModules(props.formCode).then(res => {
        if (Array.isArray(res.data) && res.data.length > 0) {
            approvaNodeList.value = res.data.map(item => {
                return {
                    id: item.id,
                    nodeName: item.nodeName,
                    approversList: []
                }
            });
        }
        //console.log('approvaNodeList.value========',JSON.stringify(approvaNodeList.value));
    });
});
/**是否显示添加按钮 */
const canShowaddButton = (node) => {
    return node.approversList.length < multiplelimit;
}
/**打开人员选择弹框 */
const openUserDialog = (node) => {
    userDialogVisible.value = true;
    selectApproveNode.value = node;
}

/**保存人员选择弹框 */
const saveUserDialog = (nodeData) => {
    approvaNodeList.value.forEach(node => {
        if (node.id === nodeData.id) {
            node.approversList = nodeData.approversList;
        }
    });
    emits('chooseApprove', formatReturnData(approvaNodeList.value));
}
/**删除选中人员 */
const onDeleteTag = (node, tag) => {
    node.approversList = node.approversList.filter(item => {
        return item.id !== tag.id;
    });
    emits('chooseApprove', formatReturnData(approvaNodeList.value));
};

/**格式化返回数据 */
const formatReturnData = (nodeArr) => {
    let returnData = {
        approvers: {},
        nodeVaild: false
    };
    let nodesGroup = {};
    for (let t of nodeArr) {
        if (nodesGroup.hasOwnProperty(t.id)) {
            nodesGroup[t.id].push(...t.approversList);
        } else {
            if (t.approversList.length > 0) {
                nodesGroup[t.id] = t.approversList;
            }
        }
    }
    //console.log('nodesGroup==keys======',JSON.stringify(Object.keys(nodesGroup)));
    returnData.approvers = nodesGroup;
    returnData.nodeVaild = Object.keys(nodesGroup).length == approvaNodeList.value.length;
    return returnData;
} 
</script>
<style lang="scss" scoped>
.optional-approver-title {
    border-top-left-radius: 5px;
    border-top-right-radius: 5px;
    display: inline-block;
    padding: 5px;
    width: 100%;
    font-weight: 900;
    font-size: 14px;
    background-color: var(--el-border-color);
    color: var(--el-text-color-regular);
}

.optional-approver-content {
    padding: 1px 20px 0;
    width: 100%;

    min-height: 572px;
    overflow: hidden;
    margin-bottom: 20px;
    border-bottom: 1px solid #f2f2f2;
    border: 1px solid var(--el-border-color);
    border-bottom-left-radius: 5px;
    border-bottom-right-radius: 5px;
    background-color: #fff;
}

.optional-node-title {
    align-items: flex-start;
    box-sizing: border-box;
    line-height: 32px;
    color: var(--el-text-color-regular);
    font-weight: 700;
    font-size: 14px;
}

.optional-node-title::before {
    content: "*";
    color: var(--el-color-danger);
    margin-right: 4px;
}
</style>