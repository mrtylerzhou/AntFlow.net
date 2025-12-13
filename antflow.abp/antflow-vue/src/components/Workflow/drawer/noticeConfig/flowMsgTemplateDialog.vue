<template>
  <!-- 选择消息模板 -->
  <el-dialog title="选择消息模板" v-model="visibleDialog" style="width: 800px;height: 550px" :before-close="handleClose"
    append-to-body>
    <el-form :model="qform" ref="queryRef" :inline="true">
      <el-form-item label="关键字" prop="name">
        <el-input v-model="qform.name" placeholder="请输入关键字" clearable style="width: 150px" size="default"
          @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" size="default" @click="handleQuery">搜索</el-button>
        <el-button icon="Refresh" @click="resetQuery">重置</el-button>
      </el-form-item>
      <el-form-item class="pull-right">
        <el-button type="success" size="default" :disabled="!canCommit" @click="saveDialog">确定</el-button>
        <el-button type="warning" size="default" @click="handleClose">取消</el-button>
      </el-form-item>
    </el-form>
    <el-radio-group class="radio-table" style="width: 100%;" v-model="selectId" @change="clickedRadio">
      <el-table row-key="id" :data="msgTempList" v-loading="loading" height="350px"
        @selection-change="handleSelectionChange">
        <el-table-column v-if="multiple" align="center" type="selection" width="50px" :selectable="canSelectable" />
        <el-table-column v-else align="center" width="50px">
          <template v-slot="scope">
            <el-radio :value="scope.row.id"></el-radio>
          </template>
        </el-table-column>
        <el-table-column label="模板编号" align="center" prop="num" width="120" />
        <el-table-column label="模板名称" align="center" prop="name" width="160">
          <template #default="item">
            <el-tooltip class="box-item" effect="dark" placement="right">
              <template #content>
                <span>{{ item.row.name }}</span>
              </template>
              {{ substringHidden(item.row.name) }}
            </el-tooltip>
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" prop="status">
          <template #default="scope">
            <el-tag size="default">
              {{ scope.row.status == "0" ? "正常" : "停用" }}</el-tag>
          </template>
        </el-table-column>
      </el-table>
    </el-radio-group>
    <template #footer>
      <div class="dialog-footer">
        <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
          :layout="layoutSize" @pagination="getPageList" />
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { getFlowMsgTempleteList } from "@/api/workflow/flowMsgApi";
import { ref, watch } from "vue";
const { proxy } = getCurrentInstance();
const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  multiple: {
    type: Boolean,
    default: false,
  },
  multiplelimit: {
    type: Number,
    default: 3,
  },
  checkedData: {
    type: Array,
    default: [],
  },
});

const emits = defineEmits(["update:visible", "update:checkedData", "change"]);
const loading = ref(false);
const msgTempList = ref([]);
const total = ref(0);
const layoutSize = 'total, prev, pager, next';

// 单选下选中的消息模板
const selectId = ref(null);
// 多选下选中的消息模板列表
const multiSelectFlag = ref([]);
let visibleDialog = computed({
  get() {
    return props.visible;
  },
  set() {
    closeDialog();
  },
});

const queryParams = reactive({
  qform: {
    name: undefined,
  },
  pageDto: {
    page: 1,
    pageSize: 10
  },
});
const { pageDto, qform } = toRefs(queryParams);
const canCommit = computed(() => {
  return props.multiple ? !proxy.isEmptyArray(multiSelectFlag.value) && multiSelectFlag.value.length <= props.multiplelimit : !proxy.isEmpty(selectId.value);
});
const canSelectable = (row) => {
  return !props.checkedData?.some((item) => item.id === row.id);
}

watch(() => props.visible, (val) => {
  if (val) {
    getPageList();
  }
})

onMounted(() => {
  multiSelectFlag.value = props.checkedData;
  if (!proxy.isEmptyArray(props.checkedData) && !props.multiple) {
    selectId.value = props.checkedData[0].id;
  } else {
    selectId.value = null;
  }
});
// 查询表数据
const getPageList = async () => {
  loading.value = true;
  await getFlowMsgTempleteList(pageDto.value, qform.value).then((res) => {
    loading.value = false;
    total.value = res.pagination.totalCount;
    pageDto.value.page = res.pagination.page;
    msgTempList.value = res.data.map((item) => {
      return {
        id: item.id,
        num: item.num,
        name: item.name,
        status: item.status,
      };
    });
  }).catch((res) => {
    proxy.$modal.msgError("获取消息模板列表失败" + res.message);
  });
}

/** 搜索按钮操作 */
async function handleQuery() {
  pageDto.page = 1;
  await getPageList();
}
/** 点击单框选中数据 */
function clickedRadio(id) {
  let selectInfo = msgTempList.value.find((item) => item.id === id);
  if (!proxy.isEmpty(selectInfo)) {
    multiSelectFlag.value = [{
      id: selectInfo.id,
      num: selectInfo.num,
      name: selectInfo.name,
    }];
  }
}

/** 点击多选框选中数据 */
function handleSelectionChange(selection) {
  const selectArr = selection.map(item => ({
    id: item.id,
    name: item.name,
  }));
  multiSelectFlag.value = selectArr;
  if (!proxy.isEmptyArray(props.checkedData)) {
    for (let psd of props.checkedData) {
      if (!multiSelectFlag.value.some(c => c.id == psd.id)) {
        multiSelectFlag.value.push(psd);
      }
    }
  };
}
/**
 * 确认/保存
 */
let saveDialog = () => {
  let uniquemultiSelectFlag = uniqueArr(multiSelectFlag.value);
  emits("change", uniquemultiSelectFlag);
  handleClose();
};
/**对象数组去重 */
const uniqueArr = (arr) => {
  return Array.from(new Set(arr.map(item => item.id))).map(id => {
    return arr.find(item => item.id === id);
  });
};
/**
 * 关闭弹窗
 */
const closeDialog = () => {
  handleClose();
};
const handleClose = () => {
  msgTempList.value = [];
  multiSelectFlag.value = [];
  emits("update:visible", false);
};
/** 重置按钮操作 */
function resetQuery() {
  qform.value = {
    name: null,
  };
  handleQuery();
}
</script>
<style lang="css" scoped>
.tip {
  padding: 8px 16px;
  background-color: rgb(197.7, 225.9, 255);
  border-left: 5px solid #1890ff;
}
</style>
