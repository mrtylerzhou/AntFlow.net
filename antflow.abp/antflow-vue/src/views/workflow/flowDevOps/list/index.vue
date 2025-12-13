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
        </div>
        <div class="table-box">
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
                <el-table-column label="创建时间" align="center" prop="createTime">
                    <template #default="scope">
                        <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
                    </template>
                </el-table-column>
            </el-table>
            <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
                @pagination="getList" />
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { getUserEntrustListPage } from "@/api/workflow/index";
const { proxy } = getCurrentInstance();
const entrustList = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const total = ref(0);
const data = reactive({
    pageDto: {
        page: 1,
        pageSize: 10
    },
    taskMgmtVO: {}
});
const { pageDto, taskMgmtVO } = toRefs(data);
onMounted(async () => {
    await getList();
})

/** 查询列表 */
async function getList() {
    loading.value = true;
    await getUserEntrustListPage(pageDto.value, taskMgmtVO.value).then(response => {
        entrustList.value = response.data;
        total.value = response.pagination.totalCount;
        loading.value = false;
    });
}

/** 搜索按钮操作 */
function handleQuery() {
    pageDto.value.page = 1;
    getList();
}

/** 重置按钮操作 */
function resetQuery() {
    taskMgmtVO.value = {};
    proxy.resetForm("queryRef");
    handleQuery();
}

</script>