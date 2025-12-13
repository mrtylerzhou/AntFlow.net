<template>
  <div class="app-container">
    <div class="query-box">
      <el-form :model="vo" ref="queryRef" :inline="true" v-show="showSearch">
        <el-form-item label="业务表单名称" prop="remark">
          <el-input v-model="vo.remark" placeholder="请输入关键字" clearable style="width: 200px"
            @keyup.enter="handleQuery" />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
          <el-button icon="Refresh" @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
    <div class="table-box">
      <el-table v-loading="loading" :data="list" @selection-change="handleSelectionChange">
        <el-table-column type="selection" align="center" width="35" />
        <el-table-column label="租户标识" align="center" prop="businessCode" width="150" />
        <el-table-column label="租户名称" align="center" prop="businessName" width="150" />
        <el-table-column align="center" prop="processKey" width="250">
          <template #header>
            <span>
              应用标识
              <el-tooltip content="唯一标识即：FormCode" placement="top">
                <el-icon><question-filled /></el-icon>
              </el-tooltip>
            </span>
          </template>
        </el-table-column>
        <el-table-column label="应用名称" align="center" prop="name" />
        <el-table-column label="业务类型" align="center" prop="applyTypeName" width="120" />
        <el-table-column label="创建时间" align="center" prop="createTime" width="160">
          <template #default="scope">
            <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" width="150" align="center" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-tooltip content="编辑" placement="top">
              <el-button link type="primary" icon="Edit" @click="handleEdit(scope.row)"></el-button>
            </el-tooltip>
            <el-tooltip content="设置" placement="top">
              <el-button link type="primary" icon="Setting" @click="settingPage(scope.row)"></el-button>
            </el-tooltip>
            <el-tooltip content="设计流程" placement="top">
              <el-button link type="primary" icon="Promotion" @click="handleFlowDesign(scope.row)"></el-button>
            </el-tooltip>
            <!-- <el-button link type="primary" icon="Delete" @click="handleDelete(scope.row)">删除</el-button> -->
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="total > 0" :total="total" v-model:page="page.page" v-model:limit="page.pageSize"
        @pagination="getList" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import {
  getApplicationsPageList,
  getApplicationDetail,
  getCallbackUrlConfList,
  getApproveTemplatelist
} from "@/api/workflow/outsideApi";
const { proxy } = getCurrentInstance();
const list = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const total = ref(0);
const open = ref(false);
const title = ref("");
const data = reactive({
  appForm: {},
  page: {
    page: 1,
    pageSize: 10
  },
  vo: {
    remark: undefined
  }
});
const { page, vo, appForm } = toRefs(data);
onMounted(async () => {
  getList();
})

/** 查询注册业务表单列表 */
function getList() {
  loading.value = true;
  getApplicationsPageList(page.value, vo.value).then(response => {
    list.value = response.data.data;
    total.value = response.data.pagination.totalCount;
    loading.value = false;
  }).catch(() => {
    loading.value = false;
  });
}

/** 多选框选中数据 */
function handleSelectionChange(selection) {
  single.value = selection.length != 1;
  multiple.value = !selection.length;
}

/** 修改按钮操作 */
function handleEdit(row) {
  const id = row.id;
  if (id == 1 || id == 2) {
    proxy.$modal.msgError("演示数据不允许修改操作！");
    return;
  }
  proxy.$modal.loading();
  getApplicationDetail(id).then(response => {
    appForm.value = response.data;
    appForm.value.applyType = appForm.value.applyType.toString();
    appForm.value.isSon = appForm.value.isSon?.toString();
    appForm.value.businessName = row.businessName;
    open.value = true;
    title.value = "编辑应用";
  });
  proxy.$modal.closeLoading();
}

/** 重置按钮操作 */
function resetQuery() {
  vo.value = {};
  proxy.resetForm("queryRef");
  handleQuery();
}
/** 搜索按钮操作 */
function handleQuery() {
  page.value.page = 1;
  getList();
}
/** 删除按钮操作 */
function handleDelete(row) {
  proxy.$modal.msgError("演示环境不允许删除操作！");
}
const settingPage = (row) => {
  const params = {
    appId: row.id,
    appName: encodeURIComponent(row.name),
    pId: row.businessPartyId,
    pName: encodeURIComponent(row.businessName),
    fc: row.processKey
  };
  let obj = { path: "app-setting", query: params };
  proxy.$tab.closeOpenPage(obj);
};

async function handleFlowDesign(row) {
  proxy.$modal.loading();
  const resultCheckApprove = await checkApproveConfig(row);
  const resultCheckCallback = await checkCallBackConfig(row);
  if (!resultCheckApprove) {
    proxy.$modal.closeLoading();
    proxy.$modal.msgError("请先设置审批人");
    return;
  }
  if (!resultCheckCallback) {
    proxy.$modal.closeLoading();
    proxy.$modal.msgError("请先设置流程回调");
    return;
  }
  const param = {
    bizid: row.businessPartyId,
    bizname: encodeURIComponent(row.businessName),
    bizcode: row.businessCode,
    appid: row.id,
    fc: row.processKey,
    fcname: encodeURIComponent(row.name),
  };
  proxy.$modal.closeLoading();
  const obj = { path: "/outsideMgt/outsideDesign", query: param };
  proxy.$tab.closeOpenPage(obj);
}
const checkApproveConfig = async (row) => {
  return await getApproveTemplatelist(row.id).then(response => {
    if (response.code == 200) {
      return response.data.length > 0
    } else {
      return false;
    }
  }).catch(err => {
    return false;
  });
}
const checkCallBackConfig = async (row) => {
  return await getCallbackUrlConfList(row.processKey).then(response => {
    if (response.code == 200) {
      return response.data.length > 0
    } else {
      return false;
    }
  }).catch(err => {
    return false;
  });
}
</script>