<template>
  <!-- 选择用户 -->
  <el-dialog title="选择用户" v-model="visibleDialog" style="width: 800px;height: 550px" :before-close="handleClose"
    append-to-body>
    <el-form :model="qform" ref="queryRef" :inline="true">
      <el-form-item label="用户名称" prop="description">
        <el-input v-model="qform.description" placeholder="请输入关键字" clearable style="width: 150px" size="default"
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
    <el-radio-group class="radio-table" style="width: 100%;" v-model="selectUserId" @change="clickedRadio">
      <el-table row-key="userId" :data="userList" v-loading="loading" height="350px"
        @selection-change="handleSelectionChange">
        <el-table-column v-if="multiple" align="center" type="selection" width="50px" :selectable="canSelectable" />
        <el-table-column v-else align="center" width="50px">
          <template v-slot="scope">
            <el-radio :value="scope.row.userId"></el-radio>
          </template>
        </el-table-column>
        <el-table-column label="用户名称" prop="userName" :show-overflow-tooltip="true" />
        <el-table-column label="邮箱" prop="email" :show-overflow-tooltip="true" />
        <el-table-column label="状态" align="center" prop="status">
          <template #default="scope">
            <el-tag size="default">
              {{ scope.row.status === "0" ? "正常" : "停用" }}</el-tag>
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
import { getUserPageList } from "@/api/workflow/mock";
import { ref } from "vue";
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
    type: Object,
    default: undefined,
  },
});

const emits = defineEmits(["update:visible", "change"]);
const loading = ref(false);
const userList = ref([]);
const total = ref(23);
const layoutSize = 'total, prev, pager, next';

// 单选下选中的用户
const selectUserId = ref(null);
// 多选下选中的用户列表
const multiSelectUser = ref([]);
let visibleDialog = computed({
  get() {
    getPageList();
    return props.visible;
  },
  set() {
    closeDialog();
  },
});

const queryParams = reactive({
  qform: {
    description: null,
  },
  pageDto: {
    page: 1,
    pageSize: 10
  }
});
const { pageDto, qform } = toRefs(queryParams);


const canCommit = computed(() => {
  return props.multiple ? multiSelectUser.value.length > 0 && multiSelectUser.value.length <= props.multiplelimit : (selectUserId.value != null && selectUserId.value !== '');
});
const canSelectable = (row) => {
  return !props.checkedData || !props.checkedData.approversList.some((item) => item.id === row.userId);
}
// 查询表数据
const getPageList = async () => {
  loading.value = true;
  await getUserPageList(pageDto.value, qform.value).then((res) => {
    loading.value = false;
    userList.value = res.data.map((item) => {
      return {
        userId: item.id,
        userName: item.name,
        email: "574427343@qq.com",
        status: 1,
      };
    });
  }).catch((res) => {
    proxy.$modal.msgError("获取用户列表失败" + res.message);
  });
}

/** 搜索按钮操作 */
async function handleQuery() {
  pageDto.page = 1;
  await getPageList();
}
/** 点击单框选中数据 */
function clickedRadio(id) {
  let selectInfo = userList.value.find((item) => item.userId === id);
  if (!proxy.isEmpty(selectInfo)) {
    multiSelectUser.value = [{
      id: selectInfo.userId,
      name: selectInfo.userName,
    }];
  }
}

/** 点击多选框选中数据 */
function handleSelectionChange(selection) {
  const selectArr = selection.map(item => ({
    id: item.userId,
    name: item.userName,
  }));
  multiSelectUser.value = selectArr;
  if (!proxy.isEmpty(props.checkedData) && !proxy.isEmptyArray(props.checkedData.approversList)) {
    for (let psd of props.checkedData.approversList) {
      if (!multiSelectUser.value.some(c => c.id == psd.id)) {
        multiSelectUser.value.push(psd);
      }
    }
  };
}
/**
 * 确认/保存
 */
let saveDialog = () => {
  let uniqueMultiSelectUser = uniqueArr(multiSelectUser.value);

  props.checkedData.approversList = uniqueMultiSelectUser;
  emits("change", props.checkedData.approversList);
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
  userList.value = [];
  multiSelectUser.value = [];
  emits("update:visible", false);
};
/** 重置按钮操作 */
function resetQuery() {
  qform.value = {
    description: null,
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
