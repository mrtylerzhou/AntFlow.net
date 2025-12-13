<template>
    <div class="app-container">
        <div class="query-box">
            <el-form :model="taskMgmtVO" ref="queryRef" :inline="true" v-show="showSearch">
                <el-form-item label="人员名称" prop="receiverName">
                    <el-input v-model="taskMgmtVO.receiverName" placeholder="请输入关键字" clearable style="width: 200px"
                        @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item label="流程模板编号" prop="powerId">
                    <el-input v-model="taskMgmtVO.powerId" placeholder="请输入关键字" clearable style="width: 200px"
                        @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
                    <el-button icon="Refresh" @click="resetQuery">重置</el-button>
                </el-form-item>
            </el-form>
            <el-row :gutter="10" class="mb8">
                <el-col :span="1.5">
                    <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
                </el-col>
            </el-row>
        </div>

        <div class="query-box">
            <el-table v-loading="loading" :data="entrustList">
                <el-table-column label="模板类型" align="center" prop="powerId" />

                <el-table-column label="审批人姓名" align="center" prop="name" />
                <el-table-column label="委托人姓名" align="center" prop="receiverName" />

                <el-table-column label="委托开始" align="center" prop="beginTime">
                    <template #default="scope">
                        <span>{{ parseTime(scope.row.beginTime, "{y}-{m}-{d}") }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="委托结束" align="center" prop="endTime">
                    <template #default="scope">
                        <span>{{ parseTime(scope.row.endTime, "{y}-{m}-{d}") }}</span>
                    </template>
                </el-table-column>
                <!-- <el-table-column label="状态" align="center" prop="effectiveStatus">
             <template #default="item">
                <el-tag>{{ item.row.effectiveStatus == 1 ? '活跃' : '不活跃' }}</el-tag> 
             </template>
          </el-table-column> -->
                <el-table-column label="创建时间" align="center" prop="createTime">
                    <template #default="scope">
                        <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="220" align="center" class-name="small-padding fixed-width">
                    <template #default="scope">
                        <!-- <el-button link type="primary" @click="handleEdit(scope.row)">编辑</el-button> -->
                        <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
                @pagination="getList" />

        </div>
        <!-- 添加或修改委托对话框 -->
        <el-dialog :title="title" v-model="open" width="550px" append-to-body>
            <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
                <el-row>
                    <el-col :span="24" style="margin-bottom: 20px;">
                        <el-form-item label="类型" prop="selectType">
                            <el-radio-group v-model="tabPosition">
                                <el-radio-button value="oneflow">具体流程</el-radio-button>
                                <el-radio-button value="allflow">全部流程</el-radio-button>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24" v-if="tabPosition == 'oneflow'">
                        <el-form-item label="流程模板" prop="powerId">
                            <el-select v-model="form.powerId" placeholder="请选择模板类型" :style="{ width: '220px' }">
                                <el-option v-for="(item, index) in formCodeOptions" :key="index" :label="item.value"
                                    :value="item.key"></el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="审批人" prop="sender">
                            <TagUserSelect v-model:list="userSelectedList" placeholder="请选择审批人" style="width: 220px;" />
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="委托人" prop="receiverId">
                            <el-select v-model="form.receiverId" filterable placeholder="请选择委托人" style="width: 220px">
                                <el-option v-for="item in userOptions" :key="item.value" :label="item.label"
                                    :value="item.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="开始时间" prop="beginTime">
                            <el-date-picker :disabled-date="disabledBeginDate" v-model="form.beginTime" type="date"
                                value-format="YYYY-MM-DD" placeholder="请选择开始时间" />
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="24">
                        <el-form-item label="结束时间" prop="endTime">
                            <el-date-picker :disabled-date="disabledEndDate" v-model="form.endTime" type="date"
                                value-format="YYYY-MM-DD" placeholder="请选择结束时间" />
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <template #footer>
                <div class="dialog-footer">
                    <el-button type="primary" @click="submitForm">确 定</el-button>
                    <el-button @click="cancel">取 消</el-button>
                </div>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import TagUserSelect from "@/components/BizSelects/TagUserSelect/index.vue";
import { getEntrustListPage, setEntrust, getDIYFromCodeData } from "@/api/workflow/index";
import { getUsers } from "@/api/workflow/mock.js";
const { proxy } = getCurrentInstance();
const entrustList = ref([]);
const loading = ref(false);
const open = ref(false);
const title = ref("");
const showSearch = ref(true);
const total = ref(0);
let formCodeOptions = ref([]);
let userOptions = ref([]);
const tabPosition = ref('oneflow');
const data = reactive({
    form: {
        id: undefined,
        powerId: undefined,
        sender: undefined,
        name: undefined,
        receiverId: undefined,
        receiverName: undefined,
        beginTime: undefined,
        endTime: undefined,
    },
    pageDto: {
        page: 1,
        pageSize: 10
    },
    taskMgmtVO: {},
    rules: {
        powerId: [{ required: true, message: '请选择流程模板类型', trigger: ['change', 'blur'] }],
        sender: [{ required: true, message: '请选择当前审批人', trigger: ['change', 'blur'] }],
        receiverId: [{ required: true, message: '请选择委托人', trigger: ['change', 'blur'] }]
    }
});
const { pageDto, taskMgmtVO, form, rules } = toRefs(data);


let userSelectedList = ref([]);//{id:1,name:'张三'},{id:2,name:'李四'}

watch(() => userSelectedList.value, (newVal) => {
    if (!proxy.isEmptyArray(newVal)) {
        form.value.sender = newVal[0].id;
    } else {
        form.value.sender = undefined;
    }
}, { deep: true });

const disabledBeginDate = (time) => {
    return time.getTime() > new Date(form?.endTime ?? "");
}
const disabledEndDate = (time) => {
    return time.getTime() < new Date(form?.beginTime ?? "");
}

watch(() => form.value.receiverId, (newVal, oldVal) => {
    if (newVal) {
        form.value.receiverName = getReceiverLabel(newVal);
    }
}, { deep: true });

const getReceiverLabel = (value) => {
    let obj = userOptions.value.filter(item => item.value == value)[0];
    return obj.label;
}

/** 重置操作表单 */
function reset() {
    form.value = {};
    userSelectedList.value = [];
};
onMounted(async () => {
    reset();
    await initFromCode();
    await getList();
    await getUserList();
})
const initFromCode = async () => {
    await getDIYFromCodeData().then((res) => {
        if (res.code == 200) {
            formCodeOptions.value = res.data;
        }
    });
}
const getUserList = async () => {
    await getUsers().then(res => {
        if (res.code == 200) {
            userOptions.value = res.data.map(item => {
                return {
                    label: item.name,
                    value: item.id
                }
            });
        }
    });
}

/** 查询列表 */
async function getList() {
    loading.value = true;
    await getEntrustListPage(pageDto.value, taskMgmtVO.value).then(response => {
        entrustList.value = response.data;
        total.value = response.pagination.totalCount;
        loading.value = false;
    });
}

// const handleEdit = (row) => {
//     reset();
//     entrustDetail(row.id).then(response => {
//         form.value = response.data;
//         console.log('form.value=========',JSON.stringify(form.value));
//         title.value = "编辑委托";
//         open.value = true;
//     });
// }

/** 搜索按钮操作 */
function handleQuery() {
    pageDto.value.page = 1;
    getList();
}

function handleAdd() {
    reset();
    title.value = "添加委托";
    open.value = true;
}

function submitForm() {
    form.value.ids = [];
    if (tabPosition.value == "oneflow") {
        form.value.ids.push({
            id: form.value.id ?? 0,
            powerId: form.value.powerId
        });
    } else {
        for (const fc of formCodeOptions.value) {
            form.value.ids.push({
                id: fc.id ?? 0,
                powerId: fc.key
            });
        }
    }
    proxy.$refs["formRef"].validate(valid => {
        if (valid) {
            if (form.value.id != undefined) {
                setEntrust(form.value).then(response => {
                    proxy.$modal.msgSuccess("修改成功");
                    open.value = false;
                    getList();
                });
            } else {
                setEntrust(form.value).then(response => {
                    if (response.code != 200) {
                        proxy.$modal.msgError("新增失败");
                        return;
                    }
                    proxy.$modal.msgSuccess("新增成功");
                    open.value = false;
                    getList();
                });
            }
        } else {
            return false;
        }
    });
}
function cancel() {
    open.value = false;
    reset();
}
/** 重置按钮操作 */
function resetQuery() {
    taskMgmtVO.value = {};
    proxy.resetForm("queryRef");
    handleQuery();
}

/** 删除按钮操作 */
function handleDelete(row) {
    proxy.$modal.msgError("演示环境不允许删除操作！");
}

</script>