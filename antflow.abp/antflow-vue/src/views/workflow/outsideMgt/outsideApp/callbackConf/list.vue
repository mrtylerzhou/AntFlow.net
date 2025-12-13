<template>
  <el-dialog title="审批人模板列表" v-model="visible" style="width: 800px !important;" append-to-body>

    <el-table v-loading="loading" :data="list">
      <el-table-column label="业务标识" align="center" prop="approveTypeName" width="100"/>
      <!-- <el-table-column label="ClientId" align="center" prop="apiClientId" />
      <el-table-column label="ClientSecret" align="center" prop="apiClientSecret" />
      <el-table-column label="Token" align="center" prop="apiToken" /> -->
      <el-table-column label="流程流转回调URL" align="center" prop="apiUrl" />
      <!-- <el-table-column label="备注" align="center" prop="remark" />-->
      <el-table-column label="创建时间" align="center" prop="createTime">
        <template #default="scope">
          <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
        </template>
      </el-table-column>
      <!-- <el-table-column label="操作" width="220" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Remove" @click="handleDisabled(scope.row)">禁用</el-button>
        </template>
      </el-table-column>  -->
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
import { getApproveTemplatelist } from "@/api/workflow/outsideApi";
const { proxy } = getCurrentInstance();

const list = ref([]);
const loading = ref(false);
const visible = ref(false);
let businessPartyId = ref(0);
let applicationId = ref(0);
/** 查询接入业务方列表 */
function getList() {
  loading.value = true;
  getApproveTemplatelist(applicationId.value).then(response => {
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