<template>
  <div class="app-container">
    <div class="query-box">
      <el-form :model="vo" ref="queryRef" :inline="true" v-show="showSearch">
        <el-form-item label="租户标识" prop="businessPartyMark">
          <el-input v-model="vo.businessPartyMark" placeholder="请输入关键字" clearable style="width: 200px"
            @keyup.enter="handleQuery" />
        </el-form-item>
        <el-form-item label="租户名字" prop="name">
          <el-input v-model="vo.name" placeholder="请输入关键字" clearable style="width: 200px" @keyup.enter="handleQuery" />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
          <el-button icon="Refresh" @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" icon="Plus" @click="handleAdd">新增租户</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
      </el-row>
    </div>
    <div class="table-box">
      <el-table v-loading="loading" :data="list">
        <el-table-column label="租户标识" align="center" prop="businessPartyMark" v-if="columns[0].visible"
          :show-overflow-tooltip="true" />
        <el-table-column label="租户名称" align="center" prop="name" v-if="columns[1].visible"
          :show-overflow-tooltip="true" />
        <el-table-column label="接入类型" align="center" prop="accessTypeName" v-if="columns[2].visible"
          :show-overflow-tooltip="true" />
        <el-table-column label="备注" align="center" prop="remark" v-if="columns[3].visible"
          :show-overflow-tooltip="true" />
        <el-table-column label="创建时间" align="center" prop="createTime" v-if="columns[4].visible" width="160">
          <template #default="scope">
            <span>{{ parseTime(scope.row.createTime, '{y}-{m}-{d} {h}:{i}') }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" fixed="right" width="150" align="center" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-tooltip content="新增APP" placement="top">
              <el-button link type="primary" icon="Plus" @click="handleAddApp(scope.row)"></el-button>
            </el-tooltip>
            <el-tooltip content="编辑" placement="top">
              <el-button link type="primary" icon="Edit" @click="handleEdit(scope.row)"></el-button>
            </el-tooltip>
            <el-tooltip content="删除" placement="top">
              <el-button link type="primary" icon="Delete" @click="handleDelete(scope.row)"></el-button>
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>
      <pagination v-show="total > 0" :total="total" v-model:page="page.page" v-model:limit="page.pageSize"
        @pagination="getList" />
    </div>
    <!-- 添加或修改委托对话框 -->
    <el-dialog :title="title" v-model="open" width="550px" append-to-body>
      <el-form :model="form" :rules="rules" ref="formRef" label-width="130px" style="margin: 0 20px;"
        label-position="top">
        <el-row>
          <el-col :span="24">
            <el-form-item prop="businessPartyMark">
              <template #label>
                <span>
                  <el-tooltip content="项目唯一标识" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  租户标识
                </span>
              </template>
              <el-input v-model="form.businessPartyMark" placeholder="请输入租户标识" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="租户名称" prop="name">
              <el-input v-model="form.name" placeholder="请输入租户名称" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="接入类型" prop="accessType">
              <el-radio-group v-model="form.accessType">
                <el-radio value="1" :disabled=true>嵌入式</el-radio>
                <el-radio value="0">调入式</el-radio>
              </el-radio-group>
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
          <el-button @click="cancel">取 消</el-button>
          <el-button type="primary" @click="submitForm">确 定</el-button>

        </div>
      </template>
    </el-dialog>
    <app-form v-model:visible="openAddApp" v-model:appformData="appData" @refresh="getList" />
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import appForm from "../outsideApp/form.vue";
import { getBusinessPartyList, setBusinessParty, getBusinessPartyDetail } from "@/api/workflow/outsideApi";
const { proxy } = getCurrentInstance();
const list = ref([]);
const loading = ref(false);
const showSearch = ref(true);
const total = ref(0);
const open = ref(false);
let openAddApp = ref(false);
const title = ref("");
const data = reactive({
  appData: {},
  form: {
    accessType: '0'
  },
  page: {
    page: 1,
    pageSize: 10
  },
  vo: {},
  rules: {
    businessPartyMark: [{
      required: true,
      pattern: /^[a-zA-Z]{4,10}$/,
      message: '请输入项目唯一标识(只能是大小写字母,4-10位长度)',
      trigger: ['blur', 'change']
    }],
    name: [
      { required: true, message: '请输入租户名称', trigger: 'blur' },
      { pattern: /^.{2,10}$/, message: '长度必须在2到10位之间', trigger: 'blur' }
    ],
    accessType: [{ required: true, message: '', trigger: 'change' }]
  }
});
const { page, vo, form, rules, appData } = toRefs(data);

onMounted(async () => {
  getList();
})

// 列显隐信息
const columns = ref([
  { key: 0, label: `租户标识`, visible: true },
  { key: 1, label: `项目名字`, visible: true },
  { key: 2, label: `接入类型`, visible: true },
  { key: 3, label: `备注`, visible: true },
  { key: 4, label: `创建时间`, visible: true }
]);

/** 查询接入项目列表 */
function getList() {
  loading.value = true;
  getBusinessPartyList(page.value, vo.value).then(response => {
    list.value = response.data.data;
    total.value = response.data.pagination.totalCount;
    loading.value = false;
  }).catch(() => {
    loading.value = false;
  });
}
/** 新增接入项目 */
function handleAdd() {
  reset();
  title.value = "添加项目";
  open.value = true;
}
/** 提交表单 */
function submitForm() {
  proxy.$refs["formRef"].validate(valid => {
    if (valid) {
      if (form.value.id != undefined) {
        setBusinessParty(form.value).then(response => {
          proxy.$modal.msgSuccess("修改成功");
          open.value = false;
          getList();
        });
      } else {
        setBusinessParty(form.value).then(response => {
          if (response.code != 200) {
            proxy.$modal.msgError("新增失败");
            return;
          }
          proxy.$modal.msgSuccess("新增成功");
          open.value = false;
          getList();
        });
      }
    }
  });
}
/** 修改按钮操作 */
function handleEdit(row) {
  reset();
  const id = row.id;
  if (id == 1) {
    proxy.$modal.msgError("演示数据不允许修改操作！");
    return;
  }
  open.value = true;
  getBusinessPartyDetail(id).then(response => {
    form.value = response.data;
    form.value.accessType = form.value.accessType.toString();
    open.value = true;
    title.value = "编辑项目";
  });
}
/**
 * 新增应用
 * @param row 
 */
function handleAddApp(row) {
  appData.value.applyType = '2';
  appData.value.isSon = '1';
  appData.value.businessName = row.name;
  appData.value.businessCode = row.businessPartyMark;
  openAddApp.value = true;
}

/** 删除按钮操作 */
function handleDelete(row) {
  proxy.$modal.msgError("演示环境不允许删除操作！");
}

/** 取消操作表单 */
function cancel() {
  open.value = false;
  reset();
}

/** 重置操作表单 */
function reset() {
  form.value = {
    id: undefined,
    businessPartyMark: undefined,
    name: undefined,
    accessType: '0',
    remark: undefined
  };
  proxy.resetForm("queryRef");
};

/** 搜索按钮操作 */
function handleQuery() {
  page.value.page = 1;
  getList();
}

/** 重置按钮操作 */
function resetQuery() {
  vo.value = {};
  proxy.resetForm("queryRef");
  handleQuery();
}

</script>