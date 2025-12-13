<template>
    <div class="app-container">
        <el-tabs v-model="activeName" @tab-click="handleClickTab">
            <el-tab-pane label="流程分类(LF)" name="LFTab">
                <div class="query-box">
                    <el-form :model="taskMgmtVO" ref="queryRef" :inline="true" v-show="showSearch">
                        <el-form-item label="关键字" prop="description">
                            <el-input v-model="taskMgmtVO.description" placeholder="请输入关键字" clearable
                                style="width: 200px" @keyup.enter="handleQuery" />
                        </el-form-item>

                        <el-form-item>
                            <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
                            <el-button icon="Refresh" @click="resetQuery">重置</el-button>
                            <el-button type="success" plain icon="CirclePlus" @click="createLFTemp">新增(LF)</el-button>
                        </el-form-item>
                        <el-form-item style="float: right;">
                            <right-toolbar v-model:showSearch="showSearch" @queryTable="getLFPageList"
                                :columns="columns"></right-toolbar>
                        </el-form-item>
                    </el-form>
                </div>
                <div class="table-box">
                    <el-table v-loading="loading" :data="LFPageList" height="50vh">
                        <el-table-column label="类型标识" align="center" prop="key" v-if="columns[0].visible"
                            :show-overflow-tooltip="true">
                            <template #default="item">
                                <el-tooltip class="box-item" effect="dark" placement="right">
                                    <template #content>
                                        <span>{{ item.row.key }}</span>
                                    </template>
                                    {{ substringHidden(item.row.key) }}
                                </el-tooltip>
                            </template>
                        </el-table-column>
                        <el-table-column label="类型名称" align="center" prop="value" v-if="columns[1].visible"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="流程分类" align="center" prop="type" v-if="columns[2].visible"
                            :show-overflow-tooltip="true">
                            <template #default="item">
                                <el-tag v-if="item.row.type === 'LF'" type="success">{{ item.row.type }}</el-tag>
                                <el-tag v-else type="warning">{{ item.row.type }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="备注" align="center" prop="remark" v-if="columns[3].visible"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[4].visible">
                            <template #default="scope">
                                <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" fixed="right" width="320" align="center"
                            class-name="small-padding fixed-width">
                            <template #default="scope">
                                <el-tooltip content="设计流程" placement="top">
                                    <el-button link type="primary" icon="Promotion"
                                        @click="handleLFDesign(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="流程版本" placement="top">
                                    <el-button link type="warning" icon="Operation"
                                        @click="handleVersion(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="查看表单" placement="top">
                                    <el-button link type="success" icon="View"
                                        @click="handleLFTemp(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="通知设置" placement="top">
                                    <el-button
                                        v-if="scope.row.processNotices?.findIndex(item => item.active) > -1 || scope.row.templateVos?.length > 0"
                                        link type="danger" icon="BellFilled"
                                        @click="handleFlowMsg(scope.row)"></el-button>
                                    <el-button v-else link icon="BellFilled" type="info"
                                        @click="handleFlowMsg(scope.row)"></el-button>
                                </el-tooltip>
                            </template>
                        </el-table-column>
                    </el-table>
                    <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page"
                        v-model:limit="pageDto.pageSize" @pagination="getLFPageList" />
                </div>
            </el-tab-pane>
            <el-tab-pane label="流程分类(DIY)" name="DIYTab">
                <div class="query-box">
                    <el-row :gutter="10" class="mb8">
                        <el-col :span="1.5">
                            <el-button type="primary" plain icon="CirclePlus" @click="handleDIYTemp">新增(DIY)</el-button>
                        </el-col>
                        <right-toolbar v-model:showSearch="showSearch" @queryTable="getDIYList"
                            :columns="columns"></right-toolbar>
                    </el-row>
                </div>
                <div class="table-box">
                    <el-table v-loading="loading" :data="DIYList" height="50vh">
                        <el-table-column label="类型标识" align="center" prop="key" v-if="columns[0].visible"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="类型名称" align="center" prop="value" v-if="columns[1].visible"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="流程分类" align="center" prop="type" v-if="columns[2].visible"
                            :show-overflow-tooltip="true">
                            <template #default="item">
                                <el-tag v-if="item.row.type === 'LF'" type="success">{{ item.row.type }}</el-tag>
                                <el-tag v-else type="warning">{{ item.row.type }}</el-tag>
                            </template>
                        </el-table-column>
                        <el-table-column label="备注" align="center" prop="remark" v-if="columns[3].visible"
                            :show-overflow-tooltip="true" />
                        <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[4].visible">
                            <template #default="scope">
                                <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                            </template>
                        </el-table-column>
                        <el-table-column label="操作" fixed="right" width="320" align="center"
                            class-name="small-padding fixed-width">
                            <template #default="scope">
                                <el-tooltip content="设计流程" placement="top">
                                    <el-button link type="primary" icon="Promotion"
                                        @click="handleDIYDesign(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="流程版本" placement="top">
                                    <el-button link type="warning" icon="Operation"
                                        @click="handleVersion(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="查看表单" placement="top">
                                    <el-button link type="success" icon="View"
                                        @click="handleLFTemp(scope.row)"></el-button>
                                </el-tooltip>
                                <el-tooltip content="通知设置" placement="top">
                                    <el-button
                                        v-if="scope.row.processNotices?.findIndex(item => item.active) > -1 || scope.row.templateVos?.length > 0"
                                        link type="danger" icon="BellFilled"
                                        @click="handleFlowMsg(scope.row)"></el-button>
                                    <el-button v-else link icon="BellFilled" type="info"
                                        @click="handleFlowMsg(scope.row)"></el-button>
                                </el-tooltip>

                            </template>
                        </el-table-column>
                    </el-table>
                </div>
            </el-tab-pane>
        </el-tabs>
        <!-- 添加类型标识 -->
        <el-dialog :title="title" v-model="openForm" append-to-body>
            <el-form :model="form" :rules="rules" ref="formRef" label-width="130px" style="margin: 0 20px;">

                <el-row>
                    <el-col :span="24">
                        <el-form-item label="类型标识" prop="key">
                            <el-input v-model="form.key" placeholder="请输入类型唯一标识" />
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="类型名称" prop="value">
                            <el-input v-model="form.value" placeholder="请输入类型名称" />
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="备注">
                            <el-input v-model="form.remark" type="textarea" placeholder="请输入内容"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <template #footer>
                <div class="dialog-footer">
                    <el-button @click="closeDialog">关 闭</el-button>
                    <el-button type="primary" @click="submitForm">确 定</el-button>
                </div>
            </template>
        </el-dialog>

        <set-msg-drawer v-model:visible="openFlowMsgDialog" :formMsgData="formMsgData" @refresh="refreshList" />
        <view-form-drawer v-model:visible="open" :viewFormData="viewFormData" />
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import SetMsgDrawer from './setMsgDrawer.vue';
import ViewFormDrawer from './viewFormDrawer.vue';
import { getDIYFromCodeData } from "@/api/workflow/index";
import { createLFFormCode, getLFFormCodePageList } from '@/api/workflow/lowcodeApi';
const { proxy } = getCurrentInstance();
const DIYList = ref([]);
const LFPageList = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const open = ref(false);
const openForm = ref(false);

const openFlowMsgDialog = ref(false);
const formMsgData = ref(null);

const title = ref("");
let viewFormData = ref(null);

const activeName = ref('LFTab');
const total = ref(0);

const data = reactive({
    form: {},
    pageDto: {
        page: 1,
        pageSize: 10
    },
    taskMgmtVO: {
        description: undefined
    },
    rules: {
        key: [{
            required: true,
            pattern: /^[A-Z]{4,10}$/,
            message: '请输入类型唯一标识(只能是大写字母,4-10位长度)',
            trigger: ['blur', 'change']
        }],
        value: [{
            required: true,
            pattern: /^[\u4e00-\u9fa5]{4,10}$/,
            message: '请输入类型名称(必须包含汉字,4-10位长度)',
            trigger: ['blur', 'change']
        }],
    }
});
const { pageDto, taskMgmtVO, form, rules } = toRefs(data);

// 列显隐信息
const columns = ref([
    { key: 0, label: `类型标识`, visible: true },
    { key: 1, label: `类型名称`, visible: true },
    { key: 2, label: `流程分类`, visible: true },
    { key: 3, label: `备注`, visible: true },
    { key: 4, label: `创建时间`, visible: true }
]);
onMounted(async () => {
    getLFPageList();
})

/** 查询接入业务方列表 */
function getDIYList() {
    loading.value = true;
    getDIYFromCodeData().then(response => {
        DIYList.value = response.data;
        loading.value = false;
    }).catch((err) => {
        loading.value = false;
        proxy.$modal.msgError("加载列表失败:" + err.message);
    });
}

/** 查询接入业务方列表 */
function getLFPageList() {
    loading.value = true;
    getLFFormCodePageList(pageDto.value, taskMgmtVO.value).then(response => {
        LFPageList.value = response.data;
        total.value = response.pagination.totalCount;
        loading.value = false;
    }).catch((err) => {
        loading.value = false;
        proxy.$modal.msgError("加载列表失败:" + err.message);
    });
}

/** 添加自定义业务表单FromCode */
function handleDIYTemp() {
    proxy.$modal.msgSuccess("后端添加流程适配以后自动查询出来");
}
/** 添加低代码业务表单FromCode */
function createLFTemp() {
    reset();
    title.value = "添加类型";
    openForm.value = true;

}
/** 提交表单 */
function submitForm() {
    proxy.$refs["formRef"].validate(valid => {
        if (valid) {
            createLFFormCode(form.value).then(response => {
                if (response.code != 200) {
                    proxy.$modal.msgError("新增失败");
                    return;
                }
                proxy.$modal.msgSuccess("新增成功");
                openForm.value = false;
                getLFPageList();
            });
        }
    });
}


/** 查看表单操作 */
const handleLFTemp = async (row) => {
    viewFormData.value = row;
    open.value = true;
}

async function handleDIYDesign(row) {
    proxy.$modal.loading();
    const param = {
        fcname: encodeURIComponent(row.value),
        fc: row.key
    };
    proxy.$modal.closeLoading();
    const obj = { path: "/workflow/diy-design", query: param };
    proxy.$tab.openPage(obj);
}

async function handleLFDesign(row) {
    proxy.$modal.loading();
    const param = {
        fcname: encodeURIComponent(row.value),
        fc: row.key
    };
    proxy.$modal.closeLoading();
    const obj = { path: "/workflow/lf-design", query: param };
    proxy.$tab.openPage(obj);
}
/** 选择通知消息设置 */
async function handleFlowMsg(row) {
    formMsgData.value = {
        formCode: row.key,
        flowType: row.type,
        processNotices: row.processNotices || [],
        templateVos: row.templateVos || []
    };
    openFlowMsgDialog.value = true;
}
/** 刷新列表 */
const refreshList = (type) => {
    if (type === 'LF') {
        getLFPageList();
    } else {
        getDIYList();
    }
}

/**
 * 跳转到版本管理
 * @param row 
 */
const handleVersion = async (row) => {
    const params = {
        formCode: row.key
    };
    let obj = { path: "flow-version", query: params };
    proxy.$tab.openPage(obj);
}

/** 搜索按钮操作 */
function handleQuery() {
    pageDto.value.page = 1;
    getLFPageList();
}

/** 重置按钮操作 */
function resetQuery() {
    pageDto.value.page = 1;
    proxy.resetForm("queryRef");
    handleQuery();
}
function closeDialog() {
    open.value = false;
    openForm.value = false;
}
/** 重置操作表单 */
function reset() {
    form.value = {
        key: undefined,
        value: undefined,
        remark: undefined
    };
    proxy.resetForm("formRef");
};

function handleClickTab(tab, event) {
    activeName.value = tab.paneName;
    if (tab.paneName == 'DIYTab') {
        getDIYList();
    } else {
        getLFPageList();
    }
}
</script>
<style lang="scss" scoped>
.component {
    background: white !important;
    padding: 30px !important;
    max-width: 720px !important;
    left: 0 !important;
    right: 0 !important;
    margin: auto !important;
}
</style>