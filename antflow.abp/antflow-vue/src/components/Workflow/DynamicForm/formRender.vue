<template>
  <div class="app-container">
    <el-row :gutter="20">
      <el-col :span="hasChooseApprove == 'true' ? 16 : 24">
        <div class="form-container" :style="hasChooseApprove == 'true' ? {} : { maxWidth: '80vw', margin: '0 auto' }">
          <div class="el-main">
            <v-form-render ref="vFormRef" :form-json="formJson" :form-data="formData" :option-data="optionData">
            </v-form-render>
          </div>
          <div class="el-footer" v-if="!isPreview && props.showSubmit">
            <el-button type="primary" @click="submitForm">提交</el-button>
          </div>
        </div>
      </el-col>
      <el-col :span="8" v-if="hasChooseApprove == 'true'">
        <TagApproveSelect v-model:formCode="formCode" @chooseApprove="chooseApprovers" />
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, reactive, getCurrentInstance, onBeforeMount } from 'vue';
import TagApproveSelect from "@/components/BizSelects/TagApproveSelect/index.vue";
const isEmpty = data => data === null || data === undefined || data == '' || data == '{}' || data == '[]' || data == 'null';
const { proxy } = getCurrentInstance();
const route = useRoute();
const formCode = route.query?.formCode ?? '';
const hasChooseApprove = route.query?.hasChooseApprove ?? 'false';
// watch(() => formRenderConfig.value, () => {
//   console.log("formCode====", JSON.stringify(formCode));
//   console.log("hasChooseApprove====", JSON.stringify(hasChooseApprove));
//   console.log("formRenderConfig.value====", JSON.stringify(formRenderConfig.value));
// });
let props = defineProps({
  lfFormData: {//业务表单字段
    type: String,
    default: "{}",
  },
  lfFieldsData: {//字段权限控制
    type: String,
    default: "{}",
  },
  lfFieldPerm: {//表单字段控制权限
    type: String,
    default: "[]",
  },
  showSubmit: {//是否显示提交按钮
    type: Boolean,
    default: false,
  },
  isPreview: {//是否预览
    type: Boolean,
    default: true,
  }
});
/* 注意：formJson是指表单设计器导出的json，此处演示的formJson只是一个空白表单json！！ */
const formJson = reactive(JSON.parse(props.lfFormData || "{}"));//表单字段渲染
const formData = reactive(JSON.parse(props.lfFieldsData || "{}"));//表单字段输入值渲染 
const lfFieldPermData = reactive(JSON.parse(props.lfFieldPerm || "{}"));//表单字段权限控制 
const optionData = reactive({});
const vFormRef = ref(null);
/**表单渲染预处理 */
const advanceHandleFormData = () => {
  if (!isEmpty(props.lfFieldsData)) {
    traverseFieldWidgetsList(formJson.widgetList, handlerFn);
  }
}
/**表单字段权限控制 */
const handlerFn = (w) => {
  w.options.hidden = false;//字段都隐藏，隐藏后表单字段不会自动补位
  const numberFields = ['number', 'select', 'radio'];
  if (numberFields.includes(w.type)) {
    if (!w.options.multiple) {
      formData[w.options.name] = Number(formData[w.options.name]);
    }
  }
  if (props.showSubmit) {
    w.options.disabled = false;
    w.options.readonly = false;
  }
  else if (!isEmpty(props.lfFieldPerm)) {
    let info = lfFieldPermData.find(function (ele) { return ele.fieldId == w.options.name; });
    if (info) {
      if (info.perm == 'R') {
        w.options.disabled = true;
      } else if (info.perm == 'E') {
        w.options.readonly = false;
      } else if (info.perm == 'H') {//隐藏字段处理：将所以字段类型转化为input格式，value 赋值为 ****** 
        if (w.type != 'textarea' && w.options.type != 'input') {
          w.type = 'input';
          w.options.type = 'text';
        }
        formData[w.options.name] = '******';
        delete w.options.format;
        delete w.options.valueFormat;
        w.options.disabled = true;
      } else {
        w.options.disabled = false;
        w.options.readonly = true;
      }
    }
  } else {
    if (props.isPreview == true) {
      w.options.disabled = true;
      w.options.readonly = true;
    }
  }
}
/**递归处理表单中所有字段 */
const traverseFieldWidgetsList = function (widgetList, handler) {
  if (!widgetList) {
    return
  }
  widgetList.map(w => {
    if (w.formItemFlag) {
      handler(w)
    } else if (w.type === 'grid') {
      w.cols.map(col => {
        traverseFieldWidgetsList(col.widgetList, handler, w)
      })
    } else if (w.type === 'table') {
      w.rows.map(row => {
        row.cols.map(cell => {
          traverseFieldWidgetsList(cell.widgetList, handler, w)
        })
      })
    } else if (w.type === 'tab') {
      w.tabs.map(tab => {
        traverseFieldWidgetsList(tab.widgetList, handler, w)
      })
    } else if (w.type === 'sub-form') {
      traverseFieldWidgetsList(w.widgetList, handler, w)
    } else if (w.category === 'container') {  //自定义容器
      traverseFieldWidgetsList(w.widgetList, handler, w)
    }
  })
}
onBeforeMount(() => {
  // console.log("isPreview======", JSON.stringify(props.isPreview));
  // console.log("showSubmit======", JSON.stringify(props.showSubmit));
  advanceHandleFormData();
})
onMounted(() => {
  nextTick(() => {
    vFormRef.value.setFormJson(formJson)
  }).then(() => {
    vFormRef.value.setFormData(formData)
  })
})
onBeforeUnmount(() => {
  // 清除数据
  Object.keys(formJson).forEach(key => delete formJson[key]);
  Object.keys(formData).forEach(key => delete formData[key]);
  lfFieldPermData.splice(0, lfFieldPermData.length);
})
const submitForm = () => {
  vFormRef.value.getFormData().then(res => {
    //replaceEmptyStringWithNull(res);
    //res["select81554"] = null;
    //console.log("Form Validation===", JSON.stringify(res, null, 2))
    proxy.$emit("handleBizBtn", JSON.stringify(res))
  }).catch(error => {
    proxy.$modal.msgError(error);
  })
}
const handleValidate = () => {
  return new Promise((resolve, reject) => {
    try {
      vFormRef.value.validateForm((isValid) => {
        if (!isValid) {
          reject(false);
        }
        else {
          if (hasChooseApprove == 'true' && (!formData.approversValid || formData.approversValid == false)) {
            proxy.$modal.msgError('请选择自选审批人');
            reject(false);
          }
          else {
            resolve(isValid);
          }
        }
      });
    } catch (error) {
      reject(false);
    }
  });
}

const getFromData = () => {
  return new Promise((resolve, reject) => {
    try {
      vFormRef.value.getFormData().then(res => {
        if (hasChooseApprove == 'true') {
          Object.assign(res, {
            approversList: formData.approversList,
            approversValid: formData.approversValid
          });
        }
        resolve(JSON.stringify(res));
      }).catch(err => {
        reject("");
      })
    } catch (error) {
      reject("");
    }
  });
}
/**自选审批人 */
const chooseApprovers = (data) => {
  formData.approversList = data.approvers;
  formData.approversValid = data.nodeVaild;
}

/* 替换空字符串为null*/
const replaceEmptyStringWithNull = (obj) => {
  if (obj && typeof obj === 'object') {
    Object.keys(obj).forEach(key => {
      if (obj[key] === "") {
        obj[key] = null;
      } else if (typeof obj[key] === 'object') {
        replaceEmptyStringWithNull(obj[key]);
      }
    });
  }
  return obj;
}
defineExpose({
  handleValidate,
  getFromData
})
</script>
<style scoped lang="scss">
.form-container {
  /* 新增父级定位 */
  display: flex;
  flex-direction: column;
  margin: auto;
  background: #eee !important;
}

.form-container .el-main {
  background-color: #fff;
  flex: 1 1 auto;
}

.form-container .el-footer {
  background-color: #fff;
  border-top: 2px solid #f0f0f0;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding-right: 24px;
}
</style>