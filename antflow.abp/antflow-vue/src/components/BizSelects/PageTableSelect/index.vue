<template>
  <el-select clearable :value="value" :multiple="multiple" :placeholder="placeholder" :style="{ width: '100%' }">
    <div style="max-height: 800px;margin: 10px;">
      <el-form :model="qform" ref="queryRef" :inline="true" v-show="showSearch">
        <el-form-item prop="keyword">
          <el-input v-model="qform.keyword" placeholder="请输入关键字" clearable style="width: 200px"
            @keyup.enter="handleQuery" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
          <el-button icon="Refresh" @click="resetQuery">重置</el-button>
        </el-form-item>
      </el-form>

      <el-option :disabled="true">
        <el-row :gutter="20">
          <el-col :span="5">
            <div style="font-weight: 700;color: black;">姓名</div>
          </el-col>
          <el-col :span="8">
            <div style="font-weight: 700;color: black;">日期</div>
          </el-col>
          <el-col :span="11">
            <div style="font-weight: 700;color: black;">地址</div>
          </el-col>
        </el-row>
      </el-option> 
      <el-option v-for="(item, index) in options" :key="index" :label="item.name" :value="item.id">
        <el-row :gutter="20">
          <el-col :span="5">
            <div>
              {{ item.name }}</div>
          </el-col>
          <el-col :span="8">
            <div>
              {{ item.date }}</div>
          </el-col>
          <el-col :span="11">
            <div>
              {{ item.address }}</div>
          </el-col>
        </el-row>
      </el-option>

      <pagination v-show="total > 0" :total="total" v-model:page="page.page" v-model:limit="page.pageSize" :layout="layout"
        @pagination="getList" />
    </div>
  </el-select>
</template>

<script setup>
import { onMounted } from "vue";
const { proxy } = getCurrentInstance();

const props = defineProps({
  /**当前双向数据绑定的值 */
  value: {
    type: [String, Number],
    default: ''
  },
  multiple: {
    type: Boolean,
    default: false
  },
  /**输入框内部的文字 */
  placeholder: {
    type: String,
    default: ''
  }
})
/**
 * import PageTableSelect from '@/components/PageTableSelect/index.vue'
 
  <el-form-item label="哈哈哈哈哈" prop="title">
       <page-table-select id="size-select" v-model="form.title" :multiple=true :placeholder="title" />
  </el-form-item>

 */

const layout=ref('total,  prev, pager, next');
let loading = ref(false);
let showSearch = ref(true);
let total = ref(10);
let options = ref([]);
const data = reactive({
  qform: {
    keyword: undefined
  },
  page: {
    page: 1,
    pageSize: 10
  },
});
const { qform, page } = toRefs(data);
const tableData = [
  {
    id: 1,
    date: '2016-05-03',
    name: 'Tom',
    address: '北京',
  },
  {
    id: 2,
    date: '2016-05-02',
    name: 'lil',
    address: '上海',
  },
  {
    id: 3,
    date: '2016-05-04',
    name: 'max',
    address: '河南',
  },
]
//const emits = defineEmits(['update:value'])
const emits = defineEmits(['update:value', 'update:label', 'change']);
onMounted(() => {
  options.value = tableData
})

function getList() {
  page.value.page = 1;
}

/** 搜索按钮操作 */
function handleQuery() {
  page.value.page = 1;
  //getList();
}

/** 重置按钮操作 */
function resetQuery() {
  qform.value = {};
  proxy.resetForm("queryRef");
  handleQuery();
}
/** 多选框选中数据 */
function handleSelectionChange(header) {
  console.log('header======', JSON.stringify(header));
  if (!header) return;
  //let selectIds = selection.map(item => item.id); 
  console.log('header[0].id======', JSON.stringify(header[0].id));
  emits('update:value', header[0].id);
  emits('update:label', header[0].name);
  emits('change', header[0] || {});
}

</script>

<style lang='scss' scoped>
.el-input__inner {
  height: 30px;
  /* 自定义高度 */
}
</style>