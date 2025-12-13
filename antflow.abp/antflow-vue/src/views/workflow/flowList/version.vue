<template>
   <div class="app-container">
      <div class="query-box">
         <el-row class="mb10">
            <el-col :span="12">
               <el-alert type="warning" show-icon :closable="false">
                  <div class="marquee-container">
                     <div class="marquee-text">【温馨提示】点击【启用】按钮，流程设计才能生效哦</div>
                  </div>
               </el-alert>
            </el-col>
            <el-col :span="12" style="text-align: right;">
               <el-button type="primary" plain @click="backPage()"><el-icon>
                     <Back />
                  </el-icon>返回 </el-button>
            </el-col>
         </el-row>
      </div>
      <div class="table-box">
         <el-table v-loading="loading" :data="configList">
            <el-table-column label="类型标识" align="center" prop="formCode" :show-overflow-tooltip="true" />
            <el-table-column label="类型名称" align="center" prop="formCodeDisplayName" :show-overflow-tooltip="true" />
            <el-table-column label="版本编号" align="center" prop="bpmnCode" :show-overflow-tooltip="true" />
            <el-table-column label="版本名称" align="center" prop="bpmnName" :show-overflow-tooltip="true" />
            <el-table-column label="流程分类" align="center" prop="isLowCodeFlow">
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
            <el-table-column label="创建时间" align="center" prop="updateTime" width="160">
               <template #default="scope">
                  <span>{{ parseTime(scope.row.updateTime, '{y}-{m}-{d} {h}:{i}') }}</span>
               </template>
            </el-table-column>
            <el-table-column label="操作" fixed="right" width="150" align="center" class-name="small-padding fixed-width">
               <template #default="scope">
                  <el-button v-if="scope.row.effectiveStatus == 1" type="info" disabled link>启动</el-button>
                  <el-button v-else type="success" link @click="effectiveById(scope.row)">启动</el-button>
                  <el-button link type="primary" @click="handleEdit(scope.row)">编辑</el-button>
                  <el-button link type="success" @click="handlePreview(scope.row)">预览</el-button>
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
import { getBpmnConflistPage, getEffectiveBpmn } from "@/api/workflow/index";
const { query } = useRoute();
const { proxy } = getCurrentInstance();
const configList = ref([]);
const loading = ref(false);
const total = ref(0);
const data = reactive({
   form: {},
   pageDto: {
      page: 1,
      pageSize: 10
   },
   taskMgmtVO: {
      effectiveStatus: undefined,
      isOutSideProcess: 0,
      isLowCodeFlow: undefined,
      formCode: query?.formCode || '0',
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
/**
 * 复制操作
 * @param row
 */
const handleEdit = (row) => {
   const params = {
      id: row.id
   };
   if (row.isLowCodeFlow == '1') {
      let obj = { path: "/workflow/lf-design", query: params };
      proxy.$tab.closeOpenPage(obj);
   } else {
      let obj = { path: "/workflow/diy-design", query: params };
      proxy.$tab.closeOpenPage(obj);
   }
}
/**
 * 启动流程
 * @param data
 */
const effectiveById = async (data) => {
   await getEffectiveBpmn(data).then(async (res) => {
      if (res.code == 200) {
         getList();
         proxy.$modal.msgSuccess("操作成功");
      } else {
         proxy.$modal.msgError("操作失败");
      }
   });
}

/** 预览 */
function handlePreview(row) {
   const params = {
      id: row.id
   };
   let obj = { path: "/workflow/flowPreview", query: params };
   proxy.$tab.openPage(obj);
}
/**  返回按钮操作 */
const backPage = () => {
   const obj = { path: "/workflow/flowlist" };
   proxy.$tab.closeOpenPage(obj);
}
</script>
<style lang="scss" scoped>
.marquee-container {
   overflow: hidden;
   white-space: nowrap;
   position: relative;
   font-size: 18px;
}

.marquee-text {
   display: inline-block;
   animation: marquee 10s linear infinite;
}

.marquee-container:hover .marquee-text {
   transform: translateX(0);
   animation-play-state: paused;
   /* 鼠标悬停时暂停动画 */
}

@keyframes marquee {
   from {
      transform: translateX(100%);
   }

   to {
      transform: translateX(-100%);
   }
}
</style>