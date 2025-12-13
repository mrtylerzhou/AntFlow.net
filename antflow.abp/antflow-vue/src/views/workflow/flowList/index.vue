<template>
   <div class="app-container">
      <div class="query-box">
         <el-form :model="taskMgmtVO" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
            <el-form-item label="版本编号" prop="bpmnCode">
               <el-input v-model="taskMgmtVO.bpmnCode" placeholder="请输入关键字" clearable style="width: 200px"
                  @keyup.enter="handleQuery" />
            </el-form-item>
            <el-form-item label="版本名称" prop="bpmnName">
               <el-input v-model="taskMgmtVO.bpmnName" placeholder="请输入关键字" clearable style="width: 200px"
                  @keyup.enter="handleQuery" />
            </el-form-item>
            <el-form-item>
               <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
               <el-button icon="Refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
         </el-form>
      </div>
      <div class="table-box">
         <el-table v-loading="loading" :data="configList">
            <el-table-column label="类型标识" align="center" prop="formCode" :show-overflow-tooltip="true" width="150" />
            <el-table-column label="类型名称" align="center" prop="formCodeDisplayName" :show-overflow-tooltip="true" />
            <el-table-column label="版本编号" align="center" prop="bpmnCode" :show-overflow-tooltip="true" />
            <el-table-column label="版本名称" align="center" prop="bpmnName" :show-overflow-tooltip="true" />
            <el-table-column label="流程分类" align="center" prop="isLowCodeFlow" :show-overflow-tooltip="true">
               <template #default="item">
                  <el-tooltip v-if="item.row.isLowCodeFlow != 1" content="自定义表单+流程设计器" placement="top">
                     <el-tag type="warning" round>DIY</el-tag>
                  </el-tooltip>
                  <el-tooltip v-if="item.row.isLowCodeFlow == 1" content="低代码表单+流程设计器" placement="top">
                     <el-tag type="success" round>LF</el-tag>
                  </el-tooltip>
               </template>
            </el-table-column>
            <el-table-column label="是否去重" align="center" prop="deduplicationType" width="80">
               <template #default="item">
                  {{ item.row.deduplicationType == 1 ? '否' : '是' }}
               </template>
            </el-table-column>

            <el-table-column label="状态" align="center" prop="effectiveStatus" width="70">
               <template #default="item">
                  <el-tag>{{ item.row.effectiveStatus == 1 ? '活跃' : '不活跃' }}</el-tag>
               </template>
            </el-table-column>
            <el-table-column label="描述说明" align="center" prop="remark" width="160" :show-overflow-tooltip="true" />
            <el-table-column label="创建时间" align="center" prop="updateTime" width="160" :show-overflow-tooltip="true">
               <template #default="scope">
                  <span>{{ parseTime(scope.row.updateTime, '{y}-{m}-{d} {h}:{i}') }}</span>
               </template>
            </el-table-column>
            <el-table-column label="操作" fixed="right" width="150" align="center" class-name="small-padding fixed-width">
               <template #default="scope">
                  <el-button link type="success" @click="handleCopy(scope.row)">复制</el-button>
                  <el-button link type="primary" @click="handleVersion(scope.row)">版本管理</el-button>
               </template>
            </el-table-column>
         </el-table>
         <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
            @pagination="getList" />
      </div>
   </div>
</template>

<script setup>
import { ref, onMounted, getCurrentInstance } from "vue";
import { getBpmnConflistPage } from "@/api/workflow/index";
const { proxy } = getCurrentInstance()
const configList = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const total = ref(0);

const data = reactive({
   form: {},
   pageDto: {
      page: 1,
      pageSize: 10
   },
   taskMgmtVO: {
      effectiveStatus: 1,
      isOutSideProcess: 0,
      isLowCodeFlow: undefined,
      bpmnCode: undefined,
      bpmnName: undefined
   }
});
const { pageDto, taskMgmtVO } = toRefs(data);
onMounted(async () => {
   loading.value = true;
   await getList();
   loading.value = false;
})
/** 查询列表 */
const getList = async () => {
   await getBpmnConflistPage(pageDto.value, taskMgmtVO.value).then(response => {
      let res = response.data;
      configList.value = res.data;
      total.value = res.pagination.totalCount;
   }).catch((r) => {
      console.log(r);
      proxy.$modal.msgError("加载列表失败:" + r.message);
   });
}

const handleVersion = async (row) => {
   const params = {
      t: Date.now(),
      formCode: row.formCode
   };
   let obj = { path: "flow-version", query: params };
   proxy.$tab.openPage(obj);
}

/** 搜索按钮操作 */
function handleQuery() {
   pageDto.value.page = 1;
   getList();
}

/** 重置按钮操作 */
function resetQuery() {
   taskMgmtVO.value = {
      isOutSideProcess: 0,
      bpmnCode: undefined,
      bpmnName: undefined
   };
   proxy.resetForm("queryRef");
   handleQuery();
}
/**
 * 复制操作
 * @param row
 */
const handleCopy = (row) => {
   const params = {
      id: row.id,
      copy: 1,
   };
   let obj = {};
   if (row.isLowCodeFlow == '1') {
      obj = { path: "/workflow/lf-design", query: params };
   } else {
      obj = { path: "/workflow/diy-design", query: params };
   }
   proxy.$tab.openPage(obj);
}
</script>
