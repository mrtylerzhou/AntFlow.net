<template>
  <el-dialog title="选择条件" v-model="visibleDialog" :width="480" append-to-body class="condition_list">
    <p>请选择用来区分审批流程的条件字段</p>
    <p class="check_box">
      <template v-for="(item, index) in conditions" :key="index">
        <a :class="$func.toggleClass(conditionList, item, 'formId') && 'active'"
          @click="$func.toChecked(conditionList, item, 'formId')">{{ item?.showName }}</a>
        <br v-if="(index + 1) % 3 == 0" />
      </template>
    </p>
    <template #footer>
      <el-button @click="closeDialog">取 消</el-button>
      <el-button type="primary" @click="sureCondition">确 定</el-button>
    </template>
  </el-dialog>
</template>

<script setup name="selectConditionDialog">
import { watch } from "vue"
import { getConditions } from '@/api/workflow/mock'
import { useStore } from '@/store/modules/workflow'
import { NodeUtils } from '@/utils/antflow/nodeUtils'
import $func from '@/utils/antflow/index'
import { condition_filedTypeMap, condition_filedValueTypeMap, condition_columnTypeMap } from '@/utils/antflow/const'
const route = useRoute()
const routePath = route.path || ''
const store = useStore()
let tableId = computed(() => store.tableId)
let lowCodeFormFields = computed(() => store.lowCodeFormField)
let configData = ref(null);
let conditionsConfig1 = computed(() => store.conditionsConfig1)

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  activeGroupIdx: {
    type: Number,
    default: null,
  },
});
let emits = defineEmits(["update:visible"]);
let conditions = ref([])//添加条件弹框显示
let conditionList = ref([])//添加条件弹框显示>是否选中 

let visibleDialog = computed({
  get() {
    return props.visible
  },
  set() {
    closeDialog()
  }
});

watch(conditionsConfig1, (val) => {
  configData.value = val.value?.conditionNodes[val.priorityLevel - 1];
});
watch(() => props.visible, (val) => {
  if (val) {
    getCondition();
  }
});
/**加载条件 */
const getCondition = async () => {
  conditionList.value = [];
  conditions.value = routePath.indexOf('diy-design') > 0 ? await loadDIYFormCondition() : await loadLFFormCondition();
  if (configData.value.conditionList) {
    for (var i = 0; i < configData.value.conditionList[props.activeGroupIdx].length; i++) {
      var { formId, columnId } = configData.value.conditionList[props.activeGroupIdx][i];
      if (columnId == 0) {
        conditionList.value.push({ formId: formId, columnId: 0 })
      } else {
        conditionList.value.push(conditions.value.filter(item => { return item.formId == formId; })[0])
      }
    }
  }
  conditionList.value = conditionList.value.filter(item => { return item; })
}
/**
 * 确认/保存
 */
const sureCondition = () => {
  chooseCondition();
  handleClose();
};
/**自定义表单条件加载 */
const loadDIYFormCondition = () => {
  return new Promise(async (resolve, reject) => {
    let { data } = await getConditions({ tableId: tableId.value });
    resolve(data);
    reject([]);
  });
}
/**低代码表单条件加载 */
const loadLFFormCondition = () => {
  return new Promise((resolve, reject) => {
    let conditionArr = [];
    if (!lowCodeFormFields.value.hasOwnProperty("formFields")) {
      resolve(conditionArr);
    }
    conditionArr = lowCodeFormFields.value.formFields.filter(item => { return item.fieldTypeName; }).map((item, index) => {
      if (item.fieldTypeName && condition_filedTypeMap.has(item.fieldTypeName)) {
        let optionGroup = [];
        if (item.optionItems) {
          optionGroup = item.optionItems.map(c => {
            let convertValue = parseInt(c.value);
            if (!isNaN(convertValue)) {
              return { key: convertValue, value: c.label }
            }
          });
          optionGroup = optionGroup.filter(c => c);
        }
        return {
          formId: index + 1,
          columnId: condition_columnTypeMap.get(item.fieldTypeName),
          showType: condition_filedTypeMap.get(item.fieldTypeName),
          showName: item.label,
          columnName: item.name,
          columnType: condition_filedValueTypeMap.get(item.fieldTypeName),
          fieldTypeName: item.fieldTypeName,
          multiple: item.multiple,
          multipleLimit: item.multipleLimit,
          fixedDownBoxValue: JSON.stringify(optionGroup)
        }
      }
    })
    conditionArr = conditionArr.filter(nullableFilter);
    resolve(conditionArr);
    reject([]);
  });
};

/**过滤空值 */
const nullableFilter = (elm) => {
  return (elm != null && elm !== false && elm !== "");
}
/**
 * 选择条件
 */
const chooseCondition = () => {
  for (var i = 0; i < conditionList.value.length; i++) {
    var { formId, columnId, showName, columnName, showType, columnType, fieldTypeName, multiple, multipleLimit, fixedDownBoxValue } = conditionList.value[i];
    if ($func.toggleClass(configData.value.conditionList[props.activeGroupIdx], conditionList.value[i], "formId")) {
      continue;
    }
    const judgeObj = NodeUtils.createJudgeNode(formId, columnId, 2, showName, showType, columnName, columnType, fieldTypeName, multiple, multipleLimit, fixedDownBoxValue);
    if (columnId == 0) {
      configData.value.conditionList[props.activeGroupIdx].push({ formId: formId, columnId: columnId, type: 1, showName: '发起人' });
    } else {
      configData.value.conditionList[props.activeGroupIdx].push(judgeObj)
    }
  }
  for (let i = configData.value.conditionList[props.activeGroupIdx].length - 1; i >= 0; i--) {
    if (!$func.toggleClass(conditionList.value, configData.value.conditionList[props.activeGroupIdx][i], "formId")) {
      configData.value.conditionList[props.activeGroupIdx].splice(i, 1);
    }
  }
  configData.value.conditionList[props.activeGroupIdx].sort(function (a, b) { return a.columnId - b.columnId; });
}

/**
 * 关闭弹窗
 */
const closeDialog = () => {
  handleClose();
};
const handleClose = () => {
  emits("update:visible", false);
}; 
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";

.condition_list {
  .el-dialog__body {
    padding: 16px 26px;
  }

  p {
    color: #666666;
    margin-bottom: 10px;

    &>.check_box {
      margin-bottom: 0;
      line-height: 36px;
    }
  }
}
</style>