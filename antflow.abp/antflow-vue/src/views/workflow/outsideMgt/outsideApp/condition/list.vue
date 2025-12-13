<template>
  <el-dialog title="条件模板列表" v-model="visible" style="width: 800px !important;" append-to-body>
    <el-form :model="vo" ref="queryRef" :inline="true" v-show="showSearch">
      <el-form-item label="条件名称" prop="name">
        <el-input v-model="vo.name" placeholder="请输入关键字" clearable style="width: 200px" @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
    </el-form>
    <el-table v-loading="loading" :data="list">
      <el-table-column label="条件标识" align="center" prop="templateMark" />
      <el-table-column label="条件名称" align="center" prop="name" />
      <el-table-column label="备注" align="center" prop="remark" />
      <el-table-column label="创建时间" align="center" prop="createTime">
        <template #default="scope">
          <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="220" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Remove" @click="handleDisabled(scope.row)">禁用</el-button>
        </template>
      </el-table-column>
    </el-table>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="visible = false">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref } from "vue";
import { getConditionTemplatelist } from "@/api/workflow/outsideApi";
const { proxy } = getCurrentInstance();

const list = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const visible = ref(false);
let businessPartyId = ref(0);
let applicationId = ref(0);
const data = reactive({
  page: {
    page: 1,
    pageSize: 10
  },
  vo: {
    name: undefined
  }
});
const { page, vo } = toRefs(data);

/** 查询接入业务方列表 */
function getList() {
  loading.value = true;
  getConditionTemplatelist(applicationId.value).then(response => {
    list.value = response.data;
    loading.value = false;
  }).catch(() => {
    loading.value = false;
  });
}

/** 删除按钮操作 */
function handleDisabled(row) {
  proxy.$modal.msgError("演示环境不允许删除操作！");
}

/** 搜索按钮操作 */
function handleQuery() {
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  vo.value = {};
  proxy.resetForm("queryRef");
  handleQuery();
}
// 显示弹框
function show(partyId, appId) {
  visible.value = true;
  businessPartyId.value = partyId;
  applicationId.value = appId;
  getList();
}
defineExpose({
  show,
});
</script>