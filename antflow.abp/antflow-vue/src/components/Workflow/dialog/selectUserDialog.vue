<template>
  <!-- 授权用户 -->
  <el-dialog title="选择用户" v-model="visibleDialog" style="width: 800px !important" :before-close="handleClose"
    append-to-body>
    <el-row :gutter="10">
      <el-col :span="16">
        <el-row>
          <el-col :span="24">
            <el-form :model="qform" ref="queryRef" :inline="true">
              <el-form-item label="用户名称" prop="description">
                <el-input v-model="qform.description" placeholder="请输入用户名称" clearable style="width: 150px"
                  size="default" @keyup.enter.native="handleQuery" />
              </el-form-item>
              <el-form-item>
                <el-button type="primary" size="default" @click="handleQuery">搜索</el-button>
              </el-form-item>
            </el-form>
            <el-table ref="refTable" :data="userList" v-loading="loading" border height="500px"
              :row-style="{ height: '30px' }">
              <el-table-column label="操作" width="55" align="center" class-name="small-padding fixed-width">
                <template #default="scope">
                  <el-button link type="primary" size="default" icon="CirclePlus"
                    @click="handleSelectUser(scope.row)" />
                </template>
              </el-table-column>
              <el-table-column label="用户名称" prop="userName" :show-overflow-tooltip="true">
                <template #default="scope">
                  <el-avatar size="small">{{ scope.row.userName.substring(0, 1) }}</el-avatar>
                  {{ scope.row.userName }}
                </template>
              </el-table-column>
              <el-table-column label="邮箱" prop="email" :show-overflow-tooltip="true" />
              <el-table-column label="状态" align="center" prop="status">
                <template #default="scope">
                  <el-tag size="default">
                    {{ scope.row.status == "0" ? "正常" : "停用" }}</el-tag>
                </template>
              </el-table-column>
            </el-table>
          </el-col>
          <el-col :span="24">
            <pagination v-show="total > 0" :total="total" v-model:page="pageDto.page" v-model:limit="pageDto.pageSize"
              :layout="layoutSize" @pagination="getPageList" />
          </el-col>
        </el-row>
      </el-col>
      <el-col :span="8">
        <p class="tip">已选中列表</p>
        <el-table ref="selectedTable" :data="checkedUsersList" border height="540px">
          <el-table-column label="操作" width="55" align="center" class-name="small-padding fixed-width">
            <template #default="scope">
              <el-button link type="primary" size="default" icon="Delete" @click="handleRemove(scope.row)" />
            </template>
          </el-table-column>
          <el-table-column label="用户名称" prop="userName" :show-overflow-tooltip="true">
            <template #default="scope">
              <el-avatar size="small">{{ scope.row.userName.substring(0, 1) }}</el-avatar>
              {{ scope.row.userName }}
            </template>
          </el-table-column>
        </el-table>
      </el-col>

    </el-row>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" size="default" @click="saveDialog">确 定</el-button>
        <el-button size="default" @click="closeDialog">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup name="selectUserDialog">
import { ref } from "vue";
import { getUserPageList } from "@/api/workflow/mock";
const { proxy } = getCurrentInstance();
let emits = defineEmits(["update:visible", "change"]);

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  data: {
    type: Array,
    default: [],
  },
});

const loading = ref(false);
const layoutSize = 'total, prev, pager, next';
const userList = ref([]);
const total = ref(0);
let checkedUsersList = ref([]);
const queryParams = reactive({
  qform: {
    description: undefined
  },
  pageDto: {
    page: 1,
    pageSize: 10
  },
});
const { pageDto, qform } = toRefs(queryParams);
let visibleDialog = computed({
  get() {
    if (props.visible) {
      checkedUsersList.value = props.data.map((item) => {
        return {
          userId: item.targetId ?? item.id,
          userName: item.name,
        };
      });
    }
    return props.visible;
  },
  set() {
    closeDialog();
  },
});

// 用户数据
const getPageList = () => {
  loading.value = true;
  getUserPageList(pageDto.value, qform.value).then((res) => {
    loading.value = false;
    total.value = res.pagination.totalCount;
    pageDto.value.page = res.pagination.page;
    userList.value = res.data.map((item) => {
      return {
        userId: item.id,
        userName: item.name,
        email: "574427343@qq.com",
        status: 0, // 0: 正常, 1: 停用
      };
    });
  }).catch((res) => {
    proxy.$modal.msgError("获取用户列表失败" + res.message);
  });
}
getPageList();
/** 搜索按钮操作 */
function handleQuery() {
  pageDto.value.page = 1;
  getPageList();
}

/** 选择授权用户操作 */
function handleSelectUser(row) {
  if (!row || row.userId == "") {
    proxy.$modal.msgError("请选择用户");
    return;
  }
  if (checkedUsersList.value.some((c) => c.userId == row.userId)) {
    proxy.$modal.msgError("用户已被选中");
  } else {
    checkedUsersList.value.push(row);
  }
}
/** 移除选中用户操作 */
function handleRemove(row) {
  if (checkedUsersList.value.some((c) => c.userId == row.userId)) {
    checkedUsersList.value = checkedUsersList.value.filter(
      (item) => item.userId != row.userId
    );
  } else {
    proxy.$modal.msgError("用户已被移除");
  }
}
/**
 * 确认/保存
 */
let saveDialog = () => {
  let checkedList = [...checkedUsersList.value].map((item) => ({
    type: 1,
    targetId: item.userId,
    name: item.userName,
  }));
  handleClose();
  emits("change", checkedList);
};

/**
 * 关闭弹窗
 */
const closeDialog = () => {
  handleClose();
};
const handleClose = () => {
  checkedUsersList.value = [];
  emits("update:visible", false);
};
</script>
<style lang="css" scoped>
.tip {
  font-weight: 700;
  padding: 2px 13px;
  background-color: rgb(197.7, 225.9, 255);
  border-left: 5px solid #1890ff;
}
</style>
