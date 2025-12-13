/**
 * 表单接口，用来便于集成不同的表单，方便自由替换
 */
import {ref,computed} from "vue";
import { useStore } from '@/store/modules/workflow';
let store = useStore();
export const formRef = ref(null)
//表单内每个字段ID，与对应字段的配置
export const formItemMap = new Map()
// //表单字段列表
// export const formFields = computed(() => store.lowCodeFormField)

// watch(formFields, () => {  
//   getFormPermFields();
// }, {deep: true})

 
/**
 * 获取表单字段列表
 * @returns [{表单字段信息}]
 */
export const getFormPermFields = (defaultPerm = 'R') => {  
  let items = []
  //formItemMap.clear() //清空map
  const addItem = (item) => {
    items.push({
      fieldId: item.name,
      //key: item.name,
      fieldName: `${item.label}`,
      //required: item.required,
      perm: item.perm ? item.perm : defaultPerm
    })
  }
  loadFormItem(store.lowCodeFormField, addItem)
  return items
}

 /**
 * 加载表单组件选项
 * @param fields 表单字段 
 * @param addItemFunc 添加的函数
 */
const loadFormItem = (obj,addItemFunc) => { 
  if(!obj.formFields){
    return;
  } 
  if (Array.isArray(obj.formFields)){
    obj.formFields.forEach(item => {
      addItemFunc(item)
    })
  } 
  //formItemMap.set(item.id, item)
}
/**
 * 显示表单字段
 * @param fieldIds 字段id列表
 */
export function showFields(fieldIds) {

}

/**
 * 隐藏表单字段
 * @param fieldIds 字段id列表
 */
export function hideFields(fieldIds) {

}

/**
 * 禁用表单字段
 * @param fieldIds 字段id列表
 */
export function disableFields(fieldIds) {

}

/**
 * 允许编辑表单字段
 * @param fieldIds 字段id列表
 */
export function enableFields(fieldIds) {

}
