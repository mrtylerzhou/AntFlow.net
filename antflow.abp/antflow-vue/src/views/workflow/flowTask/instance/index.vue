<template>
   <div class="app-container">
      <div class="query-box">
         <el-form :model="taskMgmtVO" ref="queryRef" :inline="true" v-show="showSearch">
            <el-form-item label="流程编号" prop="processNumber">
               <el-input v-model="taskMgmtVO.processNumber" placeholder="请输入关键字" clearable style="width: 200px"
                  @keyup.enter="handleQuery" />
            </el-form-item>
            <el-form-item label="流程描述" prop="description">
               <el-input v-model="taskMgmtVO.description" placeholder="请输入关键字" clearable style="width: 200px"
                  @keyup.enter="handleQuery" />
            </el-form-item>

            <el-form-item>
               <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
               <el-button icon="Refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
         </el-form>
      </div>
      <div class="table-box">
         <el-table v-loading="loading" :data="dataList">
            <el-table-column label="流程类型" align="center" prop="processKey" :show-overflow-tooltip="true">
               <template #default="item">
                  {{ substringHidden(item.row.processKey) }}
                  <el-tooltip v-if="item.row.isOutSideProcess" content="外部(第三方)业务方表单接入流程引擎" placement="top">
                     <el-tag type="warning" round>OUT</el-tag>
                  </el-tooltip>
               </template>
            </el-table-column>
            <el-table-column label="类型名称" align="center" prop="processTypeName" :show-overflow-tooltip="true" />
            <el-table-column label="流程编号" align="center" prop="processNumber" :show-overflow-tooltip="true">
               <template #default="item">
                  <el-tooltip class="box-item" effect="dark" placement="right">
                     <template #content>
                        <span>{{ item.row.processNumber }}</span>
                     </template>
                     {{ substringHidden(item.row.processNumber) }}
                  </el-tooltip>
               </template>
            </el-table-column>
            <el-table-column label="流程描述" align="center" prop="description" :show-overflow-tooltip="true" />
            <el-table-column label="状态" align="center" prop="effectiveStatus">
               <template #default="item">
                  <el-tag v-if="item.row.processState == 2" type="primary">{{ item.row.taskState }}</el-tag>
                  <el-tag v-else-if="item.row.processState == 6" type="danger">{{ item.row.taskState }}</el-tag>
                  <el-tag v-else type="success">{{ item.row.taskState }}</el-tag>
               </template>
            </el-table-column>
            <!-- <el-table-column label="创建时间" align="center" prop="createTime" width="180">
            <template #default="scope">
               <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
            </template>
         </el-table-column> -->
            <el-table-column label="更新时间" align="center" prop="runTime" width="160">
               <template #default="scope">
                  <span>{{ parseTime(scope.row.runTime, '{y}-{m}-{d} {h}:{i}') }}</span>
               </template>
            </el-table-column>
            <el-table-column label="操作" fixed="right" width="160" align="center" class-name="small-padding fixed-width">
               <template #default="scope">
                  <el-button link type="primary" icon="View" @click="handlePreview(scope.row)">查看</el-button>
                  <el-button link type="primary" size="small">
                     <el-dropdown>
                        <el-button link size="small" type="primary">
                           更多<el-icon class="el-icon--left"><arrow-down /></el-icon>
                        </el-button>
                        <template #dropdown>
                           <el-dropdown-menu>
                              <el-dropdown-item @click="handleFlowRemoveSign(scope.row)">
                                 <el-icon>
                                    <Remove />
                                 </el-icon>减签
                              </el-dropdown-item>
                              <el-dropdown-item @click="handleFlowAddSign(scope.row)">
                                 <el-icon>
                                    <CirclePlus />
                                 </el-icon>加签
                              </el-dropdown-item>
                              <el-dropdown-item @click="handleFlowChange(scope.row)">
                                 <el-icon>
                                    <Switch />
                                 </el-icon>变更
                              </el-dropdown-item>
                              <el-dropdown-item @click="handleFlowCancel(scope.row)">
                                 <el-icon>
                                    <RefreshLeft />
                                 </el-icon>撤回
                              </el-dropdown-item>
                              <el-dropdown-item @click="handleFlowRepeal(scope.row)">
                                 <el-icon>
                                    <CircleClose />
                                 </el-icon>作废
                              </el-dropdown-item>
                           </el-dropdown-menu>
                        </template>
                     </el-dropdown>
                  </el-button>
               </template>
            </el-table-column>
         </el-table>
         <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
            @pagination="getList" />
      </div>
      <previewDrawer v-if="visible" />
   </div>
</template>

<script setup>
import { getAllProcesslistPage, processOperation } from "@/api/workflow/index";
import { useStore } from '@/store/modules/workflow';
import previewDrawer from "@/views/workflow/components/previewDrawer.vue";
import { onMounted } from "vue";
const router = useRouter();
const { proxy } = getCurrentInstance();
let store = useStore()
let { setPreviewDrawer, setPreviewDrawerConfig } = store
let previewDrawerVisible = computed(() => store.previewDrawer)
const dataList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const total = ref(0);

let visible = computed({
   get() {
      return previewDrawerVisible.value
   },
   set() {
      closeDrawer()
   }
})
const data = reactive({
   form: {},
   pageDto: {
      page: 1,
      pageSize: 10
   },
   taskMgmtVO: {
      processNumber: undefined,
      processTypeName: undefined
   },
   rules: {
      processNumber: [{ required: true, message: "关键字不能为空", trigger: "blur" }],
      processTypeName: [{ required: true, message: "关键字不能为空", trigger: "blur" }],
   }
});
const { pageDto, taskMgmtVO } = toRefs(data);

onMounted(() => {
   getList();
});
/** 查询流程监控列表 */
const getList = async () => {
   loading.value = true;
   await getAllProcesslistPage(pageDto.value, taskMgmtVO.value).then(response => {
      dataList.value = response.data;
      total.value = response.pagination.totalCount;
      loading.value = false;
   }).catch((r) => {
      loading.value = false;
      console.log(r);
      proxy.$modal.msgError("加载列表失败:" + r.message);
   });
}

/** 搜索按钮操作 */
const handleQuery = async () => {
   pageDto.value.page = 1;
   await getList();
}
function resetQuery() {
   taskMgmtVO.value = {
      processNumber: undefined,
      processTypeName: undefined
   };
   handleQuery();
}

function handlePreview(row) {
   setPreviewDrawer(true);
   setPreviewDrawerConfig({
      formCode: row.processKey,
      processNumber: row.processNumber,
      isOutSideAccess: row.isOutSideProcess,
      isLowCodeFlow: row.isLowCodeFlow,
      processState: row.processState,
   })
}

/** 减签 */
function handleFlowRemoveSign(row) {
   const processNumber = row.processNumber
   router.push({
      path: "/workflow/instance/removeSign/processNumber/" + processNumber,
      query: row
   });
}

/** 加签 */
function handleFlowAddSign(row) {
   const processNumber = row.processNumber
   router.push({
      path: "/workflow/instance/addSign/processNumber/" + processNumber,
      query: row
   });
}

/** 变更 */
function handleFlowChange(row) {
   const processNumber = row.processNumber
   router.push({
      path: "/workflow/instance/changeSign/processNumber/" + processNumber,
      query: row
   });
}

/** 撤回 */
function handleFlowCancel(row) {
   let pramForm = {
      operationType: 29,
      formCode: row.processKey,
      processNumber: row.processNumber,
      isLowCodeFlow: row.isLowCodeFlow
   };
   proxy.$confirm('确认撤回编号为"' + row.processNumber + '"的流程吗？', "温馨提示").then(() => {
      //proxy.$modal.msgSuccess("撤销功能开发中，敬请期待！")
      proxy.$modal.loading();
      processOperation(pramForm).then((res) => {
         if (res.code == 200) {
            getList();
            proxy.$modal.msgSuccess("操作成功");
            //close();
         } else {
            proxy.$modal.msgError("操作失败:" + res.errMsg);
         }
      });
      proxy.$modal.closeLoading();
   }).catch(() => { })
}
/** 作废 */
function handleFlowRepeal(row) {
   let pramForm = {
      operationType: 7,
      formCode: row.processKey,
      processNumber: row.processNumber,
      isLowCodeFlow: row.isLowCodeFlow
   };
   proxy.$confirm('确认作废编号为"' + row.processNumber + '"的流程吗？', "温馨提示").then(() => {
      //proxy.$modal.msgSuccess("撤销功能开发中，敬请期待！")
      proxy.$modal.loading();
      processOperation(pramForm).then((res) => {
         if (res.code == 200) {
            getList();
            proxy.$modal.msgSuccess("操作成功");
            //close();
         } else {
            proxy.$modal.msgError("操作失败:" + res.errMsg);
         }
      });
      proxy.$modal.closeLoading();
   }).catch(() => { })
}

</script>